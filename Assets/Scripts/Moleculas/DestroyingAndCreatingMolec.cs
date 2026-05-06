using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingAndCreatingMolec : MonoBehaviour
{
    public GameObject spawnPrefab;
    public GameObject spawnInObj;

    private void Start()
    {
        spawnInObj = GameObject.FindWithTag("SpawnInObj");

    }
    void OnDestroy()
    { 
        Instantiate(spawnPrefab, transform.position, gameObject.transform.rotation, spawnInObj.transform);
    }
}
