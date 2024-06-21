using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamgingEmul : MonoBehaviour
{
    public MeshRenderer[] fluidsWater;
    public MeshRenderer[] meshMaterial;

    public Material reded;
    public Material blued;
    public Material greened;
    private int countWater = 1;

    private void Update()
    {
        if (countWater == 1)
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(1, 1, 1, 0.6f);
            }

            foreach (MeshRenderer item in meshMaterial)
            {
                item.material = blued;
            }
        }
        if (countWater == 2)
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(0.7f, 1, 0, 0.6f);
            }

            foreach (MeshRenderer item in meshMaterial)
            {
                item.material = reded;
            }
        }
        if (countWater == 3)
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(0, 0, 0, 0.6f);
            }

            foreach (MeshRenderer item in meshMaterial)
            {
                item.material = greened;
            }
        }
        if (countWater > 3)
        {
            countWater = 1;
        }
    }
    public void ChangingE()
    {
        countWater++;
    }
}
