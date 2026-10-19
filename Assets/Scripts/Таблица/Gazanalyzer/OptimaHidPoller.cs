using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using HidSharp;                          // HidSharp.dll -> Assets/Plugins
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Захват данных с газоанализатора MRU Optima (USB HID, Windows).
/// Запускается и останавливается вместе с объектом (или вручную: StartCapture / StopCapture).
/// Читает ТОЛЬКО командами чтения, в фоновом потоке, не блокируя кадры.
///
/// Использование из других скриптов:
///   if (OptimaHidPoller.Instance.TryGet("CO_ppm", out float co)) { ... }
///   OptimaHidPoller.Instance.OnData += values => { ... };   // вызывается в главном потоке
///
/// Ключи: O2_pct, CO2_pct, CO_ppm, CO_mgm3, NO_ppm, NO_mgm3, NO2_ppm, NO2_mgm3,
///        NOx_ppm, NOx_mgm3, SO2_ppm, SO2_mgm3 (только SN317718), P_abs_hPa.
/// Закройте MRU4Win перед запуском.
/// </summary>
[DefaultExecutionOrder(-50)]
public class OptimaHidPoller : MonoBehaviour
{
    public static OptimaHidPoller Instance { get; private set; }

    [Header("Запуск")]
    [Tooltip("Старт захвата при включении объекта (начало симуляции), остановка при выключении.")]
    public bool autoStart = true;
    public float pollInterval = 1.0f;
    [Tooltip("Переподключаться, если прибор отключили или он не найден.")]
    public bool autoReconnect = true;
    [Tooltip("Отправлять команды старта/конца онлайн-режима из лога MRU4Win (по умолчанию выключено).")]
    public bool sendLiveStartStop = false;

    [Header("Отладка")]
    public bool writeCsv = false;       // CSV с сырыми блоками в persistentDataPath
    public bool verbose = false;

    const int VendorId = 0x152A;
    const int PidOptima7 = 0x83F0;      // SN317718: O2/CO/NO/NO2/NOx/SO2
    const int PidOptimaNew = 0x83F7;    // SN355211: O2/CO2/CO/NO/NO2/NOx

    public event Action<IReadOnlyDictionary<string, float>> OnData;   // главный поток

    public bool IsRunning => _running;
    public bool IsConnected => _connected;
    public double SecondsSinceData
    {
        get
        {
            long t = Interlocked.Read(ref _lastTicks);
            return t == 0 ? double.PositiveInfinity : (Stopwatch.GetTimestamp() - t) / (double)Stopwatch.Frequency;
        }
    }

    // ---- состояние ----
    readonly Dictionary<string, float> _values = new Dictionary<string, float>();
    readonly object _lock = new object();
    Thread _thread;
    volatile bool _running, _connected, _dirty;
    long _lastTicks;
    SafeFileHandle _h;
    int _outLen = 61, _inLen = 61, _pid;
    bool _liveStarted;
    string _csvPath;
    readonly Stopwatch _clock = Stopwatch.StartNew();
    readonly HashSet<string> _dumped = new HashSet<string>();

    // ---- команды (из лога MRU4Win) ----
    static byte[] Cmd(params byte[] head) { var c = new byte[30]; Array.Copy(head, c, head.Length); return c; }
    static byte[] Hex(string s) => Enumerable.Range(0, 30).Select(i => Convert.ToByte(s.Substring(i * 2, 2), 16)).ToArray();
    static readonly byte[] LiveStart = Hex("0401000006063207000002000000ff0a0105000000000000000000000000");
    static readonly byte[] LiveStop  = Hex("0401000006063207000002000000ff0a00ff000000000000000000000000");
    static readonly byte[][] ReadCommands =
    {
        Cmd(0x04,0x00,0x00,0x00,0x04,0x04,0xB8,0x0B,0x00,0x00,0x48,0x00),   // блок p0
        Cmd(0x04,0x00,0x00,0x01,0x04,0x04,0xB8,0x0B,0x00,0x00,0x48,0x00),   // блок p1
        Cmd(0x04,0x00,0x00,0x02,0x04,0x04,0xB8,0x0B,0x00,0x00,0x48,0x00),   // блок p2
    };

    [DllImport("hid.dll", SetLastError = true)] static extern bool HidD_SetOutputReport(SafeFileHandle h, byte[] b, int n);
    [DllImport("hid.dll", SetLastError = true)] static extern bool HidD_GetInputReport(SafeFileHandle h, byte[] b, int n);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern SafeFileHandle CreateFile(string name, uint access, uint share, IntPtr sec, uint disp, uint flags, IntPtr tmpl);

    // ================= жизненный цикл =================
    void OnEnable()
    {
        Instance = this;
        if (autoStart) StartCapture();
    }

