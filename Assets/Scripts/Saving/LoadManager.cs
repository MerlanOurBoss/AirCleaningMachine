using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public SceneData[] sceneDatas; // Array to hold multiple SceneData instances
    public GameObject[] prefabs;

    public GameObject sborCO2; // Object Sbor CO2
    public PipeConnector pipe;
    private Dictionary<int, List<GameObject>> sceneObjects = new Dictionary<int, List<GameObject>>();
    private void Start()
    {
        int idx = ConstructionSelector.SelectedIndex;
        if (idx >= 0)
        {
            Load(idx);
            Debug.Log(idx);

            ConstructionSelector.SelectedIndex = -1;
        }
        else
        {
            Debug.LogWarning("Не задан SelectedIndex — нечего загружать.");
        }
    }
    public void Load(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            SceneData sceneData = sceneDatas[sceneIndex];

            if (sceneData.objectsData != null && sceneData.objectsData.Length > 0)
            {
                Debug.Log($"Loading data from SceneData at index {sceneIndex}.");

                if (!sceneObjects.ContainsKey(sceneIndex))
                {
                    sceneObjects[sceneIndex] = new List<GameObject>();
                }

                foreach (ObjectData data in sceneData.objectsData)
                {
                    GameObject prefab = System.Array.Find(prefabs, p => p.name == data.objectName);

                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab, data.position, data.rotation);
                        if (obj.tag == "Pipe")
                        {

                        }
                        else if (obj.tag == "Klapon")
                        {
                            obj.GetComponent<KlaponManager>().isSelected = false;
                        }
                        else if (obj.tag == "Pipe_T")
                        {
                            obj.GetComponent<MoveObjectWithMouse>().isDragging = false;
                            obj.GetComponent<MoveObjectWithMouse>().isSelected = false;
                        }
                        else
                        {
                            obj.GetComponent<MovingFacilities>().isDragging = false;
                            obj.GetComponent<BoxCollider>().enabled = false;
                        }
                        obj.transform.localScale = data.scale;
                        obj.transform.parent = sborCO2.transform; // Set parent to Sbor CO2
                        sceneObjects[sceneIndex].Add(obj); // Track the instantiated object
                        Debug.Log($"Instantiated {prefab.name} at position {data.position}.");
                    }
                    else
                    {
                        Debug.LogError($"Prefab with name {data.objectName} not found.");
                    }
                }
            }

            else
            {
                Debug.LogWarning($"SceneData at index {sceneIndex} is empty or null.");
            }

            Invoke(nameof(ConnectPipe), 0.5f);
        }
        else
        {
            Debug.LogError($"SceneIndex {sceneIndex} is out of bounds for SceneDatas array.");
        }
    }

    void ConnectPipe()
    {
        pipe.ConnectComponents();
    }
}
