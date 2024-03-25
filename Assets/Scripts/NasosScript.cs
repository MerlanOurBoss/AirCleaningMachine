using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using UnityEngine.UI;

public class NasosScript : MonoBehaviour
{
    public Slider mySliderNasos;
    public Slider mySliderWater;
    public GameObject[] fluids;
    public MeshRenderer[] fluidsWater;
    public GameObject nasoso;
    public GameObject water;
    private int count = 1;
    public int countWater = 1;
    private bool isEnableNasos = false;
    private bool isEnableWater = false;

    private void Update()
    {
        mySliderNasos.value = count;
        mySliderWater.value = countWater;
        if (count == 1 || count == 2)
        {
            foreach (GameObject item in fluids)
            {
                item.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        if (count == 3)
        {
            foreach (GameObject item in fluids)
            {
                item.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
            }
        }
        if (count == 4)
        {
            foreach (GameObject item in fluids)
            {
                item.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
        }
        if (count > 4)
        {
            count = 1;
        }


        if (countWater == 1)
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(1, 1, 1, 0.6f);
            }
        }

        if (countWater == 2 )
        {
            foreach(MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(0.7f, 1, 0, 0.6f);
            }
        }

        if (countWater == 3)
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(0, 0, 0, 0.6f);
            }
        }
        if (countWater > 3)
        {
            countWater = 1;
        }

    }


    public void PlusCount()
    {
        count++;
    }

    public void PlusCountWater()
    {
        countWater++;
    }


    public void OnorOffObjectNasos()
    {
        if (isEnableNasos == false) { nasoso.SetActive(true); isEnableNasos = true;
            water.SetActive(false); isEnableWater = false;
            Debug.Log("is EnableWater" + isEnableWater);
            Debug.Log("is EnableNasos" + isEnableNasos);
        }
        else { nasoso.SetActive(false); isEnableNasos = false; isEnableWater = false;

        }
    }

    public void OnorOffObjectWater()
    {
        if (isEnableWater == false) { water.SetActive(true); isEnableWater = true;
            nasoso.SetActive(false); isEnableNasos = false;
        }
        else { water.SetActive(false); isEnableWater = false; isEnableNasos = false;
        }
    }
}
