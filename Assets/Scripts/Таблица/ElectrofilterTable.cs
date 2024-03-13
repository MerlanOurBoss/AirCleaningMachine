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

    public TextMeshProUGUI[] tablesData;

    public bool isEnable = false;
    private void Start()
    {
        electro_Temperature = float.Parse(tablesData[0].text);
        electro_Dust = float.Parse(tablesData[1].text);
        electro_SolidParticles = float.Parse(tablesData[2].text);
        electro_Zola = float.Parse(tablesData[3].text);
    }

    void Update()
    {
        
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;
            if (delay < 0)
            {
                foreach (var e in ElectroFilter_Temperature)
                {
                    e.text = electro_Temperature.ToString("0.");
                }
                if (electro_Temperature > float.Parse(tablesData[0].text) * 0.215)
                {
                    float n = electro_Temperature / 2;
                    electro_Temperature -= n * Time.deltaTime;
                }

                foreach (var e in ElectroFilter_Dust)
                {
                    e.text = electro_Dust.ToString("0.");
                }
                if (electro_Dust > float.Parse(tablesData[1].text) * 0.1)
                {
                    float n = electro_Dust / 2;
                    electro_Dust -= n * Time.deltaTime;
                }

                foreach (var e in ElectroFilter_SolidParticles)
                {
                    e.text = electro_SolidParticles.ToString("0.");
                }
                if (electro_SolidParticles > float.Parse(tablesData[2].text) * 0.093)
                {
                    float n = electro_SolidParticles / 2;
                    electro_SolidParticles -= n * Time.deltaTime;
                }

                foreach (var e in ElectroFilter_Zola)
                {
                    e.text = electro_Zola.ToString("0.");
                }
                if (electro_Zola > float.Parse(tablesData[3].text) * 0.095)
                {
                    float n = electro_Zola / 2;
                    electro_Zola -= n * Time.deltaTime;
                }
            }
        }
    }
}
