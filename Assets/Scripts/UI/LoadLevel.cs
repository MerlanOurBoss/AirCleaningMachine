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
        string type = Normalize(typeOfSystem.text);

        // Сравнение с запасом: в исходных строках были перемешаны латинская и кириллическая «с»,
        // и выбор, введённый с другой буквой или с пробелом на конце, не срабатывал.
        if (type == Normalize("Классический с 2 эмульгатор"))
        {
            SceneManager.LoadScene(scenes[2]);
        }
        else if (type == Normalize("4 Классический с 2 эмульгатор"))
        {
            SceneManager.LoadScene(scenes[3]);
        }
        else if (type == Normalize("Конструктор"))
        {
            SceneManager.LoadScene(scenes[1]);
        }
    }

    private static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        // Латинские двойники кириллических букв → кириллица, регистр и лишние пробелы не важны.
        var chars = s.Trim().ToLowerInvariant().ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            switch (chars[i])
            {
                case 'c': chars[i] = 'с'; break;
                case 'a': chars[i] = 'а'; break;
                case 'e': chars[i] = 'е'; break;
                case 'o': chars[i] = 'о'; break;
                case 'p': chars[i] = 'р'; break;
                case 'x': chars[i] = 'х'; break;
                case 'k': chars[i] = 'к'; break;
                case 'm': chars[i] = 'м'; break;
                case 't': chars[i] = 'т'; break;
            }
        }
        return System.Text.RegularExpressions.Regex.Replace(new string(chars), @"\s+", " ");
    }


    public void LoadLevelFromInt(int i)
    {
        SceneManager.LoadScene(i);
    }
}
