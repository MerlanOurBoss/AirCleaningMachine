using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ConnectionZone : MonoBehaviour
{
    public string[] targetTags;
    public string[] targetTags2;
    public string[] targetTags3; 
    public string[] targetTags4;

    public Transform attractorPoint;
    public float attractionForce = 3f; 
    private Animator animator;

    private int totalObjects;
    private int totalObjects2;
    private int totalObjects3;
    private int totalObjects4;

    // Молекулы, для которых уже запущено разрушение. OnTriggerStay вызывается каждый физический кадр,
    // и раньше Play("Destroyng") перезапускал анимацию каждый кадр — она застревала на первом кадре.
    private readonly HashSet<GameObject> _destroying = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        _destroying.RemoveWhere(o => o == null);

        totalObjects = targetTags.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        totalObjects2 = targetTags2.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        totalObjects3 = targetTags3.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
        totalObjects4 = targetTags4.Sum(tag => GameObject.FindGameObjectsWithTag(tag).Length);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.CO) || other.CompareTag(Tags.OO))
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
                        Attract(obj.transform);
                        if (_destroying.Add(obj))
                        {
                            animator = obj.GetComponent<Animator>();
                            if (animator != null) animator.Play("Destroyng");
                            Destroy(obj, 1.7f);
                        }
                    }
                }
            }
        }
        else if (other.CompareTag(Tags.CHH) || other.CompareTag(Tags.HH) || other.CompareTag(Tags.OO2))
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
                        Attract(obj.transform);
                        if (_destroying.Add(obj))
                        {
                            animator = obj.GetComponent<Animator>();
                            if (animator != null) animator.Play("Destroyng");
                            Destroy(obj, 1.7f);
                        }
                    }
                }
            }
        }
        else if (other.CompareTag(Tags.SOO) || other.CompareTag(Tags.OO3))
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
                        Attract(obj.transform);
                        if (_destroying.Add(obj))
                        {
                            animator = obj.GetComponent<Animator>();
                            if (animator != null) animator.Play("Destroyng");
                            Destroy(obj, 1.7f);
                        }
                    }
                }
            }
        }
        else if (other.CompareTag(Tags.ON) || other.CompareTag(Tags.ON2))
        {
            foreach (string tag in targetTags4)
            {
                GameObject[] objectss = GameObject.FindGameObjectsWithTag(tag);
                if (totalObjects4 < 2)
                {
                    Debug.Log("Недостаточно молекул во втором");
                }
                else
                {
                    foreach (GameObject obj in objectss)
                    {
                        Attract(obj.transform);
                        if (_destroying.Add(obj))
                        {
                            animator = obj.GetComponent<Animator>();
                            if (animator != null) animator.Play("Destroyng");
                            Destroy(obj, 1.7f);
                        }
                    }
                }
            }
        }
    }
    void Attract(Transform attractedObject)
    {
        Vector3 direction = attractorPoint.position - attractedObject.position;

        float forceMagnitude = attractionForce * Time.deltaTime;

        attractedObject.Translate(direction.normalized * forceMagnitude, Space.World);
    }
}
