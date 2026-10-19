using UnityEngine;

public abstract class MathModuleBase : MonoBehaviour, IFacilityIdentified
{
    [Header("Other Components")]
    [SerializeField] protected Translator translator;

    [Header("Identity")]
    [Tooltip("Стабильный ID, который выставляется РУКАМИ в инспекторе и должен совпадать " +
             "с таким же значением на связанном GeneralManagerForXxx (поле facilityId там). " +
             "Замена instanceID/globalCounter: значение не генерируется в рантайме, поэтому " +
             "не зависит от порядка вызова Start() и не может 'перепутаться' между экземплярами.")]
    [SerializeField] protected string facilityId;
    public string FacilityId => facilityId;

    private string _lastSnapshot;

    protected virtual void Start()
    {
        if (string.IsNullOrWhiteSpace(facilityId))
            Debug.LogWarning($"{GetType().Name} ({name}): facilityId не задан в инспекторе — " +
                              "связанный GeneralManagerForXxx не сможет найти этот модуль по ID, " +
                              "если у него не назначена прямая ссылка.");

        if (translator == null)
        {
            var translatorObj = GameObject.FindGameObjectWithTag("Translator");
            if (translatorObj == null)
            {
                Debug.LogError($"{GetType().Name} ({name}): объект с тегом 'Translator' не найден на сцене.");
            }
            else
            {
                translator = translatorObj.GetComponent<Translator>();
            }
        }

        if (translator != null)
        {
            translator.OnLanguageChanged += HandleLanguageChanged;
            HandleLanguageChanged(translator.currentLanguage);
        }
    }

    protected virtual void OnDestroy()
    {
        if (translator != null)
            translator.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(Translator.Language lang)
    {
        OnLanguageChanged(lang);
        
        _lastSnapshot = null;
        Recalculate();
    }

    private void Update()
    {
        if (translator == null) return;

        string snapshot = BuildDirtySnapshot();
        if (snapshot == _lastSnapshot) return;

        _lastSnapshot = snapshot;
        Recalculate();
    }

    protected abstract string BuildDirtySnapshot();

    protected abstract void OnLanguageChanged(Translator.Language lang);

    protected abstract void Recalculate();

    /// <summary>
    /// Совпадает ли текст поля с одним из вариантов (без учёта регистра и пробелов по краям).
    /// Нужен, чтобы расчёты не зависели от языка интерфейса: передаются названия на всех языках.
    /// </summary>
    protected static bool MatchesAny(string text, params string[] options)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        string t = text.Trim();
        foreach (var option in options)
            if (string.Equals(t, option.Trim(), System.StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    /// <summary>Сравнение числа из поля ("1,5 м³/с", "1.5 m³/s") с ожидаемым значением.</summary>
    protected static bool NumberEquals(string text, float expected, float tolerance = 0.001f)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        return Mathf.Abs(ParseLeadingNumber(text) - expected) <= tolerance;
    }

    protected static float ParseLeadingNumber(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0f;

        int spaceIdx = text.IndexOf(' ');
        string numberPart = spaceIdx >= 0 ? text[..spaceIdx] : text;
        numberPart = numberPart.Replace(',', '.');

        return float.TryParse(numberPart, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out float value) ? value : 0f;
    }
}