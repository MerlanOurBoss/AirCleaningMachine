using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Анализ данных цифрового двойника. Раз в checkInterval читает входы и выходы строк параметров
/// всех аппаратов цепочки LoadSmoke, сравнивает их с нормами ProcessLimits, подсвечивает значения
/// в таблицах аппаратов и собирает тревоги с рекомендациями ProcessAdvisor (показывает AlertPanelUI).
///
/// В сцены с LoadSmoke (Simulation, Simulation2, CHP) добавляется сам вместе с панелью тревог.
/// Если поставить ProcessMonitor в сцену вручную, автодобавление пропускается.
/// </summary>
public class ProcessMonitor : MonoBehaviour
{
    public static ProcessMonitor Instance { get; private set; }

    [Tooltip("Нормы. Пусто — Resources/ProcessLimits, а если его нет — значения по умолчанию из кода.")]
    [SerializeField] private ProcessLimits limits;

    [Tooltip("Цепочка аппаратов. Пусто — ищется на сцене.")]
    [SerializeField] private LoadSmoke chain;

    [Tooltip("Пусто — объект с тегом Translator.")]
    [SerializeField] private Translator translator;

    [Header("Подсветка значений в таблицах")]
    [SerializeField] private Color warningColor = new Color(1f, 0.72f, 0.1f);
    [SerializeField] private Color criticalColor = new Color(1f, 0.25f, 0.2f);

    /// <summary>Список тревог изменился: уровень, значение или текст рекомендаций.</summary>
    public event Action OnAlertsChanged;

    public IReadOnlyList<ProcessAlert> Alerts => _alerts;
    public ProcessLimits Limits => limits;
    public Translator.Language Language => translator != null ? translator.currentLanguage : Translator.Language.Russian;

    private readonly List<ProcessAlert> _alerts = new List<ProcessAlert>();
    private readonly List<GeneralManagerBase> _modules = new List<GeneralManagerBase>();
    private readonly Dictionary<(ProcessLimits.Rule, GeneralManagerBase), AlertLevel> _levels =
        new Dictionary<(ProcessLimits.Rule, GeneralManagerBase), AlertLevel>();

    // Подсветка: какой уровень нужен тексту сейчас и какой цвет у него был до подсветки.
    private readonly Dictionary<TextMeshProUGUI, AlertLevel> _wantedHighlight = new Dictionary<TextMeshProUGUI, AlertLevel>();
    private readonly Dictionary<TextMeshProUGUI, Color> _originalColors = new Dictionary<TextMeshProUGUI, Color>();
    private readonly List<TextMeshProUGUI> _restoreBuffer = new List<TextMeshProUGUI>();

    private float _nextCheck;
    private string _signature;

    // ================= автодобавление в сцены =================

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallSceneHook()
    {
        // -= на случай отключённой перезагрузки домена (Enter Play Mode Options): подписка не задвоится.
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        AttachToScene();
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode) => AttachToScene();

    private static void AttachToScene()
    {
        if (FindAnyObjectByType<ProcessMonitor>() != null) return;
        // Мониторинг нужен только там, где есть цепочка аппаратов.
        if (FindAnyObjectByType<LoadSmoke>() == null) return;

        var go = new GameObject("ProcessMonitor (auto)");
        go.AddComponent<ProcessMonitor>();
        go.AddComponent<AlertPanelUI>();
    }

    // ================= жизненный цикл =================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"На сцене больше одного ProcessMonitor ('{Instance.name}' и '{name}') — второй выключен.");
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (limits == null)
            limits = Resources.Load<ProcessLimits>(ProcessLimits.ResourcesPath);

        if (limits == null)
        {
            Debug.LogWarning("ProcessMonitor: Resources/ProcessLimits не найден — используются нормы по умолчанию из кода.");
            limits = ScriptableObject.CreateInstance<ProcessLimits>();
        }

        if (chain == null)
            chain = FindAnyObjectByType<LoadSmoke>();

        if (translator == null)
        {
            var translatorObj = GameObject.FindGameObjectWithTag(Tags.Translator);
            if (translatorObj != null)
                translator = translatorObj.GetComponent<Translator>();
        }

        if (translator != null)
            translator.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDestroy()
    {
        if (translator != null)
            translator.OnLanguageChanged -= HandleLanguageChanged;

        _wantedHighlight.Clear();
        ApplyHighlight();

        if (Instance == this) Instance = null;
    }

    // Тексты рекомендаций собираются на текущем языке — пересобрать сразу.
    private void HandleLanguageChanged(Translator.Language lang) => _nextCheck = 0f;

    private void Update()
    {
        if (limits == null || Time.unscaledTime < _nextCheck) return;
        _nextCheck = Time.unscaledTime + limits.checkInterval;

        Evaluate();
    }

    // ================= проверка =================

    private void Evaluate()
    {
        RefreshModules();
        _alerts.Clear();
        _wantedHighlight.Clear();

        if (_modules.Count > 0)
        {
            var lang = Language;
            foreach (var rule in limits.rules)
            {
                if (rule == null || !rule.enabled || string.IsNullOrWhiteSpace(rule.parameterId)) continue;

                if (rule.stage == ProcessStage.SystemOutlet)
                {
                    Check(rule, _modules[_modules.Count - 1], CheckPoint.Out, lang);
                    continue;
                }

                foreach (var module in _modules)
                    if (StageOf(module) == rule.stage)
                        Check(rule, module, rule.point, lang);
            }
        }

        _alerts.Sort((a, b) => a.Level != b.Level ? b.Level.CompareTo(a.Level) : a.Order.CompareTo(b.Order));
        ApplyHighlight();

        string signature = BuildSignature();
        if (signature == _signature) return;

        _signature = signature;
        OnAlertsChanged?.Invoke();
    }

