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

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            if (other.CompareTag(Tags.ConnectionPoint))
            {
                if (!isMouseOverTrigger)
                {
                    Debug.Log("connected for " + other.gameObject.name);
                    isActivated = true;
                }
            }
        }
    }
}
