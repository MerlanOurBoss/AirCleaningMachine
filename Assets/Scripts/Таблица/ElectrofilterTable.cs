using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElectrofilterTable : MonoBehaviour
{
    public TextMeshProUGUI[] ElectroFilter_Temperature;
    public TextMeshProUGUI[] ElectroFilter_Dust;
    public TextMeshProUGUI[] ElectroFilter_SolidParticles;
    public TextMeshProUGUI[] ElectroFilter_Zola;

    private float electro_Temperature;
    private float electro_Dust;
    private float electro_SolidParticles;
    private float electro_Zola;

    private float delay = 2f;
    private void Start()
    {
        electro_Temperature = 560;
        electro_Dust = 100;
        electro_SolidParticles = 215;
        electro_Zola = 42;
    }

    void Update()
    {
        delay -= 1 * Time.deltaTime;
        if (delay < 0)
        {
            foreach (var e in ElectroFilter_Temperature)
            {
                e.text = electro_Temperature.ToString("0.");
            }
            if (electro_Temperature > 120)
            {
                float n = electro_Temperature / 2;
                electro_Temperature -= n * Time.deltaTime;
            }

            foreach (var e in ElectroFilter_Dust)
            {
                e.text = electro_Dust.ToString("0.");
            }
            if (electro_Dust > 10)
            {
                float n = electro_Dust / 2;
                electro_Dust -= n * Time.deltaTime;
            }

            foreach (var e in ElectroFilter_SolidParticles)
            {
                e.text = electro_SolidParticles.ToString("0.");
            }
            if (electro_SolidParticles > 20)
            {
                float n = electro_SolidParticles / 2;
                electro_SolidParticles -= n * Time.deltaTime;
            }

            foreach (var e in ElectroFilter_Zola)
            {
                e.text = electro_Zola.ToString("0.");
            }
            if (electro_Zola > 4)
            {
                float n = electro_Zola / 2;
                electro_Zola -= n * Time.deltaTime;
            }
        }
    }
}
