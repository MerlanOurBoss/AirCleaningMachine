using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ConnectionZone1 : MonoBehaviour
{
    public string[] targetTagss; // ������ ����� ��������, ������� �� ������ ���������
    public Transform attractorPoint; // �����, � ������� ����� ������������� �������
    public float attractionForce = 3f; // ���� ����������
    //public GameObject spawnPrefab;
    private Animator animator;

    private int totalObjectss;

    private void OnTriggerEnter(Collider other)
    {
        totalObjectss = targetTagss.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        Debug.Log(totalObjectss);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("CHH") || other.CompareTag("HH") || other.CompareTag("OO2"))
        {
            foreach (string tag in targetTagss)
            {
                // ������� ��� ������� � ������� �����
                GameObject[] objectss = GameObject.FindGameObjectsWithTag(tag);
                if (totalObjectss < 3)
                {
                    Debug.Log("������������ ������� �� ������");
                }
                else
                {
                    // ����������� ������ ������ � �����
                    foreach (GameObject obj in objectss)
                    {
                        animator = obj.GetComponent<Animator>();
                        Attract(obj.transform);
                        animator.Play("Destroyng");
                        Destroy(obj, 2f);
                    }
                }
            }
        }
    }
    void Attract(Transform attractedObject)
    {
        // ��������� ����������� � ����� ����������
        Vector3 direction = attractorPoint.position - attractedObject.position;

        // ��������� ���� ����������
        float forceMagnitude = attractionForce * Time.deltaTime;

        // ��������� ���� � �������
        attractedObject.Translate(direction.normalized * forceMagnitude, Space.World);
    }

}
