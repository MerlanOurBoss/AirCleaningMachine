using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadManager : MonoBehaviour
{
    public SceneData[] sceneDatas; // Array to hold multiple SceneData instances
    public GameObject[] prefabs;

    public GameObject sborCO2; // Object Sbor CO2
    public PipeConnector pipe;
    private Dictionary<int, List<GameObject>> sceneObjects = new Dictionary<int, List<GameObject>>();

    [Header("UIManager")]
    public UIManager uiManager;
    
    
    [System.Serializable]
    private class SceneDataWrapper
    {
        public ObjectData[] objectsData;
        public string screenshotPath;
    }

    private void Awake()
    {
        LoadAllSceneDataFromFiles();

        int idx = ConstructionSelector.SelectedIndex;
        if (idx >= 0)
        {
            Load(idx);
            Debug.Log(idx);
            ConstructionSelector.SelectedIndex = -1;
        }
        else
        {
            Debug.LogWarning("Не задан SelectedIndex — загрузка конструкции не выполняется.");
        }
    }

    private void LoadAllSceneDataFromFiles()
    {
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            LoadSceneDataFromFile(i);
        }
    }

    private void LoadSceneDataFromFile(int sceneIndex)
    {
        string filePath = GetSaveFilePath(sceneIndex);

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                SceneDataWrapper wrapper = JsonUtility.FromJson<SceneDataWrapper>(json);

                if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
                {
                    sceneDatas[sceneIndex].objectsData = wrapper.objectsData;
                    sceneDatas[sceneIndex].screenshotPath = wrapper.screenshotPath;
                    Debug.Log($"Data loaded from file: {filePath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading file {filePath}: {e.Message}");
            }
        }
        else
        {
            Debug.Log($"No save file found at: {filePath}");
            if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
            {
                sceneDatas[sceneIndex].objectsData = null;
                sceneDatas[sceneIndex].screenshotPath = null;
            }
        }
    }

    private string GetSaveFilePath(int sceneIndex)
    {
        string directory = Path.Combine(Application.persistentDataPath, "SavedConstructions");
        return Path.Combine(directory, $"construction_{sceneIndex}.json");
    }

    public void Load(int sceneIndex)
    {
        LoadSceneDataFromFile(sceneIndex);

        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            SceneData sceneData = sceneDatas[sceneIndex];

            if (sceneData.objectsData != null && sceneData.objectsData.Length > 0)
            {
                Debug.Log($"Loading data from SceneData at index {sceneIndex}.");

                if (sceneObjects.ContainsKey(sceneIndex))
                {
                    foreach (GameObject obj in sceneObjects[sceneIndex])
                    {
                        if (obj != null)
                        {
                            Destroy(obj);
                        }
                    }
                    sceneObjects[sceneIndex].Clear();
                }
                else
                {
                    sceneObjects[sceneIndex] = new List<GameObject>();
                }

                foreach (ObjectData data in sceneData.objectsData)
                {
                    GameObject prefab = System.Array.Find(prefabs, p => p != null && p.name == data.objectName);
                    
                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab, data.position, data.rotation);
                        
                        if (uiManager != null)
                            uiManager.RegisterUI(obj);
                        
                        // Пустой тег (старые сохранения клапанов) не ставим — Unity бросит исключение
                        // и прервёт загрузку остальных объектов.
                        if (!string.IsNullOrEmpty(data.tag))
                            obj.tag = data.tag;
                        var movingFacilities = obj.GetComponent<MovingFacilities>();
                        if (movingFacilities != null)
                        {
                            movingFacilities.enabled = false;

                        }
                        
                        // У труб и клапанов коллайдер оставляем, у аппаратов — выключаем.
                        if (!obj.CompareTag(Tags.Pipe) && !obj.CompareTag(Tags.Klapon) && !obj.CompareTag(Tags.Pipe_T))
                        {
                            var collider = obj.GetComponent<BoxCollider>();
                            if (collider != null)
                            {
                                collider.enabled = false;
                            }
                        }

                        obj.transform.localScale = data.scale;

                        if (sborCO2 != null)
                        {
                            obj.transform.parent = sborCO2.transform;
                        }

                        sceneObjects[sceneIndex].Add(obj);
                        Debug.Log($"Instantiated {prefab.name} at position {data.position}.");
                    }
                    else
                    {
                        // Сгенерированный меш труб (PipeMesh_Standard, PipeMeshStandardPrefab) не загружаем:
                        // его заново строит ConnectPipe. Проверка как в SaveLoadManager.IsGeneratedPipeMesh.
                        if (string.IsNullOrEmpty(data.objectName) || !data.objectName.StartsWith("PipeMesh"))
                            Debug.LogError($"Prefab with name {data.objectName} not found.");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"SceneData at index {sceneIndex} is empty or null.");
            }

            if (pipe != null)
            {
                Invoke(nameof(ConnectPipe), 0.5f);
            }
        }
        else
        {
            Debug.LogError($"SceneIndex {sceneIndex} is out of bounds for SceneDatas array.");
        }
    }

    void ConnectPipe()
    {
        if (pipe != null)
        {
            pipe.ConnectComponents();
        }
    }

    public void ReloadAllData()
    {
        LoadAllSceneDataFromFiles();
        Debug.Log("All data reloaded from files");
    }

    public bool HasSaveData(int sceneIndex)
    {
        string filePath = GetSaveFilePath(sceneIndex);
        return File.Exists(filePath);
    }

    public int GetSavedObjectsCount(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            return sceneDatas[sceneIndex].objectsData?.Length ?? 0;
        }
        return 0;
    }
}