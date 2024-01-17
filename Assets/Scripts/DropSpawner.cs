using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public GameObject spawnAreaCube;
    public float spawnInterval = 0.001f;
    public float _delay;

    void Start()
    {
        // Запускаем корутину SpawnCoroutine
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(_delay);
        while (true)
        {
            
            // Получаем размеры куба
            Vector3 spawnAreaSize = spawnAreaCube.transform.localScale;

            // Генерируем случайные координаты в пределах размеров куба
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            // Создаем префаб в случайной позиции в пределах куба
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition + spawnAreaCube.transform.position, Quaternion.identity);

            // Устанавливаем созданный префаб как дочерний куба (или другого объекта)
            spawnedPrefab.transform.SetParent(spawnAreaCube.transform);

            // Ждем указанное время перед следующим спауном
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
