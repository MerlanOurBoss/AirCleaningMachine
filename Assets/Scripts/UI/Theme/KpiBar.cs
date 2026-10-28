using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Карточки ключевых показателей внизу 3D-вида: эффективность очистки, CO и твёрдые частицы на выходе.
/// Значения читаются из таблиц аппаратов цепочки LoadSmoke (как в ProcessMonitor), цвет — по нормам ProcessLimits.
/// У каждой карточки — мини-график последних значений. До «Симулировать» карточки пустые (LoadSmoke.MonitoringActive).
/// </summary>
public class KpiBar : MonoBehaviour
{
    [SerializeField] private LoadSmoke chain;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Sprite background;
    [SerializeField] private float sampleInterval = 2f;
    [SerializeField] private int historyLength = 16;

    private static readonly string[] Pollutants = { "Твердые частицы", "CO", "NO", "NO2", "SO2", "H2S" };

    private class Card
    {
        public TextMeshProUGUI Title, Value;
        public Image[] Bars;
        public readonly Queue<float> History = new Queue<float>();
    }

    private Card _efficiency, _co, _dust;
    private float _next;
    private bool _wasActive;
    private readonly List<GeneralManagerBase> _modules = new List<GeneralManagerBase>();

    private void Awake()
    {
        if (chain == null) chain = FindAnyObjectByType<LoadSmoke>();
        var layout = gameObject.GetComponent<HorizontalLayoutGroup>();
        if (layout == null) layout = gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = layout.childForceExpandHeight = true;

        _efficiency = CreateCard();
        _co = CreateCard();
        _dust = CreateCard();
    }

    private void Update()
    {
        // После перекомпиляции во время Play поля карточек обнуляются, а Awake не вызывается повторно.
        if (_efficiency == null) return;

        bool active = chain == null || chain.MonitoringActive;
        if (active != _wasActive) { _wasActive = active; _next = 0f; } // запуск/остановка — обновить сразу
        if (Time.time < _next) return;
        _next = Time.time + sampleInterval;
        Sample();
    }

    private void Sample()
    {
        var lang = ProcessMonitor.Instance != null ? ProcessMonitor.Instance.Language : Translator.Language.Russian;
        _efficiency.Title.text = MonitoringText.Pick(lang, "Эффективность очистки", "Тазарту тиімділігі", "Cleaning efficiency");
        _co.Title.text = MonitoringText.Pick(lang, "CO на выходе", "Шығыстағы CO", "CO at outlet");
        _dust.Title.text = MonitoringText.Pick(lang, "Твёрдые частицы на выходе", "Шығыстағы қатты бөлшектер", "Particulates at outlet");

        if (chain != null && !chain.MonitoringActive)
        {
            foreach (var card in new[] { _efficiency, _co, _dust })
            {
                card.History.Clear();
                SetCard(card, "—", float.NaN, AlertLevel.Normal);
            }
            return;
        }

        _modules.Clear();
        if (chain != null)
            foreach (var m in chain.ParameterModules)
                if (m is GeneralManagerBase module && module != null) _modules.Add(module);

        if (_modules.Count == 0)
        {
            foreach (var card in new[] { _efficiency, _co, _dust }) SetCard(card, "—", float.NaN, AlertLevel.Normal);
            return;
        }

        var first = _modules[0];
        var last = _modules[_modules.Count - 1];

        float sum = 0f; int n = 0;
        foreach (var id in Pollutants)
        {
            if (Read(first, id, true, out float inlet) && Read(last, id, false, out float outlet) && inlet > 0.0001f)
            {
                sum += Mathf.Clamp01(1f - outlet / inlet);
                n++;
            }
        }
        if (n > 0) { float eff = sum / n * 100f; SetCard(_efficiency, $"{eff:0} %", eff, eff >= 80f ? AlertLevel.Normal : eff >= 50f ? AlertLevel.Warning : AlertLevel.Critical); }
        else SetCard(_efficiency, "—", float.NaN, AlertLevel.Normal);

        SetOutlet(_co, last, "CO");
        SetOutlet(_dust, last, "Твердые частицы");
    }

