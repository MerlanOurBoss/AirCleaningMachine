using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fullscreen : MonoBehaviour
{
    public Image[] toggleBack;
    public Sprite fullscreen;
    public Sprite windowed;
    public Toggle[] myToggle;
    public void Change()
    {
        Screen.fullScreen = !Screen.fullScreen;

        for (int i = 0; i < toggleBack.Length; i++) 
        {
            if (myToggle[i].isOn == true)
            {
                foreach (var toggle in toggleBack)
                {
                    toggle.sprite = fullscreen;
                }
            }
            else
            {
                foreach (var toggle in toggleBack)
                {
                    toggle.sprite = windowed;
                }
            }
        }
            
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
