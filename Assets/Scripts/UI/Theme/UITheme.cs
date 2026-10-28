using UnityEngine;

/// <summary>
/// Цвета тёмной темы интерфейса и скруглённые спрайты для элементов, которые создаются кодом.
/// Для элементов сцены используются те же формы из Assets/UI/Theme (Rounded, RoundedOutline).
/// </summary>
public static class UITheme
{
    public static readonly Color Panel = FromHex("1C232DEB");
    public static readonly Color Panel2 = FromHex("252E3AFF");
    public static readonly Color Line = FromHex("3A4656FF");
    public static readonly Color Text = FromHex("E8EDF2FF");
    public static readonly Color Muted = FromHex("93A1B0FF");
    public static readonly Color Accent = FromHex("1D9E75FF");
    public static readonly Color AccentHover = FromHex("23B585FF");
    public static readonly Color Normal = FromHex("1D9E75FF");
    public static readonly Color Warning = FromHex("EF9F27FF");
    public static readonly Color Danger = FromHex("E24B4AFF");

    public static Color ForLevel(AlertLevel level) =>
        level == AlertLevel.Critical ? Danger : level == AlertLevel.Warning ? Warning : Normal;

    /// <summary>Цвет текста поверх тёмной панели в цвете уровня — светлее, чтобы читался.</summary>
    public static Color TextForLevel(AlertLevel level) => Color.Lerp(ForLevel(level), Color.white, 0.35f);

    private static Sprite _rounded;
    private static Sprite _outline;

    /// <summary>Скруглённый прямоугольник (радиус 12 px) для Image.type = Sliced.</summary>
    public static Sprite Rounded => _rounded != null ? _rounded : (_rounded = CreateRounded(0));

    /// <summary>Рамка со скруглением (толщина 2 px) для Image.type = Sliced.</summary>
    public static Sprite Outline => _outline != null ? _outline : (_outline = CreateRounded(2));

    private static Sprite _chevron;

    /// <summary>Белая стрелка «вниз» для выпадающих списков (красится через Image.color).</summary>
    public static Sprite Chevron => _chevron != null ? _chevron : (_chevron = CreateChevron());

    private static Sprite CreateChevron()
    {
        const int size = 32;
        const float half = 1.6f; // половина толщины линии, px
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            name = "UITheme.Chevron"
        };

        var a = new Vector2(8f, 20f);
        var b = new Vector2(16f, 12f);
        var c = new Vector2(24f, 20f);
        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            var p = new Vector2(x + 0.5f, y + 0.5f);
            float dist = Mathf.Min(DistanceToSegment(p, a, b), DistanceToSegment(p, b, c));
            pixels[y * size + x] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(half - dist + 0.5f) * 255f));
        }
        tex.SetPixels32(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        var ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
        return Vector2.Distance(p, a + ab * t);
    }

    private static Sprite CreateRounded(int strokeWidth)
    {
        const int size = 48, radius = 12, border = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            name = strokeWidth > 0 ? "UITheme.Outline" : "UITheme.Rounded"
        };

        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float outer = Coverage(x, y, size, radius, 0f);
            float alpha = strokeWidth > 0 ? outer - Coverage(x, y, size, radius - strokeWidth, strokeWidth) : outer;
            pixels[y * size + x] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(alpha) * 255f));
        }
        tex.SetPixels32(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
            SpriteMeshType.FullRect, new Vector4(border, border, border, border));
    }

    // Доля пикселя внутри скруглённого прямоугольника с отступом inset (сглаживание краёв).
    private static float Coverage(int x, int y, int size, float radius, float inset)
    {
        float px = x + 0.5f, py = y + 0.5f;
        float min = inset, max = size - inset;
        float cx = Mathf.Clamp(px, min + radius, max - radius);
        float cy = Mathf.Clamp(py, min + radius, max - radius);
        float dist = Vector2.Distance(new Vector2(px, py), new Vector2(cx, cy));
        if (px < min || px > max || py < min || py > max) return 0f;
        return Mathf.Clamp01(radius - dist + 0.5f);
    }

    private static Color FromHex(string rgba)
    {
        ColorUtility.TryParseHtmlString("#" + rgba, out var color);
        return color;
    }
}
