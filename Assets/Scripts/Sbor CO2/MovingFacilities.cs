using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFacilities : MonoBehaviour
{
    public bool isDragging = false;
    public LayerMask parentLayer;
    public LayerMask ignoreLayer;

    public bool isSelected = true;
    private Collider objectCollider;
    private Camera mainCamera;
    public bool isConnected = false;

    public Vector3 mouseOffset = new Vector3(1.0f, 1.0f, 0.0f);

    private string startTag;
    void Start()
    {
        objectCollider = GetComponent<Collider>();
        mainCamera = Camera.main;

        startTag = gameObject.tag;


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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, parentLayer))
            {
                if (!IsChildCollider(hit.collider))
                {
                    if (hit.transform == transform && (gameObject.tag == "Facilities" || gameObject.tag == "Facilities_Capsul" || gameObject.tag == "Facilities_Steam" 
                        || gameObject.tag == "Facilities_DryAir" || gameObject.tag == "Facilities_Oven" || gameObject.tag == "Facilities_Emul" 
                        || gameObject.tag == "Facilities_Cool" || gameObject.tag == "Facilities_Electro" || gameObject.tag == "Facilities_Kataz" || gameObject.tag == "Facilities_NewCapsul"
                        || gameObject.tag == "Facilities_Emul_Reagent" || gameObject.tag == "Facilities_Emul_Soda"))
                    {
                        isSelected = true;
                        isDragging = !isDragging;

                        Vector3 mousePosition = Input.mousePosition + mouseOffset;
                        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
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
            gameObject.GetComponent<Collider>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (gameObject.GetComponent<ChamgingEmul>()  != null && isSelected == true)
            {
                gameObject.GetComponent<ChamgingEmul>().ChangingE();
            }
                
        }
        if (isSelected)
        {
            if (gameObject.GetComponent<ChamgingEmul>() != null )
            {
                FindAnyObjectByType<FindEmulTag>().tip.SetActive(true);
            }
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
            gameObject.GetComponent<Collider>().enabled = false;
            //gameObject.tag = "Untagged";
        }
        if (!isSelected)
        {
            if (gameObject.GetComponent<ChamgingEmul>() != null )
            {
                FindAnyObjectByType<FindEmulTag>().tip.SetActive(false);
            }
        }

        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
            Vector3 newPosition = mainCamera.ScreenToWorldPoint(mousePosition);

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
