using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRotation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public Transform cam;
    public Transform orbitTarget;

    [Header("Sensitivity & Limits")]
    public float mouseSensitivity = 3f;
    public float verticalLimit = 85f;

    [Header("Orbit Settings")]
    public float orbitZoomSpeed = 5f;
    public float orbitMinDistance = 2f;
    public float orbitMaxDistance = 100f;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float fastMoveMultiplier = 3f;
    public bool isMouseMoving = false;

    [Header("Height Limit")]
    public float minWorldY = -1f;

    private float rotX;
    private float rotY;
    private float orbitDistance;
    private bool isPointerDown = false;
    private bool wasMovingLastFrame = false;

    void Start()
    {
        if (cam == null) cam = Camera.main.transform;

        if (orbitTarget != null)
            orbitDistance = Vector3.Distance(cam.position, orbitTarget.position);

        Vector3 angles = cam.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;
        if (rotY > 180f) rotY -= 360f;
    }

    public void OnPointerDown(PointerEventData eventData) => isPointerDown = true;
    public void OnPointerUp(PointerEventData eventData) => isPointerDown = false;

    void Update()
    {
        if (!isPointerDown || cam == null) return;

        bool isMoving = HasMovementInput();

        if (!isMoving && wasMovingLastFrame && orbitTarget != null)
        {
            RecalculateOrbitFromCurrentPosition();
        }

        // ЛКМ — орбита вокруг цели
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            isMouseMoving = true;
            OrbitRotate();
        }

        // ПКМ — вращение вокруг своей оси (free look)
        if (Input.GetMouseButton(1))
        {
            isMouseMoving = true;
            RotateFree();
        }

        if (isMoving)
        {
            FreeMove();
        }

        HandleZoom();
        ClampCameraHeight();

        wasMovingLastFrame = isMoving;
    }

    void ClampCameraHeight()
    {
        Vector3 pos = cam.position;
        if (pos.y < minWorldY)
        {
            cam.position = new Vector3(pos.x, minWorldY, pos.z);

            // Если камера упёрлась в пол во время орбиты — пересчитываем орбиту,
            // чтобы не было рывка при следующем движении
            if (orbitTarget != null)
                orbitTarget.position = cam.position + cam.forward * orbitDistance;
        }
    }

    void RecalculateOrbitFromCurrentPosition()
    {
        orbitTarget.position = cam.position + cam.forward * orbitDistance;

        Vector3 angles = cam.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;
        if (rotY > 180f) rotY -= 360f;
    }

    bool HasMovementInput()
    {
        return Input.GetAxisRaw("Horizontal") != 0 ||
               Input.GetAxisRaw("Vertical") != 0 ||
               Input.GetKey(KeyCode.E) ||
               Input.GetKey(KeyCode.Q);
    }

    public void GetMouseOff()
    {
        isMouseMoving = false;
    }

    void RotateFree()
    {
        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, -verticalLimit, verticalLimit);

        cam.rotation = Quaternion.Euler(rotY, rotX, 0);

        if (orbitTarget != null)
            orbitTarget.position = cam.position + cam.forward * orbitDistance;
    }

    void OrbitRotate()
    {
        if (orbitTarget == null) return;

        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, -verticalLimit, verticalLimit);

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 newPos = orbitTarget.position + rotation * new Vector3(0, 0, -orbitDistance);

        // Не применяем позицию если она ниже минимума
        if (newPos.y >= minWorldY)
        {
            cam.position = newPos;
        }
        else
        {
            cam.position = new Vector3(newPos.x, minWorldY, newPos.z);
        }

        cam.LookAt(orbitTarget);
    }

    void FreeMove()
    {
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= fastMoveMultiplier;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = cam.forward * v + cam.right * h;
        if (Input.GetKey(KeyCode.E)) dir += cam.up;
        if (Input.GetKey(KeyCode.Q)) dir -= cam.up;

        Vector3 newPos = cam.position + dir.normalized * (currentSpeed * Time.deltaTime);
        newPos.y = Mathf.Max(newPos.y, minWorldY);
        cam.position = newPos;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.001f) return;

        orbitDistance -= scroll * orbitZoomSpeed;
        orbitDistance = Mathf.Clamp(orbitDistance, orbitMinDistance, orbitMaxDistance);

        if (orbitTarget != null && !HasMovementInput())
        {
            Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
            Vector3 newPos = orbitTarget.position + rotation * new Vector3(0, 0, -orbitDistance);
            newPos.y = Mathf.Max(newPos.y, minWorldY);
            cam.position = newPos;
        }
    }
}