    void OnDisable()
    {
        StopCapture();
        if (Instance == this) Instance = null;
    }

    public void StartCapture()
    {
        if (_running) return;
        if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer)
        { Debug.LogError("[Optima] Поддерживается только Windows"); return; }

        _csvPath = null;
        if (writeCsv)
        {
            _csvPath = Path.Combine(Application.persistentDataPath, $"optima_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            File.WriteAllText(_csvPath, "t;block;f0;f1;f2;f3;f4;f5;f6;f7;f8;f9;f10;f11;f12;f13\n");
            Debug.Log("[Optima] CSV: " + _csvPath);
        }
        Interlocked.Exchange(ref _lastTicks, 0);
        _running = true;
        _thread = new Thread(Loop) { IsBackground = true, Name = "OptimaPoll" };
        _thread.Start();
        Debug.Log("[Optima] Захват запущен");
    }

    // Сколько объектов (таблиц) сейчас используют захват. Раньше каждая таблица в OnDisable
    // вызывала StopCapture, и выключение одной останавливало приём данных для всех.
    private int _captureUsers;

    /// <summary>Начать пользоваться захватом (захват запускается, если ещё не идёт).</summary>
    public void AcquireCapture()
    {
        _captureUsers++;
        StartCapture();
    }

    /// <summary>Перестать пользоваться захватом; остановка — когда отпустил последний пользователь.</summary>
    public void ReleaseCapture()
    {
        if (_captureUsers > 0) _captureUsers--;
        if (_captureUsers == 0) StopCapture();
    }

    public void StopCapture()
    {
        if (!_running) return;
        _running = false;
        if (_thread != null && _thread.IsAlive && !_thread.Join(3000))
            Debug.LogWarning("[Optima] Поток опроса не завершился вовремя");
        _thread = null;
        lock (_lock) _values.Clear();
        _dirty = false;
        Debug.Log("[Optima] Захват остановлен");
    }

    // ================= публичный доступ к данным =================
    public bool TryGet(string key, out float value)
    {
        lock (_lock) return _values.TryGetValue(key, out value);
    }

    public Dictionary<string, float> Snapshot()
    {
        lock (_lock) return new Dictionary<string, float>(_values);
    }

    void Update()
    {
        if (_dirty)
        {
            _dirty = false;
            OnData?.Invoke(Snapshot());
        }
    }

