using System;
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
    public int countWater;

    private void Start()
    {
        if (gameObject.tag == "Facilities_Emul")
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(1, 1, 1, 0.6f);
            }

            foreach (MeshRenderer item in meshMaterial)
            {
                item.material = blued;
            }

            countWater = 1;
        }
        else if (gameObject.tag == "Facilities_Emul_Reagent")
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(0.7f, 1, 0, 0.6f);
            }

            foreach (MeshRenderer item in meshMaterial)
            {
                item.material = reded;
            }
            countWater = 2;
        }
        else if (gameObject.tag == "Facilities_Emul_Soda")
        {
            foreach (MeshRenderer item in fluidsWater)
            {
                item.material.color = new Color(0, 0, 0, 0.6f);
            }

            foreach (MeshRenderer item in meshMaterial)
            {
                item.material = greened;
            }
            countWater = 3;
        }
    }

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
            gameObject.tag = "Facilities_Emul";
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
            gameObject.tag = "Facilities_Emul_Reagent";
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
            gameObject.tag = "Facilities_Emul_Soda";
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