    private void RefreshModules()
    {
        _modules.Clear();
        if (chain == null) return;

        foreach (var parameterModule in chain.ParameterModules)
            if (parameterModule is GeneralManagerBase module && module != null)
                _modules.Add(module);
    }

    private void Check(ProcessLimits.Rule rule, GeneralManagerBase module, CheckPoint point, Translator.Language lang)
    {
        var row = FindRow(module, rule.parameterId);
        var text = row == null ? null : point == CheckPoint.In ? row.valueTextIn : row.valueTextOut;
        if (!TryReadValue(text, out float value)) return;

        var key = (rule, module);
        _levels.TryGetValue(key, out var previous);
        var level = Classify(rule, value, previous, limits.hysteresis, out bool aboveMax);

        if (level != previous)
        {
            _levels[key] = level;
            Debug.Log($"[ProcessMonitor] {module.name} · {rule.parameterId} ({point}): {previous} → {level}, значение {value}");
        }

        if (level == AlertLevel.Normal) return;

        var alert = new ProcessAlert
        {
            Rule = rule,
            Module = module,
            Stage = rule.stage,
            Point = point,
            Level = level,
            Value = value,
            AboveMax = aboveMax,
            Order = _alerts.Count
        };
        ProcessAdvisor.Advise(alert, _modules, limits, lang);
        _alerts.Add(alert);

        if (!_wantedHighlight.TryGetValue(text, out var shown) || level > shown)
            _wantedHighlight[text] = level;
    }

    /// <summary>
    /// Уже поднятый уровень держится, пока значение не уйдёт за порог с запасом hysteresis, —
    /// иначе при значении на самой границе тревога включается и выключается каждую проверку.
    /// </summary>
    private static AlertLevel Classify(ProcessLimits.Rule rule, float value, AlertLevel previous, float hysteresis,
        out bool aboveMax)
    {
        aboveMax = true;

        if (rule.checkMax)
        {
            if (value > Lowered(rule.criticalMax, previous >= AlertLevel.Critical, hysteresis)) return AlertLevel.Critical;
            if (value > Lowered(rule.warningMax, previous >= AlertLevel.Warning, hysteresis)) return AlertLevel.Warning;
        }

        if (rule.checkMin)
        {
            aboveMax = false;
            if (value < Raised(rule.criticalMin, previous >= AlertLevel.Critical, hysteresis)) return AlertLevel.Critical;
            if (value < Raised(rule.warningMin, previous >= AlertLevel.Warning, hysteresis)) return AlertLevel.Warning;
            aboveMax = true;
        }

        return AlertLevel.Normal;
    }

    private static float Lowered(float threshold, bool active, float hysteresis) =>
        active ? threshold - hysteresis * Mathf.Abs(threshold) : threshold;

    private static float Raised(float threshold, bool active, float hysteresis) =>
        active ? threshold + hysteresis * Mathf.Abs(threshold) : threshold;

    private void ApplyHighlight()
    {
        _restoreBuffer.Clear();
        foreach (var text in _originalColors.Keys)
            if (!_wantedHighlight.ContainsKey(text))
                _restoreBuffer.Add(text);

        foreach (var text in _restoreBuffer)
        {
            if (text != null) text.color = _originalColors[text];
            _originalColors.Remove(text);
        }

        foreach (var pair in _wantedHighlight)
        {
            if (!_originalColors.ContainsKey(pair.Key))
                _originalColors[pair.Key] = pair.Key.color;

            pair.Key.color = pair.Value == AlertLevel.Critical ? criticalColor : warningColor;
        }
    }

    // Панель перестраивается, только если что-то видимое поменялось.
    private string BuildSignature()
    {
        var sb = new StringBuilder();
        sb.Append(Language).Append('#');
        foreach (var alert in _alerts)
        {
            sb.Append(alert.Level).Append('|')
              .Append(_modules.IndexOf(alert.Module)).Append('|')
              .Append(alert.Rule.parameterId).Append('|')
              .Append(alert.Point).Append('|')
              .Append(MonitoringText.Number(alert.Value));
            foreach (var rec in alert.Recommendations)
                sb.Append('|').Append(rec.Text);
            sb.Append(';');
        }
        return sb.ToString();
    }

    // ================= общие помощники (их использует и ProcessAdvisor) =================

    public static ProcessStage? StageOf(GeneralManagerBase module)
    {
        switch (module)
        {
            case GeneralManagerForElectroFilter _: return ProcessStage.Electrofilter;
            case GeneralManagerForEmul _: return ProcessStage.Emulsifier;
            case GeneralManagerForKataz _: return ProcessStage.Catalyst;
            case GeneralManagerForSborCO2 _: return ProcessStage.CO2Collector;
            default: return null;
        }
    }

    /// <summary>Строка параметра аппарата по id (без учёта регистра и пробелов по краям), как в LoadSmoke.</summary>
    public static ParameterRowUI FindRow(GeneralManagerBase module, string id)
    {
        if (!(module is IParameterModule parameterModule) || parameterModule.Rows == null || id == null) return null;

        foreach (var row in parameterModule.Rows)
            if (row != null && string.Equals(row.id?.Trim(), id.Trim(), StringComparison.OrdinalIgnoreCase))
                return row;
        return null;
    }

    /// <summary>Число из поля таблицы: "12.5", "12,5", "12 мг/м³".</summary>
    public static bool TryReadValue(TextMeshProUGUI text, out float value)
    {
        value = 0f;
        if (text == null || string.IsNullOrWhiteSpace(text.text)) return false;

        string s = text.text.Trim();
        int space = s.IndexOf(' ');
        if (space >= 0) s = s.Substring(0, space);

        return float.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
