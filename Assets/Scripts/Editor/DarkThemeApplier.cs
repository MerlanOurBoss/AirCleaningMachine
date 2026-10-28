using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Применяет тёмную тему к интерфейсу сцен. Simulation и CHP устроены одинаково: панель «Настройка системы»,
/// правая панель иконок, выпадающие окна, плюс строка состояния, карточки показателей и подписи над установками.
/// У Constructor своя раскладка (плитки установок, кнопки действий, сохранённые конструкции).
/// Можно запускать повторно: уже созданные элементы обновляются, а не дублируются.
/// </summary>
public static class DarkThemeApplier
{
    private const string SimulationScene = "Assets/Scenes/Simulation.unity";
    private const string ChpScene = "Assets/Scenes/CHP.unity";
    private const string ConstructorScene = "Assets/Scenes/Constructor.unity";
    private const string SavedSlotPrefab = "Assets/Prefabs/Constructor/SaveModel/1.prefab";
    private const string IconDir = "Assets/UI/Theme/Icons";

    [MenuItem("Tools/UI/Apply Dark Theme (Simulation)")]
    public static void Apply() => ApplyToSimulationScene(SimulationScene, styleFrame: true);

    [MenuItem("Tools/UI/Apply Dark Theme (CHP)")]
    public static void ApplyChp() => ApplyToSimulationScene(ChpScene, styleFrame: false);

    [MenuItem("Tools/UI/Apply Dark Theme (Constructor)")]
    public static void ApplyConstructor()
    {
        if (!OpenScene(ConstructorScene, out var scene)) return;
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/UI/Font/Roboto-Regular SDF.asset");
        var rounded = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Theme/Rounded.png");
        var outline = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Theme/RoundedOutline.png");
        var canvas = GameObject.Find("CameraUI").GetComponent<Canvas>();
        var root = canvas.transform;

        StyleConstructorPanel(root.Find("System settings"), rounded, outline, font);
        StyleIconBar(root.Find("Settings"), rounded, font);
        StylePopup(root.Find("Saved Concepts/Panel"), rounded);
        StyleSavedSlotPrefab(rounded, font);
        // 3D-модели в плитках установок должна рисовать UI-камера вместе с плитками, иначе фон плитки ляжет поверх.
        foreach (Transform row in root.Find("System settings"))
            if (row.name.StartsWith("Facilities"))
                foreach (Transform tile in row)
                    SetLayerRecursive(tile.Find("3d Model"), canvas.gameObject.layer);
        SetupUICamera(canvas);

        Save(scene);
    }

    [MenuItem("Tools/UI/Apply Dark Theme (All)")]
    public static void ApplyAll()
    {
        Apply();
        ApplyChp();
        ApplyConstructor();
    }

    // styleFrame — внешняя рамка и левое меню (пока только в Simulation).
    private static void ApplyToSimulationScene(string scenePath, bool styleFrame)
    {
        if (!OpenScene(scenePath, out var scene)) return;

        var rounded = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Theme/Rounded.png");
        var outline = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Theme/RoundedOutline.png");
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/UI/Font/Roboto-Regular SDF.asset");
        var main = GameObject.Find("Main UI/Main").transform;

        StyleSystemPanel(main.Find("System settings"), rounded, outline, font);
        StyleToolbar(main.Find("RightPanel"), rounded, font);
        StylePopup(main.Find("Таблицы/Sliders"), rounded);
        StylePopup(main.Find("Стартовые значения/Image"), rounded);
        RecolorDarkTexts(main.Find("Таблицы"));
        RecolorDarkTexts(main.Find("Стартовые значения"));
        AddKpiBar(main, rounded, font);
        AddFacilityLabels(main, rounded, font);
        AddRightPanelTheme(main.Find("RightPanel"));
        if (styleFrame) StyleFrame(GameObject.Find("Panel UI/Main").transform, rounded, outline, font);
        SetupUICamera(GameObject.Find("Main UI").GetComponent<Canvas>());

        Save(scene);
    }

    private static bool OpenScene(string path, out UnityEngine.SceneManagement.Scene scene)
    {
        scene = EditorSceneManager.GetActiveScene();
        if (scene.path == path) return true;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
        scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        return true;
    }

