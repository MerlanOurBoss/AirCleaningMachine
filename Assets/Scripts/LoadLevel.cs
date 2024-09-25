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
        if (typeOfSystem.text == "������������ c 2 ����������")
        {
            SceneManager.LoadScene(scenes[2]);
        }
        if (typeOfSystem.text == "4 ������������ � 2 ����������")
        {
            SceneManager.LoadScene(scenes[3]);
        }
        if (typeOfSystem.text == "�����������")
        {
            SceneManager.LoadScene(scenes[1]);
        }
    }


    public void LoadLevelFromInt(int i)
    {
        SceneManager.LoadScene(i);
    }
}
