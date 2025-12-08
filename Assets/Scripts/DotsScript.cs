using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsScript : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnArea;
    public BoxCollider spawnCollider;
    public int numberOfPrefabs = 200;
    public float respawnTime = 10f;
    public Transform parentObject;
    
    [Header("Spawn area (локальные размеры)")]
    public Vector3 spawnBoxSize = new Vector3(1f, 1f, 1f);
    
    private int spawnedCount = 0;
    private bool isFull = false;
    private bool isPaused = false;
    private List<GameObject> spawnedPrefabs = new List<GameObject>(); 

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
        spawnArea.gameObject.SetActive(true);
        InvokeRepeating("SpawnPrefab", 1f, .05f);
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
        spawnArea.gameObject.SetActive(false); 
    }

    public void ResumeSpawning()
    {
        isPaused = false;
        spawnArea.gameObject.SetActive(true);
        InvokeRepeating("SpawnPrefab", 1f, .05f);
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (spawnCollider == null)
        {
            // fallback на вариант 1, если коллайдера нет
            Vector3 half = spawnArea.localScale * 0.5f;
            Vector3 localRandomPosFallback = new Vector3(
                Random.Range(-half.x, half.x),
                Random.Range(-half.y, half.y),
                Random.Range(-half.z, half.z)
            );
            return spawnArea.TransformPoint(localRandomPosFallback);
        }

        // центр и размер из коллайдера (они уже в локальных координатах)
        Vector3 center = spawnCollider.center;
        Vector3 size = spawnCollider.size;
        Vector3 halfSize = size * 0.5f;

        Vector3 localRandomPos = new Vector3(
            Random.Range(center.x - halfSize.x, center.x + halfSize.x),
            Random.Range(center.y - halfSize.y, center.y + halfSize.y),
            Random.Range(center.z - halfSize.z, center.z + halfSize.z)
        );

        return spawnArea.TransformPoint(localRandomPos);
    }

    void ActivateRigidbodies()
    {
        spawnedPrefabs.RemoveAll(prefab => prefab == null);

        foreach (GameObject prefab in spawnedPrefabs)
        {
            if (prefab != null)
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
