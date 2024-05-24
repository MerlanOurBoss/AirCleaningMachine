using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationTrube : MonoBehaviour
{
    public GameObject pipePrefab; // ������ ��������
    public Transform startPoint;  // ��������� �����
    public Transform endPoint;    // �������� �����

    void Start()
    {
        GeneratePipe(startPoint.position, endPoint.position);
    }

    void GeneratePipe(Vector3 start, Vector3 end)
    {
        // ������� ����� �������
        GameObject pipe = Instantiate(pipePrefab);

        // ��������� ��������� �������� ����� ��������� � �������� �������
        pipe.transform.position = (start + end) / 2;

        // ��������� ���������� ����� �������
        float distance = Vector3.Distance(start, end);

        // ������ ������ �������� (�� ��� Y) � ������������ � �����������
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z);

        // ���������� ������� �� ������ � �����
        pipe.transform.up = end - start;
    }
}
