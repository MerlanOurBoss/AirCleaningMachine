using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ConnectionZone1 : MonoBehaviour
{
    public string[] targetTagss; // Массив тегов объектов, которые вы хотите притянуть
    public Transform attractorPoint; // Точка, к которой будут притягиваться объекты
    public float attractionForce = 3f; // Сила притяжения
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
                // Находим все объекты с текущим тегом
                GameObject[] objectss = GameObject.FindGameObjectsWithTag(tag);
                if (totalObjectss < 3)
                {
                    Debug.Log("Недостаточно молекул во втором");
                }
                else
                {
                    // Притягиваем каждый объект к точке
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
        // Вычисляем направление к точке притяжения
        Vector3 direction = attractorPoint.position - attractedObject.position;

        // Вычисляем силу притяжения
        float forceMagnitude = attractionForce * Time.deltaTime;

        // Применяем силу к объекту
        attractedObject.Translate(direction.normalized * forceMagnitude, Space.World);
    }

}
