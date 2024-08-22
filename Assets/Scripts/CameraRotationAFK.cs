using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationAFK : MonoBehaviour
{
    [SerializeField] private CameraRotation cam;
    public float time = 300f;

    void Update()
    {
        if (!cam.isMoving)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                transform.Rotate(0, 5 * Time.deltaTime, 0);
                time = 0;
            }
        }
        else
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y = 0;
            transform.eulerAngles = currentRotation;
            time = 300f;
        }
    }
}
