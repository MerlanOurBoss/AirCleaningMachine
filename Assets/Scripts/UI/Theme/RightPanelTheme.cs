using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Тёмная тема правой панели аппаратов. Карточки аппаратов создаёт LoadRightPanel из префабов,
/// общих с другими сценами, поэтому префабы не меняются: карточки перекрашиваются здесь, когда появляются.
/// Выбранная вкладка подсвечивается акцентным цветом. Результаты расчётов («Время адсорбции: 0,21 ч»)
/// раскладываются таблицей: название слева, значение справа (см. PanelValueFormat).
/// </summary>
public class RightPanelTheme : MonoBehaviour
{
    [SerializeField] private LoadRightPanel panel;
    [Tooltip("Колонка вкладок (Vertical Group), в неё LoadRightPanel кладёт карточки аппаратов.")]
    [SerializeField] private Transform tabs;
    [SerializeField] private Image background;
    [SerializeField] private Image topBar;
    [Tooltip("Как часто искать новые элементы (карточки и пункты выпадающих списков создаются в рантайме).")]
    [SerializeField] private float rescanInterval = 0.5f;

    private static readonly Color TabColumn = new Color(0.09f, 0.11f, 0.14f, 1f);
    private static readonly Color Field = new Color(0.075f, 0.094f, 0.12f, 1f);

    private const float ValueFontSize = 20f;
    private const string PairBreak = "<line-height=130%>\n</line-height>"; // между парами в одном тексте (габариты)
    private const float RowGap = 4f;
    private const string ColumnGap = "\u2003\u2003"; // минимальный зазор между названием и значением
    private const int PadX = 12, PadY = 8; // отступы внутри секции

    private readonly HashSet<Graphic> _styled = new HashSet<Graphic>();
    private readonly List<Graphic> _graphics = new List<Graphic>();
    private Button _selected;
    private bool _selectionDirty = true;

    // Тексты с результатами: что в них записал матмодуль и во что мы это превратили.
    private class ValueText
    {
        public TMP_Text Text;
        public string Source, Formatted;
    }
    private readonly List<ValueText> _values = new List<ValueText>();
    private float _nextScan;

    private void Awake()
    {
        if (panel == null) panel = GetComponentInChildren<LoadRightPanel>(true);
        if (tabs == null && panel != null) tabs = panel.transform;

        var opaquePanel = UITheme.Panel;
        opaquePanel.a = 0.97f;
        if (background != null) { background.sprite = null; background.color = opaquePanel; }
        if (topBar != null) { topBar.sprite = null; topBar.color = UITheme.Panel2; }
        var column = tabs != null ? tabs.GetComponent<Image>() : null;
        if (column != null) { column.sprite = null; column.color = TabColumn; }
    }

    private void Update()
    {
        if (tabs == null) return;

        if (Time.unscaledTime >= _nextScan)
        {
            _nextScan = Time.unscaledTime + rescanInterval;
            Scan();
        }

        var selected = panel != null ? panel.GetSelectedButton() : null;
        if (selected != _selected || _selectionDirty)
        {
            _selected = selected;
            _selectionDirty = false;
            foreach (Transform tab in tabs) PaintTab(tab, tab.GetComponent<Button>() == selected);
        }
    }

    private void Scan()
    {
        _styled.RemoveWhere(g => g == null);
        foreach (Transform tab in tabs)
        {
            tab.GetComponentsInChildren(true, _graphics);
            foreach (var graphic in _graphics)
            {
                if (!_styled.Add(graphic)) continue;
                if (graphic.transform == tab) { StyleTab(tab); _selectionDirty = true; }
                else Style(graphic, tab);
            }
        }
    }

    // ================= вкладки =================

    private static void StyleTab(Transform tab)
    {
        var image = tab.GetComponent<Image>();
        image.sprite = UITheme.Rounded;
        image.type = Image.Type.Sliced;

        var button = tab.GetComponent<Button>();
        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = colors.selectedColor = Color.white;
            colors.highlightedColor = new Color(1.18f, 1.18f, 1.18f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
        }
    }

    private static void PaintTab(Transform tab, bool selected)
    {
        var image = tab.GetComponent<Image>();
        if (image != null) image.color = selected ? UITheme.Accent : UITheme.Panel2;
        var letter = TabLetter(tab);
        if (letter != null) letter.color = selected ? Color.white : UITheme.Muted;
    }

    private static TMP_Text TabLetter(Transform tab)
    {
        var overlay = tab.Find("Panel");
        return overlay != null ? overlay.GetComponentInChildren<TMP_Text>(true) : null;
    }

    // ================= содержимое карточки =================

