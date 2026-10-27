using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Управление основной камерой. Скрипт висит на UI-области над 3D-видом (Trigger Area / Button).
///
/// Мышь (над областью камеры):
///   ЛКМ — вращение вокруг точки (орбита);
///   ПКМ — осмотр вокруг себя (курсор скрывается, пока кнопка зажата);
///   СКМ или Shift+ЛКМ — панорама (сдвиг вида);
///   колесо — приближение к точке под курсором;
///   двойной клик ЛКМ — подлететь к объекту.
/// Клавиши (пока курсор над областью или зажата кнопка мыши, и не идёт ввод текста):
///   WASD / стрелки — полёт, Q/E — вниз/вверх, Shift — быстрее;
///   F — подлететь к объекту под курсором; Home — вид по умолчанию;
///   1 — спереди, 2 — сверху, 3 — изометрия, 4…9 — точки обзора из списка View Points.
///
/// Объект orbitTarget больше не двигается: он задаёт только начальную точку вращения.
/// </summary>
public class CameraRotation : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Transform cam;
    [Tooltip("Начальная точка вращения (и точка для «вида по умолчанию»). Сам объект не двигается.")]
    [SerializeField] private Transform orbitTarget;

    [Header("Sensitivity & Limits")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float verticalLimit = 85f;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitMinDistance = 2f;
    [SerializeField] private float orbitMaxDistance = 100f;
    [Tooltip("На сколько процентов меняется дистанция за одно деление колеса (не зависит от масштаба сцены).")]
    [Range(1f, 50f)] public float zoomStepPercent = 15f;
    [Tooltip("Колесо приближает к точке под курсором, а не к центру экрана.")]
    [SerializeField] private bool zoomToCursor = true;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastMoveMultiplier = 3f;
    [Tooltip("Скорость полёта меняется вместе с дистанцией до точки вращения: вблизи медленнее, издалека быстрее. " +
             "Move Speed — скорость на стартовой дистанции.")]
    [SerializeField] private bool scaleSpeedWithDistance = true;
    [Tooltip("Клавиши работают при наведении курсора на область камеры, без зажатой кнопки мыши. " +
             "Выключите, если в сцене эти клавиши заняты другими действиями (например, в конструкторе).")]
    [SerializeField] private bool keysOnHover = true;
    public bool isMouseMoving = false;

    [Header("Smoothing (0 = без сглаживания)")]
    [SerializeField] private float rotationSmoothing = 15f;
    [SerializeField] private float zoomSmoothing = 10f;
    [Tooltip("Разгон и торможение при полёте на клавишах.")]
    [SerializeField] private float moveSmoothing = 8f;
    [Tooltip("Плавность перелётов: фокус на объекте, вид по умолчанию, готовые ракурсы.")]
    [SerializeField] private float transitionSmoothing = 5f;

    [Header("Inertia")]
    [SerializeField] private bool useInertia = true;
    [Tooltip("Чем больше, тем быстрее затухает движение после отпускания кнопки.")]
    [SerializeField] private float inertiaDamping = 6f;

    [Header("Height Limit")]
    [SerializeField] private float minWorldY = -1f;

    [Header("Focus & Views")]
    [Tooltip("Дистанция при фокусе = размер объекта × этот множитель (в пределах Orbit Min/Max Distance).")]
    [SerializeField] private float focusDistanceMultiplier = 2.5f;
    [Tooltip("Точки обзора (пустые объекты, повёрнутые как камера). Клавиши 4…9 или метод GoToViewPoint(index).")]
    [SerializeField] private Transform[] viewPoints;

    [Header("Scene Bounds")]
    [Tooltip("Не давать точке вращения уходить за пределы сцены (считается по моделям сцены).")]
    [SerializeField] private bool limitToSceneBounds = true;
    [Tooltip("Запас за краями сцены, в долях её размера.")]
    [Range(0f, 1f)] public float boundsPadding = 0.25f;

    [Header("Obstacles")]
    [Tooltip("При вращении камера подходит ближе, если между ней и точкой вращения стена, и не залезает внутрь объектов. " +
             "Не включайте в сценах, где точка вращения стоит внутри модели установки: камеру будет затягивать внутрь.")]
    [SerializeField] private bool avoidObstacles = false;
    public LayerMask obstacleMask = Physics.DefaultRaycastLayers & ~(1 << 5);

    [Header("Cursor")]
    [Tooltip("Скрывать и фиксировать курсор, пока зажата ПКМ (осмотр).")]
    [SerializeField] private bool lockCursorOnFreeLook = true;

    // ---------- состояние ----------
    private const float WheelNotch = 0.1f;     // одно деление колеса в Input Manager по умолчанию
    private const float ClickMoveThreshold = 5f; // пикселей: больше — это уже перетаскивание, а не клик

    private Camera _camera;
    private bool _initialized;

    // Текущие (сглаженные) значения и цели
    private Vector3 _pivot, _pivotTarget;
    private float _yaw, _pitch, _dist;
    private float _yawTarget, _pitchTarget, _distTarget;

    // Инерция
    private float _yawVel, _pitchVel;
    private Vector3 _panVel;
    private Vector3 _moveVel;
    private Vector3 _dolly;          // остаток плавного сдвига камеры колесом при осмотре/полёте
    private bool _lastRotateWasFree;
    private bool _collisionMoving;

    private bool _transition;
    private bool _wasActive;
    private bool _wasAnchoredToCamera;

    // Последнее известное положение камеры — чтобы заметить, что её сдвинул другой скрипт (например, CameraRotationAFK).
    private Vector3 _knownPos;
    private Quaternion _knownRot;

    // Столкновения
    private float _collisionDist;
    private bool _clipped;
    private readonly RaycastHit[] _hits = new RaycastHit[16];

    // Ввод
    private bool _hovered;
    private readonly HashSet<PointerEventData.InputButton> _pressed = new HashSet<PointerEventData.InputButton>();
    private Vector3 _lastMousePos;
    private float _pressMoveDistance;
    private bool _cursorLocked;

    // Вид по умолчанию
    private Vector3 _homePivot;
    private float _homeYaw, _homePitch, _homeDist;
    private float _startDistance = 1f;

    // Границы сцены
    private Bounds _sceneBounds;
    private bool _hasBounds;
    private float _boundsTime = float.NegativeInfinity;

    // ================= Unity =================

    private void Start()
    {
        TryInitialize();
    }

    private void OnDisable()
    {
        _pressed.Clear();
        _hovered = false;
        UnlockCursor();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed.Add(eventData.button);
        _pressMoveDistance = 0f;
        _lastMousePos = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed.Remove(eventData.button);
        if (eventData.button == PointerEventData.InputButton.Right) UnlockCursor();
    }

    public void OnPointerEnter(PointerEventData eventData) => _hovered = true;
    public void OnPointerExit(PointerEventData eventData) => _hovered = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left &&
            eventData.clickCount == 2 &&
            _pressMoveDistance < ClickMoveThreshold)
        {
            FocusUnderCursor();
        }
    }

    /// <summary>Вызывается из EventTrigger (PointerUp) в сценах. Флаг всё равно пересчитывается каждый кадр.</summary>
    public void GetMouseOff()
    {
        isMouseMoving = false;
    }

    private void LateUpdate()
    {
        if (!TryInitialize()) return;
        if (!cam.gameObject.activeInHierarchy)
        {
            _pressed.Clear();
            UnlockCursor();
            return;
        }

        float dt = Mathf.Max(Time.unscaledDeltaTime, 1e-5f);
        Vector3 mousePos = Input.mousePosition;

        RefreshPressedButtons();
        if (_pressed.Count > 0 && !_cursorLocked)
            _pressMoveDistance += (mousePos - _lastMousePos).magnitude;

        bool typing = IsTypingInInputField();
        bool left = _pressed.Contains(PointerEventData.InputButton.Left);
        bool right = _pressed.Contains(PointerEventData.InputButton.Right);
        bool middle = _pressed.Contains(PointerEventData.InputButton.Middle);
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        bool pan = middle || (left && shift && !right);
        bool freeLook = right && !middle;
        bool orbit = left && !right && !middle && !shift;

        bool areaActive = _hovered || _pressed.Count > 0;
        bool keysAllowed = !typing && (_pressed.Count > 0 || (keysOnHover && _hovered));
        Vector3 moveInput = keysAllowed ? ReadMoveInput() : Vector3.zero;
        float scroll = areaActive ? Input.GetAxis("Mouse ScrollWheel") : 0f;

        bool userInput = orbit || freeLook || pan || moveInput != Vector3.zero || Mathf.Abs(scroll) > 1e-4f;

        if ((userInput || (!typing && areaActive && AnyHotkeyDown())) && !_wasActive)
            SyncWithExternalChanges();

        if (!typing && areaActive)
            HandleHotkeys();

        if (userInput) _transition = false;

        // Курсор: фиксируем только при перетаскивании ПКМ, чтобы обычный клик ПКМ не уводил курсор в центр.
        if (freeLook && lockCursorOnFreeLook && !_cursorLocked && _pressMoveDistance > ClickMoveThreshold)
            LockCursor();
        else if (!freeLook && _cursorLocked)
            UnlockCursor();

        // ---------- Вращение ----------
        if (orbit || freeLook)
        {
            float dx = Input.GetAxis("Mouse X") * mouseSensitivity;
            float dy = -Input.GetAxis("Mouse Y") * mouseSensitivity;
            _yawTarget += dx;
            _pitchTarget += dy;
            const float maxVel = 1500f; // град/с — чтобы рывок мыши в один кадр не раскручивал камеру
            _yawVel = Mathf.Clamp(Mathf.Lerp(_yawVel, dx / dt, 0.5f), -maxVel, maxVel);
            _pitchVel = Mathf.Clamp(Mathf.Lerp(_pitchVel, dy / dt, 0.5f), -maxVel, maxVel);
            _lastRotateWasFree = freeLook;
        }
        else if (useInertia)
        {
            _yawTarget += _yawVel * dt;
            _pitchTarget += _pitchVel * dt;
            float decay = Mathf.Exp(-inertiaDamping * dt);
            _yawVel *= decay;
            _pitchVel *= decay;
            if (Mathf.Abs(_yawVel) < 0.5f) _yawVel = 0f;
            if (Mathf.Abs(_pitchVel) < 0.5f) _pitchVel = 0f;
        }
        else
        {
            _yawVel = _pitchVel = 0f;
        }

        // ---------- Полёт на клавишах (разгон/торможение) ----------
        Quaternion rotNow = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 desiredMove = Vector3.zero;
        if (moveInput != Vector3.zero)
        {
            Vector3 dir = rotNow * new Vector3(moveInput.x, moveInput.y, moveInput.z);
            desiredMove = dir.normalized * CurrentMoveSpeed(shift);
        }
        if (useInertia || desiredMove != Vector3.zero)
            _moveVel = Vector3.Lerp(_moveVel, desiredMove, Factor(moveSmoothing, dt));
        else
            _moveVel = Vector3.zero;
        if (desiredMove == Vector3.zero && _moveVel.magnitude < CurrentMoveSpeed(false) * 0.01f)
            _moveVel = Vector3.zero;

        // Камера «стоит на месте» (а точка вращения перед ней) при осмотре, полёте и их инерции.
        bool rotationSettling = Mathf.Abs(_yawTarget - _yaw) > 0.01f || Mathf.Abs(_pitchTarget - _pitch) > 0.01f ||
                                _yawVel != 0f || _pitchVel != 0f;
        bool anchoredToCamera = freeLook || _moveVel != Vector3.zero || _dolly != Vector3.zero ||
                                (_lastRotateWasFree && rotationSettling && !orbit);

        Vector3 anchorPos = _pivot - Forward(_yaw, _pitch) * _dist;

        // ---------- Панорама ----------
        if (pan)
        {
            Vector3 deltaPx = mousePos - _lastMousePos;
            float upp = UnitsPerPixel();
            Vector3 delta = -(rotNow * Vector3.right * deltaPx.x + rotNow * Vector3.up * deltaPx.y) * upp;
            _pivotTarget += delta;
            if (anchoredToCamera) anchorPos += delta;
            _panVel = Vector3.Lerp(_panVel, delta / dt, 0.5f);
        }
        else if (useInertia && _panVel != Vector3.zero)
        {
            Vector3 delta = _panVel * dt;
            _pivotTarget += delta;
            if (anchoredToCamera) anchorPos += delta;
            _panVel *= Mathf.Exp(-inertiaDamping * dt);
            if (_panVel.magnitude < _dist * 0.01f) _panVel = Vector3.zero;
        }
        else
        {
            _panVel = Vector3.zero;
        }

        // ---------- Колесо ----------
        if (Mathf.Abs(scroll) > 1e-4f)
        {
            float factor = Mathf.Pow(1f - zoomStepPercent / 100f, scroll / WheelNotch);
            float newDist = _distTarget * factor;
            if (factor < 1f) newDist = Mathf.Max(newDist, Mathf.Min(orbitMinDistance, _distTarget));
            else newDist = Mathf.Min(newDist, Mathf.Max(orbitMaxDistance, _distTarget));

            if (anchoredToCamera)
            {
                // При осмотре/полёте колесо двигает камеру вдоль луча под курсором.
                Vector3 rayDir = CursorRay().direction;
                _dolly += rayDir * (_distTarget - newDist);
            }
            else
            {
                float f = _distTarget > 1e-6f ? newDist / _distTarget : 1f;
                if (zoomToCursor)
                {
                    Vector3 p = CursorPoint();
                    _pivotTarget = p + (_pivotTarget - p) * f;
                }
                _distTarget = newDist;
            }
        }

        // ---------- Ограничения целей ----------
        _pitchTarget = Mathf.Clamp(_pitchTarget, -verticalLimit, verticalLimit);
        if (!anchoredToCamera)
        {
            // Не даём орбите опустить камеру ниже minWorldY: ограничиваем угол, а не позицию —
            // иначе камера смотрела не туда, куда указывают углы, и на следующем движении прыгала.
            float s = _distTarget > 1e-6f ? (minWorldY - _pivotTarget.y) / _distTarget : -1f;
            if (s > -1f && s < 1f)
            {
                float pitchMin = Mathf.Asin(s) * Mathf.Rad2Deg;
                if (pitchMin < verticalLimit && _pitchTarget < pitchMin)
                {
                    _pitchTarget = pitchMin;
                    if (_pitchVel < 0f) _pitchVel = 0f;
                }
            }
        }
        if (Mathf.Abs(_pitchTarget) >= verticalLimit - 1e-3f) _pitchVel = 0f;

        // ---------- Сглаживание ----------
        float kRot = Factor(_transition ? transitionSmoothing : rotationSmoothing, dt);
        float kZoom = Factor(_transition ? transitionSmoothing : zoomSmoothing, dt);

        _yaw = Mathf.Lerp(_yaw, _yawTarget, kRot);
        _pitch = Mathf.Lerp(_pitch, _pitchTarget, kRot);
        _dist = Mathf.Lerp(_dist, _distTarget, kZoom);

        if (anchoredToCamera)
        {
            anchorPos += _moveVel * dt;
            if (_dolly != Vector3.zero)
            {
                Vector3 step = _dolly * kZoom;
                anchorPos += step;
                _dolly -= step;
                if (_dolly.magnitude < _dist * 1e-4f) _dolly = Vector3.zero;
            }
            anchorPos = ClampToBounds(anchorPos, _pivot - Forward(_yaw, _pitch) * _dist, cameraBounds: true);
            if (anchorPos.y < minWorldY) anchorPos.y = minWorldY;
            _pivot = anchorPos + Forward(_yaw, _pitch) * _dist;
            _pivotTarget = anchorPos + Forward(_yawTarget, _pitchTarget) * _distTarget;
        }
        else
        {
            _pivotTarget = ClampToBounds(_pivotTarget, _pivot, cameraBounds: false);
            _pivot = Vector3.Lerp(_pivot, _pivotTarget, kZoom);
        }

        // Осмотр/полёт закончились — переносим точку вращения на поверхность перед камерой,
        // чтобы следующая орбита и проверка препятствий шли от видимого объекта, а не из-за стены.
        if (avoidObstacles && _wasAnchoredToCamera && !anchoredToCamera)
            ReanchorPivotToSurface();
        _wasAnchoredToCamera = anchoredToCamera;

        bool settled = SnapIfSettled();
        if (_transition && settled) _transition = false;

        bool active = userInput || !settled || _collisionMoving;

        // Применяем и в кадр, когда движение закончилось: CameraRotationAFK в этом кадре ещё видел
        // isMouseMoving == true и мог сбросить поворот камеры.
        if (active || _wasActive)
            Apply(anchoredToCamera, dt);

        isMouseMoving = active;
        _wasActive = active;
        _lastMousePos = mousePos;
    }

    // ================= Публичные действия (можно вешать на кнопки UI) =================

    /// <summary>Плавно вернуться к виду при запуске сцены.</summary>
    public void ResetView()
    {
        if (!TryInitialize()) return;
        StartTransition(_homePivot, _homeYaw, _homePitch, _homeDist);
    }

    public void ViewFront()
    {
        if (!TryInitialize()) return;
        StartTransition(_homePivot, _homeYaw, 10f, _homeDist);
    }

    public void ViewTop()
    {
        if (!TryInitialize()) return;
        StartTransition(_homePivot, _homeYaw, Mathf.Min(89f, verticalLimit), _homeDist);
    }

    public void ViewIso()
    {
        if (!TryInitialize()) return;
        StartTransition(_homePivot, _homeYaw + 45f, Mathf.Min(35f, verticalLimit), _homeDist);
    }

    /// <summary>Перелёт к точке обзора из списка viewPoints (камера встаёт так же, как повёрнут объект).</summary>
    public void GoToViewPoint(int index)
    {
        if (!TryInitialize()) return;
        if (viewPoints == null || index < 0 || index >= viewPoints.Length || viewPoints[index] == null) return;

        Transform t = viewPoints[index];
        Vector3 e = t.rotation.eulerAngles;
        float pitch = e.x > 180f ? e.x - 360f : e.x;
        float dist = Mathf.Clamp(_distTarget, orbitMinDistance, orbitMaxDistance);
        StartTransition(t.position + t.forward * dist, e.y, pitch, dist);
    }

    /// <summary>Подлететь к объекту под курсором.</summary>
    public void FocusUnderCursor()
    {
        if (!TryInitialize() || _camera == null) return;
        if (!RaycastScene(CursorRay(), Mathf.Infinity, out RaycastHit hit)) return;

        Bounds b = GetFocusBounds(hit.collider);
        if (b.extents.magnitude * focusDistanceMultiplier > orbitMaxDistance)
        {
            // Слишком большой объект (пол, здание целиком) — просто переносим точку вращения туда, куда кликнули.
            StartTransition(hit.point, _yawTarget, _pitchTarget, _distTarget);
            return;
        }
        FocusOn(b);
    }

    /// <summary>Подлететь к объекту (по его габаритам).</summary>
    public void FocusOn(GameObject target)
    {
        if (target == null || !TryInitialize()) return;
        bool any = false;
        Bounds b = default;
        foreach (var r in target.GetComponentsInChildren<Renderer>())
        {
            if (!IsModelRenderer(r)) continue;
            if (!any) { b = r.bounds; any = true; } else b.Encapsulate(r.bounds);
        }
        if (!any) b = new Bounds(target.transform.position, Vector3.zero);
        FocusOn(b);
    }

    public void FocusOn(Bounds bounds)
    {
        if (!TryInitialize()) return;
        float radius = bounds.extents.magnitude;
        float dist = radius > 1e-4f ? radius * focusDistanceMultiplier : _distTarget;
        dist = Mathf.Clamp(dist, orbitMinDistance, orbitMaxDistance);

        if (_hasBounds) _sceneBounds.Encapsulate(bounds);
        StartTransition(bounds.center, _yawTarget, _pitchTarget, dist);
    }

    // ================= Инициализация и синхронизация =================

    private bool TryInitialize()
    {
        if (cam == null)
        {
            if (Camera.main == null) return false;
            cam = Camera.main.transform;
        }
        if (_camera == null)
        {
            var c = cam.GetComponent<Camera>();
            _camera = c != null ? c : Camera.main;
        }

        if (_initialized) return true;

        InitFromCamera(useTarget: true);
        _homePivot = _pivotTarget;
        _homeYaw = _yawTarget;
        _homePitch = _pitchTarget;
        _homeDist = _distTarget;
        _startDistance = Mathf.Max(_distTarget, 1e-3f);
        _lastMousePos = Input.mousePosition;
        _initialized = true;
        return true;
    }

    private void InitFromCamera(bool useTarget)
    {
        Vector3 camPos = cam.position;
        Vector3 forward = cam.forward;
        Vector3 e = cam.eulerAngles;
        float yaw = e.y;
        float pitch = e.x > 180f ? e.x - 360f : e.x;
        float dist = _dist > 1e-4f ? _dist : Mathf.Max(orbitMinDistance, 1f);
        Vector3 pivot;

        if (useTarget && orbitTarget != null && (orbitTarget.position - camPos).sqrMagnitude > 1e-8f)
        {
            Vector3 toTarget = orbitTarget.position - camPos;
            dist = toTarget.magnitude;
            if (Vector3.Angle(forward, toTarget) < 5f)
            {
                // Камера смотрит на цель — вращаемся точно вокруг неё.
                pivot = orbitTarget.position;
                Vector3 look = Quaternion.LookRotation(toTarget).eulerAngles;
                yaw = look.y;
                pitch = look.x > 180f ? look.x - 360f : look.x;
            }
            else
            {
                // Камера смотрит мимо цели — точку берём перед камерой на той же дистанции, без рывка.
                pivot = camPos + forward * dist;
            }
        }
        else
        {
            pivot = camPos + forward * dist;
        }

        pitch = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);

        _yaw = _yawTarget = yaw;
        _pitch = _pitchTarget = pitch;
        _dist = _distTarget = dist;
        _pivot = _pivotTarget = pivot;
        _yawVel = _pitchVel = 0f;
        _panVel = _moveVel = _dolly = Vector3.zero;
        _clipped = false;
        _transition = false;
        _wasAnchoredToCamera = false;

        _knownPos = camPos;
        _knownRot = cam.rotation;
    }

    /// <summary>
    /// Если камеру сдвинул другой скрипт (автоповорот CameraRotationAFK и т.п.), продолжаем от её
    /// текущего положения, а не прыгаем к старому.
    /// </summary>
    private void SyncWithExternalChanges()
    {
        bool moved = (cam.position - _knownPos).sqrMagnitude > 1e-6f * Mathf.Max(1f, _dist * _dist) ||
                     Quaternion.Angle(cam.rotation, _knownRot) > 0.05f;
        if (moved) InitFromCamera(useTarget: false);

        RefreshSceneBounds();
    }

    // ================= Применение =================

    private void Apply(bool anchoredToCamera, float dt)
    {
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 back = rot * Vector3.back;
        float dist = _dist;
        float before = _clipped ? _collisionDist : _dist;

        if (avoidObstacles && !anchoredToCamera)
        {
            float target = ObstacleDistance(_pivot, back, _dist, out bool hit);
            if (hit)
            {
                // Подходим ближе сразу, отходим плавно.
                _collisionDist = (!_clipped || target < _collisionDist)
                    ? target
                    : Mathf.Lerp(_collisionDist, target, Factor(zoomSmoothing, dt));
                _clipped = true;
            }
            else if (_clipped)
            {
                _collisionDist = Mathf.Lerp(_collisionDist, _dist, Factor(zoomSmoothing, dt));
                if (Mathf.Abs(_collisionDist - _dist) <= _dist * 0.002f) _clipped = false;
            }
            if (_clipped) dist = Mathf.Min(_collisionDist, _dist);
        }
        else
        {
            _clipped = false;
        }
        // Камера ещё отъезжает от препятствия — продолжаем обновлять, даже если пользователь ничего не делает.
        float after = _clipped ? _collisionDist : _dist;
        _collisionMoving = _clipped && Mathf.Abs(after - before) > Mathf.Max(_dist, 1e-3f) * 1e-4f;

        Vector3 pos = _pivot + back * dist;

        // Страховка от ухода под minWorldY: поднимаем камеру вместе с точкой — ориентация не меняется.
        if (pos.y < minWorldY)
        {
            float up = minWorldY - pos.y;
            pos.y += up;
            _pivot.y += up;
            _pivotTarget.y += up;
        }

        cam.SetPositionAndRotation(pos, rot);
        _knownPos = pos;
        _knownRot = rot;
    }

    private bool SnapIfSettled()
    {
        float distEps = Mathf.Max(_dist, 1e-3f) * 1e-4f;
        bool settled =
            Mathf.Abs(_yawTarget - _yaw) < 0.01f &&
            Mathf.Abs(_pitchTarget - _pitch) < 0.01f &&
            Mathf.Abs(_distTarget - _dist) < distEps &&
            (_pivotTarget - _pivot).sqrMagnitude < distEps * distEps &&
            _yawVel == 0f && _pitchVel == 0f &&
            _panVel == Vector3.zero && _moveVel == Vector3.zero && _dolly == Vector3.zero;

        if (settled)
        {
            _yaw = _yawTarget;
            _pitch = _pitchTarget;
            _dist = _distTarget;
            _pivot = _pivotTarget;
        }
        return settled;
    }

    private void StartTransition(Vector3 pivot, float yaw, float pitch, float dist)
    {
        if (!_wasActive) SyncWithExternalChanges();

        _pivotTarget = ClampToBounds(pivot, _pivot, cameraBounds: false);
        _yawTarget = _yaw + Mathf.DeltaAngle(_yaw, yaw); // кратчайший поворот
        _pitchTarget = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);
        _distTarget = Mathf.Max(dist, 1e-3f);
        _yawVel = _pitchVel = 0f;
        _panVel = _moveVel = _dolly = Vector3.zero;
        _lastRotateWasFree = false;
        _transition = true;
    }

    private void ReanchorPivotToSurface()
    {
        Vector3 camPos = _pivot - Forward(_yaw, _pitch) * _dist;
        Ray ray = new Ray(camPos, Forward(_yaw, _pitch));
        if (RaycastScene(ray, _distTarget, out RaycastHit hit) && hit.distance > 1e-3f)
        {
            _dist = _distTarget = hit.distance;
            _pivot = _pivotTarget = camPos + ray.direction * hit.distance;
        }
    }

    // ================= Ввод =================

    private void RefreshPressedButtons()
    {
        // Страховка: если PointerUp не пришёл (окно потеряло фокус и т.п.), снимаем кнопку по факту.
        if (_pressed.Count == 0) return;
        if (!Input.GetMouseButton(0)) _pressed.Remove(PointerEventData.InputButton.Left);
        if (!Input.GetMouseButton(1)) _pressed.Remove(PointerEventData.InputButton.Right);
        if (!Input.GetMouseButton(2)) _pressed.Remove(PointerEventData.InputButton.Middle);
    }

    private static Vector3 ReadMoveInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float y = (Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f);
        return new Vector3(h, y, v);
    }

    private static bool AnyHotkeyDown()
    {
        if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.F)) return true;
        for (KeyCode k = KeyCode.Alpha1; k <= KeyCode.Alpha9; k++)
            if (Input.GetKeyDown(k)) return true;
        return false;
    }

    private void HandleHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Home)) ResetView();
        if (Input.GetKeyDown(KeyCode.F)) FocusUnderCursor();
        if (Input.GetKeyDown(KeyCode.Alpha1)) ViewFront();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ViewTop();
        if (Input.GetKeyDown(KeyCode.Alpha3)) ViewIso();
        for (int i = 0; i < 6; i++)
            if (Input.GetKeyDown(KeyCode.Alpha4 + i)) GoToViewPoint(i);
    }

    /// <summary>Идёт ввод в текстовое поле — клавиши камеры не трогаем.</summary>
    private static bool IsTypingInInputField()
    {
        var es = EventSystem.current;
        if (es == null || es.currentSelectedGameObject == null) return false;
        var go = es.currentSelectedGameObject;
        var tmp = go.GetComponent<TMP_InputField>();
        if (tmp != null && tmp.isFocused) return true;
        var legacy = go.GetComponent<InputField>();
        return legacy != null && legacy.isFocused;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cursorLocked = true;
    }

    private void UnlockCursor()
    {
        if (!_cursorLocked) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _cursorLocked = false;
    }

    // ================= Вспомогательное =================

    private static float Factor(float smoothing, float dt) =>
        smoothing <= 0f ? 1f : 1f - Mathf.Exp(-smoothing * dt);

    private static Vector3 Forward(float yaw, float pitch) => Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;

    private float CurrentMoveSpeed(bool fast)
    {
        float speed = moveSpeed * (fast ? fastMoveMultiplier : 1f);
        if (scaleSpeedWithDistance)
            speed *= Mathf.Clamp(_dist / _startDistance, 0.05f, 20f);
        return speed;
    }

    /// <summary>Сколько мировых единиц в одном пикселе на дистанции точки вращения (для панорамы «за курсором»).</summary>
    private float UnitsPerPixel()
    {
        if (_camera == null) return _dist * 0.002f;
        float h = Mathf.Max(_camera.pixelHeight, 1);
        if (_camera.orthographic) return 2f * _camera.orthographicSize / h;
        return 2f * _dist * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad) / h;
    }

    private Ray CursorRay()
    {
        if (_camera != null) return _camera.ScreenPointToRay(Input.mousePosition);
        return new Ray(cam.position, cam.forward);
    }

    private Vector3 CursorPoint()
    {
        Ray ray = CursorRay();
        if (RaycastScene(ray, _dist * 20f, out RaycastHit hit)) return hit.point;
        return ray.GetPoint(_dist);
    }

    /// <summary>Луч по статичным объектам сцены (без триггеров, летящих молекул и т.п.).</summary>
    private bool RaycastScene(Ray ray, float maxDistance, out RaycastHit best)
    {
        best = default;
        int n = Physics.RaycastNonAlloc(ray, _hits, maxDistance, obstacleMask, QueryTriggerInteraction.Ignore);
        float bestDist = float.MaxValue;
        bool found = false;
        for (int i = 0; i < n; i++)
        {
            var h = _hits[i];
            if (!IsStaticObstacle(h.collider)) continue;
            if (h.distance < bestDist)
            {
                bestDist = h.distance;
                best = h;
                found = true;
            }
        }
        return found;
    }

    private float ObstacleDistance(Vector3 from, Vector3 dir, float maxDist, out bool hit)
    {
        hit = false;
        float radius = Mathf.Max(_camera != null ? _camera.nearClipPlane * 2f : 0.1f, maxDist * 0.01f);
        int n = Physics.SphereCastNonAlloc(from, radius, dir, _hits, maxDist, obstacleMask, QueryTriggerInteraction.Ignore);
        float best = maxDist;
        for (int i = 0; i < n; i++)
        {
            var h = _hits[i];
            if (h.distance <= 0f) continue; // объект, внутри которого стоит точка вращения
            if (!IsStaticObstacle(h.collider)) continue;
            if (h.distance < best)
            {
                best = h.distance;
                hit = true;
            }
        }
        return Mathf.Max(best, radius);
    }

    private bool IsStaticObstacle(Collider c)
    {
        if (c == null) return false;
        if (c.transform.IsChildOf(cam)) return false;
        var rb = c.attachedRigidbody;
        return rb == null || rb.isKinematic; // молекулы и прочие летящие тела не мешают камере
    }

    private Bounds GetFocusBounds(Collider c)
    {
        Bounds b = c.bounds;
        foreach (var r in c.GetComponentsInChildren<Renderer>())
            if (IsModelRenderer(r)) b.Encapsulate(r.bounds);
        return b;
    }

    private static bool IsModelRenderer(Renderer r) =>
        r != null && r.enabled && (r is MeshRenderer || r is SkinnedMeshRenderer) && r.gameObject.layer != 5;

    private void RefreshSceneBounds()
    {
        if (!limitToSceneBounds) { _hasBounds = false; return; }
        if (_hasBounds && Time.unscaledTime - _boundsTime < 10f) return;
        _boundsTime = Time.unscaledTime;

        bool any = false;
        Bounds b = default;
        foreach (var r in FindObjectsOfType<Renderer>())
        {
            if (!IsModelRenderer(r) || r.transform.IsChildOf(cam)) continue;
            if (!any) { b = r.bounds; any = true; } else b.Encapsulate(r.bounds);
        }
        if (!any) { _hasBounds = false; return; }

        b.Encapsulate(_pivotTarget);
        b.Encapsulate(_homePivot);
        _sceneBounds = b;
        _hasBounds = true;
    }

    /// <summary>
    /// Ограничение точки (или камеры) границами сцены. Если точка уже снаружи, её не дёргает обратно —
    /// просто не даёт уйти ещё дальше.
    /// </summary>
    private Vector3 ClampToBounds(Vector3 p, Vector3 previous, bool cameraBounds)
    {
        if (!limitToSceneBounds || !_hasBounds) return p;

        Vector3 size = _sceneBounds.size;
        float pad = Mathf.Max(size.x, size.y, size.z) * boundsPadding;
        if (cameraBounds) pad += orbitMaxDistance;

        Vector3 lo = _sceneBounds.min - Vector3.one * pad;
        Vector3 hi = _sceneBounds.max + Vector3.one * pad;
        for (int i = 0; i < 3; i++)
            p[i] = Mathf.Clamp(p[i], Mathf.Min(lo[i], previous[i]), Mathf.Max(hi[i], previous[i]));
        return p;
    }
}
