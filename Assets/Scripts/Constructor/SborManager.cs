using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SborManager : MonoBehaviour
{
    public GameObject prefabSborCO2;

    private GameObject obj;
    private void Awake()
    {
        SpawnPrefab();
        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (obj == null)
        {
            SpawnPrefab();
        }
    }

    void SpawnPrefab()
    {
        obj = Instantiate(prefabSborCO2, gameObject.transform);
    }
}
