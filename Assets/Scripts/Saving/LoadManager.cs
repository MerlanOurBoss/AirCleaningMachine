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

    // Класс-обертка для десериализации JSON
    [System.Serializable]
    private class SceneDataWrapper
    {
        public ObjectData[] objectsData;
        public string screenshotPath;
    }

    private void Start()
    {
        // Загружаем все данные из файлов при старте
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
            Debug.LogWarning("Не задан SelectedIndex — нечего загружать.");
        }
    }

    // Метод для загрузки всех SceneData из файлов
    private void LoadAllSceneDataFromFiles()
    {
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            LoadSceneDataFromFile(i);
        }
    }

    // Метод для загрузки конкретного SceneData из файла
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
            // Очищаем SceneData если файла нет
            if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
            {
                sceneDatas[sceneIndex].objectsData = null;
                sceneDatas[sceneIndex].screenshotPath = null;
            }
        }
    }

    // Получаем путь к файлу сохранения
    private string GetSaveFilePath(int sceneIndex)
    {
        string directory = Path.Combine(Application.persistentDataPath, "SavedConstructions");
        return Path.Combine(directory, $"construction_{sceneIndex}.json");
    }

    public void Load(int sceneIndex)
    {
        // ПЕРЕД загрузкой обновляем данные из файла
        LoadSceneDataFromFile(sceneIndex);

        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            SceneData sceneData = sceneDatas[sceneIndex];

            if (sceneData.objectsData != null && sceneData.objectsData.Length > 0)
            {
                Debug.Log($"Loading data from SceneData at index {sceneIndex}.");

                // Очищаем старые объекты если они есть
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

                // Создаем новые объекты
                foreach (ObjectData data in sceneData.objectsData)
                {
                    GameObject prefab = System.Array.Find(prefabs, p => p.name == data.objectName);

                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab, data.position, data.rotation);

                        // Настраиваем компоненты в зависимости от тега
                        if (obj.CompareTag("Pipe"))
                        {
                            // Логика для Pipe
                        }
                        else if (obj.CompareTag("Klapon"))
                        {
                            var klaponManager = obj.GetComponent<KlaponManager>();
                            if (klaponManager != null)
                                klaponManager.isSelected = false;
                        }
                        else if (obj.CompareTag("Pipe_T"))
                        {
                            var moveObject = obj.GetComponent<MoveObjectWithMouse>();
                            if (moveObject != null)
                            {
                                moveObject.isDragging = false;
                                moveObject.isSelected = false;
                            }
                        }
                        else
                        {
                            var movingFacilities = obj.GetComponent<MovingFacilities>();
                            if (movingFacilities != null)
                            {
                                movingFacilities.isDragging = false;
                            }
                            var collider = obj.GetComponent<BoxCollider>();
                            if (collider != null)
                            {
                                collider.enabled = false;
                            }
                        }

                        obj.transform.localScale = data.scale;

                        // Устанавливаем родителя
                        if (sborCO2 != null)
                        {
                            obj.transform.parent = sborCO2.transform;
                        }

                        sceneObjects[sceneIndex].Add(obj);
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

            // Запускаем соединение труб
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

    // Метод для принудительной перезагрузки данных из файлов
    public void ReloadAllData()
    {
        LoadAllSceneDataFromFiles();
        Debug.Log("All data reloaded from files");
    }

    // Метод для проверки существования сохранения
    public bool HasSaveData(int sceneIndex)
    {
        string filePath = GetSaveFilePath(sceneIndex);
        return File.Exists(filePath);
    }

    // Метод для получения количества сохраненных объектов
    public int GetSavedObjectsCount(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            return sceneDatas[sceneIndex].objectsData?.Length ?? 0;
        }
        return 0;
    }
}