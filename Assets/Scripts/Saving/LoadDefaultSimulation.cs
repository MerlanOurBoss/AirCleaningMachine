using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadDefaultSimulation : MonoBehaviour
{
    [Header("Default simulation assets")]
    public TextAsset[] defaultSimulationTexts;
    public Texture2D[] defaultSimulationTextures;

    private void Awake()
    {
        SaveDefaultSimulationJsons();
        SaveDefaultSimulationScreenshots();
    }

    private void SaveDefaultSimulationJsons()
    {
        foreach (TextAsset textAsset in defaultSimulationTexts)
        {
            if (textAsset == null) continue;

            string path = GetSaveFilePath(textAsset.name);

            if (File.Exists(path))
            {
                Debug.Log($"[LoadDefaultSimulation] Save file already exists, skipping: {path}");
                continue;
            }

            File.WriteAllText(path, textAsset.text);
            Debug.Log($"[LoadDefaultSimulation] Default simulation saved to: {path}");
        }
    }

    private void SaveDefaultSimulationScreenshots()
    {
        foreach (Texture2D texture in defaultSimulationTextures)
        {
            if (texture == null) continue;

            string path = GetScreenshotFilePath(texture.name);

            if (File.Exists(path))
            {
                Debug.Log($"[LoadDefaultSimulation] Screenshot already exists, skipping: {path}");
                continue;
            }

            byte[] pngBytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngBytes);
            Debug.Log($"[LoadDefaultSimulation] Default screenshot saved to: {path}");
        }
    }

    private string GetSaveFilePath(string fileName)
    {
        string directory = Path.Combine(
            Application.persistentDataPath,
            "SavedConstructions"
        );

        Directory.CreateDirectory(directory);

        return Path.Combine(directory, fileName + ".json");
    }

    private string GetScreenshotFilePath(string fileName)
    {
        string directory = Path.Combine(
            Application.persistentDataPath,
            "SavedConstructions",
            "Images"
        );

        Directory.CreateDirectory(directory);

        return Path.Combine(directory, fileName + ".png");
    }
}
