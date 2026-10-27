using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamgingEmul : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] fluidsWater;
    [SerializeField] private MeshRenderer[] meshMaterial;

    [SerializeField] private Material reded;
    [SerializeField] private Material blued;
    [SerializeField] private Material greened;
    public int countWater;

    // Какой режим уже применён к материалам. Раньше материалы и цвет переназначались каждый кадр;
    // теперь — только когда countWater меняется.
    private int appliedWater = -1;

    private void Start()
    {
        if (gameObject.CompareTag(Tags.Facilities_Emul)) countWater = 1;
        else if (gameObject.CompareTag(Tags.Facilities_Emul_Reagent)) countWater = 2;
        else if (gameObject.CompareTag(Tags.Facilities_Emul_Soda)) countWater = 3;

        ApplyWater();
    }

    private void Update()
    {
        if (countWater > 3)
        {
            countWater = 1;
        }

        if (countWater != appliedWater)
        {
            ApplyWater();
        }

        // Тег проверяется каждый кадр: MovingFacilities по клавише R возвращает стартовый тег.
        if (countWater == 1 && !gameObject.CompareTag(Tags.Facilities_Emul)) gameObject.tag = Tags.Facilities_Emul;
        if (countWater == 2 && !gameObject.CompareTag(Tags.Facilities_Emul_Reagent)) gameObject.tag = Tags.Facilities_Emul_Reagent;
        if (countWater == 3 && !gameObject.CompareTag(Tags.Facilities_Emul_Soda)) gameObject.tag = Tags.Facilities_Emul_Soda;
    }

    private void ApplyWater()
    {
        appliedWater = countWater;

        Color color;
        Material mesh;
        switch (countWater)
        {
            case 1: color = new Color(1, 1, 1, 0.6f); mesh = blued; break;
            case 2: color = new Color(0.7f, 1, 0, 0.6f); mesh = reded; break;
            case 3: color = new Color(0, 0, 0, 0.6f); mesh = greened; break;
            default: return;
        }

        foreach (MeshRenderer item in fluidsWater)
        {
            item.material.color = color;
        }

        foreach (MeshRenderer item in meshMaterial)
        {
            item.material = mesh;
        }
    }

    public void ChangingE()
    {
        countWater++;
    }
}
