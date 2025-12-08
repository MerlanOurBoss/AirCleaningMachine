using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSequenceController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera overviewCamera;            // общая камера (её выключаем в режиме стены)
    [SerializeField] private GameObject overviewCameraUI;
    
    [SerializeField] private List<Camera> componentCameras;    // камеры компонентов в нужной очередности
    [SerializeField] private string componentsTag = "ComponentsCamera";
    
    
    [Header("Viewport area (в долях экрана)")]
    [SerializeField] private Rect gridArea = new Rect(0.22f, 0.051f, 0.752f, 0.883f);
    
    [Header("Layout")]
    [Tooltip("0 = авто (квадратная сетка). Иначе фиксированное число колонок.")]
    [SerializeField] private int columns = 0;
    [Tooltip("Внутренние отступы между плитками в долях экрана (0..0.1).")]
    [Range(0f, 0.1f)] [SerializeField] private float gutter = 0.01f;
    [Tooltip("Внешний отступ от краёв экрана (0..0.2).")]
    [Range(0f, 0.2f)] [SerializeField] private float outerPadding = 0.02f;

    [Header("Reveal (раскрытие)")]
    [SerializeField] private bool enableReveal = true;
    [SerializeField] private float revealDuration = 0.35f;
    [SerializeField] private AnimationCurve revealCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] private float defaultFOV = 60f;           // стартовый FOV для плиток
    [SerializeField] private float revealFOV = 45f;            // FOV при раскрытии (если хочется лёгкий зум)

    [Header("Input")]
    [SerializeField] private bool clickToReveal = true;        // клик по плитке = фулл-скрин
    [SerializeField] private bool clickOnEmptyToRestore = true;// клик мимо плиток = вернуть стену
    [SerializeField] private float clickCooldown = 0.12f;

    private enum Mode { Overview, Wall, Revealed }
    private Mode mode = Mode.Overview;

    // запоминаем исходные rect'ы для возврата
    private readonly Dictionary<Camera, Rect> baseRects = new Dictionary<Camera, Rect>();
    private float _lastClickTime;
    private Coroutine _animRoutine;
    private Camera _revealedCam;

    // ===================== ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ UI =====================
    [ContextMenu("Refresh Cameras From Tag")]
    public void RefreshCamerasFromTag()
    {
        var found = GameObject.FindGameObjectsWithTag(componentsTag);
        componentCameras = new List<Camera>(found.Length);
        foreach (var go in found)
        {
            var cam = go.GetComponent<Camera>();
            if (cam != null) componentCameras.Add(cam);
        }

        // при желании — стабильный порядок (по имени)
        componentCameras.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
    }
    public void ShowWall()
    {
        // выключить обзор
        if (overviewCamera) overviewCamera.gameObject.SetActive(false);
        if (overviewCameraUI) overviewCameraUI.gameObject.SetActive(false);
        
        // включить все компонентные и разложить
        foreach (var cam in componentCameras)
        {
            if (!cam) continue;
            cam.gameObject.SetActive(true);
            cam.fieldOfView = defaultFOV;
        }

        LayoutGrid();
        mode = Mode.Wall;
        _revealedCam = null;
        StopAnim();
    }

    public void ShowOverview()
    {
        foreach (var cam in componentCameras)
            if (cam) cam.gameObject.SetActive(false);

        if (overviewCamera)
        {
            overviewCamera.gameObject.SetActive(true);
            overviewCamera.fieldOfView = defaultFOV;
        }
        if (overviewCameraUI) overviewCameraUI.SetActive(true);

        mode = Mode.Overview;
        _revealedCam = null;
        StopAnim();
    }

    // ========================== ЖИЗНЕННЫЙ ЦИКЛ ==========================

    private void Awake()
    {
        // стартуем в обзоре: компонентные выключены
        foreach (var cam in componentCameras)
            if (cam) cam.gameObject.SetActive(false);

        if (overviewCamera) overviewCamera.gameObject.SetActive(true);
    }
    private void Start()
    {
        // собрали камеры по тегу
        RefreshCamerasFromTag();

        // стартуем в обзоре: компонентные выключены
        foreach (var cam in componentCameras)
            if (cam != null) cam.gameObject.SetActive(false);

        if (overviewCamera) overviewCamera.gameObject.SetActive(true);
        if (overviewCameraUI) overviewCameraUI.SetActive(true);
    }
    private void Update()
    {
        if (!clickToReveal) return;
        if (Time.unscaledTime - _lastClickTime < clickCooldown) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        _lastClickTime = Time.unscaledTime;

        Vector2 mp = Input.mousePosition;
        // Определим, какая камера под курсором по её pixelRect
        Camera hitCam = null;
        foreach (var cam in componentCameras)
        {
            if (!cam || !cam.gameObject.activeInHierarchy) continue;
            if (cam.pixelRect.Contains(mp)) { hitCam = cam; break; }
        }

        if (mode == Mode.Wall)
        {
            if (hitCam != null && enableReveal)
                Reveal(hitCam);
            else if (clickOnEmptyToRestore)
                ShowOverview(); // клик вне плиток = назад к обзору (опционально)
        }
        else if (mode == Mode.Revealed)
        {
            // Любой клик (в том числе по этой же камере) — вернуть стену
            RestoreWallFromReveal();
        }
    }

    // ============================ ЛЕЙАУТ ============================

    private void LayoutGrid()
    {
        baseRects.Clear();

        int n = 0;
        foreach (var cam in componentCameras) if (cam) n++;
        if (n == 0) return;

        int cols = columns > 0 ? columns : Mathf.CeilToInt(Mathf.Sqrt(n));
        int rows = Mathf.CeilToInt((float)n / cols);

        // Работаем ВНУТРИ gridArea
        float areaX = gridArea.x;
        float areaY = gridArea.y;          // нижняя граница (Rect.y — от низа)
        float areaW = gridArea.width;
        float areaH = gridArea.height;

        // размеры ячейки с учётом межплиточного отступа (gutter)
        float cellW = (areaW - gutter * (cols - 1)) / cols;
        float cellH = (areaH - gutter * (rows - 1)) / rows;

        int i = 0;
        foreach (var cam in componentCameras)
        {
            if (!cam) continue;

            int r = i / cols;            // 0..rows-1 (сверху вниз)
            int c = i % cols;            // 0..cols-1 (слева направо)

            // y считаем от НИЗА экрана: верхняя строка — это (rows-1-r)
            float x = areaX + c * (cellW + gutter);
            float y = areaY + (rows - 1 - r) * (cellH + gutter);

            Rect rect = new Rect(x, y, cellW, cellH);
            baseRects[cam] = rect;
            cam.rect = rect;

            i++;
        }
    }

    // ============================ РАСКРЫТИЕ ============================

    public void Reveal(Camera cam)
    {
        if (!enableReveal || cam == null) return;
        _revealedCam = cam;
        mode = Mode.Revealed;

        // Все камеры остаются включены, но мы анимируем rect выбранной до фулл-скрина, остальные — сворачиваем.
        StopAnim();
        _animRoutine = StartCoroutine(Co_Reveal(cam));
    }

    private IEnumerator Co_Reveal(Camera cam)
    {
        // исходные прямоугольники
        Dictionary<Camera, Rect> startRects = new Dictionary<Camera, Rect>();
        foreach (var kv in baseRects) startRects[kv.Key] = kv.Key.rect;

        // мини-превью для прочих камер — компактная лента внутри НИЖНЕГО края gridArea
        // (можно настроить по вкусу)
        float thumbW = gridArea.width * 0.18f;
        float thumbH = gridArea.height * 0.18f;
        Rect thumbRect = new Rect(
            gridArea.x + gutter,
            gridArea.y + gutter,
            thumbW,
            thumbH
        );

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / revealDuration;
            float k = revealCurve.Evaluate(Mathf.Clamp01(t));

            foreach (var kv in baseRects)
            {
                var c = kv.Key;
                var from = startRects[c];

                if (c == cam)
                {
                    // раскрываем на всю gridArea
                    var to = gridArea;
                    c.rect = LerpRect(from, to, k);
                    c.fieldOfView = Mathf.Lerp(defaultFOV, revealFOV, k);
                }
                else
                {
                    // сворачиваем в мини-превью в пределах gridArea (внизу слева)
                    c.rect = LerpRect(from, thumbRect, k);
                    c.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV, k);
                }
            }
            yield return null;
        }
    }

    public void RestoreWallFromReveal()
    {
        if (mode != Mode.Revealed) return;
        StopAnim();
        _animRoutine = StartCoroutine(Co_RestoreWall());
    }

    private IEnumerator Co_RestoreWall()
    {
        // стартуем из текущих rect'ов к baseRects
        Dictionary<Camera, Rect> start = new Dictionary<Camera, Rect>();
        foreach (var kv in baseRects) start[kv.Key] = kv.Key.rect;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / revealDuration;
            float k = revealCurve.Evaluate(Mathf.Clamp01(t));

            foreach (var kv in baseRects)
            {
                var c = kv.Key;
                var from = start[c];
                var to = kv.Value;
                c.rect = LerpRect(from, to, k);
                c.fieldOfView = Mathf.Lerp(c.fieldOfView, defaultFOV, k);
            }

            yield return null;
        }

        // фиксация значений
        foreach (var kv in baseRects)
        {
            kv.Key.rect = kv.Value;
            kv.Key.fieldOfView = defaultFOV;
        }

        _revealedCam = null;
        mode = Mode.Wall;
        StopAnim();
    }

    // ============================ УТИЛИТЫ ============================

    private Rect LerpRect(Rect a, Rect b, float t)
    {
        return new Rect(
            Mathf.Lerp(a.x, b.x, t),
            Mathf.Lerp(a.y, b.y, t),
            Mathf.Lerp(a.width, b.width, t),
            Mathf.Lerp(a.height, b.height, t)
        );
    }

    private void StopAnim()
    {
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = null;
    }
}
