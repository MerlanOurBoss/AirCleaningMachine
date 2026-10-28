using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Переключатель языка KZ / RU / EN в тёмной теме: выбранный язык — зелёная кнопка с белым текстом,
/// остальные — без подложки, приглушённые. Следит за Translator, поэтому подсветка верна и при смене языка из кода.
/// </summary>
public class LanguageSwitchTheme : MonoBehaviour
{
    [SerializeField] private Translator translator;
    [SerializeField] private Button kazakh;
    [SerializeField] private Button russian;
    [SerializeField] private Button english;

    private void Start()
    {
        if (translator == null) translator = FindAnyObjectByType<Translator>();
        if (translator == null) return;

        translator.OnLanguageChanged += Paint;
        Paint(translator.currentLanguage);
    }

    private void OnDestroy()
    {
        if (translator != null) translator.OnLanguageChanged -= Paint;
    }

    private void Paint(Translator.Language language)
    {
        Paint(kazakh, language == Translator.Language.Kazakh);
        Paint(russian, language == Translator.Language.Russian);
        Paint(english, language == Translator.Language.English);
    }

    private static void Paint(Button button, bool selected)
    {
        if (button == null) return;
        var image = button.targetGraphic;
        if (image != null) image.color = selected ? UITheme.Accent : Color.clear;
        var label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.color = selected ? Color.white : UITheme.Muted;
    }
}
