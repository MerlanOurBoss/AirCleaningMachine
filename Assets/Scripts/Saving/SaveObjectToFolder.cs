using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
public class SaveObjectToFolder : MonoBehaviour
{
    public GameObject toSave;
    public GameObject canvas;
    public GameObject buttonObject;

    [MenuItem("Tools/Save Object To Folder")]
    public void SaveObject()
    {
        string basePath = "Assets/SavedObjects/" + toSave.name + ".prefab";
        string path = basePath;
        int counter = 1;

        // ���������, ���������� �� ��� ���� � ����� ������
        while (File.Exists(path))
        {
            path = "Assets/SavedObjects/" + toSave.name + "_" + counter + ".prefab";
            counter++;
        }
        // ���������, ���������� �� �����, � ���� ���, ������� ��
        string folderPath = "Assets/SavedObjects";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "SavedObjects");
        }

        // ��������� ������ ��� Prefab
        PrefabUtility.SaveAsPrefabAsset(toSave, path);

        // ��������� AssetDatabase
        AssetDatabase.Refresh();

        // ������� ��������� ������ �� �����
        DestroyImmediate(toSave);

        Debug.Log("Object saved to " + path);

        // ������� UI Button
        CreateUIButton(path);
    }

    public void CreateUIButton(string prefabPath)
    {
        // ������� ����� Canvas, ���� ��� ��� �� �����
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            //canvas = canvasObject.GetComponent<Canvas>();
        }

        // ������� ����� UI Button
        GameObject obj = Instantiate(buttonObject, canvas.transform);

        Button button = obj.GetComponent<Button>();

        // ��������� ���������� ������� �� ������
        Debug.Log("1");
        button.onClick.AddListener(() => SpawnPrefab(prefabPath));
    }

    static void SpawnPrefab(string prefabPath)
    {
        // ��������� ������
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Debug.Log("2");
        if (prefab != null)
        {
            // ������� ��������� ������� �� �����
            GameObject.Instantiate(prefab);
            Debug.Log("Prefab instantiated from " + prefabPath);
        }
        else
        {
            Debug.LogError("Failed to load prefab at " + prefabPath);
        }
    }
}
