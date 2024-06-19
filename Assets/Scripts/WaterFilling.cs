using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterFilling : MonoBehaviour
{
    public Material water;

    private float water_filling = 0;

    private void Start()
    {
        water_filling = water.GetFloat("_Filling");
    }

    private void Update()
    {
        water.SetFloat("_Filling", water_filling);

        if (water_filling >= 4)
        {
            water_filling = -4;
        }
    }

    public void AddWater()
    {
        water_filling += 2;
    }
}
