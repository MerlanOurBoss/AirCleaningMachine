using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadDefaultSimulation : MonoBehaviour
{
    [Header("Default simulation assets")]
    public TextAsset defaultSimulationText;
    public Texture2D defaultSimulationTexture;

    private void Awake()
    {
        SaveDefaultSimulationJson();
        SaveDefaultSimulationScreenshot();
    }
    private void SaveDefaultSimulationJson()
    {
        string path = GetSaveFilePath();
        File.WriteAllText(path, defaultSimulationText.text);

    }

    private void SaveDefaultSimulationScreenshot()
    {
        string path = GetScreenshotFilePath();

        byte[] pngBytes = defaultSimulationTexture.EncodeToPNG();
        File.WriteAllBytes(path, pngBytes);
    }

    private string GetSaveFilePath()
    {
        string directory = Path.Combine(
            Application.persistentDataPath,
            "SavedConstructions"
        );
        
        Directory.CreateDirectory(directory);
        
        return Path.Combine(directory, defaultSimulationText.name + ".json");
    }
    
    private string GetScreenshotFilePath()
    {
        string directory = Path.Combine(
            Application.persistentDataPath,
            "SavedConstructions",
            "Images"
        );

        Directory.CreateDirectory(directory);

        return Path.Combine(directory, defaultSimulationTexture.name + ".png");
    }
}
