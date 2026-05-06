using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLoader : MonoBehaviour
{
    public TextMeshProUGUI uiText; 

    void Start()
    {
        LoadTextFromFile("text"); 
    }

    void LoadTextFromFile(string fileName)
    {
        TextAsset textFile = Resources.Load<TextAsset>(fileName);

        if (textFile != null)
        {
            uiText.text = textFile.text;
        }
        else
        {
            Debug.LogError("Не удалось загрузить файл: " + fileName);
        }
    }
}
