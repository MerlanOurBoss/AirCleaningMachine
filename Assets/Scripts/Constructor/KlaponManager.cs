using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KlaponManager : MonoBehaviour
{
    [SerializeField] GameObject _closedKlapon;
    [SerializeField] GameObject _openKlapon;

    public string targetTag = "Target";

    public bool isSelected = true;
    void Start()
    {
        _closedKlapon.SetActive(true);
        _openKlapon.SetActive(false);
    }

    public void OpenKlapon()
    {
        _closedKlapon.SetActive(false);
        _openKlapon.SetActive(true);
    }

    public void CloseKlapon()
    {
        _closedKlapon.SetActive(true);
        _openKlapon.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform == transform && gameObject.tag == "Klapon")
                {
                    isSelected = true;
                }
                else if (hit.collider.CompareTag(targetTag) && isSelected)
                {
                    Vector3 targetPosition = hit.collider.transform.position;
                    Quaternion targetRotation = hit.collider.transform.rotation;
                    transform.rotation = targetRotation;

                    Vector3 directionToMove = hit.point - targetPosition;

                    if (Mathf.Abs(directionToMove.x) > Mathf.Abs(directionToMove.y) && Mathf.Abs(directionToMove.x) > Mathf.Abs(directionToMove.z))
                    {
                        transform.position = new Vector3(hit.point.x, targetPosition.y, targetPosition.z);
                    }
                    else if (Mathf.Abs(directionToMove.y) > Mathf.Abs(directionToMove.x) && Mathf.Abs(directionToMove.y) > Mathf.Abs(directionToMove.z))
                    {
                        transform.position = new Vector3(targetPosition.x, hit.point.y, targetPosition.z);
                    }
                    else
                    {
                        transform.position = new Vector3(targetPosition.x, targetPosition.y, hit.point.z);
                    }
                }
                else
                {
                    isSelected = false;
                }
            }
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
        }
        if (Input.GetMouseButtonDown(1))
        {
            isSelected = false;
        }
    }
}
