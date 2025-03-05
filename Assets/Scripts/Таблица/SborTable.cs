using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SborTable : MonoBehaviour
{
    public Slider Sbor_Temperature_enter;
    public Slider Sbor_Dust_enter;
    public Slider Sbor_SolidParticle_enter;
    public Slider Sbor_Zola_enter;

    public Slider Sbor_CO_enter;
    public Slider Sbor_NO_enter;
    public Slider Sbor_NO2_enter;

    public Slider Sbor_CO2;
    public Slider Sbor_SO2;
    public Slider Sbor_CH4;
    public Slider Sbor_H2S;

    public Slider Sbor_O2;
    public Slider Sbor_N2;

    public TextMeshProUGUI Sbor_Temperature_exit;
    public TextMeshProUGUI Sbor_Dust_exit;
    public TextMeshProUGUI Sbor_SolidParticle_exit;
    public TextMeshProUGUI Sbor_Zola_exit;

    public TextMeshProUGUI Sbor_CO_exit;
    public TextMeshProUGUI Sbor_NO_exit;
    public TextMeshProUGUI Sbor_NO2_exit;

    public TextMeshProUGUI Sbor_CO2_exit;
    public TextMeshProUGUI Sbor_SO2_exit;
    public TextMeshProUGUI Sbor_CH4_exit;
    public TextMeshProUGUI Sbor_H2S_exit;

    public TextMeshProUGUI Sbor_O2_exit;
    public TextMeshProUGUI Sbor_N2_exit;

    private float sbor_Temperature;
    private float sbor_Dust;
    private float sbor_SolidParticles;
    private float sbor_Zola;

    private float sbor_CO_enter;
    private float sbor_NO_enter;
    private float sbor_NO2_enter;

    private float sbor_CO2;
    private float sbor_SO2;
    private float sbor_CH4;
    private float sbor_H2S;
    private float sbor_O2;
    private float sbor_N2;

    private float sbor_Temperature_exit;
    private float sbor_Dust_exit;
    private float sbor_SolidParticles_exit;
    private float sbor_Zola_exit;

    private float sbor_CO_exit;
    private float sbor_NO_exit;
    private float sbor_NO2_exit;

    private float sbor_CO2_exit;
    private float sbor_SO2_exit;
    private float sbor_CH4_exit;
    private float sbor_H2S_exit;

    private float delay = 6f;

    public TextMeshProUGUI[] tablesData;

    public bool isEnable = false;

    void Start()
    {
        sbor_Temperature = float.Parse(tablesData[0].text);
        sbor_Dust = float.Parse(tablesData[1].text);
        sbor_SolidParticles = float.Parse(tablesData[2].text);
        sbor_Zola = float.Parse(tablesData[3].text);
        
        sbor_CO_enter = float.Parse(tablesData[4].text);
        sbor_NO_enter = float.Parse(tablesData[6].text);
        sbor_NO2_enter = float.Parse(tablesData[7].text);
        
        sbor_CO2 = float.Parse(tablesData[5].text);
        sbor_SO2 = float.Parse(tablesData[8].text);
        sbor_CH4 = float.Parse(tablesData[9].text);
        sbor_H2S = float.Parse(tablesData[10].text);
        sbor_O2 = float.Parse(tablesData[11].text);
        sbor_N2 = float.Parse(tablesData[12].text);

        sbor_Temperature_exit = float.Parse(tablesData[0].text);
        sbor_Dust_exit = float.Parse(tablesData[1].text);
        sbor_SolidParticles_exit = float.Parse(tablesData[2].text);
        sbor_Zola_exit = float.Parse(tablesData[3].text);

        sbor_CO_exit = float.Parse(tablesData[4].text);
        sbor_NO_exit = float.Parse(tablesData[6].text);
        sbor_NO2_exit = float.Parse(tablesData[7].text);

        sbor_CO2_exit = float.Parse(tablesData[5].text);
        sbor_SO2_exit = float.Parse(tablesData[8].text);
        sbor_CH4_exit = float.Parse(tablesData[9].text);
        sbor_H2S_exit = float.Parse(tablesData[10].text);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;
            if (delay < 2)
            {
                Sbor_Temperature_enter.value = sbor_Temperature;
                Sbor_Dust_enter.value = sbor_Dust;
                Sbor_SolidParticle_enter.value = sbor_SolidParticles;
                Sbor_Zola_enter.value = sbor_Zola;
                Sbor_CO_enter.value = sbor_CO_enter;
                Sbor_NO_enter.value = sbor_NO_enter;
                Sbor_NO2_enter.value = sbor_NO2_enter;
                Sbor_CO2.value = sbor_CO2;
                Sbor_SO2.value = sbor_SO2;
                Sbor_CH4.value = sbor_CH4;
                Sbor_H2S.value = sbor_H2S;
                Sbor_O2.value = sbor_O2;
                Sbor_N2.value = sbor_N2;

                sbor_Temperature = Mathf.Lerp(sbor_Temperature, float.Parse(tablesData[0].text), 2 * Time.deltaTime);
                sbor_Dust = Mathf.Lerp(sbor_Dust, float.Parse(tablesData[1].text), 2 * Time.deltaTime);
                sbor_SolidParticles = Mathf.Lerp(sbor_SolidParticles, float.Parse(tablesData[2].text), 2 * Time.deltaTime);
                sbor_Zola = Mathf.Lerp(sbor_Zola, float.Parse(tablesData[3].text), 2 * Time.deltaTime);
                sbor_CO_enter = Mathf.Lerp(sbor_CO_enter, float.Parse(tablesData[4].text), 2 * Time.deltaTime);
                sbor_NO_enter = Mathf.Lerp(sbor_NO_enter, float.Parse(tablesData[6].text), 2 * Time.deltaTime);
                sbor_NO2_enter = Mathf.Lerp(sbor_NO2_enter, float.Parse(tablesData[7].text), 2 * Time.deltaTime);
                sbor_CO2 = Mathf.Lerp(sbor_CO2, float.Parse(tablesData[5].text), 2 * Time.deltaTime);
                sbor_SO2 = Mathf.Lerp(sbor_SO2, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                sbor_CH4 = Mathf.Lerp(sbor_CH4, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                sbor_H2S = Mathf.Lerp(sbor_H2S, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                sbor_O2 = Mathf.Lerp(sbor_O2, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                sbor_N2 = Mathf.Lerp(sbor_N2, float.Parse(tablesData[12].text), 2 * Time.deltaTime);

            }
            if (delay < 0)
            {
                Sbor_Temperature_exit.text = sbor_Temperature_exit.ToString("0.");
                Sbor_Dust_exit.text = sbor_Dust_exit.ToString("0.");
                Sbor_SolidParticle_exit.text = sbor_SolidParticles_exit.ToString("0.");
                Sbor_Zola_exit.text = sbor_Zola_exit.ToString("0.");
                Sbor_CO_exit.text = sbor_CO_exit.ToString("0.");
                Sbor_NO_exit.text = sbor_NO_exit.ToString("0.");
                Sbor_NO2_exit.text = sbor_NO2_exit.ToString("0.");

                Sbor_CO2_exit.text = sbor_CO2_exit.ToString("0.");
                Sbor_SO2_exit.text = sbor_SO2_exit.ToString("0.");
                Sbor_CH4_exit.text = sbor_CH4_exit.ToString("0.");
                Sbor_H2S_exit.text = sbor_H2S_exit.ToString("0.");
                Sbor_O2_exit.text = sbor_O2.ToString("0.");
                Sbor_N2_exit.text = sbor_N2.ToString("0.");

                sbor_Temperature_exit = Mathf.Lerp(sbor_Temperature_exit, float.Parse(tablesData[0].text) * 0.6f, 2 * Time.deltaTime);
                sbor_Dust_exit = Mathf.Lerp(sbor_Dust_exit, float.Parse(tablesData[1].text) * 0f, 2 * Time.deltaTime);
                sbor_SolidParticles_exit = Mathf.Lerp(sbor_SolidParticles_exit, float.Parse(tablesData[2].text) * 1f, 2 * Time.deltaTime);
                sbor_Zola_exit = Mathf.Lerp(sbor_Zola_exit, float.Parse(tablesData[3].text) * 0f, 2 * Time.deltaTime);
                sbor_CO_exit = Mathf.Lerp(sbor_CO_exit, float.Parse(tablesData[4].text) * 1f, 2 * Time.deltaTime);
                sbor_NO_exit = Mathf.Lerp(sbor_NO_exit, float.Parse(tablesData[6].text) * 1f, 2 * Time.deltaTime);
                sbor_NO2_exit = Mathf.Lerp(sbor_NO2_exit, float.Parse(tablesData[7].text) * 1f, 2 * Time.deltaTime);
                sbor_CO2_exit = Mathf.Lerp(sbor_CO2_exit, float.Parse(tablesData[5].text) * 0.008f, 2 * Time.deltaTime);
                sbor_SO2_exit = Mathf.Lerp(sbor_SO2_exit, float.Parse(tablesData[8].text) * 0f, 2 * Time.deltaTime);
                sbor_CH4_exit = Mathf.Lerp(sbor_CH4_exit, float.Parse(tablesData[9].text) * 1f, 2 * Time.deltaTime);
                sbor_H2S_exit = Mathf.Lerp(sbor_H2S_exit, float.Parse(tablesData[10].text) * 0f, 2 * Time.deltaTime);
                sbor_O2 = Mathf.Lerp(sbor_O2, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                sbor_N2 = Mathf.Lerp(sbor_N2, float.Parse(tablesData[12].text), 2 * Time.deltaTime);
                
            }
            else if (delay < -10)
            {
                isEnable = false;

            }
        }
    }

    public void RepeatCalculateSbor()
    {
        delay = 19f;
        isEnable = true;
        sbor_Temperature = float.Parse(tablesData[0].text);
        sbor_Dust = float.Parse(tablesData[1].text);
        sbor_SolidParticles = float.Parse(tablesData[2].text);
        sbor_Zola = float.Parse(tablesData[3].text);
        sbor_CO_enter = float.Parse(tablesData[4].text);
        sbor_NO_enter = float.Parse(tablesData[5].text);
        sbor_NO2_enter = float.Parse(tablesData[6].text);
        sbor_CO2 = float.Parse(tablesData[7].text);
        sbor_SO2 = float.Parse(tablesData[8].text);
        sbor_CH4 = float.Parse(tablesData[9].text);
        sbor_H2S = float.Parse(tablesData[10].text);
        sbor_O2 = float.Parse(tablesData[11].text);
        sbor_N2 = float.Parse(tablesData[12].text);
    }
}
