using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private TMP_InputField typeOfSystem;
    [SerializeField] private TMP_InputField numberOfEmul;

    [SerializeField] private int[] scenes;

    public void LoadLevelString()
    {
        if (typeOfSystem.text == "Классический " && numberOfEmul.text == "1")
        {
            SceneManager.LoadScene(scenes[0]);
        }
        if (typeOfSystem.text == "Классический " && numberOfEmul.text == "2")
        {
            SceneManager.LoadScene(scenes[1]);
        }
        if (typeOfSystem.text == "Классический " && numberOfEmul.text == "3")
        {
            SceneManager.LoadScene(scenes[2]);
        }
    }
}
