using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public SceneData[] sceneDatas;
    public GameObject[] prefabs;

    public GameObject sborCO2;

    public GameObject buttonPrefab;
    public Transform buttonContainer;

    public Camera cam;
    public PipeConnector pipe;

    private GameObject CameraUI;

    private Dictionary<Button, int> buttonToSceneDataMap = new Dictionary<Button, int>();
    private Dictionary<int, List<GameObject>> sceneObjects = new Dictionary<int, List<GameObject>>();
    
    [System.Serializable]
    private class SceneDataWrapper
    {
        public ObjectData[] objectsData;
        public string screenshotPath;
    }

    private void Awake()
    {
        ResolveSborCO2();
    }

    private void Update()
    {
        // SborManager пересоздаёт объект SborCO2 после очистки — поэтому ссылку проверяем постоянно.
        ResolveSborCO2();
    }

    /// <summary>
    /// Возвращает объект с тегом "SborCO2" (родитель конструкции).
    /// Ссылка из инспектора используется, только если у объекта действительно тег SborCO2:
    /// в сцене "Constructor second scene" в поле sborCO2 назначен CameraUI, и при сохранении
    /// в слот попадали кнопки интерфейса (Button, Settings, System settings, Tips).
    /// </summary>
    private GameObject ResolveSborCO2()
    {
        if (sborCO2 == null || !sborCO2.CompareTag("SborCO2"))
            sborCO2 = GameObject.FindGameObjectWithTag("SborCO2");
        return sborCO2;
    }

    private void Start()
    {
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
            Debug.LogWarning("Не задан SelectedIndex — загрузка конструкции не выполняется.");
        }

        if (gameObject.transform.parent != null && gameObject.transform.parent.parent != null)
        {
            CameraUI = gameObject.transform.parent.parent.gameObject;
        }

        CreateLoadButtonsForSavedData();
    }

    private void LoadAllSavedData()
    {
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            LoadSceneDataFromFile(i);
        }
    }

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

    private string GetSaveFilePath(int sceneIndex)
    {
        string directory = Path.Combine(Application.persistentDataPath, "SavedConstructions");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"construction_{sceneIndex}.json");
    }

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
        else if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            // SceneData — ScriptableObject-ассет: в редакторе данные прошлых запусков остаются в нём
            // и слот выглядит занятым, хотя файла сохранения нет. Очищаем.
            sceneDatas[sceneIndex].objectsData = null;
            sceneDatas[sceneIndex].screenshotPath = null;
        }
    }

    public void Save()
    {
        // Сначала проверяем, есть ли куда сохранять. Раньше при занятых слотах объекты
        // всё равно уничтожались, и конструкция пропадала без сохранения.
        SceneData currentSceneData = FindAvailableSceneData();
        if (currentSceneData == null)
        {
            Debug.LogWarning("Нет свободного слота для сохранения. Удалите одно из сохранений — " +
                             "текущая конструкция не сохранена и не удалена.");
            return;
        }

        if (ResolveSborCO2() == null)
        {
            Debug.LogWarning("SaveLoadManager: объект с тегом 'SborCO2' не найден — сохранять нечего.");
            return;
        }

        List<ObjectData> dataList = new List<ObjectData>();
        List<GameObject> objectsToDelete = new List<GameObject>();

        foreach (GameObject pipeObject in GameObject.FindGameObjectsWithTag("Klapon"))
        {
            if (!pipeObject.activeInHierarchy) continue;

            // Клапаны внутри SborCO2 обработаются в цикле ниже — не пишем их дважды.
            if (pipeObject.transform.IsChildOf(sborCO2.transform)) continue;

            TryAddObjectData(pipeObject, dataList);
            objectsToDelete.Add(pipeObject);
        }

        foreach (Transform child in sborCO2.transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            TryAddObjectData(child.gameObject, dataList);
            objectsToDelete.Add(child.gameObject);
        }

        // Если среди объектов нет ни одного префаба из prefabs[], слот не занимаем и ничего не удаляем.
        if (dataList.Count == 0)
        {
            Debug.LogWarning("SaveLoadManager: на сцене нет объектов из списка prefabs[] — сохранять нечего.");
            return;
        }

        currentSceneData.objectsData = dataList.ToArray();
        int index = System.Array.IndexOf(sceneDatas, currentSceneData);

        ScreenShotTake(index);
        CreateLoadButton(index);
        SaveSceneDataToFile(index);
        Debug.Log($"Конструкция сохранена в слот {index}.");

        foreach (GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }

        if (pipe != null)
            pipe.ClearAllPipesAndFacilities();
    }

    // Сохраняем только объекты, для которых есть префаб с таким же именем в prefabs[]:
    // остальные (сгенерированные меши труб и т.п.) всё равно нельзя восстановить при загрузке.
    private bool TryAddObjectData(GameObject go, List<ObjectData> dataList)
    {
        string cleanName = GetCleanName(go);

        bool isKnownPrefab = prefabs != null &&
                             System.Array.Exists(prefabs, p => p != null && p.name == cleanName);

        dataList.Add(CreateObjectData(go, cleanName));
        return true;
    }

    private static string GetCleanName(GameObject go)
    {
        string cleanName = go.name.Trim();
        if (cleanName.EndsWith("(Clone)"))
            cleanName = cleanName.Substring(0, cleanName.Length - "(Clone)".Length).TrimEnd();
        return cleanName;
    }

    private static ObjectData CreateObjectData(GameObject go, string cleanName)
    {
        return new ObjectData
        {
            objectName = cleanName,
            position = go.transform.position,
            rotation = go.transform.rotation,
            scale = go.transform.localScale,
            tag = go.tag // раньше у клапанов тег не сохранялся
        };
    }

    public void Load(int sceneIndex)
    {
        LoadSceneDataFromFile(sceneIndex);

        if (sceneIndex < 0 || sceneIndex >= sceneDatas.Length)
        {
            Debug.LogError($"SceneIndex {sceneIndex} is out of bounds for SceneDatas array.");
            return;
        }

        SceneData sceneData = sceneDatas[sceneIndex];

        if (sceneData.objectsData == null || sceneData.objectsData.Length == 0)
        {
            Debug.LogWarning($"SceneData at index {sceneIndex} is empty or null.");
            return;
        }

        Debug.Log($"Loading data from SceneData at index {sceneIndex}.");

        // Повторная загрузка того же слота раньше создавала дубликаты — убираем уже загруженное.
        if (sceneObjects.TryGetValue(sceneIndex, out var previouslyLoaded))
        {
            foreach (GameObject obj in previouslyLoaded)
                if (obj != null) Destroy(obj);
            previouslyLoaded.Clear();
        }
        else
        {
            sceneObjects[sceneIndex] = new List<GameObject>();
        }

        ResolveSborCO2();

        foreach (ObjectData data in sceneData.objectsData)
        {
            GameObject prefab = System.Array.Find(prefabs, p => p != null && p.name == data.objectName);

            if (prefab == null)
            {
                Debug.LogError($"Prefab with name {data.objectName} not found.");
                continue;
            }

            GameObject obj = Instantiate(prefab, data.position, data.rotation);

            // Восстанавливаем тег (тип эмульгатора и т.п.). Пустой тег не ставим — Unity бросит исключение.
            if (!string.IsNullOrEmpty(data.tag))
                obj.tag = data.tag;

            if (obj.CompareTag("Pipe"))
            {
            }
            else if (obj.CompareTag("Klapon"))
            {
                var klapon = obj.GetComponent<KlaponManager>();
                if (klapon != null) klapon.isSelected = false;
            }
            else if (obj.CompareTag("Pipe_T"))
            {
                var mover = obj.GetComponent<MoveObjectWithMouse>();
                if (mover != null)
                {
                    mover.isDragging = false;
                    mover.isSelected = false;
                }
            }
            else
            {
                var facility = obj.GetComponent<MovingFacilities>();
                if (facility != null) facility.isDragging = false;

                var box = obj.GetComponent<BoxCollider>();
                if (box != null) box.enabled = false;
            }

            obj.transform.localScale = data.scale;
            if (sborCO2 != null)
                obj.transform.parent = sborCO2.transform;

            sceneObjects[sceneIndex].Add(obj);
            Debug.Log($"Instantiated {prefab.name} at position {data.position}.");
        }

        Invoke(nameof(ConnectPipe), 0.5f);
    }

    void ConnectPipe()
    {
        if (pipe != null)
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

    private static string ScreenshotsDirectory =>
        Path.Combine(Application.persistentDataPath, "SavedConstructions", "Images");

    // В SceneData храним относительный путь; старые сохранения могли содержать абсолютный.
    private static string ResolveScreenshotPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        return Path.IsPathRooted(path) ? path : Path.Combine(Application.persistentDataPath, path);
    }

    private void ScreenShotTake(int index)
    {
        if (cam == null || index < 0) return;

        int width = Screen.width * 3;
        int height = Screen.height * 3;
        Rect previousRect = cam.rect;

        if (CameraUI != null) CameraUI.SetActive(false);
        cam.rect = new Rect(0, 0, 1, 1);

        RenderTexture screenTexture = new RenderTexture(width, height, 24);
        Texture2D renderedTexture = null;
        try
        {
            cam.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            cam.Render();

            renderedTexture = new Texture2D(width, height);
            renderedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            renderedTexture.Apply();

            byte[] byteArray = renderedTexture.EncodeToPNG();

            Directory.CreateDirectory(ScreenshotsDirectory);
            string relativePath = Path.Combine("SavedConstructions", "Images", $"cameracapture_{index}.png");
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, relativePath), byteArray);

            // Раньше здесь путь перезаписывался абсолютным — сейв ломался на другом компьютере.
            sceneDatas[index].screenshotPath = relativePath;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Не удалось сохранить скриншот конструкции: {e.Message}");
        }
        finally
        {
            RenderTexture.active = null;
            cam.targetTexture = null;
            cam.rect = previousRect;
            if (CameraUI != null) CameraUI.SetActive(true);

            // Освобождаем память (раньше RenderTexture и Texture2D утекали при каждом сохранении).
            screenTexture.Release();
            Destroy(screenTexture);
            if (renderedTexture != null) Destroy(renderedTexture);
        }
    }
    
    private void ClearSceneData(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            SceneData sceneData = sceneDatas[sceneIndex];
            if (sceneData != null)
            {
                // Сохраняем путь ДО обнуления
                string screenshotPath = sceneData.screenshotPath;

                // Удаляем скриншот: путь из сохранения (относительный или старый абсолютный)
                // и стандартный файл слота — иначе в меню оставалась картинка без конструкции.
                var screenshotCandidates = new HashSet<string>();
                string resolved = ResolveScreenshotPath(screenshotPath);
                if (resolved != null) screenshotCandidates.Add(Path.GetFullPath(resolved));
                screenshotCandidates.Add(Path.GetFullPath(Path.Combine(ScreenshotsDirectory, $"cameracapture_{sceneIndex}.png")));

                foreach (string candidate in screenshotCandidates)
                {
                    if (File.Exists(candidate))
                    {
                        File.Delete(candidate);
                        Debug.Log($"Screenshot deleted: {candidate}");
                    }
                }

                // Удаляем JSON-файл сохранения
                string saveFilePath = GetSaveFilePath(sceneIndex);
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                    Debug.Log($"Save file deleted: {saveFilePath}");
                }

                // Теперь обнуляем данные
                sceneData.objectsData = null;
                sceneData.screenshotPath = null;

                // Уничтожаем объекты сцены
                if (sceneObjects.ContainsKey(sceneIndex))
                {
                    foreach (GameObject obj in sceneObjects[sceneIndex])
                    {
                        if (obj != null)
                            Destroy(obj);
                    }
                    sceneObjects.Remove(sceneIndex);
                }

                Debug.Log($"Cleared SceneData and destroyed objects at index {sceneIndex}.");
            }
        }
    }
}