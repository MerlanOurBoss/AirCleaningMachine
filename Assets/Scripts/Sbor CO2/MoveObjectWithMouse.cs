using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MoveObjectWithMouse : MonoBehaviour
{
    public bool isDragging = false;
    public LayerMask parentLayer;
    public LayerMask ignoreLayer;

    public bool isSelected = true;
    private Color originalColor;
    private Renderer objectRenderer;
    private Collider objectCollider;
    private Camera mainCamera;
    public bool isConnected = false;
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
        mainCamera = Camera.main;
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
            ChangeColor(Color.red);
        }
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
        if (isConnected)
        {
            StartCoroutine(ChangeColorForDuration(gameObject, Color.green, .1f));
        }
        if (Input.GetMouseButtonDown(0))
        {
            // Создаем луч из камеры в точку на экране
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем, попал ли луч в какой-то объект
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, parentLayer))
            {
                if (!IsChildCollider(hit.collider))
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
            }
            else
            {
                isSelected = false;
                isDragging = false;
                ChangeColor(originalColor);
                
                int childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    child.GetComponent<PipeConnection>().isClicked = false;
                    
                }
            }
        }

        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y));
            transform.GetComponent<Rigidbody>().MovePosition(new Vector3(mousePosition.x, transform.position.y + Input.GetAxis("Mouse ScrollWheel") * 100, mousePosition.z));
        }

        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(0f, -90f, 0f);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(0f, 90f, 0f);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                transform.Rotate(0f, 0f, -90f);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                transform.Rotate(0f, 0f, 90f);
            }
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
            objectCollider.enabled  = enabled;
        }
    }
    public IEnumerator ChangeColorForDuration(GameObject obj, Color color, float duration)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Color originalColor = rend.material.color;
            rend.material.color = color;
            yield return new WaitForSeconds(duration);
            isConnected = false;
            rend.material.color = originalColor;
        }
    }
}