    private static void Save(UnityEngine.SceneManagement.Scene scene)
    {
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"[DarkThemeApplier] Тёмная тема применена к {scene.name}.");
    }

    // ---------- панель «Настройка системы» ----------

    private static void StyleSystemPanel(Transform panel, Sprite rounded, Sprite outline, TMP_FontAsset font)
    {
        SetImage(panel.GetComponent<Image>(), rounded, UITheme.Panel);

        var head = panel.Find("Head").GetComponent<TextMeshProUGUI>();
        StyleText(head, font, UITheme.Text, 26f);
        head.alignment = TextAlignmentOptions.MidlineLeft;
        head.margin = new Vector4(18f, 0f, 18f, 0f);

        // Строка состояния под заголовком: место под неё — сдвиг блока кнопок и рост панели.
        var panelRt = (RectTransform)panel;
        var status = panel.Find("Status");
        if (status == null)
        {
            const float statusHeight = 28f;
            var go = new GameObject("Status", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, "Status");
            status = go.transform;
            status.SetParent(panel, false);
            status.SetSiblingIndex(head.transform.GetSiblingIndex() + 1);
            var rt = (RectTransform)status;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(18f, -44f);
            rt.sizeDelta = new Vector2(panelRt.sizeDelta.x - 36f, statusHeight);

            var components = (RectTransform)panel.Find("Components");
            components.anchoredPosition += new Vector2(0f, -statusHeight);
            panelRt.sizeDelta += new Vector2(0f, statusHeight);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.raycastTarget = false;
            go.AddComponent<ExcludeFromTranslation>();
            go.AddComponent<SimulationStatusLabel>();
        }
        StyleText(status.GetComponent<TextMeshProUGUI>(), font, UITheme.Muted, 18f);
        var label = new SerializedObject(status.GetComponent<SimulationStatusLabel>());
        label.FindProperty("chain").objectReferenceValue = Object.FindAnyObjectByType<LoadSmoke>(FindObjectsInactive.Include);
        label.ApplyModifiedPropertiesWithoutUndo();

        foreach (Transform row in panel.Find("Components"))
        {
            SetImage(row.GetComponent<Image>(), rounded, UITheme.Panel2);
            SetButtonTint(row.GetComponent<Button>());
            var text = row.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text != null) StyleText(text, font, UITheme.Text, 22f);
            var arrow = row.Find("Image");
            if (arrow != null) SetIcon(arrow.GetComponent<Image>(), UITheme.Muted);
        }

        var simulate = panel.Find("Simulation Button");
        SetImage(simulate.GetComponent<Image>(), rounded, UITheme.Accent);
        SetButtonTint(simulate.GetComponent<Button>());
        StyleText(simulate.GetComponentInChildren<TextMeshProUGUI>(true), font, Color.white, 22f);

        var stop = panel.Find("Stop Simulation Buttons");
        SetImage(stop.GetComponent<Image>(), outline, UITheme.Danger);
        SetButtonTint(stop.GetComponent<Button>());
        StyleText(stop.GetComponentInChildren<TextMeshProUGUI>(true), font, Color.Lerp(UITheme.Danger, Color.white, 0.2f), 22f);
    }

    // ---------- правая панель иконок ----------

    private static void StyleToolbar(Transform rightPanel, Sprite rounded, TMP_FontAsset font)
    {
        var settings = rightPanel.Find("Settings");
        StyleIconBar(settings, rounded, font);

        Tooltip(settings.Find("Kataz"), "Состояние катализатора", "Катализатор күйі", "Catalyst status", font, rounded);
        Tooltip(settings.Find("HDR"), "Стены и потолок", "Қабырғалар мен төбе", "Walls and ceiling", font, rounded);
        Tooltip(settings.Find("bb"), "Основной вид", "Негізгі көрініс", "Main view", font, rounded);
        Tooltip(settings.Find("Light"), "Таблицы на установках", "Қондырғылардағы кестелер", "Unit tables", font, rounded);
        foreach (var name in new[] { "Kataz", "HDR", "bb", "Light" })
            SetIcon(settings.Find(name)?.GetComponent<Image>(), UITheme.Text);

        var move = rightPanel.Find("ButtonForMove");
        if (move != null)
        {
            SetImage(move.GetComponent<Image>(), rounded, UITheme.Panel);
            SetIcon(move.Find("Strelka")?.GetComponent<Image>(), UITheme.Text);
        }
    }

    /// <summary>Тёмная подложка панели иконок, «Полный экран» и красный «Выход» последним, за разделителем.</summary>
    private static void StyleIconBar(Transform settings, Sprite rounded, TMP_FontAsset font)
    {
        if (settings == null) return;
        var background = settings.GetComponent<Image>();
        SetImage(background, rounded, UITheme.Panel);
        if (background != null) background.enabled = true; // в Constructor подложка была выключена
        var layout = settings.GetComponent<VerticalLayoutGroup>();
        if (layout != null) { layout.padding = new RectOffset(8, 8, 10, 10); layout.spacing = 8f; }

        Tooltip(settings.Find("Toggle"), "Полный экран", "Толық экран", "Fullscreen", font, rounded);
        Tooltip(settings.Find("Power"), "Выход", "Шығу", "Exit", font, rounded);
        SetIcon(settings.Find("Toggle/Background")?.GetComponent<Image>(), UITheme.Text);
        SetIcon(settings.Find("Power")?.GetComponent<Image>(), UITheme.Danger);

        // «Выход» — последним, после разделителя: опасное действие отдельно от инструментов вида.
        var power = settings.Find("Power");
        var toggle = settings.Find("Toggle");
        if (power == null) return;

        var sep = settings.Find("Separator");
        if (sep == null)
        {
            var go = new GameObject("Separator", typeof(RectTransform), typeof(Image));
            Undo.RegisterCreatedObjectUndo(go, "Separator");
            go.layer = settings.gameObject.layer;
            sep = go.transform;
            sep.SetParent(settings, false);
            go.GetComponent<Image>().color = UITheme.Line;
            go.GetComponent<Image>().raycastTarget = false;
        }
        // Layout не управляет размерами детей, поэтому размер задаём явно.
        ((RectTransform)sep).sizeDelta = new Vector2(48f, 2f);
        if (toggle != null) toggle.SetAsLastSibling();
        sep.SetAsLastSibling();
        power.SetAsLastSibling();
    }

    // ---------- Constructor ----------

    private static void StyleConstructorPanel(Transform panel, Sprite rounded, Sprite outline, TMP_FontAsset font)
    {
        SetImage(panel.GetComponent<Image>(), rounded, UITheme.Panel);

        var head = panel.Find("Head").GetComponent<TextMeshProUGUI>();
        StyleText(head, font, UITheme.Text, 26f);
        head.alignment = TextAlignmentOptions.MidlineLeft;
        head.margin = new Vector4(18f, 0f, 18f, 0f);
        SetColor(panel.Find("Head/Image")?.GetComponent<Image>(), UITheme.Line);
        SetColor(panel.Find("Image (2)")?.GetComponent<Image>(), UITheme.Line);
        SetColor(panel.Find("Image (3)")?.GetComponent<Image>(), UITheme.Line);

        // Подзаголовки разделов — мелкие приглушённые, слева, как подписи полей.
        foreach (var name in new[] { "Установки (текст)", "Вспомогательные (текст)" })
        {
            var section = panel.Find(name)?.GetComponent<TextMeshProUGUI>();
            if (section == null) continue;
            StyleText(section, font, UITheme.Muted, 20f);
            section.alignment = TextAlignmentOptions.MidlineLeft;
            section.margin = new Vector4(18f, 0f, 18f, 0f);
        }

        foreach (Transform row in panel)
        {
            if (!row.name.StartsWith("Facilities")) continue;
            SetColor(row.GetComponent<Image>(), Color.clear);
            foreach (Transform tile in row)
            {
                SetImage(tile.GetComponent<Image>(), rounded, UITheme.Panel2);
                SetButtonTint(tile.GetComponent<Button>());
            }
        }

        StyleActionButton(panel.Find("Сохранить"), rounded, UITheme.Accent, Color.white, font);
        StyleActionButton(panel.Find("Соединить трубы"), rounded, UITheme.Panel2, UITheme.Text, font);
        StyleActionButton(panel.Find("Удалить все"), outline, UITheme.Danger, Color.Lerp(UITheme.Danger, Color.white, 0.2f), font);
        StyleActionButton(panel.Find("Simulation Button"), rounded, UITheme.Accent, Color.white, font);
        StyleActionButton(panel.Find("Stop Simulation Button"), outline, UITheme.Danger, Color.Lerp(UITheme.Danger, Color.white, 0.2f), font);
    }

    private static void StyleActionButton(Transform button, Sprite sprite, Color background, Color textColor, TMP_FontAsset font)
    {
        if (button == null) return;
        SetImage(button.GetComponent<Image>(), sprite, background);
        SetButtonTint(button.GetComponent<Button>());
        StyleText(button.GetComponentInChildren<TextMeshProUGUI>(true), font, textColor, 20f);
    }

    // Кнопка сохранённой конструкции (CHP и Constructor): тёмная плитка с номером и красным крестиком.
    private static void StyleSavedSlotPrefab(Sprite rounded, TMP_FontAsset font)
    {
        var root = PrefabUtility.LoadPrefabContents(SavedSlotPrefab);
        try
        {
            SetImage(root.GetComponent<Image>(), rounded, UITheme.Panel2);
            SetButtonTint(root.GetComponent<Button>());
            var number = root.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
            StyleText(number, font, UITheme.Text, 30f);
            var close = root.transform.Find("ChildButton");
            if (close != null)
            {
                SetColor(close.GetComponent<Image>(), UITheme.Danger);
                SetIcon(close.Find("Image")?.GetComponent<Image>(), Color.white);
            }
            PrefabUtility.SaveAsPrefabAsset(root, SavedSlotPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    // ---------- выпадающие окна ----------

    private static void StylePopup(Transform popup, Sprite rounded)
    {
        if (popup == null) return;
        SetImage(popup.GetComponent<Image>(), rounded, UITheme.Panel);
    }

    // Тёмные тексты (на бывшем светлом фоне) — в светлые; поля ввода и слайдеры не трогаем.
    private static void RecolorDarkTexts(Transform root)
    {
        if (root == null) return;
        foreach (var text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text.GetComponentInParent<TMP_InputField>(true) != null) continue;
            Color.RGBToHSV(text.color, out _, out _, out float v);
            if (v > 0.45f) continue;
            Undo.RecordObject(text, "Recolor");
            text.color = UITheme.Text;
        }
    }

    // ---------- внешняя рамка и левое меню ----------

    private static readonly Color FrameBackground = new Color(0.078f, 0.102f, 0.133f, 1f); // #141A22
    private static readonly Color ViewBackground = new Color(0.165f, 0.196f, 0.251f, 1f);  // #2A3240 — поля вокруг 3D-вида
    private static readonly Color MenuText = new Color(0.79f, 0.82f, 0.86f, 1f);           // неактивный пункт меню

    private static void StyleFrame(Transform main, Sprite rounded, Sprite outline, TMP_FontAsset font)
    {
        var bar = UITheme.Panel;
        bar.a = 1f;

        SetImage(main.GetComponent<Image>(), null, FrameBackground);
        var background = main.Find("BackGround Panel");
        SetImage(background.Find("Top Panel")?.GetComponent<Image>(), null, bar);
        SetImage(background.Find("Bottom Panel")?.GetComponent<Image>(), null, bar);
        SetImage(background.Find("Panel Shadow")?.GetComponent<Image>(), null, UITheme.Line); // тонкая рамка 3D-вида
        SetImage(background.Find("Panel")?.GetComponent<Image>(), null, ViewBackground);
        foreach (var shadow in new[] { "Top Panel Shadow", "Bottom Panel Shadow", "Shadows" })
            SetEnabled(background.Find(shadow)?.GetComponent<Image>(), false);

        var title = main.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
        StyleText(title, font, UITheme.Muted, 18f);
        if (title != null) title.characterSpacing = 6f;
        StyleText(main.Find("Certificate/Сертификат ")?.GetComponent<TextMeshProUGUI>(), font, UITheme.Muted, 18f);

        var buttons = main.Find("Buttons");
        SetImage(buttons.GetComponent<Image>(), rounded, bar);

        // Пункты — плоские строки с иконкой; текущий раздел (мы в Simulation) — с подложкой и зелёной иконкой.
        var items = new (string name, string icon, float y)[]
        {
            ("Симуляция", "play", 236f), ("Конструктор", "grid", 176f), ("Описание", "doc", 116f),
            ("3D/2D", "cube", 56f), ("Разрешение", "monitor", -4f)
        };
        foreach (var (name, icon, y) in items)
            StyleMenuItem(FindChild(buttons, name), rounded, MenuIcon(icon), font, y, name == "Симуляция");

        var exit = buttons.Find("Выход");
        if (exit != null)
        {
            StyleMenuItem(exit, outline, MenuIcon("exit"), font, -244f, false);
            SetImage(exit.GetComponent<Image>(), outline, UITheme.Danger);
            SetButtonTint(exit.GetComponent<Button>());
            StyleText(exit.GetComponentInChildren<TextMeshProUGUI>(true), font, Color.Lerp(UITheme.Danger, Color.white, 0.25f), 20f);
            SetColor(exit.Find("Icon")?.GetComponent<Image>(), UITheme.Danger);
        }

        StyleLanguageSwitch(buttons.Find("Language"), rounded, font);
    }

    private static void StyleMenuItem(Transform item, Sprite sprite, Sprite icon, TMP_FontAsset font, float y, bool current)
    {
        if (item == null) return;
        var rt = (RectTransform)item;
        Undo.RecordObject(rt, "Theme");
        rt.sizeDelta = new Vector2(340f, 52f);
        rt.anchoredPosition = new Vector2(0f, y);
        FlattenDepth(rt);

        SetImage(item.GetComponent<Image>(), sprite, Color.white);
        var button = item.GetComponent<Button>();
        if (button != null)
        {
            Undo.RecordObject(button, "Theme");
            // Цвет строки задаёт ColorTint: без наведения — прозрачная (у текущего раздела — подложка).
            var idle = current ? UITheme.Panel2 : new Color(UITheme.Panel2.r, UITheme.Panel2.g, UITheme.Panel2.b, 0f);
            var colors = button.colors;
            colors.normalColor = colors.selectedColor = colors.disabledColor = idle;
            colors.highlightedColor = current ? UITheme.Line : UITheme.Panel2;
            colors.pressedColor = UITheme.Line;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;
        }

        var label = item.GetComponentInChildren<TextMeshProUGUI>(true);
        StyleText(label, font, current ? UITheme.Text : MenuText, 20f);
        if (label != null)
        {
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.margin = new Vector4(64f, 0f, 12f, 0f);
            label.characterSpacing = 4f;
        }

        var iconTransform = item.Find("Icon");
        if (iconTransform == null)
        {
            var go = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            Undo.RegisterCreatedObjectUndo(go, "Icon");
            go.layer = item.gameObject.layer;
            iconTransform = go.transform;
            iconTransform.SetParent(item, false);
        }
        var iconRect = (RectTransform)iconTransform;
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(34f, 0f);
        iconRect.sizeDelta = new Vector2(26f, 26f);
        var iconImage = iconTransform.GetComponent<Image>();
        iconImage.sprite = icon;
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        iconImage.color = current ? UITheme.Accent : UITheme.Muted;
    }

    // KZ / RU / EN — один переключатель; выбранный язык подсвечивает LanguageSwitchTheme во время работы.
    private static void StyleLanguageSwitch(Transform language, Sprite rounded, TMP_FontAsset font)
    {
        if (language == null) return;
        var rt = (RectTransform)language;
        Undo.RecordObject(rt, "Theme");
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(340f, 52f);
        rt.anchoredPosition = new Vector2(0f, 100f);
        FlattenDepth(rt);
        SetImage(language.GetComponent<Image>(), rounded, FrameBackground);

        var layout = language.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
        {
            Undo.RecordObject(layout, "Theme");
            layout.padding = new RectOffset(4, 4, 4, 4);
            layout.spacing = 4f;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = true;
        }

        foreach (Transform button in language)
        {
            bool selected = button.name == "RU";
            SetImage(button.GetComponent<Image>(), rounded, selected ? UITheme.Accent : Color.clear);
            SetButtonTint(button.GetComponent<Button>());
            var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            StyleText(label, font, selected ? Color.white : UITheme.Muted, 20f);
            if (label != null)
            {
                label.text = label.text.Trim(); // было «KZ\n» — перевод строки сдвигал букву вверх
                label.alignment = TextAlignmentOptions.Center;
            }
        }

        var theme = language.GetComponent<LanguageSwitchTheme>();
        if (theme == null) theme = Undo.AddComponent<LanguageSwitchTheme>(language.gameObject);
        var so = new SerializedObject(theme);
        so.FindProperty("translator").objectReferenceValue = Object.FindAnyObjectByType<Translator>(FindObjectsInactive.Include);
        so.FindProperty("kazakh").objectReferenceValue = language.Find("KZ")?.GetComponent<Button>();
        so.FindProperty("russian").objectReferenceValue = language.Find("RU")?.GetComponent<Button>();
        so.FindProperty("english").objectReferenceValue = language.Find("EN")?.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // Иконки меню — линии на сетке 24×24 (как у наборов иконок), штрих 2; рисуются в белый PNG 96×96.
    private static Sprite MenuIcon(string name)
    {
        string path = $"{IconDir}/menu_{name}.png";
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null) return existing;

        var shapes = MenuIconShapes(name);
        const int size = 96;
        const float scale = size / 24f, halfStroke = 1f;
        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            var p = new Vector2((x + 0.5f) / scale, 24f - (y + 0.5f) / scale);
            float dist = float.MaxValue;
            foreach (var line in shapes)
                for (int i = 0; i < line.Length - 1; i++)
                    dist = Mathf.Min(dist, DistanceToSegment(p, line[i], line[i + 1]));
            float alpha = Mathf.Clamp01((halfStroke - dist) * scale + 0.5f);
            pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
        }

        Directory.CreateDirectory(IconDir);
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.SetPixels32(pixels);
        tex.Apply();
        File.WriteAllBytes(Path.GetFullPath(path), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // Ломаные в координатах сетки 24×24, ось Y вниз.
    private static Vector2[][] MenuIconShapes(string name)
    {
        Vector2 P(float x, float y) => new Vector2(x, y);
        switch (name)
        {
            case "play": return new[] { new[] { P(7, 4), P(20, 12), P(7, 20), P(7, 4) } };
            case "grid":
                return new[]
                {
                    new[] { P(4, 4), P(10, 4), P(10, 10), P(4, 10), P(4, 4) },
                    new[] { P(14, 4), P(20, 4), P(20, 10), P(14, 10), P(14, 4) },
                    new[] { P(4, 14), P(10, 14), P(10, 20), P(4, 20), P(4, 14) },
                    new[] { P(14, 14), P(20, 14), P(20, 20), P(14, 20), P(14, 14) }
                };
            case "doc":
                return new[]
                {
                    new[] { P(6, 3), P(14, 3), P(18, 7), P(18, 21), P(6, 21), P(6, 3) },
                    new[] { P(14, 3), P(14, 7), P(18, 7) },
                    new[] { P(9, 12), P(15, 12) },
                    new[] { P(9, 16), P(15, 16) }
                };
            case "cube":
                return new[]
                {
                    new[] { P(12, 3), P(20, 7.5f), P(20, 16.5f), P(12, 21), P(4, 16.5f), P(4, 7.5f), P(12, 3) },
                    new[] { P(4, 7.5f), P(12, 12), P(20, 7.5f) },
                    new[] { P(12, 12), P(12, 21) }
                };
            case "monitor":
                return new[]
                {
                    new[] { P(3, 4), P(21, 4), P(21, 16), P(3, 16), P(3, 4) },
                    new[] { P(12, 16), P(12, 20) },
                    new[] { P(8, 20), P(16, 20) }
                };
            case "exit":
                return new[]
                {
                    new[] { P(13, 4), P(6, 4), P(6, 20), P(13, 20) },
                    new[] { P(10, 12), P(21, 12) },
                    new[] { P(18, 9), P(21, 12), P(18, 15) }
                };
            default: return new Vector2[0][];
        }
    }

    private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        var ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
        return Vector2.Distance(p, a + ab * t);
    }

    // ---------- новые элементы ----------

    private static void AddKpiBar(Transform main, Sprite rounded, TMP_FontAsset font)
    {
        var bar = main.Find("KPI Bar");
        if (bar == null)
        {
            var go = new GameObject("KPI Bar", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, "KPI Bar");
            bar = go.transform;
            bar.SetParent(main, false);
            go.AddComponent<KpiBar>();
        }
        bar.SetSiblingIndex(main.Find("RightPanel").GetSiblingIndex());
        var rt = (RectTransform)bar;
        rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.zero;
        rt.anchoredPosition = new Vector2(16f, 16f);
        rt.sizeDelta = new Vector2(960f, 110f);

        var so = new SerializedObject(bar.GetComponent<KpiBar>());
        so.FindProperty("chain").objectReferenceValue = Object.FindAnyObjectByType<LoadSmoke>(FindObjectsInactive.Include);
        so.FindProperty("font").objectReferenceValue = font;
        so.FindProperty("background").objectReferenceValue = rounded;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // Карточки аппаратов создаются в рантайме из общих префабов — их перекрашивает компонент на панели.
    private static void AddRightPanelTheme(Transform rightPanel)
    {
        var theme = rightPanel.GetComponent<RightPanelTheme>();
        if (theme == null) theme = Undo.AddComponent<RightPanelTheme>(rightPanel.gameObject);

        var tabs = rightPanel.Find("Panel/Vertical Group");
        var top = rightPanel.Find("Panel/TopPanel");
        var so = new SerializedObject(theme);
        so.FindProperty("panel").objectReferenceValue = tabs.GetComponent<LoadRightPanel>();
        so.FindProperty("tabs").objectReferenceValue = tabs;
        so.FindProperty("background").objectReferenceValue = rightPanel.GetComponent<Image>();
        so.FindProperty("topBar").objectReferenceValue = top != null ? top.GetComponent<Image>() : null;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AddFacilityLabels(Transform main, Sprite rounded, TMP_FontAsset font)
    {
        var labels = main.Find("Facility Labels");
        if (labels == null)
        {
            var go = new GameObject("Facility Labels", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, "Facility Labels");
            labels = go.transform;
            labels.SetParent(main, false);
            go.AddComponent<FacilityLabels>();
        }
        // Над областью вращения камеры, но под панелями.
        labels.SetSiblingIndex(main.Find("Trigger Area").GetSiblingIndex() + 1);
        var rt = (RectTransform)labels;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var so = new SerializedObject(labels.GetComponent<FacilityLabels>());
        so.FindProperty("chain").objectReferenceValue = Object.FindAnyObjectByType<LoadSmoke>(FindObjectsInactive.Include);
        var cam = GameObject.Find("Camera Rig/Main Camera");
        so.FindProperty("sceneCamera").objectReferenceValue = cam != null ? cam.GetComponent<Camera>() : null;
        so.FindProperty("font").objectReferenceValue = font;
        so.FindProperty("background").objectReferenceValue = rounded;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ---------- UI-камера без постобработки ----------

    /// <summary>
    /// Канвас рисовала основная камера с постобработкой (Bloom, Vignette, цветокоррекция) — интерфейс
    /// высветлялся и светился. Теперь его рисует overlay-камера в стеке основной, без постобработки.
    /// </summary>
    private static void SetupUICamera(Canvas canvas)
    {
        var baseCamera = canvas.worldCamera;
        if (baseCamera == null) return;
        if (baseCamera.name == "UI Camera") // уже настроено — убедиться, что область вывода синхронизируется
        {
            if (baseCamera.GetComponent<MatchParentCameraRect>() == null) baseCamera.gameObject.AddComponent<MatchParentCameraRect>();
            baseCamera.rect = baseCamera.transform.parent.GetComponent<Camera>().rect;
            baseCamera.farClipPlane = UIFarClip(canvas);
            EditorUtility.SetDirty(baseCamera);
            return;
        }

        var baseData = baseCamera.GetComponent<UniversalAdditionalCameraData>();
        if (baseData == null) return;

        var uiTransform = baseCamera.transform.Find("UI Camera");
        Camera uiCamera;
        if (uiTransform == null)
        {
            var go = new GameObject("UI Camera");
            Undo.RegisterCreatedObjectUndo(go, "UI Camera");
            go.transform.SetParent(baseCamera.transform, false);
            uiCamera = go.AddComponent<Camera>();
            go.AddComponent<UniversalAdditionalCameraData>();
        }
        else uiCamera = uiTransform.GetComponent<Camera>();

        int uiLayer = canvas.gameObject.layer;
        uiCamera.cullingMask = 1 << uiLayer;
        uiCamera.clearFlags = CameraClearFlags.Depth;
        // EventSystem ставит выше попадания камеры с большей глубиной: иначе пол (PhysicsRaycaster основной камеры)
        // перекрывает Trigger Area, и камера не крутится.
        uiCamera.depth = baseCamera.depth + 1;
        uiCamera.orthographic = baseCamera.orthographic;
        uiCamera.fieldOfView = baseCamera.fieldOfView;
        uiCamera.nearClipPlane = 0.1f;
        uiCamera.farClipPlane = UIFarClip(canvas);
        uiCamera.rect = baseCamera.rect;
        if (uiCamera.GetComponent<MatchParentCameraRect>() == null) uiCamera.gameObject.AddComponent<MatchParentCameraRect>();

        var uiData = uiCamera.GetComponent<UniversalAdditionalCameraData>();
        uiData.renderType = CameraRenderType.Overlay;
        uiData.renderPostProcessing = false;
        if (!baseData.cameraStack.Contains(uiCamera)) baseData.cameraStack.Add(uiCamera);
        EditorUtility.SetDirty(baseData);
        EditorUtility.SetDirty(uiData);

        Undo.RecordObject(canvas, "UI Camera");
        canvas.worldCamera = uiCamera;
    }

    // Канвас стоит на planeDistance от камеры (в CHP — 100): дальняя плоскость с запасом, иначе элементы,
    // чуть сдвинутые по глубине, отсекаются и сквозь интерфейс видна сцена.
    private static float UIFarClip(Canvas canvas) => Mathf.Max(100f, canvas.planeDistance * 2f);

    // ---------- помощники ----------

    private static void SetImage(Image image, Sprite sprite, Color color)
    {
        if (image == null) return;
        Undo.RecordObject(image, "Theme");
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 1f;
        image.color = color;
    }

    // Рамку рисует перспективная камера с канвасом в 1 единице от неё: разный Z (4 и −1) и масштаб 0,858
    // заметно сдвигали и сжимали пункты меню. Все пункты — в плоскости канваса, без масштаба.
    private static void FlattenDepth(RectTransform rt)
    {
        var p = rt.localPosition;
        rt.localPosition = new Vector3(p.x, p.y, 0f);
        rt.localScale = Vector3.one;
    }

    // Transform.Find считает «/» разделителем пути — «3D/2D» так не найти.
    private static Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
            if (child.name == name) return child;
        return null;
    }

    private static void SetEnabled(Behaviour behaviour, bool enabled)
    {
        if (behaviour == null) return;
        Undo.RecordObject(behaviour, "Theme");
        behaviour.enabled = enabled;
    }

    private static void SetColor(Graphic graphic, Color color)
    {
        if (graphic == null) return;
        Undo.RecordObject(graphic, "Theme");
        graphic.color = color;
    }

    private static void SetLayerRecursive(Transform root, int layer)
    {
        if (root == null) return;
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            Undo.RecordObject(t.gameObject, "Theme");
            t.gameObject.layer = layer;
        }
    }

    private static void SetButtonTint(Selectable selectable)
    {
        if (selectable == null) return;
        Undo.RecordObject(selectable, "Theme");
        var colors = selectable.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.18f, 1.18f, 1.18f, 1f);
        colors.selectedColor = Color.white;
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.disabledColor = new Color(1f, 1f, 1f, 0.45f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        selectable.colors = colors;
    }

    private static void StyleText(TextMeshProUGUI text, TMP_FontAsset font, Color color, float size)
    {
        if (text == null) return;
        Undo.RecordObject(text, "Theme");
        if (font != null) text.font = font;
        text.color = color;
        text.fontSize = size;
        text.fontStyle = FontStyles.Normal;
    }

    private static void Tooltip(Transform target, string ru, string kk, string en, TMP_FontAsset font, Sprite background)
    {
        if (target == null) return;
        var tip = target.GetComponent<UITooltip>();
        if (tip == null) tip = Undo.AddComponent<UITooltip>(target.gameObject);
        tip.Setup(ru, kk, en, font, background);
        EditorUtility.SetDirty(tip);
    }

    /// <summary>Иконка в цвет темы: тёмные PNG заменяются белой копией (альфа из яркости), затем подкрашиваются.</summary>
    private static void SetIcon(Image image, Color color)
    {
        if (image == null || image.sprite == null) return;
        Undo.RecordObject(image, "Theme");
        image.sprite = WhiteVersion(image.sprite);
        image.color = color;
    }

    private static Sprite WhiteVersion(Sprite sprite)
    {
        string srcPath = AssetDatabase.GetAssetPath(sprite);
        if (string.IsNullOrEmpty(srcPath) || srcPath.StartsWith(IconDir)) return sprite;

        string dstPath = $"{IconDir}/{sprite.name}_white.png";
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(dstPath);
        if (existing != null) return existing;

        Directory.CreateDirectory(IconDir);
        var tex = sprite.texture;
        var rt = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Graphics.Blit(tex, rt);
        var prev = RenderTexture.active; RenderTexture.active = rt;
        var copy = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        copy.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        copy.Apply();
        RenderTexture.active = prev; RenderTexture.ReleaseTemporary(rt);

        var pixels = copy.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            var p = pixels[i];
            float lum = (0.299f * p.r + 0.587f * p.g + 0.114f * p.b) / 255f;
            byte a = (byte)(p.a * (1f - lum));     // тёмное — непрозрачное, светлый фон — прозрачный
            pixels[i] = new Color32(255, 255, 255, a);
        }
        copy.SetPixels32(pixels);
        File.WriteAllBytes(Path.GetFullPath(dstPath), copy.EncodeToPNG());
        Object.DestroyImmediate(copy);

        AssetDatabase.ImportAsset(dstPath);
        var importer = (TextureImporter)AssetImporter.GetAtPath(dstPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(dstPath);
    }
}
