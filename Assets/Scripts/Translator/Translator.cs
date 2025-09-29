using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class Translator : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("CSV (UTF-8) with header: OriginalName,Kazakh,English,Russian")]
    public TextAsset translationsCsv;

    [Header("UI")]
    public TMP_FontAsset newFont;
    public TextMeshProUGUI[] allTextComponents;

    public enum Language { Kazakh, English, Russian }
    [SerializeField] public Language currentLanguage = Language.Russian;

    private readonly Dictionary<string, Dictionary<Language, string>> map =
        new Dictionary<string, Dictionary<Language, string>>(StringComparer.Ordinal);

    private readonly Dictionary<TextMeshProUGUI, string> originalKeys =
        new Dictionary<TextMeshProUGUI, string>();

    void Awake()
    {
        // Автопоиск, если список пуст
        if (allTextComponents == null || allTextComponents.Length == 0)
        {
#if UNITY_2023_1_OR_NEWER
            allTextComponents = GameObject.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
#else
            allTextComponents = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
#endif
        }

        if (translationsCsv == null)
        {
            Debug.LogError("[Translator] CSV not assigned.");
            return;
        }
        LoadCsv(translationsCsv);
        CacheOriginalKeys();
        ApplyFontIfAssigned();
    }

    void Start()
    {
        SetLanguage(currentLanguage);
    }

    private void ApplyFontIfAssigned()
    {
        if (newFont == null) return;
        foreach (var tmp in allTextComponents)
            if (tmp != null) tmp.font = newFont;
    }

    private void CacheOriginalKeys()
    {
        originalKeys.Clear();
        foreach (var tmp in allTextComponents)
        {
            if (tmp == null) continue;
            originalKeys[tmp] = (tmp.text ?? string.Empty).Trim();
        }
    }

    public void ReloadFromCsv(TextAsset csv = null)
    {
        if (csv != null) translationsCsv = csv;
        map.Clear();
        LoadCsv(translationsCsv);
        SetLanguage(currentLanguage);
    }

    public void SetRussianLanguage() => SetLanguage(Language.Russian);
    public void SetKazakhLanguage() => SetLanguage(Language.Kazakh);
    public void SetEnglishLanguage() => SetLanguage(Language.English);

    public void SetLanguage(Language language)
    {
        foreach (var tmp in allTextComponents)
        {
            if (tmp == null) continue;
            if (!originalKeys.TryGetValue(tmp, out var key))
                key = (tmp.text ?? string.Empty).Trim();

            if (TryGetTranslation(key, language, out var translated))
                tmp.text = translated;
            else
                Debug.LogWarning($"[Translator] No translation for key '{key}' → {language}");
        }
        currentLanguage = language;
    }

    private bool TryGetTranslation(string originalName, Language lang, out string value)
    {
        value = null;
        if (string.IsNullOrEmpty(originalName)) return false;
        if (!map.TryGetValue(originalName, out var row)) return false;
        if (!row.TryGetValue(lang, out value) || string.IsNullOrWhiteSpace(value))
        {
            value = null;
            return false;
        }
        return true;
    }

    // ===== CSV LOADER =====

    private void LoadCsv(TextAsset csvAsset)
    {
        using var reader = new StringReader(csvAsset.text);

        // читаем «сырую» первую строку (заголовок) и определяем разделитель
        string headerRaw = reader.ReadLine();
        if (headerRaw == null)
        {
            Debug.LogError("[Translator] CSV is empty.");
            return;
        }

        char delimiter = DetectDelimiter(headerRaw);
        var header = ParseCsvLine(headerRaw, delimiter);

        int colOriginal = IndexOf(header, "OriginalName");
        int colKz = IndexOf(header, "Kazakh");
        int colEn = IndexOf(header, "English");
        int colRu = IndexOf(header, "Russian");

        if (colOriginal < 0)
        {
            Debug.LogError("[Translator] CSV must contain 'OriginalName' header.");
            return;
        }

        int rows = 0;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var row = ParseCsvLine(line, delimiter);
            if (row.Count == 0) continue;

            string original = GetCell(row, colOriginal);
            if (string.IsNullOrWhiteSpace(original)) continue;

            var dict = new Dictionary<Language, string>
            {
                { Language.Kazakh,  GetCell(row, colKz) },
                { Language.English, GetCell(row, colEn) },
                { Language.Russian, GetCell(row, colRu) }
            };

            map[original] = dict;
            rows++;
        }

        Debug.Log($"[Translator] Loaded {rows} row(s) from CSV. Delimiter='{delimiter}'");
    }

    private static char DetectDelimiter(string s)
    {
        // Простое и надёжное правило для Excel: ; в русской локали, иначе , или таб
        if (s.Contains(";")) return ';';
        if (s.Contains("\t")) return '\t';
        return ','; // по умолчанию
    }

    private static List<string> ParseCsvLine(string line, char delimiter)
    {
        var result = new List<string>();
        if (line == null) return result;

        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    bool isDouble = (i + 1 < line.Length && line[i + 1] == '"');
                    if (isDouble) { sb.Append('"'); i++; }
                    else inQuotes = false;
                }
                else sb.Append(c);
            }
            else
            {
                if (c == delimiter)
                {
                    result.Add(sb.ToString());
                    sb.Length = 0;
                }
                else if (c == '"') inQuotes = true;
                else sb.Append(c);
            }
        }
        result.Add(sb.ToString());
        return result;
    }

    private static int IndexOf(List<string> header, string name)
    {
        for (int i = 0; i < header.Count; i++)
            if (string.Equals(header[i], name, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    private static string GetCell(List<string> row, int idx)
    {
        if (idx < 0 || idx >= row.Count) return string.Empty;
        return row[idx]?.Trim() ?? string.Empty;
    }
}
