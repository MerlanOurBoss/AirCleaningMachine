using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraRotation : MonoBehaviour
{
    public Transform target;
    public Camera myCamera;
    public Vector3 offset;
    public float sensitivity = 3; 
    public float limit = 80;
    public float zoom = 1f; 
    public float zoomMax = 100;
    public float zoomMin = 3;
    private float X, Y;
    public string str;
    public bool isEnabled = false;

    public float x, y, w, h;

    private float moveSpeed = 1.0f; 

    public float sensitivityx = 0.1f; 
    public Vector3 moveLimits = new Vector3(1000, 1000, 1000);

    public bool isMoving = false;
    private Vector3 startPosition;
    void Start()
    {
        limit = Mathf.Abs(limit);
        if (limit > 90) limit = 90;
        offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax/2));
        startPosition = target.position;
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
        }
    }
    public void Blasla()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(0))
        {
            float moveX = Input.GetAxis("Mouse X") * sensitivityx * moveSpeed;
            float moveY = Input.GetAxis("Mouse Y") * sensitivityx * moveSpeed;

            Vector3 newPosition = target.position + new Vector3(moveX, moveY, 0);
            newPosition.x = Mathf.Clamp(newPosition.x, startPosition.x - moveLimits.x, startPosition.x + moveLimits.x);
            newPosition.y = Mathf.Clamp(newPosition.y, startPosition.y - moveLimits.y, startPosition.y + moveLimits.y);

            target.position = newPosition;
        }
        else if (Input.GetMouseButton(0))
        {
            isMoving = true;
            offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
            X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
            Y += Input.GetAxis("Mouse Y") * sensitivity;
            Y = Mathf.Clamp(Y, -limit, limit);
            transform.localEulerAngles = new Vector3(-Y, X, 0);
            transform.position = transform.localRotation * offset + target.position;
        }
    }

    public void onScroll()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("scrolling");
            if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoom;
            else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoom;
            offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
            transform.position = transform.localRotation * offset + target.position;
        }
        
    }
    public void setReact()
    {
        myCamera.rect = new Rect(x,y,w,h);
    }

    public void resetReact()
    {
        myCamera.rect = new Rect(0,0,1,1);
    }

    public void controlEnabled()
    {
        if (!isEnabled) isEnabled = true;
        else isEnabled = false;
    }
}
