using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Панель тревог ProcessMonitor: индикатор состояния вверху экрана и раскрывающийся по нажатию список
/// тревог с рекомендациями и кнопками «Применить». Интерфейс строится кодом на собственном Canvas,
/// поэтому панель работает в любой сцене без ручной расстановки.
/// </summary>
public class AlertPanelUI : MonoBehaviour
{
    [Tooltip("Пусто — ProcessMonitor на этом объекте или ProcessMonitor.Instance.")]
    [SerializeField] private ProcessMonitor monitor;

    [Tooltip("Шрифт с кириллицей. Пусто — шрифт Translator (newFont) или шрифт TMP по умолчанию.")]
    [SerializeField] private TMP_FontAsset font;

    [Tooltip("Раскрывать список, когда появляется авария.")]
    [SerializeField] private bool openOnCritical = true;

    [SerializeField] private int sortingOrder = 500;

    [Header("Положение")]
    [Tooltip("Камера 3D-вида: индикатор и список ставятся по центру сверху её области на экране. Пусто — Camera.main.")]
    [SerializeField] private Camera viewCamera;

    [Tooltip("Область 3D-вида в долях экрана, пока камера не найдена или выключена (стена камер, раскрытие аппарата). " +
             "По умолчанию — rect Main Camera в сценах симуляции.")]
    [SerializeField] private Rect fallbackArea = new Rect(0.22f, 0.051f, 0.752f, 0.883f);

    [Header("Размеры (эталон 1920×1080)")]
    [SerializeField] private float topOffset = 10f;
    [SerializeField] private float badgeWidth = 640f;
    [SerializeField] private float panelWidth = 700f;
    [SerializeField] private float panelMaxHeight = 560f;

    [Tooltip("Сколько секунд кнопка показывает «Применено»: новые значения доходят по цепочке с задержкой " +
             "(updateInterval модулей и LoadSmoke).")]
    [SerializeField] private float appliedHoldSeconds = 15f;

    private static readonly Color ButtonColor = UITheme.Accent;
    private static readonly Color ButtonDoneColor = UITheme.Line;

    private const float HeaderHeight = 44f;
    private const float Padding = 10f;
    private const float BadgeHeight = 44f;

    // Верх списка — под индикатором.
    private float PanelTop => topOffset + BadgeHeight + 8f;

    private class RecommendationRow
    {
        public GameObject Root;
        public TextMeshProUGUI Text;
        public Button Button;
        public Image ButtonImage;
        public TextMeshProUGUI ButtonLabel;
    }

    private class AlertEntry
    {
        public GameObject Root;
        public Image Background;
        public Image Stripe;
        public TextMeshProUGUI Title;
        public readonly List<RecommendationRow> Rows = new List<RecommendationRow>();
    }

    private RectTransform _area;        // область 3D-вида, внутри неё — индикатор и список
    private Rect _appliedArea;
    private RectTransform _panel;
    private RectTransform _content;
    private TextMeshProUGUI _header;
    private Image _badge;
    private Image _badgeBorder;
    private TextMeshProUGUI _badgeText;
    private readonly List<AlertEntry> _entries = new List<AlertEntry>();

    // Когда нажали «Применить» (по тексту рекомендации) — чтобы кнопку не жали повторно, пока идёт пересчёт.
    private readonly Dictionary<string, float> _appliedAt = new Dictionary<string, float>();
    private readonly List<string> _expired = new List<string>();

    private AlertLevel _shownWorst;
    private bool _open;
    private bool _fitPending;
    private float _nextExpiryCheck;

    private void Start()
    {
        if (monitor == null) monitor = GetComponent<ProcessMonitor>();
        if (monitor == null) monitor = ProcessMonitor.Instance;
        if (monitor == null)
        {
            Debug.LogWarning("AlertPanelUI: ProcessMonitor не найден — панель тревог выключена.");
            enabled = false;
            return;
        }

        if (font == null)
        {
            var translator = FindAnyObjectByType<Translator>();
            if (translator != null) font = translator.newFont;
        }
        if (font == null) font = TMP_Settings.defaultFontAsset;

        if (FindAnyObjectByType<EventSystem>() == null)
            Debug.LogWarning("AlertPanelUI: на сцене нет EventSystem — кнопки панели тревог не будут нажиматься.");

        Build();
        monitor.OnAlertsChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (monitor != null) monitor.OnAlertsChanged -= Refresh;
    }

