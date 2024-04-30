using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public GameObject spawnAreaCube;
    public float spawnInterval = 0.001f;
    public float _delay;

    public void startCor()
    {
        StartCoroutine(SpawnCoroutine());
    }
    public void stopCor()
    {
        StopAllCoroutines();
    }
    IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(_delay);
        while (true)
        {
            
            Vector3 spawnAreaSize = spawnAreaCube.transform.localScale;

            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition + spawnAreaCube.transform.position, Quaternion.identity);

            spawnedPrefab.transform.SetParent(spawnAreaCube.transform);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
