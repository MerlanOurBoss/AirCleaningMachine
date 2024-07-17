using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickForDestroy : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // ��������, �������� �� ������ �������� �������� �������� �������
                if (hit.transform.IsChildOf(transform))
                {
                    Destroy(gameObject); // �������� ������������� �������
                }
            }
        }
    }
}
