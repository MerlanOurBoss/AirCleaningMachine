using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseTable : MonoBehaviour
{
    [SerializeField] private Slider[] sliders;
    [SerializeField] private TextMeshProUGUI[] texts;

    [Header("Газоанализатор")]
    [Tooltip("Брать значения слайдеров с прибора. Если выключено или нет связи, работают ручные слайдеры, как раньше.")]
    [SerializeField] private bool useAnalyzer = true;

    [Tooltip("Компонент OptimaHidPoller. Если пусто, берётся OptimaHidPoller.Instance.")]
    [SerializeField] private OptimaHidPoller poller;

    [Tooltip("Ключ канала для каждого слайдера (тот же порядок, что в Sliders). Пусто = слайдер ручной.\n" +
             "Ключи: O2_pct, CO2_pct, CO_ppm, NO_ppm, NO2_ppm, NOx_ppm, SO2_ppm (старый прибор), " +
             "CO_mgm3, NO_mgm3, NO2_mgm3, NOx_mgm3, SO2_mgm3, P_abs_hPa")]
    [SerializeField] private string[] analyzerKeys;

    [Tooltip("Запускать захват при включении этого объекта и останавливать при выключении (начало/конец симуляции).")]
    [SerializeField] private bool controlCapture = true;

    [Tooltip("Пока идут данные с прибора, блокировать ручное перетаскивание слайдеров.")]
    [SerializeField] private bool lockSlidersWhenLive = true;

    [Tooltip("Через сколько секунд без данных вернуться к ручным значениям.")]
    [SerializeField] private float staleAfter = 3f;

    private bool _locked;
    private OptimaHidPoller _acquiredPoller; // у какого поллера мы «взяли» захват

    private OptimaHidPoller Poller => poller != null ? poller : OptimaHidPoller.Instance;

    private void OnEnable()
    {
        if (!useAnalyzer || !controlCapture) return;
        var p = Poller;
        if (p != null)
        {
            // Счётчик пользователей: захват остановится, только когда выключится последняя таблица.
            p.AcquireCapture();
            _acquiredPoller = p;
        }
        else Debug.LogWarning("[BaseTable] OptimaHidPoller не найден: добавьте его в сцену или назначьте в поле Poller");
    }

    private void OnDisable()
    {
        if (_acquiredPoller != null)
        {
            _acquiredPoller.ReleaseCapture();
            _acquiredPoller = null;
        }
        SetLocked(false);
    }

    private void Update()
    {
        var p = Poller;
        bool live = useAnalyzer && p != null && p.IsRunning && p.IsConnected && p.SecondsSinceData <= staleAfter;
        SetLocked(live && lockSlidersWhenLive);

        int count = Mathf.Min(texts.Length, sliders.Length);
        if (_shownValue == null || _shownValue.Length != count)
        {
            _shownValue = new float[count];
            _shownLive = new bool[count];
            _shownText = new string[count];
        }

        for (int i = 0; i < count; i++)
        {
            if (sliders[i] == null || texts[i] == null) continue;

            float v;
            bool fromAnalyzer = false;
            if (live && HasKey(i) && p.TryGet(analyzerKeys[i], out v))
            {
                fromAnalyzer = true;
                sliders[i].value = v;                              // слайдер ограничит значение своими Min/Max
            }
            else
            {
                v = sliders[i].value;
            }

            // Строку пересобираем только при изменении значения, а не каждый кадр (ToString каждый кадр создаёт мусор).
            if (_shownText[i] != null && _shownValue[i] == v && _shownLive[i] == fromAnalyzer && texts[i].text == _shownText[i])
                continue;

            _shownValue[i] = v;
            _shownLive[i] = fromAnalyzer;
            _shownText[i] = fromAnalyzer ? v.ToString(Format(analyzerKeys[i])) : v.ToString();   // как было раньше
            texts[i].text = _shownText[i];
        }
    }

    private float[] _shownValue;
    private bool[] _shownLive;
    private string[] _shownText;

    private bool HasKey(int i) => analyzerKeys != null && i < analyzerKeys.Length && !string.IsNullOrEmpty(analyzerKeys[i]);

    private void SetLocked(bool locked)
    {
        if (_locked == locked) return;
        _locked = locked;
        for (int i = 0; i < sliders.Length; i++)
            if (HasKey(i) && sliders[i] != null) sliders[i].interactable = !locked;
    }

    private static string Format(string key)
    {
        if (key.EndsWith("_pct")) return "0.00";
        if (key.EndsWith("_ppm")) return "0";
        return "0.0";
    }
}