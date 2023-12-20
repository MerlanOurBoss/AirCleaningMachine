using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingAndCreatingMolec : MonoBehaviour
{
    public GameObject spawnPrefab;
    public Vector3 spawnAreaSize = new Vector3(10f, 10f, 10f); // Размер области спавна

    void OnDestroy()
    {
        float randomX = Random.Range(transform.position.x - spawnAreaSize.x / 2, transform.position.x + spawnAreaSize.x / 2);
        float randomZ = Random.Range(transform.position.z - spawnAreaSize.z / 2, transform.position.z + spawnAreaSize.z / 2);
        float randomY = Random.Range(transform.position.y - spawnAreaSize.y / 2, transform.position.y + spawnAreaSize.y / 2);

        // Создаем новый объект в случайных координатах
        Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

        Instantiate(spawnPrefab, spawnPosition, gameObject.transform.rotation);
    }
}
