using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public SceneData[] sceneDatas; // Array to hold multiple SceneData instances
    public GameObject[] prefabs;

    public GameObject sborCO2; // Object Sbor CO2

    public GameObject buttonPrefab; // Prefab for the button
    public Transform buttonContainer; // Parent for the buttons

    public Camera cam;
    public PipeConnector pipe;

    private GameObject CameraUI;

    private Dictionary<Button, int> buttonToSceneDataMap = new Dictionary<Button, int>();
    private Dictionary<int, List<GameObject>> sceneObjects = new Dictionary<int, List<GameObject>>();

    // Добавляем класс-обертку для сериализации
    [System.Serializable]
    private class SceneDataWrapper
    {
        public ObjectData[] objectsData;
        public string screenshotPath;
    }

    private void Awake()
    {
        if (sborCO2 == null)
            sborCO2 = GameObject.FindWithTag("SborCO2");
    }

    private void Update()
    {
        sborCO2 = GameObject.FindGameObjectWithTag("SborCO2");
    }

    private void Start()
    {
        // Загружаем все сохраненные данные при старте
        LoadAllSavedData();

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

        if (gameObject.transform.parent != null)
        {
            CameraUI = gameObject.transform.parent.parent.gameObject;
        }

        // Создаем кнопки для всех сохраненных сцен
        CreateLoadButtonsForSavedData();
    }

    // Метод для загрузки всех сохраненных данных
    private void LoadAllSavedData()
    {
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            LoadSceneDataFromFile(i);
        }
    }

    // Метод для создания кнопок на основе сохраненных данных
    private void CreateLoadButtonsForSavedData()
    {
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            if (sceneDatas[i].objectsData != null && sceneDatas[i].objectsData.Length > 0)
            {
                CreateLoadButton(i);
            }
        }
    }

    // Получаем путь для сохранения файла (работает в билде)
    private string GetSaveFilePath(int sceneIndex)
    {
        string directory = Path.Combine(Application.persistentDataPath, "SavedConstructions");
        Directory.CreateDirectory(directory); // Создаем папку если не существует
        return Path.Combine(directory, $"construction_{sceneIndex}.json");
    }

    // Сохраняем SceneData в файл
    private void SaveSceneDataToFile(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            SceneData sceneData = sceneDatas[sceneIndex];

            SceneDataWrapper wrapper = new SceneDataWrapper
            {
                objectsData = sceneData.objectsData,
                screenshotPath = sceneData.screenshotPath
            };

            string json = JsonUtility.ToJson(wrapper, true);
            string filePath = GetSaveFilePath(sceneIndex);

            try
            {
                File.WriteAllText(filePath, json);
                Debug.Log($"Data saved to file: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving file: {e.Message}");
            }
        }
    }

    // Загружаем SceneData из файла
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
                Debug.LogError($"Error loading file: {e.Message}");
            }
        }
    }

    public void Save()
    {
        List<ObjectData> dataList = new List<ObjectData>();
        List<GameObject> objectsToDelete = new List<GameObject>();

        foreach (GameObject pipeObject in GameObject.FindGameObjectsWithTag("Klapon"))
        {
            if (pipeObject.activeInHierarchy)
            {
                string cleanName = pipeObject.name;
                if (cleanName.EndsWith("(Clone)"))
                {
                    cleanName = cleanName.Substring(0, cleanName.Length - "(Clone)".Length);
                }

                ObjectData data = new ObjectData
                {
                    objectName = cleanName,
                    position = pipeObject.transform.position,
                    rotation = pipeObject.transform.rotation,
                    scale = pipeObject.transform.localScale
                };
                dataList.Add(data);

                objectsToDelete.Add(pipeObject);
            }
        }

        foreach (Transform child in sborCO2.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                string cleanName = child.gameObject.name;
                if (cleanName.EndsWith("(Clone)"))
                {
                    cleanName = cleanName.Substring(0, cleanName.Length - "(Clone)".Length);
                }

                ObjectData data = new()
                {
                    objectName = cleanName,
                    position = child.transform.position,
                    rotation = child.transform.rotation,
                    scale = child.transform.localScale
                };
                dataList.Add(data);

                objectsToDelete.Add(child.gameObject);
            }
        }

        SceneData currentSceneData = FindAvailableSceneData();

        if (currentSceneData != null)
        {
            currentSceneData.objectsData = dataList.ToArray();
            Debug.Log("Data saved to SceneData.");

            int index = System.Array.IndexOf(sceneDatas, currentSceneData);
            ScreenShotTake(index);

            if (index != -1)
            {
                CreateLoadButton(index);
                // СОХРАНЯЕМ В ФАЙЛ - это ключевое изменение!
                SaveSceneDataToFile(index);
            }
            else
            {
                Debug.LogWarning("SceneData instance not found in the array.");
            }
        }
        else
        {
            Debug.LogWarning("No available SceneData instance found.");
        }

        foreach (GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }
    }

    public void Load(int sceneIndex)
    {
        // ПЕРЕД загрузкой убеждаемся, что данные актуальны из файла
        LoadSceneDataFromFile(sceneIndex);

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
                            // Логика для Pipe
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
                        obj.transform.parent = sborCO2.transform;
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

    private SceneData FindAvailableSceneData()
    {
        foreach (SceneData data in sceneDatas)
        {
            if (data.objectsData == null || data.objectsData.Length == 0)
            {
                return data;
            }
        }
        return null;
    }

    private void CreateLoadButton(int sceneIndex)
    {
        if (buttonContainer != null)
        {
            GameObject buttonInstance = Instantiate(buttonPrefab, buttonContainer);

            Button button = buttonInstance.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = $"{sceneIndex + 1}";
            buttonToSceneDataMap[button] = sceneIndex;
            button.onClick.AddListener(() => Load(sceneIndex));

            CreateChildButton(buttonInstance, sceneIndex);
            Debug.Log($"Created load button for SceneData at index {sceneIndex}.");
        }
    }

    private void CreateChildButton(GameObject parentButton, int sceneIndex)
    {
        Button childButton = parentButton.transform.Find("ChildButton").GetComponent<Button>();
        childButton.onClick.AddListener(() =>
        {
            ClearSceneData(sceneIndex);
            Destroy(parentButton);
        });
    }

    private void ScreenShotTake(int index)
    {
        if (cam != null)
        {
            CameraUI.SetActive(false);
            cam.rect = new Rect(0, 0, 1, 1);
            RenderTexture screenTexture = new RenderTexture(Screen.width * 3, Screen.height * 3, 24);
            cam.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            cam.Render();

            Texture2D renderedTexture = new Texture2D(Screen.width * 3, Screen.height * 3);
            renderedTexture.ReadPixels(new Rect(0, 0, Screen.width * 3, Screen.height * 3), 0, 0);
            renderedTexture.Apply();
            RenderTexture.active = null;

            byte[] byteArray = renderedTexture.EncodeToPNG();

            // Используем persistentDataPath для скриншотов тоже
            string dir = Path.Combine(Application.persistentDataPath, "SavedConstructions", "Images");
            Directory.CreateDirectory(dir);
            string filePath = Path.Combine(dir, $"cameracapture_{index}.png");
            File.WriteAllBytes(filePath, byteArray);

            cam.rect = new Rect(0.22f, 0.051f, 0.753f, 0.883f);
            cam.targetTexture = null;
            CameraUI.SetActive(true);

            sceneDatas[index].screenshotPath = filePath;

            // Сохраняем изменения пути скриншота
            SaveSceneDataToFile(index);
        }
    }

    private void ClearSceneData(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            SceneData sceneData = sceneDatas[sceneIndex];
            if (sceneData != null)
            {
                // Очищаем данные
                sceneData.objectsData = null;
                sceneData.screenshotPath = null;

                // Удаляем файл сохранения
                string filePath = GetSaveFilePath(sceneIndex);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Удаляем объекты
                if (sceneObjects.ContainsKey(sceneIndex))
                {
                    foreach (GameObject obj in sceneObjects[sceneIndex])
                    {
                        if (obj != null)
                        {
                            Destroy(obj);
                        }
                    }
                    sceneObjects.Remove(sceneIndex);
                }

                Debug.Log($"Cleared SceneData and destroyed objects at index {sceneIndex}.");
            }
        }
    }
}