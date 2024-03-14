using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampManagment : MonoBehaviour
{
    [SerializeField] private GameObject[] lamps;
    private bool iEnabled = false;
    public Image lightBulb;

    public Sprite isON;
    public Sprite isOFF;
    public void OnorOffLamps()
    {
        if (!iEnabled)
        {
            foreach (var lamp in lamps)
            {
                lamp.SetActive(false);
            }
            lightBulb.sprite = isOFF;
            iEnabled = true;
        }
        else
        {
            foreach (var lamp in lamps)
            {
                lamp.SetActive(true);
            }
            lightBulb.sprite = isON;
            iEnabled = false;
        }
    }
}
