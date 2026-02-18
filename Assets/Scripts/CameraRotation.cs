using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraRotation : MonoBehaviour
{
    public float mouseSensitivity = 3f;
    public float moveSpeed = 10f;
    public float fastMoveMultiplier = 3f;
    public float limit = 85f;

    private float rotX;
    private float rotY;
    
    public bool isMoving = false;
    void Update()
    {
        Rotate();
        Move();
        
        if (Input.GetMouseButtonUp(0)) { isMoving = false; }
        else if (Input.GetMouseButton(0)) { isMoving = true; }
    }

    void Rotate()
    {
        if (Input.GetMouseButton(0)) // ЛКМ
        {
            rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
            rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotY = Mathf.Clamp(rotY, -limit, limit);

            transform.rotation = Quaternion.Euler(rotY, rotX, 0);
        }
    }

    void Move()
    {
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMoveMultiplier;

        float h = Input.GetAxis("Horizontal"); // A D
        float v = Input.GetAxis("Vertical");   // W S

        Vector3 dir = transform.forward * v + transform.right * h;

        // Подъём/спуск
        if (Input.GetKey(KeyCode.E)) dir += transform.up;
        if (Input.GetKey(KeyCode.Q)) dir -= transform.up;

        transform.position += dir * speed * Time.deltaTime;
    }
}