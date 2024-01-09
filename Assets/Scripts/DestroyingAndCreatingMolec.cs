using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingAndCreatingMolec : MonoBehaviour
{
    public GameObject spawnPrefab;
    
    void OnDestroy()
    { 
        Instantiate(spawnPrefab, transform.position, gameObject.transform.rotation);
    }
}
