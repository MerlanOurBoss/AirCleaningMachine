using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelInt : MonoBehaviour
{
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);

        ClickForDestroy[] trubes = FindObjectsOfType<ClickForDestroy>();

        foreach (ClickForDestroy obj in trubes)
        {
            obj.enabled = false;
        }
    }
}
