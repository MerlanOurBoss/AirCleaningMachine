using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public float attractionForce;

    public GameObject spawnInObj;
    // Предупреждения, уже выведенные за текущее пребывание объекта в зоне
    // (раньше LogError писался каждый физический кадр).
    private readonly HashSet<string> _warned = new HashSet<string>();

    private void OnTriggerExit(Collider other)
    {
        _warned.Clear();
    }

    private void WarnOnce(string key, string message)
    {
        if (_warned.Add(key)) Debug.LogWarning(message);
    }

    private static GameObject[] FindTag(string tag) => GameObject.FindGameObjectsWithTag(tag);

    private static void ReleaseRigidbodies(GameObject[] objects)
    {
        foreach (GameObject child in objects)
        {
            Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
            if (childRigidbody != null)
            {
                childRigidbody.isKinematic = false;
                childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            }
        }
    }

    private static void DestroyAll(GameObject[] objects, float delay)
    {
        foreach (GameObject child in objects)
            Destroy(child, delay);
    }

    // Поиск объектов по тегам теперь делается только в нужной ветке (раньше 12 поисков
    // на каждый вызов OnTriggerStay), а перед обращением к [0]/[1] проверяется длина массивов.
    private void OnTriggerStay(Collider other)
    {
        float forceMagnitude = attractionForce * Time.deltaTime;

        if (other.CompareTag(Tags.OO))
        {
            GameObject[] O = FindTag(Tags.O);
            GameObject[] CO = FindTag(Tags.CO);

            if (O.Length >= 2 && CO.Length == 2)
            {
                other.transform.DetachChildren();
                ReleaseRigidbodies(O);

                O[0].transform.position = Vector3.MoveTowards(O[0].transform.position, CO[0].transform.position, forceMagnitude * 10);
                O[1].transform.position = Vector3.MoveTowards(O[1].transform.position, CO[1].transform.position, forceMagnitude * 10);

                DestroyAll(CO, 1f);
                DestroyAll(O, 1f);
                DestroyAll(FindTag(Tags.OO), 1f);
                DestroyAll(FindTag(Tags.CO_Parent), 1f);
            }
            else
            {
                WarnOnce("OO", $"Не найдены два объекта с каждым из тегов: CO={CO.Length}, O={O.Length}");
            }
        }

        if (other.CompareTag(Tags.OO3))
        {
            GameObject[] O = FindTag(Tags.O4);
            GameObject[] SOO = FindTag(Tags.SOO);

            if (O.Length >= 2 && SOO.Length == 2)
            {
                other.transform.DetachChildren();
                ReleaseRigidbodies(O);

                O[0].transform.position = Vector3.MoveTowards(O[0].transform.position, SOO[0].transform.position, forceMagnitude * 10);
                O[1].transform.position = Vector3.MoveTowards(O[1].transform.position, SOO[1].transform.position, forceMagnitude * 10);

                DestroyAll(O, 1f);
                DestroyAll(SOO, 1f);
                DestroyAll(FindTag(Tags.OO3), 1f);
                DestroyAll(FindTag(Tags.SOO_Parent), 1f);
            }
            else
            {
                WarnOnce("OO3", $"Не найдены два объекта с каждым из тегов: SOO={SOO.Length}, O4={O.Length}");
            }
        }

        if (other.CompareTag(Tags.OO2))
        {
            GameObject[] O = FindTag(Tags.O1);
            GameObject[] CHH = FindTag(Tags.CHH);
            GameObject[] HH = FindTag(Tags.HH);

            other.transform.DetachChildren();
            ReleaseRigidbodies(O);
            DestroyAll(O, 1f);

            if (O.Length >= 2 && CHH.Length >= 1 && HH.Length >= 1)
            {
                O[0].transform.position = Vector3.MoveTowards(O[0].transform.position, CHH[0].transform.position, forceMagnitude * 10);
                O[1].transform.position = Vector3.MoveTowards(O[1].transform.position, HH[0].transform.position, forceMagnitude * 10);
            }
            else
            {
                WarnOnce("OO2", $"Недостаточно объектов для реакции OO2: O1={O.Length}, CHH={CHH.Length}, HH={HH.Length}");
            }

            DestroyAll(CHH, 1f);
            DestroyAll(FindTag(Tags.OO2), 1f);
            DestroyAll(HH, 1f);
            DestroyAll(FindTag(Tags.CHH_Parent), 1f);
        }

        if (other.CompareTag(Tags.OON))
        {
            GameObject[] O5 = FindTag(Tags.O5);
            GameObject[] N2 = FindTag(Tags.N2);

            other.transform.DetachChildren();
            ReleaseRigidbodies(O5);
            ReleaseRigidbodies(N2);

            if (O5.Length >= 2)
                O5[1].transform.position = Vector3.MoveTowards(O5[1].transform.position, O5[0].transform.position, forceMagnitude * 10);
            else
                WarnOnce("OON", $"Недостаточно объектов O5 для реакции OON: {O5.Length}");

            DestroyAll(O5, 1f);
            DestroyAll(N2, 1f);
            DestroyAll(FindTag(Tags.OON), 1f);
        }

        if (other.CompareTag(Tags.ON))
        {
            GameObject[] O3 = FindTag(Tags.O3);
            GameObject[] ON_Child = FindTag(Tags.ON_Child);

            other.transform.DetachChildren();
            ReleaseRigidbodies(O3);
            ReleaseRigidbodies(ON_Child);

            if (O3.Length >= 2)
                O3[1].transform.position = Vector3.MoveTowards(O3[1].transform.position, O3[0].transform.position, forceMagnitude * 10);
            else
                WarnOnce("ON", $"Недостаточно объектов O3 для реакции ON: {O3.Length}");

            DestroyAll(O3, 1f);
            DestroyAll(FindTag(Tags.ON), 1f);
            DestroyAll(FindTag(Tags.CO2_Parent), 1f);
            DestroyAll(ON_Child, 1f);
        }
    }

    public void DeleteAllMolec()
    {
        // Удалённые здесь молекулы не должны порождать «продукты реакции» (см. DestroyingAndCreatingMolec).
        DestroyingAndCreatingMolec.SuppressSpawnThisFrame();

        for (var i = spawnInObj.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnInObj.transform.GetChild(i).gameObject);
        }

        //foreach (Transform child in spawnInObj.transform)
        //{
        //    Destroy(child.gameObject);
        //}
    }
}
