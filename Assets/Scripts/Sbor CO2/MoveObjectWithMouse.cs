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
    public bool isActivated = false;

    private Vector3 offset;
    private PrefabReplacer replacer;
    private int count = 3;

    public Vector3 mouseOffset = new Vector3(1.0f, 1.0f, 0.0f);

    void Start()
    {
        replacer = GetComponent<PrefabReplacer>();
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
        if (Input.GetKey(KeyCode.X))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, parentLayer))
            {
                if (!IsChildCollider(hit.collider))
                {
                    if (hit.transform == transform)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, parentLayer))
            {
                if (!IsChildCollider(hit.collider))
                {
                    if (hit.transform == transform && (gameObject.tag == "Pipe" || gameObject.tag == "Pipe_45" || gameObject.tag == "Pipe_90" || gameObject.tag == "Pipe_T"))
                    {
                        isSelected = true;
                        isDragging = !isDragging;
                        ChangeColor(Color.red); // Меняем цвет на красный

                        isDragging = true;
                        Vector3 mousePosition = Input.mousePosition + mouseOffset;
                        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
                        offset = transform.position - mainCamera.ScreenToWorldPoint(mousePosition);
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
                    if (child.GetComponent<PipeConnection>())
                    {
                        child.GetComponent<PipeConnection>().isClicked = false;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            isSelected = false;
            isDragging = false;
            ChangeColor(originalColor); // Возвращаем исходный цвет
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                count++;
                if ( count > 3)
                {
                    count = 1;
                    transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else if ( count == 2 )
                {
                    transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                }
                else if ( count == 3)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
                
            }
        }
        if (isConnected && !isActivated)
        {
            StartCoroutine(ChangeColorForDuration(gameObject, Color.green, .1f));
        }
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.x = mousePosition.x - 15f;
            mousePosition.y = mousePosition.y - 15f;
            mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
            Vector3 newPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            transform.position = newPosition;

        }


    }
    void ChangeColor(Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }

    private void OnDestroy()
    {
        if (transform.parent != null)
        {
            transform.parent.gameObject.GetComponent<Collider>().enabled = true;
            transform.parent.gameObject.tag = "ConnectionPoint";
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
            isActivated = true;
            rend.material.color = originalColor;
        }
    }
}
