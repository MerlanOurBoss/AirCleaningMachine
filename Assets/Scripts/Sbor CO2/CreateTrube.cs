using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTrube : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPosition;

    public void SpawnTrube()
    {
        Instantiate(prefabToSpawn, spawnPosition.position, Quaternion.identity);
    }
}
