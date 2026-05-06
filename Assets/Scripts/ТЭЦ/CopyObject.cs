using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyObject : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject prefab;       // Перетащите префаб в инспекторе
    public int count = 5;           // Количество копий
    public float spacing = 1900f;      // Расстояние между копиями по Z

    void Start()
    {
        if (prefab == null)
        {
            Debug.LogError("Префаб не назначен!");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - i * spacing
            );

            GameObject sbor = Instantiate(prefab, spawnPosition, Quaternion.identity);
            PipeConnector pipe = sbor.GetComponent<PipeConnector>();
            
            StartCoroutine(ConnectAfterDelay(pipe));
        }
        
    }
    private IEnumerator ConnectAfterDelay(PipeConnector pipe)
    {
        yield return new WaitForSeconds(2f);
        pipe.ConnectComponents();
    }
}
