using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    //public float mouseSensitivity = 100f;
    //public Transform playerBody;


    //void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

    //        playerBody.Rotate(Vector3.up * mouseX, Space.World);
    //    }
    //    if (Input.GetMouseButton(1))
    //    {
    //        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    //        playerBody.Rotate(Vector3.right * mouseY, Space.World);
    //    }
    //}

    public Transform target;
    public Camera camera;
    public Vector3 offset;
    public float sensitivity = 3; // чувствительность мышки
    public float limit = 80; // ограничение вращения по Y
    public float zoom = 0.25f; // чувствительность при увеличении, колесиком мышки
    public float zoomMax = 100; // макс. увеличение
    public float zoomMin = 3; // мин. увеличение
    private float X, Y;

    public bool isEnabled = false;

    public float x, y, w, h;
    void Start()
    {
        limit = Mathf.Abs(limit);
        if (limit > 90) limit = 90;
        offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax));
        //transform.position = target.position + offset;
    }

    void Update()
    {
        if (isEnabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoom;
                else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoom;
                offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));

                X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
                Y += Input.GetAxis("Mouse Y") * sensitivity;
                Y = Mathf.Clamp(Y, -limit, limit);
                transform.localEulerAngles = new Vector3(-Y, X, 0);
                transform.position = transform.localRotation * offset + target.position;
            }
        }
    }

    public void setReact()
    {
        camera.rect = new Rect(x,y,w,h);
    }

    public void resetReact()
    {
        camera.rect = new Rect(0,0,1,1);
    }

    public void controlEnabled()
    {
        if (!isEnabled) isEnabled = true;
        else isEnabled = false;
    }
}
