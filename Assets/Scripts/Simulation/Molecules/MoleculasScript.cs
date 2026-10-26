using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // Скорость дрейфа молекул по оси Z, ед./с. Раньше было -0.05 за кадр (около -3 ед./с при 60 FPS),
    // поэтому скорость зависела от FPS.
    public float driftSpeed = -3f;

    private readonly List<Rigidbody> _molecBodies = new List<Rigidbody>();

    // Физику двигаем в FixedUpdate и только молекулы (дочерние объекты spawnInObj).
    // Раньше FindObjectsOfType<Rigidbody>() каждый кадр двигал ВСЕ Rigidbody на сцене (в т.ч. NPC_man).
    private void FixedUpdate()
    {
        MoveObjects(driftSpeed * Time.fixedDeltaTime);
    }

    void MoveObjects(float zOffset)
    {
        if (spawnInObj == null) return;

        spawnInObj.GetComponentsInChildren(false, _molecBodies);

        float rotation = rotationSpeed * Time.fixedDeltaTime * 1.5f;
        Quaternion deltaRotation = Quaternion.Euler(rotation, rotation, rotation);
        Vector3 movement = new Vector3(0f, 0f, zOffset);

        for (int i = 0; i < _molecBodies.Count; i++)
        {
            Rigidbody rb = _molecBodies[i];
            rb.MovePosition(rb.position + movement);
            rb.MoveRotation(rb.rotation * deltaRotation);
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
