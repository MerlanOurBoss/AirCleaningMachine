using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFacilities : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPosition;
    public Transform parentTransform;
    public float RotY = 0;
    public int PosY = 0;

    public void SpawnTrube()
    {
        Instantiate(prefabToSpawn, new Vector3(spawnPosition.position.x, spawnPosition.position.y - PosY, spawnPosition.position.z), new Quaternion(0, RotY, 0,0), parentTransform);
        prefabToSpawn.GetComponent<MovingFacilities>().isDragging = true;
    }
}
