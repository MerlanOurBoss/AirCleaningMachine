using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ConnectionZone : MonoBehaviour
{
    public string[] targetTags;// Массив тегов объектов, которые вы хотите притянуть
    public string[] targetTags2; // Массив тегов объектов, которые вы хотите притянуть
    public string[] targetTags3; // Массив тегов объектов, которые вы хотите притянуть
    public string[] targetTags4; // Массив тегов объектов, которые вы хотите притянуть

    public Transform attractorPoint; // Точка, к которой будут притягиваться объекты
    public float attractionForce = 3f; // Сила притяжения
    //public GameObject spawnPrefab;
    private Animator animator;

    private int totalObjects;
    private int totalObjects2;
    private int totalObjects3;
    private int totalObjects4;

    private void OnTriggerEnter(Collider other)
    {
        totalObjects = targetTags.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        totalObjects2 = targetTags2.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        totalObjects3 = targetTags3.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        totalObjects4 = targetTags4.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        //Debug.Log(totalObjects);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("CO") || other.CompareTag("OO"))
        {
            foreach (string tag in targetTags)
            {
                // Находим все объекты с текущим тегом
                GameObject[] objectss = GameObject.FindGameObjectsWithTag(tag);
                if (totalObjects < 3)
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
                        Destroy(obj, 1.7f);
                    }
                }
            }
        }
        else if (other.CompareTag("CHH") || other.CompareTag("HH") || other.CompareTag("OO2"))
        {
            foreach (string tag in targetTags2)
            {
                // Находим все объекты с текущим тегом
                GameObject[] objectss = GameObject.FindGameObjectsWithTag(tag);
                if (totalObjects2 < 3)
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
                        Destroy(obj, 1.7f);
                    }
                }
            }
        }
        else if (other.CompareTag("SOO") || other.CompareTag("OO3"))
        {
            foreach (string tag in targetTags3)
            {
                // Находим все объекты с текущим тегом
                GameObject[] objectss = GameObject.FindGameObjectsWithTag(tag);
                if (totalObjects3 < 2)
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
                        Destroy(obj, 1.7f);
                    }
                }
            }
        }
        else if (other.CompareTag("ON") || other.CompareTag("ON2"))
        {
            foreach (string tag in targetTags4)
            {
                // Находим все объекты с текущим тегом
                GameObject[] objectss = GameObject.FindGameObjectsWithTag(tag);
                if (totalObjects4 < 2)
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
                        Destroy(obj, 1.7f);
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
