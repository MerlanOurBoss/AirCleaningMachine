using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


public class PipeConnection : MonoBehaviour
{
    public Transform entrance;
    private bool triggerActive = true;
    private bool isConnected = false;

    public bool isClicked = false;
    private void Update()
    {
        if (isClicked)
        {
            FindSelectedObject();
        }
        if (isConnected)
        {
            isClicked = false;
        }
    }

    void OnMouseDown()
    {
        if (!isConnected)
        {
            isClicked = true;
        }

    }

    void FindSelectedObject()
    {
        MoveObjectWithMouse[] moveScript = FindObjectsOfType<MoveObjectWithMouse>();
        foreach (MoveObjectWithMouse obj in moveScript)
        {
            MoveObjectWithMouse script = obj.GetComponent<MoveObjectWithMouse>(); // Получаем компонент скрипта с условием

            if (script != null && script.isSelected)
            {
                HandlePipe(obj.gameObject, 0, 0, 0, 0, 0, 0, "pipe");
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ConnectionPoint") && isConnected)
        {
            isConnected = true;
            Debug.Log("connected for " + gameObject.name);
            this.enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.tag = "Untagged";

            PipeConnection otherScript = other.GetComponent<PipeConnection>();
            Collider collider = other.GetComponent<Collider>();
            if (otherScript != null)
            {
                otherScript.enabled = false;
                collider.enabled = false;
                other.tag = "Untagged";
            }
        }
    }

    private void HandlePipe(GameObject other, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, string debugMessage)
    {
        Debug.Log(debugMessage);
        isConnected = true;
        other.transform.SetParent(transform);
        other.transform.localPosition = new Vector3(0, 0, 0);
        other.GetComponent<MoveObjectWithMouse>().isDragging = false;
        other.GetComponent<MoveObjectWithMouse>().ToggleCollider(false);
        other.GetComponent<MoveObjectWithMouse>().isConnected = true;
        Destroy(other.GetComponent<Rigidbody>());
        triggerActive = false;
        isClicked = false;
        
    }

 
}