    private void Update()
    {
        if (_appliedAt.Count == 0 || Time.unscaledTime < _nextExpiryCheck) return;
        _nextExpiryCheck = Time.unscaledTime + 1f;

        _expired.Clear();
        foreach (var pair in _appliedAt)
            if (Time.unscaledTime - pair.Value >= appliedHoldSeconds)
                _expired.Add(pair.Key);

        if (_expired.Count == 0) return;
        foreach (var key in _expired) _appliedAt.Remove(key);
        Refresh();
    }

    // TMP досчитывает размеры текста к концу кадра — высоту панели подгоняем после этого.
    private void LateUpdate()
    {
        TrackViewArea();

        if (!_fitPending) return;
        _fitPending = false;
        FitPanelHeight();
    }

    // Rect камеры уже учитывает AspectRatioEnforcer (вписывание в 16:9). Пока камера выключена,
    // панель остаётся на прежнем месте, а не прыгает.
    private void TrackViewArea()
    {
        if (_area == null) return;

        var cam = viewCamera != null ? viewCamera : Camera.main;
        if (cam == null || !cam.isActiveAndEnabled) return;

        ApplyArea(cam.rect);
    }

    private void ApplyArea(Rect area)
    {
        if (area == _appliedArea) return;
        _appliedArea = area;

        _area.anchorMin = area.min;
        _area.anchorMax = area.max;
        _area.offsetMin = _area.offsetMax = Vector2.zero;
        _fitPending = true;
    }

    // ================= содержимое =================

    private void Refresh()
    {
        if (monitor == null || _panel == null) return;

        // До «Симулировать» (и после «Остановить») ни плашки, ни списка.
        bool active = monitor.IsActive;
        _badge.gameObject.SetActive(active);
        if (!active)
        {
            _open = false;
            _shownWorst = AlertLevel.Normal;
            _panel.gameObject.SetActive(false);
            return;
        }

        var lang = monitor.Language;
        var alerts = monitor.Alerts;

        var worst = AlertLevel.Normal;
        foreach (var alert in alerts)
            if (alert.Level > worst) worst = alert.Level;

        // Тёмная плашка с рамкой цвета уровня; при тревоге — суть самой важной из них.
        _badge.color = UITheme.Panel;
        _badgeBorder.color = UITheme.ForLevel(worst);
        _badgeText.color = UITheme.TextForLevel(worst);
        string badge = MonitoringText.Badge(worst, alerts.Count, lang);
        if (alerts.Count > 0)
        {
            var top = alerts[0];
            string unit = string.IsNullOrWhiteSpace(top.Rule.unit) ? string.Empty : " " + top.Rule.unit.Trim();
            badge += $"  ·  {MonitoringText.StageName(top.Stage, lang)}: {MonitoringText.ParameterName(top.Rule.parameterId, lang)} {MonitoringText.Number(top.Value)}{unit}";
        }
        _badgeText.text = badge;
        _header.text = MonitoringText.Pick(lang, "Тревоги и рекомендации", "Дабылдар мен ұсыныстар", "Alerts and recommendations");

        if (openOnCritical && worst == AlertLevel.Critical && _shownWorst != AlertLevel.Critical)
            _open = true;
        _shownWorst = worst;

        // Без тревог — одна строка «всё в норме».
        int count = Mathf.Max(alerts.Count, 1);
        while (_entries.Count < count) _entries.Add(CreateEntry());

        for (int i = 0; i < _entries.Count; i++)
            _entries[i].Root.SetActive(i < count);

        if (alerts.Count == 0) FillNormal(_entries[0], lang);
        else
            for (int i = 0; i < alerts.Count; i++)
                Fill(_entries[i], alerts[i], lang);

        _panel.gameObject.SetActive(_open);
        if (_open)
        {
            FitPanelHeight();
            _fitPending = true;
        }
    }

    private void Fill(AlertEntry entry, ProcessAlert alert, Translator.Language lang)
    {
        // Тёмная карточка, уровень тревоги — полосой слева и цветом заголовка.
        entry.Background.color = UITheme.Panel2;
        entry.Stripe.color = UITheme.ForLevel(alert.Level);
        entry.Title.color = UITheme.TextForLevel(alert.Level);
        entry.Title.text = MonitoringText.AlertTitle(alert, lang);

        var recs = alert.Recommendations;
        while (entry.Rows.Count < recs.Count) entry.Rows.Add(CreateRow(entry));

        for (int i = 0; i < entry.Rows.Count; i++)
        {
            var row = entry.Rows[i];
            row.Root.SetActive(i < recs.Count);
            if (i >= recs.Count) continue;

            var rec = recs[i];
            row.Text.text = "• " + rec.Text;
            row.Button.gameObject.SetActive(rec.CanApply);
            if (!rec.CanApply) continue;

            bool applied = _appliedAt.ContainsKey(rec.Text);
            row.Button.interactable = !applied;
            row.ButtonImage.color = applied ? ButtonDoneColor : ButtonColor;
            row.ButtonLabel.text = applied
                ? MonitoringText.Pick(lang, "Применено", "Қолданылды", "Applied")
                : MonitoringText.Pick(lang, "Применить", "Қолдану", "Apply");

            row.Button.onClick.RemoveAllListeners();
            row.Button.onClick.AddListener(() => ApplyRecommendation(rec));
        }
    }

