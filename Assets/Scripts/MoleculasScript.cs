using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PrefabSpawnInfo
{
    public GameObject prefab;
    public Rigidbody rigid;
    public TMP_InputField text;
}
public class MoleculasScript : MonoBehaviour
{
    public PrefabSpawnInfo[] prefabsToSpawn;
    private List<GameObject> molec = new List<GameObject>();
    public Collider spawnArea;

    public float moveSpeed = 0.5f;
    public float rotationSpeed = 1000f;

    public GameObject spawnInObj;

    private void Update()
    {
        MoveObjects(-0.05f);
        
    }
    void MoveObjects(float direction)
    {
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Vector3 movement = new Vector3(0f, 0f, direction);

            // Используйте MovePosition для перемещения объекта
            rigidbodies[i].MovePosition(rigidbodies[i].position + movement);

            float rotation = rotationSpeed * Time.deltaTime * 1.5f;
            Quaternion deltaRotation = Quaternion.Euler(rotation, rotation, rotation);

            rigidbodies[i].MoveRotation(rigidbodies[i].rotation * deltaRotation);
        }
    }
    public void StartSimulate()
    {
        StartCoroutine(SpawnPrefabsSequentially());
    }
    IEnumerator SpawnPrefabsSequentially()
    {
        while (true) 
        {
            if (prefabsToSpawn[0].text.text == "30")
            {
                prefabsToSpawn[0].text.text = "20";
            }
            if (prefabsToSpawn[1].text.text == "20") //02 -> 2
            {
                prefabsToSpawn[4].text.text = "10";
            }
            if (prefabsToSpawn[1].text.text == "30") //02 -> 3
            {
                prefabsToSpawn[4].text.text = "10";
                prefabsToSpawn[6].text.text = "10";
            }
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
                    yield return new WaitForSeconds(1f);
                }
                if (count == 5)
                {
                    yield return new WaitForSeconds(1f);
                }
                if (count == 7)
                {
                    yield return new WaitForSeconds(1f);
                }
                for (int i = 0; i < int.Parse(prefabInfo.text.text) / 10; i++)
                {
                    Vector3 randomSpawnPoint = GetRandomPointInBounds(spawnArea.bounds);
                    Instantiate(prefabInfo.prefab, randomSpawnPoint, Quaternion.identity, spawnInObj.transform);
                    molec.Add(prefabInfo.prefab);
                    yield return new WaitForSeconds(.7f); //0.2f
                } 
                count++;
            }



            yield return new WaitForSeconds(3f);
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randomY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }

}
