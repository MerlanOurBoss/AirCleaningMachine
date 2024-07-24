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
        if (typeOfSystem.text == "������������ c 1 ����������")
        {
            SceneManager.LoadScene(scenes[0]);
        }
        if (typeOfSystem.text == "������������ c 2 ����������")
        {
            SceneManager.LoadScene(scenes[1]);
        }
        if (typeOfSystem.text == "������������ c 3 ����������")
        {
            SceneManager.LoadScene(scenes[2]);
        }
        if (typeOfSystem.text == "2 ������������ � 1 ����������")
        {
            SceneManager.LoadScene(scenes[3]);
        }
        if (typeOfSystem.text == "2 ������������ � 2 ����������")
        {
            SceneManager.LoadScene(scenes[4]);
        }
        if (typeOfSystem.text == "2 ������������ � 3 ����������")
        {
            SceneManager.LoadScene(scenes[5]);
        }
        if (typeOfSystem.text == "3 ������������ � 1 ����������")
        {
            SceneManager.LoadScene(scenes[6]);
        }
        if (typeOfSystem.text == "3 ������������ � 2 ����������")
        {
            SceneManager.LoadScene(scenes[7]);
        }
        if (typeOfSystem.text == "3 ������������ � 3 ����������")
        {
            SceneManager.LoadScene(scenes[8]);
        }
        if (typeOfSystem.text == "4 ������������ � 1 ����������")
        {
            SceneManager.LoadScene(scenes[9]);
        }
        if (typeOfSystem.text == "4 ������������ � 2 ����������")
        {
            SceneManager.LoadScene(scenes[10]);
        }
        if (typeOfSystem.text == "�����������")
        {
            SceneManager.LoadScene(scenes[11]);
        }
    }
}
