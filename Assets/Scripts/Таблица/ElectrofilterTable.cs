using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElectrofilterTable : MonoBehaviour
{
    public ElectrofilterTable elec;
    public Slider ElectroFilter_Temperature_enter;
    public Slider ElectroFilter_Dust_enter;
    public Slider ElectroFilter_SolidParticles_enter;
    public Slider ElectroFilter_Zola_enter;
    public Slider ElectroFilter_CO_enter;
    public Slider ElectroFilter_CO2_enter;
    public Slider ElectroFilter_NO_enter;
    public Slider ElectroFilter_NO2_enter;
    public Slider ElectroFilter_SO2_enter;
    public Slider ElectroFilter_CH4_enter;
    public Slider ElectroFilter_H2S_enter;
    public Slider ElectroFilter_O2_enter;
    public Slider ElectroFilter_N2_enter;

    public TextMeshProUGUI ElectroFilter_Temperature;
    public TextMeshProUGUI ElectroFilter_Dust;
    public TextMeshProUGUI ElectroFilter_SolidParticles;
    public TextMeshProUGUI ElectroFilter_Zola;
    public TextMeshProUGUI ElectroFilter_CO;
    public TextMeshProUGUI ElectroFilter_CO2;
    public TextMeshProUGUI ElectroFilter_NO;
    public TextMeshProUGUI ElectroFilter_NO2;
    public TextMeshProUGUI ElectroFilter_SO2;
    public TextMeshProUGUI ElectroFilter_CH4;
    public TextMeshProUGUI ElectroFilter_H2S;
    public TextMeshProUGUI ElectroFilter_O2;
    public TextMeshProUGUI ElectroFilter_N2;
    public ParticleSystem[] smokes;

    private float electro_Temperature;
    private float electro_Dust;
    private float electro_SolidParticles;
    private float electro_Zola;
    private float electro_CO;
    private float electro_CO2;
    private float electro_NO;
    private float electro_NO2;
    private float electro_SO2;
    private float electro_CH4;
    private float electro_H2S;
    private float electro_O2;
    private float electro_N2;

    private float delay = 2f;
    public SimulationScript _scriptSim;
    public TextMeshProUGUI[] tablesData;
    public TextMeshProUGUI[] originTablesData;

    public bool isEnable = false;
    private void Start()
    {
        ElectroFilter_Temperature_enter.value = float.Parse(originTablesData[0].text);
        ElectroFilter_Dust_enter.value = float.Parse(originTablesData[1].text);
        ElectroFilter_SolidParticles_enter.value = float.Parse(originTablesData[2].text);
        ElectroFilter_Zola_enter.value = float.Parse(originTablesData[3].text);
        ElectroFilter_CO_enter.value = float.Parse(originTablesData[4].text);
        ElectroFilter_CO2_enter.value = float.Parse(originTablesData[5].text);
        ElectroFilter_NO_enter.value = float.Parse(originTablesData[6].text);
        ElectroFilter_NO2_enter.value = float.Parse(originTablesData[7].text);
        ElectroFilter_SO2_enter.value = float.Parse(originTablesData[8].text);
        ElectroFilter_CH4_enter.value = float.Parse(originTablesData[9].text);
        ElectroFilter_H2S_enter.value = float.Parse(originTablesData[10].text);
        ElectroFilter_O2_enter.value = float.Parse(originTablesData[11].text);
        ElectroFilter_N2_enter.value = float.Parse(originTablesData[12].text);

        electro_Temperature = float.Parse(tablesData[0].text);
        electro_Dust = float.Parse(tablesData[1].text);
        electro_SolidParticles = float.Parse(tablesData[2].text);
        electro_Zola = float.Parse(tablesData[3].text);

        electro_CO = float.Parse(tablesData[4].text);
        electro_CO2 = float.Parse(tablesData[5].text);
        electro_NO = float.Parse(tablesData[6].text);
        electro_NO2 = float.Parse(tablesData[7].text);
        electro_SO2 = float.Parse(tablesData[8].text);
        electro_CH4 = float.Parse(tablesData[9].text);
        electro_H2S = float.Parse(tablesData[10].text);
        electro_O2 = float.Parse(tablesData[11].text);
        electro_N2 = float.Parse(tablesData[12].text);
    }

    [System.Obsolete]
    void Update()
    {
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;

            if (delay < 0)
            {
                if (ElectroFilter_Dust_enter.value > 0 && ElectroFilter_Dust_enter.value < 100)
                {
                    smokes[0].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[1].startColor = new Color(1f, 1f, 1f, 1f);
                }
                else if (ElectroFilter_Dust_enter.value > 100 && ElectroFilter_Dust_enter.value < 200)
                {
                    smokes[0].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    smokes[1].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                else if (ElectroFilter_Dust_enter.value > 200 && ElectroFilter_Dust_enter.value < 300)
                {
                    smokes[0].startColor = new Color(0f, 0f, 0f, 1f);
                    smokes[1].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                }

                if (ElectroFilter_SolidParticles_enter.value > 0 && ElectroFilter_SolidParticles_enter.value < 300)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.emissionRate = 200f;
                        item.startSpeed = 5f;
                    }
                }
                else if (ElectroFilter_SolidParticles_enter.value > 300 && ElectroFilter_SolidParticles_enter.value < 400)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.emissionRate = 250f;
                        item.startSpeed = 4.5f;
                    }
                }
                else if (ElectroFilter_SolidParticles_enter.value > 400 && ElectroFilter_SolidParticles_enter.value < 500)  
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.emissionRate = 300f;
                        item.startSpeed = 4f;
                    }
                }


                if (ElectroFilter_Zola_enter.value > 0 && ElectroFilter_Zola_enter.value < 50)
                {
                    smokes[0].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[1].startColor = new Color(1f, 1f, 1f, 1f);
                }
                else if (ElectroFilter_Zola_enter.value > 50 && ElectroFilter_Zola_enter.value < 100)
                {
                    smokes[0].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    smokes[1].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                else if (ElectroFilter_Zola_enter.value > 100 && ElectroFilter_Zola_enter.value < 200)
                {
                    smokes[0].startColor = new Color(0f, 0f, 0f, 1f);
                    smokes[1].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                }


                ElectroFilter_Temperature.text = electro_Temperature.ToString("0.");
                ElectroFilter_Dust.text = electro_Dust.ToString("0.");
                ElectroFilter_SolidParticles.text = electro_SolidParticles.ToString("0.");
                ElectroFilter_Zola.text = electro_Zola.ToString("0.");

                ElectroFilter_CO.text = electro_CO.ToString("0.");
                ElectroFilter_CO2.text = electro_CO2.ToString("0.");
                ElectroFilter_NO.text = electro_NO.ToString("0.");
                ElectroFilter_NO2.text = electro_NO2.ToString("0.");
                ElectroFilter_SO2.text = electro_SO2.ToString("0.");
                ElectroFilter_CH4.text = electro_CH4.ToString("0.");
                ElectroFilter_H2S.text = electro_H2S.ToString("0.");
                ElectroFilter_O2.text = electro_O2.ToString("0.");
                ElectroFilter_N2.text = electro_N2.ToString("0.");

                electro_Temperature = Mathf.Lerp(electro_Temperature, float.Parse(tablesData[0].text) * 0.215f, 2 * Time.deltaTime);
                electro_Dust = Mathf.Lerp(electro_Dust, float.Parse(tablesData[1].text) * 0.1f, 2 * Time.deltaTime);
                electro_SolidParticles = Mathf.Lerp(electro_SolidParticles, float.Parse(tablesData[2].text) * 0.093f, 2 * Time.deltaTime);
                electro_Zola = Mathf.Lerp(electro_Zola, float.Parse(tablesData[3].text) * 0.095f, 2 * Time.deltaTime);

                electro_CO = Mathf.Lerp(electro_CO, float.Parse(tablesData[4].text), 2 * Time.deltaTime);
                electro_CO2 = Mathf.Lerp(electro_CO2, float.Parse(tablesData[5].text), 2 * Time.deltaTime);
                electro_NO = Mathf.Lerp(electro_NO, float.Parse(tablesData[6].text), 2 * Time.deltaTime);
                electro_NO2 = Mathf.Lerp(electro_NO2, float.Parse(tablesData[7].text), 2 * Time.deltaTime);
                electro_SO2 = Mathf.Lerp(electro_SO2, float.Parse(tablesData[8].text) , 2 * Time.deltaTime);
                electro_CH4 = Mathf.Lerp(electro_CH4, float.Parse(tablesData[9].text) , 2 * Time.deltaTime);
                electro_H2S = Mathf.Lerp(electro_H2S, float.Parse(tablesData[10].text) , 2 * Time.deltaTime);
                electro_O2 = Mathf.Lerp(electro_O2, float.Parse(tablesData[11].text) , 2 * Time.deltaTime);
                electro_N2 = Mathf.Lerp(electro_N2, float.Parse(tablesData[12].text) , 2 * Time.deltaTime);

            }

            else if (delay < -5)
            {
                isEnable = false;
            }

        }
    }

    public void RecalculateData()
    {
        ElectroFilter_Temperature_enter.value = float.Parse(originTablesData[0].text);
        ElectroFilter_Dust_enter.value = float.Parse(originTablesData[1].text);
        ElectroFilter_SolidParticles_enter.value = float.Parse(originTablesData[2].text);
        ElectroFilter_Zola_enter.value = float.Parse(originTablesData[3].text);
        ElectroFilter_CO_enter.value = float.Parse(originTablesData[4].text);
        ElectroFilter_CO2_enter.value = float.Parse(originTablesData[5].text);
        ElectroFilter_NO_enter.value = float.Parse(originTablesData[6].text);
        ElectroFilter_NO2_enter.value = float.Parse(originTablesData[7].text);
        ElectroFilter_SO2_enter.value = float.Parse(originTablesData[8].text);
        ElectroFilter_CH4_enter.value = float.Parse(originTablesData[9].text);
        ElectroFilter_H2S_enter.value = float.Parse(originTablesData[10].text);
        ElectroFilter_O2_enter.value = float.Parse(originTablesData[11].text);
        ElectroFilter_N2_enter.value = float.Parse(originTablesData[12].text);
    }
    public void RepeatCalculate()
    {
        delay = 5f;
        isEnable = true;
        electro_Temperature = float.Parse(tablesData[0].text);
        electro_Dust = float.Parse(tablesData[1].text);
        electro_SolidParticles = float.Parse(tablesData[2].text);
        electro_Zola = float.Parse(tablesData[3].text);

        electro_CO = float.Parse(tablesData[4].text);
        electro_CO2 = float.Parse(tablesData[5].text);
        electro_NO = float.Parse(tablesData[6].text);
        electro_NO2 = float.Parse(tablesData[7].text);
        electro_SO2 = float.Parse(tablesData[8].text);
        electro_CH4 = float.Parse(tablesData[9].text);
        electro_H2S = float.Parse(tablesData[10].text);
        electro_O2 = float.Parse(tablesData[11].text);
        electro_N2 = float.Parse(tablesData[12].text);
    }
}