    private void FillNormal(AlertEntry entry, Translator.Language lang)
    {
        entry.Background.color = UITheme.Panel2;
        entry.Stripe.color = UITheme.Accent;
        entry.Title.color = UITheme.TextForLevel(AlertLevel.Normal);
        entry.Title.text = MonitoringText.Pick(lang,
            "Все параметры в норме.", "Барлық параметрлер қалыпты.", "All parameters are within limits.");

        foreach (var row in entry.Rows) row.Root.SetActive(false);
    }

    private void ApplyRecommendation(Recommendation rec)
    {
        rec.Apply();
        _appliedAt[rec.Text] = Time.unscaledTime;
        Debug.Log($"[AlertPanelUI] Применена рекомендация: {rec.Text}");
        Refresh();
    }

    private void Toggle()
    {
        _open = !_open;
        Refresh();
    }

    private void FitPanelHeight()
    {
        // Панель не выходит за область 3D-вида.
        float width = Mathf.Min(panelWidth, _area.rect.width - 2f * Padding);
        float maxHeight = Mathf.Min(panelMaxHeight, _area.rect.height - PanelTop - Padding);

        _panel.sizeDelta = new Vector2(width, _panel.sizeDelta.y);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        float height = Mathf.Min(_content.rect.height + HeaderHeight + Padding, maxHeight);
        _panel.sizeDelta = new Vector2(width, Mathf.Max(height, HeaderHeight));
    }

    // ================= построение интерфейса =================

    private void Build()
    {
        var canvasRt = CreateRect("AlertPanelCanvas", transform);
        var canvasGo = canvasRt.gameObject;
        canvasGo.AddComponent<ExcludeFromTranslation>();

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        _area = CreateRect("ViewArea", canvasRt);
        ApplyArea(fallbackArea);
        TrackViewArea();

        // Индикатор
        var badgeRt = CreateRect("Badge", _area);
        PlaceTopCenter(badgeRt, -topOffset, new Vector2(badgeWidth, BadgeHeight));
        _badge = badgeRt.gameObject.AddComponent<Image>();
        _badge.sprite = UITheme.Rounded;
        _badge.type = Image.Type.Sliced;
        var badgeButton = badgeRt.gameObject.AddComponent<Button>();
        badgeButton.targetGraphic = _badge;
        badgeButton.onClick.AddListener(Toggle);
        var borderRt = CreateRect("Border", badgeRt);
        Stretch(borderRt, 0f, 0f);
        _badgeBorder = borderRt.gameObject.AddComponent<Image>();
        _badgeBorder.sprite = UITheme.Outline;
        _badgeBorder.type = Image.Type.Sliced;
        _badgeBorder.raycastTarget = false;
        _badgeText = CreateText("Label", badgeRt, 18f, FontStyles.Normal, TextAlignmentOptions.Center);
        _badgeText.enableAutoSizing = true;
        _badgeText.fontSizeMin = 13f;
        _badgeText.fontSizeMax = 18f;
        Stretch(_badgeText.rectTransform, 14f, 6f);

        // Панель со списком
        _panel = CreateRect("Panel", _area);
        PlaceTopCenter(_panel, -PanelTop, new Vector2(panelWidth, panelMaxHeight));
        var panelImage = _panel.gameObject.AddComponent<Image>();
        panelImage.sprite = UITheme.Rounded;
        panelImage.type = Image.Type.Sliced;
        panelImage.color = UITheme.Panel;

        _header = CreateText("Header", _panel, 20f, FontStyles.Bold, TextAlignmentOptions.Left);
        var headerRt = _header.rectTransform;
        headerRt.anchorMin = new Vector2(0f, 1f);
        headerRt.anchorMax = new Vector2(1f, 1f);
        headerRt.pivot = new Vector2(0.5f, 1f);
        headerRt.anchoredPosition = new Vector2(0f, -8f);
        headerRt.sizeDelta = new Vector2(-2f * (Padding + 4f), 30f);

        var viewport = CreateRect("Viewport", _panel);
        Stretch(viewport, Padding, Padding);
        viewport.offsetMax = new Vector2(-Padding, -HeaderHeight);
        viewport.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0f); // ловит прокрутку
        viewport.gameObject.AddComponent<RectMask2D>();

