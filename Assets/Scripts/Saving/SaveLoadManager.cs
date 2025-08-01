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
        int idx = ConstructionSelector.SelectedIndex;
        if (idx >= 0)
        {
            Load(idx);
            Debug.Log(idx);

            ConstructionSelector.SelectedIndex = -1;
        }
        else
        {
            Debug.LogWarning("�� ����� SelectedIndex � ������ ���������.");
        }


        if (gameObject.transform.parent != null)
        {
            CameraUI = gameObject.transform.parent.parent.gameObject;
        }
        // Create a button for each SceneData that contains saved data
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            if (sceneDatas[i].objectsData != null && sceneDatas[i].objectsData.Length > 0)
            {
                CreateLoadButton(i); // Create a button for this SceneData
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

            // Create a button for the newly saved SceneData
            int index = System.Array.IndexOf(sceneDatas, currentSceneData);
            ScreenShotTake(index);
            if (index != -1)
            {
                CreateLoadButton(index);
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

    private SceneData FindAvailableSceneData()
    {
        foreach (SceneData data in sceneDatas)
        {
            if (data.objectsData == null || data.objectsData.Length == 0)
            {
                return data; // Return the first empty SceneData
            }
        }
        return null; // All SceneData instances are filled
    }

    private void CreateLoadButton(int sceneIndex)
    {
        if (buttonContainer != null)
        {
            GameObject buttonInstance = Instantiate(buttonPrefab, buttonContainer);

            Button button = buttonInstance.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();

            // Set the text of the main button
            buttonText.text = $"{sceneIndex + 1}";

            // Map button to SceneData index
            buttonToSceneDataMap[button] = sceneIndex;

            // Add a listener to load all objects from the SceneData at this index
            button.onClick.AddListener(() => Load(sceneIndex));

            // Create and configure the child button
            CreateChildButton(buttonInstance, sceneIndex);

            Debug.Log($"Created load button for SceneData at index {sceneIndex}.");
        }

    }
    private void CreateChildButton(GameObject parentButton, int sceneIndex)
    {
        // Assuming the child button is a child of the parent button
        Button childButton = parentButton.transform.Find("ChildButton").GetComponent<Button>();

        // Add listener to the child button
        childButton.onClick.AddListener(() =>
        {
            ClearSceneData(sceneIndex);
            Destroy(parentButton); // Destroy the parent button
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

            string dir = Path.Combine(Application.dataPath, "SavedObjects", "Images");
            Directory.CreateDirectory(dir);
            string filePath = Path.Combine(dir, $"cameracapture_{index}.png");
            File.WriteAllBytes(filePath, byteArray);

            cam.rect = new Rect(0.22f, 0.051f, 0.753f, 0.883f);
            cam.targetTexture = null;
            CameraUI.SetActive(true);

            sceneDatas[index].screenshotPath = filePath;
        }
        
    }
    private void ClearSceneData(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneDatas.Length)
        {
            SceneData sceneData = sceneDatas[sceneIndex];
            if (sceneData != null)
            {
                // Clear the saved data
                sceneData.objectsData = null;

                // Destroy all instantiated objects for this SceneData
                if (sceneObjects.ContainsKey(sceneIndex))
                {
                    foreach (GameObject obj in sceneObjects[sceneIndex])
                    {
                        if (obj != null)
                        {
                            Destroy(obj);
                        }
                    }
                    sceneObjects.Remove(sceneIndex); // Remove the tracking entry
                }

                Debug.Log($"Cleared SceneData and destroyed objects at index {sceneIndex}.");
            }
        }
    }
}
