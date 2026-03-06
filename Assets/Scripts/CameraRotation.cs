using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRotation: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
    
    private float rotX;
    private float rotY;
    private float orbitDistance;
    private bool isPointerDown = false;

    void Start()
    {
        if (cam == null) cam = Camera.main.transform;

        if (orbitTarget != null)
            orbitDistance = Vector3.Distance(cam.position, orbitTarget.position);
        
        Vector3 angles = cam.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;
    }

    public void OnPointerDown(PointerEventData eventData) => isPointerDown = true;
    public void OnPointerUp(PointerEventData eventData) => isPointerDown = false;

    void Update()
    {
        if (!isPointerDown || cam == null) return;

        bool isMoving = HasMovementInput();

        if (Input.GetMouseButton(0))
        {
            isMouseMoving = true;
            if (isMoving)
            {
                // ЛКМ + Кнопки: Вращение вокруг себя (Look Around)
                RotateFree();
            }
            else
            {
                // Только ЛКМ: Вращение вокруг орбиты
                OrbitRotate();
            }
        }
        
        if (isMoving)
        {
            FreeMove();
        }

        HandleZoom();
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
    // Вращение вокруг собственной оси (режим Look Around)
    void RotateFree()
    {
        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, -verticalLimit, verticalLimit);

        cam.rotation = Quaternion.Euler(rotY, rotX, 0);

        // Чтобы после вращения "вокруг себя" точка орбиты оказалась спереди
        if (orbitTarget != null)
        {
            orbitTarget.position = cam.position + cam.forward * orbitDistance;
        }
    }

    // Вращение вокруг целевого объекта (режим Orbit)
    void OrbitRotate()
    {
        if (orbitTarget == null) return;

        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, -verticalLimit, verticalLimit);

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        cam.position = orbitTarget.position + rotation * new Vector3(0, 0, -orbitDistance);
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

        cam.position += dir * (currentSpeed * Time.deltaTime);

        // Двигаем точку орбиты за камерой
        if (orbitTarget != null)
        {
            orbitTarget.position = cam.position + cam.forward * orbitDistance;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.001f) return;

        orbitDistance -= scroll * orbitZoomSpeed;
        orbitDistance = Mathf.Clamp(orbitDistance, orbitMinDistance, orbitMaxDistance);

        // Обновляем позицию при зуме, если мы в режиме орбиты
        if (orbitTarget != null && !HasMovementInput())
        {
            Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
            cam.position = orbitTarget.position + rotation * new Vector3(0, 0, -orbitDistance);
        }
    }
}