    private void SetOutlet(Card card, GeneralManagerBase last, string id)
    {
        if (!Read(last, id, false, out float value)) { SetCard(card, "—", float.NaN, AlertLevel.Normal); return; }
        var rule = FindOutletRule(id);
        var level = AlertLevel.Normal;
        if (rule != null && rule.checkMax)
            level = value >= rule.criticalMax ? AlertLevel.Critical : value >= rule.warningMax ? AlertLevel.Warning : AlertLevel.Normal;
        string unit = rule != null && !string.IsNullOrWhiteSpace(rule.unit) ? " " + rule.unit.Trim() : string.Empty;
        SetCard(card, MonitoringText.Number(value) + unit, value, level);
    }

    private static ProcessLimits.Rule FindOutletRule(string id)
    {
        var limits = ProcessMonitor.Instance != null ? ProcessMonitor.Instance.Limits : null;
        if (limits == null) return null;
        foreach (var rule in limits.rules)
            if (rule != null && rule.enabled && rule.stage == ProcessStage.SystemOutlet && rule.parameterId == id) return rule;
        return null;
    }

    private static bool Read(GeneralManagerBase module, string id, bool inlet, out float value)
    {
        value = 0f;
        var row = ProcessMonitor.FindRow(module, id);
        return row != null && ProcessMonitor.TryReadValue(inlet ? row.valueTextIn : row.valueTextOut, out value);
    }

    private void SetCard(Card card, string text, float sample, AlertLevel level)
    {
        card.Value.text = text;
        card.Value.color = level == AlertLevel.Normal ? UITheme.Text : UITheme.TextForLevel(level);

        if (!float.IsNaN(sample))
        {
            card.History.Enqueue(sample);
            while (card.History.Count > historyLength) card.History.Dequeue();
        }

        float max = 0.0001f;
        foreach (var v in card.History) max = Mathf.Max(max, Mathf.Abs(v));
        var values = card.History.ToArray();
        for (int i = 0; i < card.Bars.Length; i++)
        {
            int idx = values.Length - card.Bars.Length + i;
            float h = idx >= 0 ? Mathf.Abs(values[idx]) / max : 0f;
            var rt = card.Bars[i].rectTransform;
            rt.anchorMax = new Vector2(rt.anchorMax.x, Mathf.Max(0.08f, h));
            card.Bars[i].color = UITheme.ForLevel(level) * new Color(1f, 1f, 1f, idx >= 0 ? 0.9f : 0.25f);
        }
    }

    private Card CreateCard()
    {
        var go = new GameObject("KPI Card", typeof(RectTransform), typeof(Image));
        go.AddComponent<ExcludeFromTranslation>();
        go.transform.SetParent(transform, false);
        var image = go.GetComponent<Image>();
        image.sprite = background != null ? background : UITheme.Rounded;
        image.type = Image.Type.Sliced;
        image.color = UITheme.Panel;
        image.raycastTarget = false;

        var card = new Card
        {
            Title = CreateText(go.transform, 16f, UITheme.Muted, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(14f, -30f), new Vector2(-14f, -8f)),
            Value = CreateText(go.transform, 28f, UITheme.Text, new Vector2(0f, 0f), new Vector2(0.62f, 1f), new Vector2(14f, 8f), new Vector2(0f, -30f)),
            Bars = new Image[historyLength]
        };

        var chart = new GameObject("Spark", typeof(RectTransform)).GetComponent<RectTransform>();
        chart.SetParent(go.transform, false);
        chart.anchorMin = new Vector2(0.62f, 0f); chart.anchorMax = new Vector2(1f, 1f);
        chart.offsetMin = new Vector2(6f, 12f); chart.offsetMax = new Vector2(-14f, -34f);
        for (int i = 0; i < historyLength; i++)
        {
            var bar = new GameObject("Bar", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            bar.transform.SetParent(chart, false);
            bar.raycastTarget = false;
            var rt = bar.rectTransform;
            float w = 1f / historyLength;
            rt.anchorMin = new Vector2(i * w, 0f);
            rt.anchorMax = new Vector2((i + 1) * w, 0.08f);
            rt.offsetMin = new Vector2(1f, 0f); rt.offsetMax = new Vector2(-1f, 0f);
            card.Bars[i] = bar;
        }
        return card;
    }

    private TextMeshProUGUI CreateText(Transform parent, float size, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
        var text = go.AddComponent<TextMeshProUGUI>();
        if (font != null) text.font = font;
        text.fontSize = size;
        text.color = color;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        return text;
    }
}
