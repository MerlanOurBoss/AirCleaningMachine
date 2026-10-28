using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Применяет тёмную тему к интерфейсу сцены Simulation (панель «Настройка системы», правая панель иконок,
/// выпадающие окна) и добавляет строку состояния, карточки показателей и подписи над установками.
/// Можно запускать повторно: уже созданные элементы обновляются, а не дублируются.
/// </summary>
public static class DarkThemeApplier
{
    private const string ScenePath = "Assets/Scenes/Simulation.unity";
    private const string IconDir = "Assets/UI/Theme/Icons";

    [MenuItem("Tools/UI/Apply Dark Theme (Simulation)")]
    public static void Apply()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

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
        SetupUICamera(GameObject.Find("Main UI").GetComponent<Canvas>());

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[DarkThemeApplier] Тёмная тема применена к Simulation.");
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
        SetImage(settings.GetComponent<Image>(), rounded, UITheme.Panel);
        var layout = settings.GetComponent<VerticalLayoutGroup>();
        if (layout != null) { layout.padding = new RectOffset(8, 8, 10, 10); layout.spacing = 8f; }

        Tooltip(settings.Find("Kataz"), "Состояние катализатора", "Катализатор күйі", "Catalyst status", font, rounded);
        Tooltip(settings.Find("HDR"), "Стены и потолок", "Қабырғалар мен төбе", "Walls and ceiling", font, rounded);
        Tooltip(settings.Find("bb"), "Основной вид", "Негізгі көрініс", "Main view", font, rounded);
        Tooltip(settings.Find("Light"), "Таблицы на установках", "Қондырғылардағы кестелер", "Unit tables", font, rounded);
        Tooltip(settings.Find("Toggle"), "Полный экран", "Толық экран", "Fullscreen", font, rounded);
        Tooltip(settings.Find("Power"), "Выход", "Шығу", "Exit", font, rounded);

        foreach (var name in new[] { "Kataz", "HDR", "bb", "Light" })
            SetIcon(settings.Find(name)?.GetComponent<Image>(), UITheme.Text);
        SetIcon(settings.Find("Toggle/Background")?.GetComponent<Image>(), UITheme.Text);
        SetIcon(settings.Find("Power")?.GetComponent<Image>(), UITheme.Danger);

        // «Выход» — последним, после разделителя: опасное действие отдельно от инструментов вида.
        var power = settings.Find("Power");
        var toggle = settings.Find("Toggle");
        if (power != null)
        {
            var sep = settings.Find("Separator");
            if (sep == null)
            {
                var go = new GameObject("Separator", typeof(RectTransform), typeof(Image));
                Undo.RegisterCreatedObjectUndo(go, "Separator");
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

        var move = rightPanel.Find("ButtonForMove");
        if (move != null)
        {
            SetImage(move.GetComponent<Image>(), rounded, UITheme.Panel);
            SetIcon(move.Find("Strelka")?.GetComponent<Image>(), UITheme.Text);
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
        uiCamera.farClipPlane = 100f;
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
