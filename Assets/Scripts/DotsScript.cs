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
    private bool isPaused = false;
    private List<GameObject> spawnedPrefabs = new List<GameObject>(); // Список созданных объектов

    private void Update()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = isPaused ? 0 : 1;
        }
    }
    public void StartDots()
    {
        isPaused = false;
        spawnArea.gameObject.SetActive(true); // Включаем точку спавна
        InvokeRepeating("SpawnPrefab", 1f, .1f);
    }

    void SpawnPrefab()
    {
        if (isPaused || spawnedCount >= numberOfPrefabs) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedPrefab.transform.parent = parentObject;
        spawnedPrefab.SetActive(true);
        spawnedPrefabs.Add(spawnedPrefab);
        spawnedCount++;

        if (spawnedCount >= numberOfPrefabs)
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

    public void PauseSpawning()
    {
        isPaused = true;
        CancelInvoke("SpawnPrefab");
        spawnArea.gameObject.SetActive(false); // Отключаем точку спавна
    }

    public void ResumeSpawning()
    {
        isPaused = false;
        spawnArea.gameObject.SetActive(true); // Включаем точку спавна
        InvokeRepeating("SpawnPrefab", 1f, .1f);
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
        // Удаляем уничтоженные объекты из списка перед активацией Rigidbody
        spawnedPrefabs.RemoveAll(prefab => prefab == null);

        foreach (GameObject prefab in spawnedPrefabs)
        {
            if (prefab != null) // Проверяем, существует ли объект
            {
                Rigidbody rb = prefab.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
        }

        spawnedCount = 0;
        InvokeRepeating(nameof(SpawnPrefab), 20f, .1f);
    }
}
