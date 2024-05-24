using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MoveObjectWithMouse : MonoBehaviour
{
    public bool isDragging = false;
    private bool isCtrlPressed = false;
    private Vector3 initialPosition;
    private bool isSelected = false;
    private Color originalColor;
    private Renderer objectRenderer;
    private Collider objectCollider;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
            ChangeColor(Color.red);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ������� ��� �� ������ � ����� �� ������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���������, ����� �� ��� � �����-�� ������
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    isSelected = true;
                    isDragging = !isDragging;
                    ChangeColor(Color.red); // ������ ���� �� �������
                }
                else
                {
                    isSelected = false;
                    isDragging = false;
                    ChangeColor(originalColor); // ���������� �������� ����
                }
            }
            else
            {
                isSelected = false;
                isDragging = false;
                ChangeColor(originalColor); // ���������� �������� ����
            }
        }

        if (isDragging)
        {
            FollowMouse();
        }
    }

    void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 750f; // ���������� �� ������ �� �������
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (!isCtrlPressed)
            {
                initialPosition = gameObject.transform.position;
                isCtrlPressed = true;
            }

            // ������� ������ �� ��� Y
            gameObject.transform.position = new Vector3(initialPosition.x, worldPosition.y, initialPosition.z);
        }
        else
        {
            isCtrlPressed = false;

            // ������� ������ �� ���� X � Z
            gameObject.transform.position = new Vector3(worldPosition.x, gameObject.transform.position.y, worldPosition.z);
        }
    }

    void ChangeColor(Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
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
