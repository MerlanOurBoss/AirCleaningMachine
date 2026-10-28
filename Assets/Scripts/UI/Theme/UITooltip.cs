using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Подсказка при наведении на иконку: появляется слева от неё, на языке интерфейса.</summary>
public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string textRu;
    [SerializeField] private string textKk;
    [SerializeField] private string textEn;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Sprite background;

    private RectTransform _tip;
    private TextMeshProUGUI _label;

    public void Setup(string ru, string kk, string en, TMP_FontAsset tipFont, Sprite tipBackground)
    {
        textRu = ru; textKk = kk; textEn = en; font = tipFont; background = tipBackground;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tip == null) Build();
        var lang = ProcessMonitor.Instance != null ? ProcessMonitor.Instance.Language : Translator.Language.Russian;
        _label.text = MonitoringText.Pick(lang, textRu, string.IsNullOrEmpty(textKk) ? textRu : textKk, string.IsNullOrEmpty(textEn) ? textRu : textEn);
        _tip.gameObject.SetActive(true);
        _tip.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tip != null) _tip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_tip != null) _tip.gameObject.SetActive(false);
    }

    private void Build()
    {
        var go = new GameObject("Tooltip", typeof(RectTransform), typeof(Image), typeof(ContentSizeFitter), typeof(HorizontalLayoutGroup));
        go.AddComponent<ExcludeFromTranslation>();
        _tip = (RectTransform)go.transform;
        _tip.SetParent(transform, false);
        _tip.anchorMin = _tip.anchorMax = new Vector2(0f, 0.5f);
        _tip.pivot = new Vector2(1f, 0.5f);
        _tip.anchoredPosition = new Vector2(-12f, 0f);

        var image = go.GetComponent<Image>();
        image.sprite = background != null ? background : UITheme.Rounded;
        image.type = Image.Type.Sliced;
        image.color = UITheme.Panel;
        image.raycastTarget = false;

        var layout = go.GetComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 6, 6);
        layout.childControlWidth = layout.childControlHeight = true;
        var fitter = go.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(_tip, false);
        _label = labelGo.AddComponent<TextMeshProUGUI>();
        if (font != null) _label.font = font;
        _label.fontSize = 18f;
        _label.color = UITheme.Text;
        _label.raycastTarget = false;
        _label.textWrappingMode = TextWrappingModes.NoWrap;
    }
}
