using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Подписи над установками в 3D: название и цветная точка состояния (норма / внимание / авария).
/// Установки берутся из цепочки LoadSmoke, состояние — из тревог ProcessMonitor по этой установке.
/// </summary>
public class FacilityLabels : MonoBehaviour
{
    [SerializeField] private LoadSmoke chain;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Sprite background;
    [Tooltip("Насколько выше верхней точки установки ставить подпись (доля высоты установки).")]
    [SerializeField] private float heightOffset = 0.08f;

    private class Label
    {
        public RectTransform Rect;
        public Image Border;
        public TextMeshProUGUI Text;
        public Bounds Bounds;
        public float BoundsAt = -1f;
    }

    private readonly Dictionary<GeneralManagerBase, Label> _labels = new Dictionary<GeneralManagerBase, Label>();
    private readonly List<GeneralManagerBase> _gone = new List<GeneralManagerBase>();
    private readonly HashSet<GeneralManagerBase> _present = new HashSet<GeneralManagerBase>();
    private RectTransform _root;

    private void Awake()
    {
        _root = (RectTransform)transform;
        if (chain == null) chain = FindAnyObjectByType<LoadSmoke>();
        if (sceneCamera == null) sceneCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (chain == null || sceneCamera == null) return;

        var lang = ProcessMonitor.Instance != null ? ProcessMonitor.Instance.Language : Translator.Language.Russian;
        _present.Clear();

        foreach (var parameterModule in chain.ParameterModules)
        {
            if (!(parameterModule is GeneralManagerBase module) || module == null || !module.isActiveAndEnabled) continue;
            var stage = ProcessMonitor.StageOf(module);
            if (stage == null) continue;
            _present.Add(module);

            if (!_labels.TryGetValue(module, out var label)) _labels[module] = label = CreateLabel();
            Place(label, module, stage.Value, lang);
        }

        _gone.Clear();
        foreach (var pair in _labels) if (!_present.Contains(pair.Key)) _gone.Add(pair.Key);
        foreach (var module in _gone)
        {
            if (_labels[module].Rect != null) Destroy(_labels[module].Rect.gameObject);
            _labels.Remove(module);
        }
    }

    private void Place(Label label, GeneralManagerBase module, ProcessStage stage, Translator.Language lang)
    {
        // Границы установки пересчитываем раз в секунду: установку могут двигать в конструкторе.
        if (Time.time - label.BoundsAt > 1f)
        {
            label.Bounds = ComputeBounds(module.transform);
            label.BoundsAt = Time.time;
        }

        Vector3 top = label.Bounds.center + Vector3.up * (label.Bounds.extents.y + label.Bounds.size.y * heightOffset);
        // Координаты вьюпорта (0..1) не зависят от размера экрана; канвас занимает ровно область камеры.
        Vector3 viewport = sceneCamera.WorldToViewportPoint(top);
        bool visible = viewport.z > 0f && viewport.x >= 0f && viewport.x <= 1f && viewport.y >= 0f && viewport.y <= 1f;
        label.Rect.gameObject.SetActive(visible);
        if (!visible) return;

        var rect = _root.rect;
        label.Rect.anchoredPosition = new Vector2((viewport.x - 0.5f) * rect.width, (viewport.y - 0.5f) * rect.height);

        // До «Симулировать» состояние неизвестно — серая точка, а не зелёная «норма».
        bool monitoring = ProcessMonitor.Instance != null && ProcessMonitor.Instance.IsActive;
        var level = WorstLevel(module);
        var dotColor = monitoring ? UITheme.ForLevel(level) : UITheme.Muted;
        string dot = $"<color=#{ColorUtility.ToHtmlStringRGB(dotColor)}>●</color>";
        label.Text.text = $"{dot} {MonitoringText.StageName(stage, lang)}";
        label.Border.color = !monitoring || level == AlertLevel.Normal ? UITheme.Line : UITheme.ForLevel(level);
    }

    private static AlertLevel WorstLevel(GeneralManagerBase module)
    {
        var worst = AlertLevel.Normal;
        if (ProcessMonitor.Instance == null) return worst;
        foreach (var alert in ProcessMonitor.Instance.Alerts)
            if (alert.Module == module && alert.Stage != ProcessStage.SystemOutlet && alert.Level > worst)
                worst = alert.Level;
        return worst;
    }

    private static Bounds ComputeBounds(Transform root)
    {
        bool has = false;
        var bounds = new Bounds(root.position, Vector3.zero);
        foreach (var r in root.GetComponentsInChildren<MeshRenderer>())
        {
            if (!r.enabled) continue;
            if (!has) { bounds = r.bounds; has = true; }
            else bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    private Label CreateLabel()
    {
        var go = new GameObject("Facility Label", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        go.AddComponent<ExcludeFromTranslation>();
        var rect = (RectTransform)go.transform;
        rect.SetParent(_root, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0f);

        var image = go.GetComponent<Image>();
        image.sprite = background != null ? background : UITheme.Rounded;
        image.type = Image.Type.Sliced;
        image.color = UITheme.Panel;
        image.raycastTarget = false;

        var layout = go.GetComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 4, 4);
        layout.childControlWidth = layout.childControlHeight = true;
        var fitter = go.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var borderGo = new GameObject("Border", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        borderGo.GetComponent<LayoutElement>().ignoreLayout = true;
        var borderRect = (RectTransform)borderGo.transform;
        borderRect.SetParent(rect, false);
        borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = borderRect.offsetMax = Vector2.zero;
        var border = borderGo.GetComponent<Image>();
        border.sprite = UITheme.Outline;
        border.type = Image.Type.Sliced;
        border.raycastTarget = false;

        var textGo = new GameObject("Text", typeof(RectTransform));
        textGo.transform.SetParent(rect, false);
        var text = textGo.AddComponent<TextMeshProUGUI>();
        if (font != null) text.font = font;
        text.fontSize = 16f;
        text.color = UITheme.Text;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.NoWrap;

        return new Label { Rect = rect, Border = border, Text = text };
    }
}