    private void Style(Graphic graphic, Transform tab)
    {
        var dropdown = graphic.GetComponentInParent<SearchableDropDown>(true);
        if (graphic is TMP_Text text) StyleText(text, tab, dropdown);
        else if (graphic is Image image) StyleImage(image, tab, dropdown);
    }

    private void StyleText(TMP_Text text, Transform tab, SearchableDropDown dropdown)
    {
        if (text == TabLetter(tab)) return; // красится вместе с вкладкой

        if (dropdown != null)
        {
            text.color = text.name == "Placeholder" ? UITheme.Muted : UITheme.Text;
            text.enableAutoSizing = false; // у пунктов списка автоподбор давал мелкий шрифт
            text.fontSize = Mathf.Max(text.fontSize, 18f);
            return;
        }

        // Подпись над полем ввода — приглушённая, значения и формулы — светлые.
        if (HasDropdownSibling(text.transform)) { text.color = UITheme.Muted; return; }

        text.color = UITheme.Text;
        // Заголовок карточки лежит прямо во внешнем Details, остальное — результаты расчётов.
        bool isTitle = text.transform.parent != null && text.transform.parent.parent == tab;
        if (!isTitle) _values.Add(new ValueText { Text = text });
    }

    private static bool HasDropdownSibling(Transform t)
    {
        if (t.parent == null) return false;
        foreach (Transform sibling in t.parent)
            if (sibling != t && sibling.GetComponent<SearchableDropDown>() != null) return true;
        return false;
    }

    private static void StyleImage(Image image, Transform tab, SearchableDropDown dropdown)
    {
        if (dropdown != null) { StyleDropdownImage(image); return; }

        var parent = image.transform.parent;
        if (parent == tab)
        {
            // Полупрозрачная подложка буквы и внешний Details — фон даёт сама панель.
            image.color = Color.clear;
            return;
        }

        if (image.GetComponent<Selectable>() != null)
        {
            SetSprite(image, UITheme.Rounded);
            image.color = Field;
            return;
        }

        // Внутренний контейнер Details/Detail — прозрачный, секции внутри него — скруглённые карточки.
        if (parent != null && parent.parent == tab && image.GetComponent<VerticalLayoutGroup>() != null)
        {
            image.color = Color.clear;
            return;
        }

        SetSprite(image, UITheme.Rounded);
        image.color = UITheme.Panel2;
    }

    private static void StyleDropdownImage(Image image)
    {
        switch (image.name)
        {
            case "InputField (TMP)":
                SetSprite(image, UITheme.Rounded);
                image.color = Field;
                var input = image.GetComponent<TMP_InputField>();
                if (input != null)
                {
                    input.transition = Selectable.Transition.None;
                    input.caretColor = UITheme.Text;
                    input.selectionColor = new Color(UITheme.Accent.r, UITheme.Accent.g, UITheme.Accent.b, 0.4f);
                }
                return;
            case "Scroll View":
                SetSprite(image, UITheme.Rounded);
                image.color = UITheme.Line;
                return;
            case "Button:Arrow":
                image.sprite = UITheme.Chevron;
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
                image.color = UITheme.Muted;
                return;
            case "Line":
                image.color = new Color(UITheme.Line.r, UITheme.Line.g, UITheme.Line.b, 0.6f);
                return;
            case "Handle":
                SetSprite(image, UITheme.Rounded);
                image.color = UITheme.Muted;
                return;
        }

        // Пункты списка (и шаблон, из которого они создаются).
        var button = image.GetComponent<Button>();
        if (button != null && image.color.a > 0.5f)
        {
            image.sprite = null;
            image.color = Color.white;
            var colors = button.colors;
            colors.normalColor = colors.selectedColor = UITheme.Panel2;
            colors.highlightedColor = UITheme.Line;
            colors.pressedColor = UITheme.Accent;
            colors.fadeDuration = 0.05f;
            button.colors = colors;
        }
        // Остальное (маска, прозрачный фон полосы прокрутки, блокер) не трогаем.
    }

    // ================= результаты расчётов =================

    // Матмодули пишут текст при пересчёте (в Update) — оформляем после них, в том же кадре.
    private void LateUpdate()
    {
        for (int i = _values.Count - 1; i >= 0; i--)
        {
            var value = _values[i];
            if (value.Text == null) { _values.RemoveAt(i); continue; }
            if (!value.Text.isActiveAndEnabled || value.Text.text == value.Formatted) continue;

            if (value.Text.text == value.Source)
            {
                value.Text.text = value.Formatted; // записали ту же строку — раскладка не меняется
                continue;
            }

            value.Source = value.Text.text;
            value.Formatted = FormatValues(value.Text);
            FitSection(value.Text.transform.parent);
        }
    }

