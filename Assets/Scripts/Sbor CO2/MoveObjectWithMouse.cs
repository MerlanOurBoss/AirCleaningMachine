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
            // Создаем луч из камеры в точку на экране
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем, попал ли луч в какой-то объект
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    isSelected = true;
                    isDragging = !isDragging;
                    ChangeColor(Color.red); // Меняем цвет на красный
                }
                else
                {
                    isSelected = false;
                    isDragging = false;
                    ChangeColor(originalColor); // Возвращаем исходный цвет
                }
            }
            else
            {
                isSelected = false;
                isDragging = false;
                ChangeColor(originalColor); // Возвращаем исходный цвет
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
        mousePosition.z = 750f; // Расстояние от камеры до объекта
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (!isCtrlPressed)
            {
                initialPosition = gameObject.transform.position;
                isCtrlPressed = true;
            }

            // Двигаем объект по оси Y
            gameObject.transform.position = new Vector3(initialPosition.x, worldPosition.y, initialPosition.z);
        }
        else
        {
            isCtrlPressed = false;

            // Двигаем объект по осям X и Z
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
