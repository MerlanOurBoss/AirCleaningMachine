using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReplacer : MonoBehaviour
{
    public GameObject currentPrefab; // ������� ������
    public GameObject newPrefab; // ����� ������, �� ������� �������

    public void ReplacePrefab()
    {
        // ������ ��������� ������ ������� �� ������� ��������
        GameObject newInstance = Instantiate(newPrefab, currentPrefab.transform.position, currentPrefab.transform.rotation);

        // ���������� ������� ������
        Destroy(currentPrefab);

        // ��������� ������ �� ������� ������
        currentPrefab = newInstance;
        newPrefab.GetComponent<MoveObjectWithMouse>().isSelected = true;
    }
}