    // ================= фоновый поток =================
    void Loop()
    {
        int fails = 0;
        try
        {
            while (_running)
            {
                // Ошибка внутри одной итерации больше не завершает поток насовсем:
                // соединение закрывается и (при autoReconnect) открывается заново.
                try
                {
                    if (_h == null)
                    {
                        if (!Open())
                        {
                            if (!autoReconnect) break;
                            SleepWhileRunning(2000);
                            continue;
                        }
                        fails = 0;
                        if (sendLiveStartStop && Transact(LiveStart, out _)) _liveStarted = true;
                    }

                    fails = PollOnce() ? 0 : fails + 1;
                    if (fails >= 3)
                    {
                        Debug.LogWarning("[Optima] Связь с прибором потеряна");
                        CloseHandle();
                        fails = 0;
                        if (!autoReconnect) break;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("[Optima] Ошибка опроса: " + e.Message);
                    CloseHandle();
                    fails = 0;
                    if (!autoReconnect) break;
                    SleepWhileRunning(2000);
                    continue;
                }

                SleepWhileRunning((int)(pollInterval * 1000));
            }
        }
        catch (Exception e) { Debug.LogError("[Optima] Ошибка потока: " + e); }
        finally
        {
            try { if (_liveStarted && _h != null && !_h.IsInvalid) Transact(LiveStop, out _); } catch { }
            _liveStarted = false;
            CloseHandle();
        }
    }

    void SleepWhileRunning(int ms)
    {
        for (int t = 0; t < ms && _running; t += 50) Thread.Sleep(50);
    }

    bool Open()
    {
        var dev = DeviceList.Local.GetHidDevices(VendorId)
                      .FirstOrDefault(d => d.ProductID == PidOptimaNew || d.ProductID == PidOptima7);
        if (dev == null)
            dev = DeviceList.Local.GetHidDevices(VendorId)
                      .FirstOrDefault(d => (d.GetProductName() ?? "").ToUpperInvariant().Contains("OPTIMA"));
        if (dev == null) { if (verbose) Debug.Log("[Optima] Прибор не найден"); return false; }

        _pid = dev.ProductID;
        _outLen = dev.GetMaxOutputReportLength();
        _inLen = dev.GetMaxInputReportLength();
        var h = CreateFile(dev.DevicePath, 0xC0000000, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
        if (h.IsInvalid)
        {
            Debug.LogWarning("[Optima] Не удалось открыть прибор (ошибка " + Marshal.GetLastWin32Error() + "). Закрыт ли MRU4Win?");
            return false;
        }
        _h = h;
        _connected = true;
        Debug.Log($"[Optima] Подключено: {dev.GetProductName()} PID={_pid:X4} out={_outLen} in={_inLen}");
        return true;
    }

    void CloseHandle()
    {
        _connected = false;
        if (_h != null) { try { _h.Close(); } catch { } _h = null; }
    }

    bool Transact(byte[] cmd, out byte[] resp)
    {
        resp = null;
        var o = new byte[_outLen];
        Array.Copy(cmd, 0, o, 1, Math.Min(30, _outLen - 1));          // [0] = Report ID 0
        if (!HidD_SetOutputReport(_h, o, o.Length)) return false;
        Thread.Sleep(100);                                            // в логе ответ приходил через ~0.1 с
        var i = new byte[_inLen];
        if (!HidD_GetInputReport(_h, i, i.Length)) return false;
        resp = i;
        if (verbose && _dumped.Add(BitConverter.ToString(cmd, 0, 12)))
            Debug.Log("[Optima] RAW " + BitConverter.ToString(cmd, 0, 12) + " -> " + BitConverter.ToString(i).Replace('-', ' '));
        return true;
    }

    // 12 служебных байт, затем float32 LE (k-й float с байта 12 ответа). 0xFE967699 (~-1e38) = "нет данных"
    static float F(byte[] r, int k) => BitConverter.ToSingle(r, 1 + 12 + k * 4);

    bool PollOnce()
    {
        var blocks = new byte[3][];
        for (int n = 0; n < 3; n++)
        {
            if (!Transact(ReadCommands[n], out blocks[n])) return false;
            LogCsv("B8/p" + n, blocks[n]);
        }
        byte[] p0 = blocks[0], p1 = blocks[1], p2 = blocks[2];

        // Ответ короче ожидаемого (другая прошивка/длина отчёта) — не читаем за пределами массива.
        const int minLength = 1 + 12 + 12 * 4;
        if (p0.Length < minLength || p1.Length < minLength || p2.Length < minLength)
        {
            if (verbose) Debug.LogWarning($"[Optima] Короткий ответ прибора ({p0.Length}/{p1.Length}/{p2.Length} байт)");
            return false;
        }

        lock (_lock)
        {
            if (_pid == PidOptimaNew)
            {
                // Найдено по логу: отношения ppm -> мг/м3 совпали точно (CO 1.250, NO 1.339, NO2/NOx 2.053), NOx = NO + NO2;
                // O2 и CO2 сверены с экраном MRU4Win.
                _values["O2_pct"] = F(p0, 0);   _values["CO2_pct"] = F(p0, 1);
                _values["CO_ppm"] = F(p0, 2);   _values["CO_mgm3"] = F(p0, 6);
                _values["NO_ppm"] = F(p0, 8);   _values["NO_mgm3"] = BitConverter.ToSingle(p1, 1 + 4);
                _values["NO2_ppm"] = F(p1, 0);  _values["NO2_mgm3"] = F(p1, 1);
                _values["NOx_ppm"] = F(p1, 2);  _values["NOx_mgm3"] = F(p1, 6);
                _values["P_abs_hPa"] = F(p2, 1);
            }
            else
            {
                // SN317718: сопоставление по коэффициентам пересчёта (O2 и T-air этот прибор не отдавал)
                _values["NO_ppm"] = F(p1, 1);   _values["NO_mgm3"] = BitConverter.ToSingle(p1, 1 + 8);
                _values["NO2_ppm"] = F(p1, 7);  _values["NO2_mgm3"] = F(p2, 3);
                _values["SO2_ppm"] = F(p1, 8);  _values["SO2_mgm3"] = F(p1, 11);
                _values["P_abs_hPa"] = F(p2, 4);
            }
            // значения "нет данных" убираем, чтобы не показывать мусор
            foreach (var k in _values.Where(kv => kv.Value < -1e30f).Select(kv => kv.Key).ToList()) _values.Remove(k);
        }
        Interlocked.Exchange(ref _lastTicks, Stopwatch.GetTimestamp());
        _dirty = true;
        return true;
    }

    void LogCsv(string tag, byte[] r)
    {
        if (_csvPath == null) return;
        var sb = new StringBuilder();
        sb.Append(_clock.Elapsed.TotalSeconds.ToString("0.00", CultureInfo.InvariantCulture)).Append(';').Append(tag);
        for (int j = 0; j < 14; j++)
        {
            float f = BitConverter.ToSingle(r, 1 + 4 + j * 4);
            sb.Append(';').Append(f < -1e30f ? "" : f.ToString("G6", CultureInfo.InvariantCulture));
        }
        File.AppendAllText(_csvPath, sb.AppendLine().ToString());
    }
}