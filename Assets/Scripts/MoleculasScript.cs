using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PrefabSpawnInfo
{
    public GameObject prefab;
    public Rigidbody rigid;
    public int count;
}
public class MoleculasScript : MonoBehaviour
{

    public PrefabSpawnInfo[] prefabsToSpawn;

    public Collider spawnArea;

    public float moveSpeed = 0.5f;
    public float rotationSpeed = 1000f;

    void Start()
    {
        StartCoroutine(SpawnPrefabsSequentially());
    }
    private void Update()
    {
        MoveObjects(-0.3f);
    }
    void MoveObjects(float direction)
    {
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb != null)
            {
                float rotation = rotationSpeed * Time.deltaTime * 1.5f;

                // Создаем вращающий кватернион
                Quaternion deltaRotation = Quaternion.Euler(rotation, rotation, rotation);

                // Применяем вращение к Rigidbody
                rb.MoveRotation(rb.rotation * deltaRotation);

                Vector3 force = new Vector3(0f, 0f, direction);

                rb.AddForce(force);                
            }
        }
    }
    IEnumerator SpawnPrefabsSequentially()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogError("Prefab array is not set up!");
            yield break;
        }
        int count = 0;
        foreach (var prefabInfo in prefabsToSpawn)
        {
            if (count == 2)
            {
                yield return new WaitForSeconds(5f);
            }
            if (count == 5)
            {
                yield return new WaitForSeconds(5f);
            }
            if (count == 7)
            {
                yield return new WaitForSeconds(5f);
            }
            for (int i = 0; i < prefabInfo.count; i++)
            {
                Vector3 randomSpawnPoint = GetRandomPointInBounds(spawnArea.bounds);
                Instantiate(prefabInfo.prefab, randomSpawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(0.2f/*Random.Range(0.3f, 0.7f)*/);
            }
            count++;
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}
