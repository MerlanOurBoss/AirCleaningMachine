using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private TMP_InputField typeOfSystem;

    [SerializeField] private int[] scenes;

    public void LoadLevelString()
    {
        if (typeOfSystem.text == "Классический c 1 эмульгатор")
        {
            SceneManager.LoadScene(scenes[0]);
        }
        if (typeOfSystem.text == "Классический c 2 эмульгатор")
        {
            SceneManager.LoadScene(scenes[1]);
        }
        if (typeOfSystem.text == "Классический c 3 эмульгатор")
        {
            SceneManager.LoadScene(scenes[2]);
        }
        if (typeOfSystem.text == "2 Классический с 1 эмульгатор")
        {
            SceneManager.LoadScene(scenes[3]);
        }
        if (typeOfSystem.text == "2 Классический с 2 эмульгатор")
        {
            SceneManager.LoadScene(scenes[4]);
        }
        if (typeOfSystem.text == "2 Классический с 3 эмульгатор")
        {
            SceneManager.LoadScene(scenes[5]);
        }
        if (typeOfSystem.text == "3 Классический с 1 эмульгатор")
        {
            SceneManager.LoadScene(scenes[6]);
        }
        if (typeOfSystem.text == "3 Классический с 2 эмульгатор")
        {
            SceneManager.LoadScene(scenes[7]);
        }
        if (typeOfSystem.text == "3 Классический с 3 эмульгатор")
        {
            SceneManager.LoadScene(scenes[8]);
        }
        if (typeOfSystem.text == "4 Классический с 1 эмульгатор")
        {
            SceneManager.LoadScene(scenes[9]);
        }
        if (typeOfSystem.text == "4 Классический с 2 эмульгатор")
        {
            SceneManager.LoadScene(scenes[10]);
        }
        if (typeOfSystem.text == "Конструктор")
        {
            SceneManager.LoadScene(scenes[11]);
        }
    }
}
