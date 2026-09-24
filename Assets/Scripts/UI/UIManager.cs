using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> uiElements = new List<GameObject>();

    public void RegisterUI(GameObject ui)
    {
        if (ui != null && !uiElements.Contains(ui))
        {
            uiElements.Add(ui);
        }
    }

    public void DisableAllUI()
    {
        foreach (GameObject ui in uiElements)
        {
            if (ui != null)
                ui.SetActive(false);
        }
    }

    public void EnableAllUI()
    {
        foreach (GameObject ui in uiElements)
        {
            if (ui != null)
                ui.SetActive(true);
        }
    }
}