using UnityEditor;
using UnityEngine;


public class PipeConnection : MonoBehaviour
{
    public Transform entrance;
    public bool isConnected = false;

    public bool isClicked = false;
    public bool isActivated = false;

    private bool isMouseOverTrigger = false;
    private void Update()
    {
        if (isClicked)
        {
            FindSelectedObject();
            FindSelectedFacilities();
        }
        if (isConnected)
        {
            isClicked = false;
        }
    }

    void OnMouseEnter()
    {
        isMouseOverTrigger = true;
        if (!isConnected)
        {
            isClicked = true;
        }
        Debug.Log("mouse entered");
    }

    void OnMouseExit()
    {
        isMouseOverTrigger = false;
        isClicked = false;
        isConnected = false;
    }
    void FindSelectedObject()
    {
        MoveObjectWithMouse[] moveScript = FindObjectsOfType<MoveObjectWithMouse>();

        foreach (MoveObjectWithMouse obj in moveScript)
        {
            MoveObjectWithMouse script = obj.GetComponent<MoveObjectWithMouse>(); 

            if (script != null && script.isSelected && script.isActivated == false)
            {
                HandlePipe(obj.gameObject, 0, 0, 0, 0, 0, 0, "pipe");
                return;
            }
        }
    }

    void FindSelectedFacilities()
    {
        MovingFacilities[] moveFace = FindObjectsOfType<MovingFacilities>();

        foreach (MovingFacilities obj in moveFace)
        {
            if (obj != null && obj.isDragging)
            {
                HandleFacilities(obj.gameObject, gameObject.transform.position.x, obj.transform.position.y, gameObject.transform.position.z, 0, 0, 0, "pipe");
                return;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            if (other.CompareTag("ConnectionPoint"))
            {
                if (!isMouseOverTrigger)
                {
                    Debug.Log("connected for " + other.gameObject.name);
                    //this.enabled = false;
                    gameObject.GetComponent<Collider>().enabled = false;
                    gameObject.tag = "Untagged";
                    PipeConnection otherScript = other.GetComponent<PipeConnection>();
                    Collider collider = other.GetComponent<Collider>();
                    
                    Transform parentTransform = other.transform.parent;
                    MoveObjectWithMouse mv = parentTransform.GetComponent<MoveObjectWithMouse>();
                    if (mv != null) {
                        parentTransform.GetComponent<MoveObjectWithMouse>().isConnected = true;
                    }

                    Transform parentTranfrom1 = transform.parent;
                    MoveObjectWithMouse mv1 = parentTranfrom1.GetComponent<MoveObjectWithMouse>();
                    if (mv1 != null) {
                        parentTranfrom1.GetComponent<MoveObjectWithMouse>().isConnected = true;
                    }
                    
                    isActivated = true;
                    if (otherScript != null)
                    {
                        //otherScript.enabled = false;
                        collider.enabled = false;
                        other.tag = "Untagged";
                    }
                }
            }                
        }
    }
    private void HandlePipe(GameObject other, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, string debugMessage)
    {
        Debug.Log(debugMessage);
        other.transform.SetParent(transform);
        other.transform.localPosition = new Vector3(posX,posY,posZ);
        other.transform.localScale = new Vector3(1,1,1);
        other.GetComponent<MoveObjectWithMouse>().isDragging = false;
        Destroy(other.GetComponent<BoxCollider>());
        other.gameObject.tag = "Untagged";
        isClicked = false;
        isConnected = true;
    }

    private void HandleFacilities(GameObject other, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, string debugMessage)
    {
        Debug.Log(debugMessage);
        //other.transform.SetParent(transform);
        other.transform.position = new Vector3(posX, posY, posZ);
        other.transform.localScale = new Vector3(1, 1, 1);
        other.GetComponent<MovingFacilities>().isDragging = false;
        //Destroy(other.GetComponent<BoxCollider>());
        //other.gameObject.tag = "Untagged";
        isClicked = false;
        isConnected = true;
    }
}
