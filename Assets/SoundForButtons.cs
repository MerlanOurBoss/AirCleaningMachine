using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundForButtons : MonoBehaviour
{
    public Button[] buttons;
    public AudioSource audi;

    public void Start()
    {
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(audi.Play);
        }
    }
}
