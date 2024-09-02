using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KatazInputSecond : MonoBehaviour
{
    public Slider mySliderNasos;

    public KatalizatorScript kataz;

    public GameObject nasoso;

    public SimulationScriptForSecondScene myscript;

    private int count = 1;
    private bool isEnableNasos = false;

    
    void Update()
    {
        mySliderNasos.value = count;

        if (count > 3)
        {
            count = 0;
        }
        if (myscript._startSimulationTemp)
        {
            nasoso.SetActive(false); isEnableNasos = false;
        }
    }

    public void PlusCount()
    {
        count++;
        if (count == 1)
        {
            kataz.ChangePosition(0);
        }
        if (count == 2)
        {
            kataz.ChangePosition(1);
        }
        else if (count == 3)
        {
            kataz.ChangePosition(2);
        } 
    }


    public void OnorOffObjectNasos()
    {
        if (isEnableNasos == false && !myscript._startSimulationTemp)
        {
            nasoso.SetActive(true); isEnableNasos = true;
        }
        else
        {
            nasoso.SetActive(false); isEnableNasos = false;
        }
    }
}
