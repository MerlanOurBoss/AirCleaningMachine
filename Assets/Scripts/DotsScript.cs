using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsScript : MonoBehaviour
{
    public GameObject prefabToSpawn; 
    public Transform spawnArea;
    public int numberOfPrefabs = 200; 
    public float respawnTime = 10f;
    public Transform parentObject;

    private int spawnedCount = 0;
    private bool isFull = false;
    public void StartDots()
    {
        InvokeRepeating("SpawnPrefab", 1f, .1f);
    }
    void SpawnPrefab()
    {
        if (spawnedCount < numberOfPrefabs)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            spawnedPrefab.transform.parent = parentObject;
            spawnedPrefab.SetActive(true); 
            spawnedCount++;
        }
        else
        {
            CancelInvoke("SpawnPrefab");
            isFull = true;
            
        }
    }

    public void DropPrefabs()
    {
        if (isFull)
        {
            ActivateRigidbodies();
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnArea.position.x - spawnArea.localScale.x / 2f, spawnArea.position.x + spawnArea.localScale.x / 2f);
        float y = Random.Range(spawnArea.position.y - spawnArea.localScale.y / 2f, spawnArea.position.y + spawnArea.localScale.y / 2f);
        float z = Random.Range(spawnArea.position.z - spawnArea.localScale.z / 2f, spawnArea.position.z + spawnArea.localScale.z / 2f);

        return new Vector3(x, y, z);
    }

    void ActivateRigidbodies()
    {
        GameObject[] spawnedPrefabs = GameObject.FindGameObjectsWithTag("Dots"); 

        foreach (GameObject prefab in spawnedPrefabs)
        {
            Rigidbody rb = prefab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; 
            }
        }

        spawnedCount = 0;
        InvokeRepeating("SpawnPrefab", 20f, .1f);
    }

}
