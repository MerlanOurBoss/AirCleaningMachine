using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterInput : MonoBehaviour
{
    public Slider mySliderNasos;
    public ParticleSystem[] fluids;
    public GameObject nasoso;

    private int count = 1;
    private bool isEnableNasos = false;

    void Update()
    {
        mySliderNasos.value = count;
        if (count == 1 )
        {
            foreach (ParticleSystem item in fluids)
            {
                item.startSpeed = 6;
            }
        }
        if (count == 2)
        {
            foreach (ParticleSystem item in fluids)
            {
                item.startSpeed = 7;
            }
        }
        if (count == 3)
        {
            foreach (ParticleSystem item in fluids)
            {
                item.startSpeed = 8;
            }
        }
        if (count > 3)
        {
            count = 1;
        }
    }

    public void PlusCount()
    {
        count++;
    }

    public void OnorOffObjectNasos()
    {
        if (isEnableNasos == false)
        {
            nasoso.SetActive(true); isEnableNasos = true;
        }
        else
        {
            nasoso.SetActive(false); isEnableNasos = false;

        }
    }
}
