using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KatalizatorTable : MonoBehaviour
{
    public Slider Katalizator_Temperature_enter;
    public Slider Katalizator_Dust_enter;
    public Slider Katalizator_SolidParticle_enter;
    public Slider Katalizator_Zola_enter;

    public Slider Katalizator_CO;
    public Slider Katalizator_NO;
    public Slider Katalizator_NO2;
    public Slider Katalizator_CO2;
    public Slider Katalizator_SO2;
    public Slider Katalizator_CH4;
    public Slider Katalizator_H2S;
    public Slider Katalizator_O2;
    public Slider Katalizator_N2;

    public TextMeshProUGUI Katalizator_Temperature_exit;
    public TextMeshProUGUI Katalizator_Dust_exit;
    public TextMeshProUGUI Katalizator_SolidParticle_exit;
    public TextMeshProUGUI Katalizator_Zola_exit;

    public TextMeshProUGUI Katalizator_CO_exit;
    public TextMeshProUGUI Katalizator_NO_exit;
    public TextMeshProUGUI Katalizator_NO2_exit;
    public TextMeshProUGUI Katalizator_CO2_exit;
    public TextMeshProUGUI Katalizator_SO2_exit;
    public TextMeshProUGUI Katalizator_CH4_exit;
    public TextMeshProUGUI Katalizator_H2S_exit;
    public TextMeshProUGUI Katalizator_O2_exit;
    public TextMeshProUGUI Katalizator_N2_exit;


    private float kata_Temperature;
    private float kata_Dust;
    private float kata_SolidParticles;
    private float kata_Zola;

    private float kata_CO;
    private float kata_NO;
    private float kata_NO2;

    private float kata_Temperature_exit;
    private float kata_Dust_exit;
    private float kata_SolidParticles_exit;
    private float kata_Zola_exit;

    private float kata_CO_exit;
    private float kata_NO_exit;
    private float kata_NO2_exit;

    private float kata_CO2;
    private float kata_SO2;
    private float kata_CH4;
    private float kata_H2S;
    private float kata_O2;
    private float kata_N2;

    private float delay = 13f;

    public TextMeshProUGUI[] tablesData;
    public ParticleSystem[] smokes;

    public bool isEnable = false;
    void Start()
    {
        kata_Temperature = float.Parse(tablesData[0].text);
        kata_Dust = float.Parse(tablesData[1].text);
        kata_SolidParticles = float.Parse(tablesData[2].text);
        kata_Zola = float.Parse(tablesData[3].text);

        kata_CO = float.Parse(tablesData[4].text);
        kata_NO = float.Parse(tablesData[6].text);
        kata_NO2 = float.Parse(tablesData[7].text);

        kata_CO2 = float.Parse(tablesData[5].text);
        kata_SO2 = float.Parse(tablesData[8].text);
        kata_CH4 = float.Parse(tablesData[9].text);
        kata_H2S = float.Parse(tablesData[10].text);
        kata_O2 = float.Parse(tablesData[11].text);
        kata_N2 = float.Parse(tablesData[12].text);

        kata_Temperature_exit = float.Parse(tablesData[0].text);
        kata_Dust_exit = float.Parse(tablesData[1].text);
        kata_SolidParticles_exit = float.Parse(tablesData[2].text);
        kata_Zola_exit = float.Parse(tablesData[3].text);

        kata_CO_exit = float.Parse(tablesData[4].text);
        kata_NO_exit = float.Parse(tablesData[6].text);
        kata_NO2_exit = float.Parse(tablesData[7].text);
    }

    void Update()
    {
        
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;
            
            if (delay < 2)
            {
                if (Katalizator_Dust_enter.value > 0 && Katalizator_Dust_enter.value < 10)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.3f);
                    }
                }
                else if (Katalizator_Dust_enter.value > 10 && Katalizator_Dust_enter.value < 20)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.9f, 0.9f, 0.9f, 0.3f);
                    }
                }
                else if (Katalizator_Dust_enter.value > 20 && Katalizator_Dust_enter.value < 30)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
                    }
                }

                if (Katalizator_Zola_enter.value > 0 && Katalizator_Zola_enter.value < 5)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.3f);
                    }
                }
                else if (Katalizator_Zola_enter.value > 5 && Katalizator_Zola_enter.value < 10)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
                    }
                }
                else if (Katalizator_Zola_enter.value > 10 && Katalizator_Zola_enter.value < 20)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.7f, 0.7f, 0.7f, 0.3f);
                    }
                }

                Katalizator_Temperature_enter.value = kata_Temperature;
                Katalizator_Dust_enter.value = kata_Dust; 
                Katalizator_SolidParticle_enter.value = kata_SolidParticles;
                Katalizator_Zola_enter.value = kata_Zola;

                Katalizator_CO.value = kata_CO;
                Katalizator_CO2.value = kata_CO2;
                Katalizator_NO.value = kata_NO;
                Katalizator_NO2.value = kata_NO2;

                Katalizator_SO2.value = kata_SO2;
                Katalizator_CH4.value = kata_CH4;
                Katalizator_H2S.value = kata_H2S;
                Katalizator_O2.value = kata_O2;
                Katalizator_N2.value = kata_N2;

                kata_Temperature = Mathf.Lerp(kata_Temperature, float.Parse(tablesData[0].text), 2 * Time.deltaTime);
                kata_Dust = Mathf.Lerp(kata_Dust, float.Parse(tablesData[1].text), 2 * Time.deltaTime);
                kata_SolidParticles = Mathf.Lerp(kata_SolidParticles, float.Parse(tablesData[2].text), 2 * Time.deltaTime);
                kata_Zola = Mathf.Lerp(kata_Zola, float.Parse(tablesData[3].text), 2 * Time.deltaTime);

                kata_CO = Mathf.Lerp(kata_CO, float.Parse(tablesData[4].text), 2 * Time.deltaTime);
                kata_CO2 = Mathf.Lerp(kata_CO2, float.Parse(tablesData[5].text), 2 * Time.deltaTime);

                kata_NO = Mathf.Lerp(kata_NO, float.Parse(tablesData[6].text), 2 * Time.deltaTime);
                kata_NO2 = Mathf.Lerp(kata_NO2, float.Parse(tablesData[7].text), 2 * Time.deltaTime);
                kata_SO2 = Mathf.Lerp(kata_SO2, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                kata_CH4 = Mathf.Lerp(kata_CH4, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                kata_H2S = Mathf.Lerp(kata_H2S, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                kata_O2 = Mathf.Lerp(kata_O2, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                kata_N2 = Mathf.Lerp(kata_N2, float.Parse(tablesData[12].text), 2 * Time.deltaTime);
            }
            if (delay < 0)
            {
                Katalizator_Temperature_exit.text = kata_Temperature_exit.ToString("0.");
                Katalizator_Dust_exit.text = kata_Dust_exit.ToString("0.");
                Katalizator_SolidParticle_exit.text = kata_SolidParticles_exit.ToString("0.");
                Katalizator_Zola_exit.text = kata_Zola_exit.ToString("0.");
                Katalizator_CO_exit.text = kata_CO_exit.ToString("0.");
                Katalizator_NO_exit.text = kata_NO_exit.ToString("0.");
                Katalizator_NO2_exit.text = kata_NO2_exit.ToString("0.");
                Katalizator_CO2_exit.text = kata_CO2.ToString("0.");
                Katalizator_SO2_exit.text = kata_SO2.ToString("0.");
                Katalizator_CH4_exit.text = kata_CH4.ToString("0.");
                Katalizator_H2S_exit.text = kata_H2S.ToString("0.");
                Katalizator_O2_exit.text = kata_O2.ToString("0.");
                Katalizator_N2_exit.text = kata_N2.ToString("0.");

                kata_Temperature_exit = Mathf.Lerp(kata_Temperature_exit, float.Parse(tablesData[0].text) * 4.17f, 2 * Time.deltaTime);
                kata_Dust_exit = Mathf.Lerp(kata_Dust_exit, float.Parse(tablesData[1].text), 2 * Time.deltaTime);
                kata_SolidParticles_exit = Mathf.Lerp(kata_SolidParticles_exit, float.Parse(tablesData[2].text), 2 * Time.deltaTime);
                kata_Zola_exit = Mathf.Lerp(kata_Zola_exit, float.Parse(tablesData[3].text), 2 * Time.deltaTime);

                kata_CO_exit = Mathf.Lerp(kata_CO_exit, float.Parse(tablesData[4].text) * 0.025f, 2 * Time.deltaTime);
                kata_CO2 = Mathf.Lerp(kata_CO2, float.Parse(tablesData[5].text), 2 * Time.deltaTime);
                kata_NO_exit = Mathf.Lerp(kata_NO_exit, float.Parse(tablesData[6].text) * 0.023f, 2 * Time.deltaTime);
                kata_NO2_exit = Mathf.Lerp(kata_NO2_exit, float.Parse(tablesData[7].text) * 0.023f, 2 * Time.deltaTime);

                kata_SO2 = Mathf.Lerp(kata_SO2, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                kata_CH4 = Mathf.Lerp(kata_CH4, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                kata_H2S = Mathf.Lerp(kata_H2S, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                kata_O2 = Mathf.Lerp(kata_O2, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                kata_N2 = Mathf.Lerp(kata_N2, float.Parse(tablesData[12].text), 2 * Time.deltaTime);
            }
            if (delay < -10)
            {
                isEnable = false;
                //Katalizator_Temperature_enter.onValueChanged.AddListener(delegate { RepeatCalculateKataz(); });
            }

        }
    }

    public void RepeatCalculateKataz()
    {
        //Katalizator_Temperature_enter.onValueChanged.RemoveListener(delegate { RepeatCalculateKataz(); });
        delay = 12f;
        isEnable = true;
        kata_Temperature = float.Parse(tablesData[0].text);
        kata_Dust = float.Parse(tablesData[1].text);
        kata_SolidParticles = float.Parse(tablesData[2].text);
        kata_Zola = float.Parse(tablesData[3].text);

        kata_CO = float.Parse(tablesData[4].text);
        kata_NO = float.Parse(tablesData[6].text);
        kata_NO2 = float.Parse(tablesData[7].text);

        kata_CO2 = float.Parse(tablesData[5].text);
        kata_SO2 = float.Parse(tablesData[8].text);
        kata_CH4 = float.Parse(tablesData[9].text);
        kata_H2S = float.Parse(tablesData[10].text);
        kata_O2 = float.Parse(tablesData[11].text);
        kata_N2 = float.Parse(tablesData[12].text);

    }
}