    private static string FormatValues(TMP_Text text)
    {
        var pairs = PanelValueFormat.Parse(text.text);
        if (pairs.Count == 0) return text.text;

        text.enableAutoSizing = false;
        text.fontSize = ValueFontSize;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.alignment = TextAlignmentOptions.TopLeft;
        // В префабах узкие (60 px) строки расширены отрицательным правым отступом — значение справа уехало бы за край.
        text.margin = Vector4.zero;
        // Интервалы добавляются и к строке с нулевым шагом — значение съезжало ниже (или выше) названия.
        text.lineSpacing = 0f;
        text.paragraphSpacing = 0f;

        var rect = text.rectTransform;
        var section = rect.parent as RectTransform;
        float width = section != null ? section.rect.width - 2 * PadX : rect.rect.width;

        string muted = ColorUtility.ToHtmlStringRGB(UITheme.Muted);
        string bright = ColorUtility.ToHtmlStringRGB(UITheme.Text);
        var sb = new StringBuilder();
        foreach (var pair in pairs)
        {
            if (sb.Length > 0) sb.Append(PairBreak);
            string label = $"<color=#{muted}>{pair.Label}</color>";
            if (pair.Value.Length == 0) { sb.Append("<align=left>").Append(label); continue; }

            string value = PanelValueFormat.Value(pair.Value, bright, muted);
            string rest = null;
            if (Width(text, label + ColumnGap + value) > width)
            {
                // Не помещается: значение остаётся в первой строке, а часть названия уходит вниз.
                if (SplitLabel(text, pair.Label, width - Width(text, ColumnGap + value), out string first, out rest))
                    label = $"<color=#{muted}>{first}</color>";
                else
                {
                    // Одно длинное слово — значение под названием.
                    sb.Append("<align=left>").Append(label).Append("\n<align=right>").Append(value);
                    continue;
                }
            }

            // Строка значения с нулевым шагом ложится на строку названия и прижимается вправо.
            sb.Append("<align=left>").Append(label).Append("<line-height=0>\n")
              .Append("<align=right>").Append(value).Append("</line-height>");
            if (rest != null) sb.Append("\n<align=left><color=#").Append(muted).Append('>').Append(rest).Append("</color>");
        }

        string formatted = sb.ToString();
        text.text = formatted;

        // Строку растягиваем на ширину секции, высоту — по тексту (раньше 50 px на любое содержимое).
        if (section != null)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(PadX, -PadY);
        }
        float height = text.GetPreferredValues(formatted, width, 0f).y;
        rect.sizeDelta = new Vector2(width, Mathf.Ceil(height));
        return formatted;
    }

    private static float Width(TMP_Text text, string content) =>
        text.GetPreferredValues(content, float.PositiveInfinity, 0f).x;

    /// <summary>
    /// Делит название по словам на две строки примерно поровну; первая должна уместиться рядом со значением.
    /// «Массовый расход газа» → «Массовый расход» + «газа».
    /// </summary>
    private static bool SplitLabel(TMP_Text text, string label, float firstLineWidth, out string first, out string rest)
    {
        first = rest = null;
        var words = label.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        float best = float.MaxValue;
        for (int i = 1; i < words.Length; i++)
        {
            string head = string.Join(" ", words, 0, i);
            string tail = string.Join(" ", words, i, words.Length - i);
            float headWidth = Width(text, head);
            if (headWidth > firstLineWidth) break; // дальше первая строка только длиннее
            float longest = Mathf.Max(headWidth, Width(text, tail));
            if (longest < best) { best = longest; first = head; rest = tail; }
        }
        return first != null;
    }

    // Высота секции по содержимому: без пустых полос внизу карточки.
    private static void FitSection(Transform sectionTransform)
    {
        if (!(sectionTransform is RectTransform section)) return;

        var layout = section.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.padding = new RectOffset(PadX, PadX, PadY, PadY);
            layout.spacing = RowGap;
        }

        float height = 2 * PadY;
        int rows = 0;
        foreach (Transform child in section)
        {
            if (!child.gameObject.activeSelf) continue;
            var element = child.GetComponent<LayoutElement>();
            if (element != null && element.ignoreLayout) continue;
            height += ((RectTransform)child).rect.height;
            rows++;
        }
        height += RowGap * Mathf.Max(0, rows - 1);

        section.anchorMin = new Vector2(section.anchorMin.x, 1f);
        section.anchorMax = new Vector2(section.anchorMax.x, 1f);
        section.sizeDelta = new Vector2(section.sizeDelta.x, height);
        if (layout != null) LayoutRebuilder.MarkLayoutForRebuild(section);
        if (section.parent is RectTransform container) LayoutRebuilder.MarkLayoutForRebuild(container);
    }

    private static void SetSprite(Image image, Sprite sprite)
    {
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
    }
}
