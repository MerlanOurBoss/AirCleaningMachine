using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Разбор строк, которые матмодули пишут в правую панель («Время адсорбции:\n      0,21 ч»,
/// «Длина аппарата: 6,0 м \nВысота аппарата: 4,0 м»), и красивое оформление значения:
/// число светлым, единица — мельче и приглушённо, группы разрядов, не больше трёх знаков после запятой.
/// </summary>
public static class PanelValueFormat
{
    public struct Pair
    {
        public string Label;
        public string Value; // пусто — строка без двоеточия, выводится как есть
    }

    private static readonly Regex NumberWithUnit = new Regex(@"^([-+−]?\d[\d\s\u00A0]*(?:[.,]\d+)?(?:[eE][-+]?\d+)?)\s*(.*)$");
    private const int MaxDecimals = 3;

    /// <summary>Пары «название — значение». Строка «Название:» без значения склеивается со следующей.</summary>
    public static List<Pair> Parse(string source)
    {
        var pairs = new List<Pair>();
        if (string.IsNullOrEmpty(source)) return pairs;

        string pendingLabel = null;
        foreach (var raw in source.Replace("\r", string.Empty).Split('\n'))
        {
            string line = raw.Trim();
            if (line.Length == 0) continue;

            if (pendingLabel != null)
            {
                pairs.Add(new Pair { Label = pendingLabel, Value = line });
                pendingLabel = null;
                continue;
            }

            int colon = line.LastIndexOf(':');
            if (colon < 0) { pairs.Add(new Pair { Label = line, Value = string.Empty }); continue; }

            string label = line.Substring(0, colon).TrimEnd(':', ' ').Trim(); // «Название:: значение»
            string value = line.Substring(colon + 1).Trim();
            if (value.Length == 0) pendingLabel = label;
            else pairs.Add(new Pair { Label = label, Value = value });
        }
        if (pendingLabel != null) pairs.Add(new Pair { Label = pendingLabel, Value = string.Empty });
        return pairs;
    }

    /// <summary>Значение в rich text: число цветом numberHex, единица — unitHex и 90 % размера.</summary>
    public static string Value(string value, string numberHex, string unitHex)
    {
        var match = NumberWithUnit.Match(value);
        if (!match.Success) return $"<color=#{numberHex}>{value}</color>";

        string number = Number(match.Groups[1].Value);
        string unit = match.Groups[2].Value.Trim();
        return unit.Length == 0
            ? $"<color=#{numberHex}>{number}</color>"
            : $"<color=#{numberHex}>{number}</color><size=90%><color=#{unitHex}>\u00A0{unit}</color></size>";
    }

    /// <summary>«65662,2» → «65 662,2», «369,4779» → «369,478». Запятая сохраняется, как в исходной строке.</summary>
    public static string Number(string number)
    {
        string compact = number.Replace(" ", string.Empty).Replace("\u00A0", string.Empty).Replace('−', '-');
        if (compact.IndexOfAny(new[] { 'e', 'E' }) >= 0) return compact;

        char separator = compact.Contains(".") ? '.' : ',';
        int point = compact.IndexOf(separator);
        string integer = point >= 0 ? compact.Substring(0, point) : compact;
        string fraction = point >= 0 ? compact.Substring(point + 1) : string.Empty;

        if (fraction.Length > MaxDecimals &&
            double.TryParse(compact.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
        {
            string rounded = parsed.ToString("0." + new string('0', MaxDecimals), CultureInfo.InvariantCulture);
            int dot = rounded.IndexOf('.');
            integer = rounded.Substring(0, dot);
            fraction = rounded.Substring(dot + 1);
        }

        return GroupThousands(integer) + (fraction.Length > 0 ? separator + fraction : string.Empty);
    }

    private static string GroupThousands(string integer)
    {
        bool negative = integer.StartsWith("-") || integer.StartsWith("+");
        string sign = negative ? integer.Substring(0, 1) : string.Empty;
        string digits = negative ? integer.Substring(1) : integer;
        if (digits.Length <= 4) return sign + digits; // 1234 читается и так

        var sb = new StringBuilder();
        for (int i = 0; i < digits.Length; i++)
        {
            if (i > 0 && (digits.Length - i) % 3 == 0) sb.Append('\u00A0');
            sb.Append(digits[i]);
        }
        return sign + sb;
    }
}
