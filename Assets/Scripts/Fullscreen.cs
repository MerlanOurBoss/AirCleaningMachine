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
    public GameObject res;

    private Resolution myres;
    public void Start()
    {
        myres = Screen.currentResolution;
    }
    public void Change()
    {
        Screen.fullScreen = !Screen.fullScreen;
        res.SetActive(!res.activeSelf);

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
    public void Resolution0()
    {
        Screen.SetResolution(myres.width, myres.height, FullScreenMode.Windowed);
    }

    public void Resolution1()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    public void Resolution2()
    {
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }

    public void Resolution3()
    {
        Screen.SetResolution(640, 480, FullScreenMode.Windowed);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
