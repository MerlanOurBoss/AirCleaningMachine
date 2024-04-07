using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KatazInput : MonoBehaviour
{
    public Slider mySliderNasos;
    public KatalizatorScript kataz;
    public GameObject nasoso;

    private int count = 1;
    private bool isEnableNasos = false;

    void Update()
    {
        mySliderNasos.value = count;
        if (count == 1)
        {
            kataz.changePosition0();
        }
        if (count == 2)
        {
            kataz.changePosition1();
        }
        if (count == 3)
        {
            kataz.changePosition2();
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
