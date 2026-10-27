using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFacilities : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform parentTransform;
    [Tooltip("Поворот создаваемого объекта вокруг вертикальной оси, в градусах.")]
    [SerializeField] private float RotY = 0;
    [SerializeField] private int PosY = 0;

    public void SpawnTrube()
    {
        // Родителя ищем в момент создания (раньше — каждый кадр в Update, с NullReference без SborCO2).
        GameObject sborCO2 = GameObject.FindGameObjectWithTag(Tags.SborCO2);
        if (sborCO2 != null) parentTransform = sborCO2.transform;

        // RotY — поворот вокруг вертикальной оси в градусах. Раньше было new Quaternion(0, RotY, 0, 0),
        // что не является углами Эйлера: любое ненулевое значение давало разворот на 180°.
        GameObject instance = Instantiate(prefabToSpawn,
            new Vector3(spawnPosition.position.x, spawnPosition.position.y - PosY, spawnPosition.position.z),
            Quaternion.Euler(0f, RotY, 0f), parentTransform);

        // Раньше флаг ставился у префаба-ассета (в редакторе это сохранялось в префаб),
        // а не у только что созданного объекта.
        var moving = instance.GetComponent<MovingFacilities>();
        if (moving != null) moving.isDragging = true;
    }
}
