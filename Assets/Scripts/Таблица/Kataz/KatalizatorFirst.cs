using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KatalizaotrFirst : MonoBehaviour
{
    public Slider Katalizaotr_Temperature_enter;
    public Slider Katalizaotr_Dust_enter;
    public Slider Katalizaotr_SolidParticles_enter;
    public Slider Katalizaotr_Zola_enter;
    public Slider Katalizaotr_CO_enter;
    public Slider Katalizaotr_CO2_enter;
    public Slider Katalizaotr_NO_enter;
    public Slider Katalizaotr_NO2_enter;
    public Slider Katalizaotr_SO2_enter;
    public Slider Katalizaotr_CH4_enter;
    public Slider Katalizaotr_H2S_enter;
    public Slider Katalizaotr_O2_enter;
    public Slider Katalizaotr_N2_enter;

    public TextMeshProUGUI Katalizaotr_Temperature;
    public TextMeshProUGUI Katalizaotr_Dust;
    public TextMeshProUGUI Katalizaotr_SolidParticles;
    public TextMeshProUGUI Katalizaotr_Zola;
    public TextMeshProUGUI Katalizaotr_CO;
    public TextMeshProUGUI Katalizaotr_CO2;
    public TextMeshProUGUI Katalizaotr_NO;
    public TextMeshProUGUI Katalizaotr_NO2;
    public TextMeshProUGUI Katalizaotr_SO2;
    public TextMeshProUGUI Katalizaotr_CH4;
    public TextMeshProUGUI Katalizaotr_H2S;
    public TextMeshProUGUI Katalizaotr_O2;
    public TextMeshProUGUI Katalizaotr_N2;
    public ParticleSystem[] smokes;

    private float kataz_Temperature;
    private float kataz_Dust;
    private float kataz_SolidParticles;
    private float kataz_Zola;
    private float kataz_CO;
    private float kataz_CO2;
    private float kataz_NO;
    private float kataz_NO2;
    private float kataz_SO2;
    private float kataz_CH4;
    private float kataz_H2S;
    private float kataz_O2;
    private float kataz_N2;

    private float delay = 2f;
    public TextMeshProUGUI[] tablesData;
    public TextMeshProUGUI[] originTablesData;

    public bool isEnable = false;
    private void Start()
    {
        Katalizaotr_Temperature_enter.value = float.Parse(originTablesData[0].text);
        Katalizaotr_Dust_enter.value = float.Parse(originTablesData[1].text);
        Katalizaotr_SolidParticles_enter.value = float.Parse(originTablesData[2].text);
        Katalizaotr_Zola_enter.value = float.Parse(originTablesData[3].text);
        Katalizaotr_CO_enter.value = float.Parse(originTablesData[4].text);
        Katalizaotr_CO2_enter.value = float.Parse(originTablesData[5].text);
        Katalizaotr_NO_enter.value = float.Parse(originTablesData[6].text);
        Katalizaotr_NO2_enter.value = float.Parse(originTablesData[7].text);
        Katalizaotr_SO2_enter.value = float.Parse(originTablesData[8].text);
        Katalizaotr_CH4_enter.value = float.Parse(originTablesData[9].text);
        Katalizaotr_H2S_enter.value = float.Parse(originTablesData[10].text);
        Katalizaotr_O2_enter.value = float.Parse(originTablesData[11].text);
        Katalizaotr_N2_enter.value = float.Parse(originTablesData[12].text);

        kataz_Temperature = float.Parse(tablesData[0].text);
        kataz_Dust = float.Parse(tablesData[1].text);
        kataz_SolidParticles = float.Parse(tablesData[2].text);
        kataz_Zola = float.Parse(tablesData[3].text);

        kataz_CO = float.Parse(tablesData[4].text);
        kataz_CO2 = float.Parse(tablesData[5].text);
        kataz_NO = float.Parse(tablesData[6].text);
        kataz_NO2 = float.Parse(tablesData[7].text);
        kataz_SO2 = float.Parse(tablesData[8].text);
        kataz_CH4 = float.Parse(tablesData[9].text);
        kataz_H2S = float.Parse(tablesData[10].text);
        kataz_O2 = float.Parse(tablesData[11].text);
        kataz_N2 = float.Parse(tablesData[12].text);
    }

    [System.Obsolete]
    void Update()
    {
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;

            if (delay < 0)
            {
                if (Katalizaotr_Dust_enter.value > 0 && Katalizaotr_Dust_enter.value < 100)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.3f);
                    }
                }
                else if (Katalizaotr_Dust_enter.value > 100 && Katalizaotr_Dust_enter.value < 200)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.9f, 0.9f, 0.9f, 0.3f);
                    }
                }
                else if (Katalizaotr_Dust_enter.value > 200 && Katalizaotr_Dust_enter.value < 300)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
                    }
                }

                if (Katalizaotr_Zola_enter.value > 0 && Katalizaotr_Zola_enter.value < 50)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.3f);
                    }
                }
                else if (Katalizaotr_Zola_enter.value > 50 && Katalizaotr_Zola_enter.value < 100)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
                    }
                }
                else if (Katalizaotr_Zola_enter.value > 100 && Katalizaotr_Zola_enter.value < 200)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.7f, 0.7f, 0.7f, 0.3f);
                    }
                }


                if (Katalizaotr_Zola_enter.value > 0 && Katalizaotr_Zola_enter.value < 50)
                {
                    smokes[0].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[1].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[2].startColor = new Color(1f, 1f, 1f, 1f);
                }
                else if (Katalizaotr_Zola_enter.value > 50 && Katalizaotr_Zola_enter.value < 100)
                {
                    smokes[0].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    smokes[1].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    smokes[2].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                else if (Katalizaotr_Zola_enter.value > 100 && Katalizaotr_Zola_enter.value < 200)
                {
                    smokes[0].startColor = new Color(0f, 0f, 0f, 1f);
                    smokes[1].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    smokes[2].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                }


                Katalizaotr_Temperature.text = kataz_Temperature.ToString("0.");
                Katalizaotr_Dust.text = kataz_Dust.ToString("0.");
                Katalizaotr_SolidParticles.text = kataz_SolidParticles.ToString("0.");
                Katalizaotr_Zola.text = kataz_Zola.ToString("0.");

                Katalizaotr_CO.text = kataz_CO.ToString("0.");
                Katalizaotr_CO2.text = kataz_CO2.ToString("0.");
                Katalizaotr_NO.text = kataz_NO.ToString("0.");
                Katalizaotr_NO2.text = kataz_NO2.ToString("0.");
                Katalizaotr_SO2.text = kataz_SO2.ToString("0.");
                Katalizaotr_CH4.text = kataz_CH4.ToString("0.");
                Katalizaotr_H2S.text = kataz_H2S.ToString("0.");
                Katalizaotr_O2.text = kataz_O2.ToString("0.");
                Katalizaotr_N2.text = kataz_N2.ToString("0.");

                kataz_Temperature = Mathf.Lerp(kataz_Temperature, float.Parse(tablesData[0].text) , 2 * Time.deltaTime);
                kataz_Dust = Mathf.Lerp(kataz_Dust, float.Parse(tablesData[1].text), 2 * Time.deltaTime);
                kataz_SolidParticles = Mathf.Lerp(kataz_SolidParticles, float.Parse(tablesData[2].text), 2 * Time.deltaTime);
                kataz_Zola = Mathf.Lerp(kataz_Zola, float.Parse(tablesData[3].text), 2 * Time.deltaTime);

                kataz_CO = Mathf.Lerp(kataz_CO, float.Parse(tablesData[4].text) * 0.025f, 2 * Time.deltaTime);
                kataz_CO2 = Mathf.Lerp(kataz_CO2, float.Parse(tablesData[5].text), 2 * Time.deltaTime);
                kataz_NO = Mathf.Lerp(kataz_NO, float.Parse(tablesData[6].text) * 0.023f, 2 * Time.deltaTime);
                kataz_NO2 = Mathf.Lerp(kataz_NO2, float.Parse(tablesData[7].text) * 0.023f, 2 * Time.deltaTime);
                kataz_SO2 = Mathf.Lerp(kataz_SO2, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                kataz_CH4 = Mathf.Lerp(kataz_CH4, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                kataz_H2S = Mathf.Lerp(kataz_H2S, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                kataz_O2 = Mathf.Lerp(kataz_O2, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                kataz_N2 = Mathf.Lerp(kataz_N2, float.Parse(tablesData[12].text), 2 * Time.deltaTime);

            }

            else if (delay < -5)
            {
                isEnable = false;
            }

        }
    }

    public void RecalculateData()
    {
        Katalizaotr_Temperature_enter.value = float.Parse(originTablesData[0].text);
        Katalizaotr_Dust_enter.value = float.Parse(originTablesData[1].text);
        Katalizaotr_SolidParticles_enter.value = float.Parse(originTablesData[2].text);
        Katalizaotr_Zola_enter.value = float.Parse(originTablesData[3].text);
        Katalizaotr_CO_enter.value = float.Parse(originTablesData[4].text);
        Katalizaotr_CO2_enter.value = float.Parse(originTablesData[5].text);
        Katalizaotr_NO_enter.value = float.Parse(originTablesData[6].text);
        Katalizaotr_NO2_enter.value = float.Parse(originTablesData[7].text);
        Katalizaotr_SO2_enter.value = float.Parse(originTablesData[8].text);
        Katalizaotr_CH4_enter.value = float.Parse(originTablesData[9].text);
        Katalizaotr_H2S_enter.value = float.Parse(originTablesData[10].text);
        Katalizaotr_O2_enter.value = float.Parse(originTablesData[11].text);
        Katalizaotr_N2_enter.value = float.Parse(originTablesData[12].text);
    }
    public void RepeatCalculate()
    {
        delay = 5f;
        isEnable = true;
        kataz_Temperature = float.Parse(tablesData[0].text);
        kataz_Dust = float.Parse(tablesData[1].text);
        kataz_SolidParticles = float.Parse(tablesData[2].text);
        kataz_Zola = float.Parse(tablesData[3].text);

        kataz_CO = float.Parse(tablesData[4].text);
        kataz_CO2 = float.Parse(tablesData[5].text);
        kataz_NO = float.Parse(tablesData[6].text);
        kataz_NO2 = float.Parse(tablesData[7].text);
        kataz_SO2 = float.Parse(tablesData[8].text);
        kataz_CH4 = float.Parse(tablesData[9].text);
        kataz_H2S = float.Parse(tablesData[10].text);
        kataz_O2 = float.Parse(tablesData[11].text);
        kataz_N2 = float.Parse(tablesData[12].text);
    }
}