        _content = CreateRect("Content", viewport);
        _content.anchorMin = new Vector2(0f, 1f);
        _content.anchorMax = new Vector2(1f, 1f);
        _content.pivot = new Vector2(0.5f, 1f);
        _content.anchoredPosition = Vector2.zero;
        _content.sizeDelta = Vector2.zero;

        var list = _content.gameObject.AddComponent<VerticalLayoutGroup>();
        list.spacing = 8f;
        list.childControlWidth = true;
        list.childControlHeight = true;
        list.childForceExpandWidth = true;
        list.childForceExpandHeight = false;
        _content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = _panel.gameObject.AddComponent<ScrollRect>();
        scroll.content = _content;
        scroll.viewport = viewport;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 30f;

        _panel.gameObject.SetActive(false);
    }

    private AlertEntry CreateEntry()
    {
        var rt = CreateRect("Alert", _content);
        var entry = new AlertEntry { Root = rt.gameObject };
        entry.Background = rt.gameObject.AddComponent<Image>();
        entry.Background.sprite = UITheme.Rounded;
        entry.Background.type = Image.Type.Sliced;

        var stripeRt = CreateRect("Stripe", rt);
        stripeRt.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        stripeRt.anchorMin = new Vector2(0f, 0f);
        stripeRt.anchorMax = new Vector2(0f, 1f);
        stripeRt.pivot = new Vector2(0f, 0.5f);
        stripeRt.offsetMin = new Vector2(0f, 6f);
        stripeRt.offsetMax = new Vector2(4f, -6f);
        entry.Stripe = stripeRt.gameObject.AddComponent<Image>();
        entry.Stripe.raycastTarget = false;

        var layout = rt.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 12, 8, 10);
        layout.spacing = 6f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        entry.Title = CreateText("Title", rt, 18f, FontStyles.Bold, TextAlignmentOptions.Left);
        return entry;
    }

    private RecommendationRow CreateRow(AlertEntry entry)
    {
        var rt = CreateRect("Recommendation", entry.Root.transform);
        var layout = rt.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        var row = new RecommendationRow { Root = rt.gameObject };

        row.Text = CreateText("Text", rt, 16f, FontStyles.Normal, TextAlignmentOptions.Left);
        row.Text.color = UITheme.Muted;
        // preferredWidth = 1 — чтобы длинный текст не выталкивал кнопку, а переносился в оставшейся ширине.
        var textLayout = row.Text.gameObject.AddComponent<LayoutElement>();
        textLayout.preferredWidth = 1f;
        textLayout.flexibleWidth = 1f;

        var buttonRt = CreateRect("Apply", rt);
        var buttonLayout = buttonRt.gameObject.AddComponent<LayoutElement>();
        buttonLayout.minWidth = buttonLayout.preferredWidth = 140f;
        buttonLayout.minHeight = buttonLayout.preferredHeight = 34f;
        row.ButtonImage = buttonRt.gameObject.AddComponent<Image>();
        row.ButtonImage.sprite = UITheme.Rounded;
        row.ButtonImage.type = Image.Type.Sliced;
        row.Button = buttonRt.gameObject.AddComponent<Button>();
        row.Button.targetGraphic = row.ButtonImage;
        row.ButtonLabel = CreateText("Label", buttonRt, 16f, FontStyles.Bold, TextAlignmentOptions.Center);
        Stretch(row.ButtonLabel.rectTransform, 4f, 2f);

        return row;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, float size, FontStyles style, TextAlignmentOptions alignment)
    {
        var text = CreateRect(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
        if (font != null) text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform)) { layer = LayerMask.NameToLayer("UI") };
        var rt = (RectTransform)go.transform;
        rt.SetParent(parent, false);
        return rt;
    }

    private static void PlaceTopCenter(RectTransform rt, float y, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, y);
        rt.sizeDelta = size;
    }

    private static void Stretch(RectTransform rt, float horizontal, float vertical)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(horizontal, vertical);
        rt.offsetMax = new Vector2(-horizontal, -vertical);
    }
}
