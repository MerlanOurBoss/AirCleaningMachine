using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFacilities : MonoBehaviour
{
    public bool isDragging = false;
    [SerializeField] private LayerMask parentLayer;
    [SerializeField] private LayerMask ignoreLayer;

    [SerializeField] private bool isSelected = true;
    private Collider objectCollider;
    private Camera mainCamera;
    public bool isConnected = false;

    public Vector3 mouseOffset = new Vector3(1.0f, 1.0f, 0.0f);

    private string startTag;
    private ChamgingEmul emulType;

    // Подсказка эмульгатора общая на сцену: показываем её, если выбран хотя бы один эмульгатор.
    // Раньше каждый эмульгатор каждый кадр включал/выключал её по-своему, и при двух — она мигала.
    private static readonly HashSet<MovingFacilities> s_selectedEmuls = new HashSet<MovingFacilities>();
    private static FindEmulTag s_emulTip;

    void Start()
    {
        objectCollider = GetComponent<Collider>();
        mainCamera = Camera.main;
        emulType = GetComponent<ChamgingEmul>();

        startTag = gameObject.tag;
    }

    private void OnDisable()
    {
        if (s_selectedEmuls.Remove(this)) UpdateEmulTip();
    }

    private static void UpdateEmulTip()
    {
        if (s_emulTip == null) s_emulTip = FindAnyObjectByType<FindEmulTag>();
        if (s_emulTip != null && s_emulTip.tip != null)
            s_emulTip.tip.SetActive(s_selectedEmuls.Count > 0);
    }
    // Camera.main бывает null (камера выключена или ещё не создана) — тогда кадр пропускаем вместо NullReferenceException.
    private Camera ActiveCamera()
    {
        if (mainCamera == null || !mainCamera.isActiveAndEnabled) mainCamera = Camera.main;
        return mainCamera;
    }

    bool IsChildCollider(Collider collider)
    {
        Transform currentTransform = collider.transform;

        while (currentTransform != null)
        {
            if (currentTransform.parent == null)
            {
                return false;
            }
            else if (currentTransform.parent == transform)
            {
                return true;
            }

            currentTransform = currentTransform.parent;
        }

        return false;
    }

    void Update()
    {
        Camera cam = ActiveCamera();

        if (Input.GetMouseButtonDown(0) && cam != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, parentLayer))
            {
                if (!IsChildCollider(hit.collider))
                {
                    if (hit.transform == transform && (gameObject.CompareTag(Tags.Facilities) || gameObject.CompareTag(Tags.Facilities_Capsul) || gameObject.CompareTag(Tags.Facilities_Steam) 
                        || gameObject.CompareTag(Tags.Facilities_DryAir) || gameObject.CompareTag(Tags.Facilities_Oven) || gameObject.CompareTag(Tags.Facilities_Emul) 
                        || gameObject.CompareTag(Tags.Facilities_Cool) || gameObject.CompareTag(Tags.Facilities_Electro) || gameObject.CompareTag(Tags.Facilities_Kataz) || gameObject.CompareTag(Tags.Facilities_NewCapsul)
                        || gameObject.CompareTag(Tags.Facilities_Emul_Reagent) || gameObject.CompareTag(Tags.Facilities_Emul_Soda)))
                    {
                        isSelected = true;
                        isDragging = !isDragging;

                        Vector3 mousePosition = Input.mousePosition + mouseOffset;
                        mousePosition.z = cam.WorldToScreenPoint(transform.position).z;
                    }
                    else
                    {
                        isSelected = false;
                        isDragging = false;
                    }
                }
            }
            else
            {
                isSelected = false;
                isDragging = false;
            }
        }
        if ( Input.GetKey(KeyCode.R))
        {
            gameObject.tag = startTag;
            if (objectCollider != null) objectCollider.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (emulType != null && isSelected)
            {
                emulType.ChangingE();
            }
        }
        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(0f, -90f, 0f);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(0f, 90f, 0f);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            isSelected = false;
            isDragging = false;
            if (objectCollider != null) objectCollider.enabled = false;
            //gameObject.tag = Tags.Untagged;
        }

        if (emulType != null)
        {
            bool changed = isSelected ? s_selectedEmuls.Add(this) : s_selectedEmuls.Remove(this);
            if (changed) UpdateEmulTip();
        }

        if (isDragging && cam != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cam.WorldToScreenPoint(transform.position).z;
            Vector3 newPosition = cam.ScreenToWorldPoint(mousePosition);

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                newPosition.x = transform.position.x; // Keep the original x position
                newPosition.z = transform.position.z; // Keep the original z position
            }
            else
            {
                newPosition.y = transform.position.y; // Keep the original y position
            }

            transform.position = newPosition;

        }


    }
    public void ToggleCollider(bool enabled)
    {
        if (objectCollider != null)
        {
            objectCollider.enabled = enabled;
        }
    }
}
