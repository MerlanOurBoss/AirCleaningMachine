using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSequenceController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera overviewCamera;
    [SerializeField] private GameObject overviewCameraUI;
    
    [SerializeField] private List<Camera> componentCameras;
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
    [SerializeField] private float defaultFOV = 60f;
    [SerializeField] private float revealFOV = 45f; 

    [Header("Input")]
    [SerializeField] private bool clickToReveal = true; 
    [SerializeField] private bool clickOnEmptyToRestore = true;
    [SerializeField] private float clickCooldown = 0.12f;

    private enum Mode { Overview, Wall, Revealed }
    private Mode mode = Mode.Overview;
    public System.Action<bool> OnRevealStateChanged;
    private readonly Dictionary<Camera, Rect> baseRects = new Dictionary<Camera, Rect>();
    private float _lastClickTime;
    private Coroutine _animRoutine;
    private Camera _revealedCam;

    [ContextMenu("Refresh Cameras From Tag")]
    public void RefreshCamerasFromTag()
    {
        var found = GameObject.FindGameObjectsWithTag(componentsTag);
        var seenNames = new HashSet<string>();
        int emulgatorCount = 0;
        const int emulgatorLimit = 2;

        componentCameras = new List<Camera>(found.Length);

        foreach (var go in found)
        {
            var cam = go.GetComponent<Camera>();
            if (cam == null) continue;

            if (cam.name == "EmulgatorCamera")
            {
                if (emulgatorCount >= emulgatorLimit)
                {
                    cam.gameObject.transform.parent.gameObject.SetActive(false);
                    continue;
                }
                
                emulgatorCount++;
                componentCameras.Add(cam);
                continue;
            }

            if (!seenNames.Add(cam.name))
            {
                cam.gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }
            componentCameras.Add(cam);
        }
        
        componentCameras.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
    }
    public void ShowWall()
    {
        if (overviewCamera) overviewCamera.gameObject.SetActive(false);
        if (overviewCameraUI) overviewCameraUI.gameObject.SetActive(false);
        
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
    

    private void Awake()
    {
        foreach (var cam in componentCameras)
            if (cam) cam.gameObject.SetActive(false);

        if (overviewCamera) overviewCamera.gameObject.SetActive(true);
    }
    private void Start()
    {
        RefreshCamerasFromTag();

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
        //if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;
        
        _lastClickTime = Time.unscaledTime;

        Vector2 mp = Input.mousePosition;
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
                ShowOverview();
        }
        else if (mode == Mode.Revealed)
        {
            if (hitCam != null)
            {
                if (hitCam == _revealedCam)
                    return;
            }

            RestoreWallFromReveal();
        }
    }

    private void LayoutGrid()
    {
        baseRects.Clear();

        int n = 0;
        foreach (var cam in componentCameras) if (cam) n++;
        if (n == 0) return;

        int cols = columns > 0 ? columns : Mathf.CeilToInt(Mathf.Sqrt(n));
        int rows = Mathf.CeilToInt((float)n / cols);

        float areaX = gridArea.x;
        float areaY = gridArea.y;
        float areaW = gridArea.width;
        float areaH = gridArea.height;

        float cellW = (areaW - gutter * (cols - 1)) / cols;
        float cellH = (areaH - gutter * (rows - 1)) / rows;

        int i = 0;
        foreach (var cam in componentCameras)
        {
            if (!cam) continue;

            int r = i / cols;
            int c = i % cols;

            float x = areaX + c * (cellW + gutter);
            float y = areaY + (rows - 1 - r) * (cellH + gutter);

            Rect rect = new Rect(x, y, cellW, cellH);
            baseRects[cam] = rect;
            cam.rect = rect;

            i++;
        }
    }
    public void Reveal(Camera cam)
    {
        if (!enableReveal || cam == null) return;
        
        OnRevealStateChanged?.Invoke(true); 
        _revealedCam = cam;
        mode = Mode.Revealed;
        StopAnim();
        _animRoutine = StartCoroutine(Co_Reveal(cam));
    }

    private IEnumerator Co_Reveal(Camera cam)
    {
        Dictionary<Camera, Rect> startRects = new Dictionary<Camera, Rect>();
        foreach (var kv in baseRects) startRects[kv.Key] = kv.Key.rect;

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
                    var to = gridArea;
                    c.rect = LerpRect(from, to, k);
                    c.fieldOfView = Mathf.Lerp(defaultFOV, revealFOV, k);
                }
                else if (c != cam)
                {
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
        OnRevealStateChanged?.Invoke(false); 
        StopAnim();
        _animRoutine = StartCoroutine(Co_RestoreWall());
    }

    private IEnumerator Co_RestoreWall()
    {
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

        foreach (var kv in baseRects)
        {
            kv.Key.rect = kv.Value;
            kv.Key.fieldOfView = defaultFOV;
        }

        _revealedCam = null;
        mode = Mode.Wall;
        StopAnim();
    }

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
