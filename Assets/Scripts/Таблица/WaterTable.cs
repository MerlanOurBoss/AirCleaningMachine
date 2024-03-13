using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaterTable : MonoBehaviour
{
    public TextMeshProUGUI[] Water_Temperature_enter;
    public TextMeshProUGUI[] Water_Dust_enter;
    public TextMeshProUGUI[] Water_SolidParticle_enter;
    public TextMeshProUGUI[] Water_Zola_enter;

    public TextMeshProUGUI[] Water_Temperature_exit;
    public TextMeshProUGUI[] Water_Dust_exit;
    public TextMeshProUGUI[] Water_SolidParticle_exit;
    public TextMeshProUGUI[] Water_Zola_exit;

    public TextMeshProUGUI[] Water_CO_enter;
    public TextMeshProUGUI[] Water_NO_enter;
    public TextMeshProUGUI[] Water_NO2_enter;

    public TextMeshProUGUI[] Water_CO_exit;
    public TextMeshProUGUI[] Water_NO_exit;
    public TextMeshProUGUI[] Water_NO2_exit;

    public TextMeshProUGUI[] Water_CO2;
    public TextMeshProUGUI[] Water_SO2;
    public TextMeshProUGUI[] Water_CH4;
    public TextMeshProUGUI[] Water_H2S;

    private float water_Temperature;
    private float water_Dust;
    private float water_SolidParticles;
    private float water_Zola;

    private float water_Temperature_exit;
    private float water_Dust_exit;
    private float water_SolidParticles_exit;
    private float water_Zola_exit;
    private float water_CO_enter;
    private float water_NO_enter;
    private float water_NO2_enter;

    private float water_CO_exit;
    private float water_NO_exit;
    private float water_NO2_exit;

    private float water_CO2;
    private float water_SO2;
    private float water_CH4;
    private float water_H2S;

    private float delay = 20f;

    public TextMeshProUGUI[] tablesData;

    public bool isEnable = false;
    void Start()
    {
        water_Temperature = float.Parse(tablesData[0].text);
        water_Dust = float.Parse(tablesData[1].text);
        water_SolidParticles = float.Parse(tablesData[2].text);
        water_Zola = float.Parse(tablesData[3].text);

        water_Temperature_exit = float.Parse(tablesData[0].text);
        water_Dust_exit = float.Parse(tablesData[1].text);
        water_SolidParticles_exit = float.Parse(tablesData[2].text);
        water_Zola_exit = float.Parse(tablesData[3].text);

        water_CO_enter = float.Parse(tablesData[4].text);
        water_NO_enter = float.Parse(tablesData[5].text);
        water_NO2_enter = float.Parse(tablesData[6].text);

        water_CO_exit = float.Parse(tablesData[4].text);
        water_NO_exit = float.Parse(tablesData[5].text);
        water_NO2_exit = float.Parse(tablesData[6].text);

        water_CO2 = float.Parse(tablesData[7].text); 
        water_SO2 = float.Parse(tablesData[8].text); 
        water_CH4 = float.Parse(tablesData[9].text); 
        water_H2S = float.Parse(tablesData[10].text); 
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;
            if (delay < 2)
            {
                foreach (var e in Water_Temperature_enter)
                {
                    e.text = water_Temperature.ToString("0.");
                }
                if (water_Temperature > float.Parse(tablesData[0].text) * 0.895)
                {
                    float n = water_Temperature / 2;
                    water_Temperature -= n * Time.deltaTime;
                }

                foreach (var e in Water_Dust_enter)
                {
                    e.text = water_Dust.ToString("0.");
                }
                if (water_Dust > float.Parse(tablesData[1].text) * 0.1)
                {
                    float n = water_Dust / 2;
                    water_Dust -= n * Time.deltaTime;
                }

                foreach (var e in Water_SolidParticle_enter)
                {
                    e.text = water_SolidParticles.ToString("0.");
                }
                if (water_SolidParticles > float.Parse(tablesData[2].text) * 0.093)
                {
                    float n = water_SolidParticles / 2;
                    water_SolidParticles -= n * Time.deltaTime;
                }

                foreach (var e in Water_Zola_enter)
                {
                    e.text = water_Zola.ToString("0.");
                }
                if (water_Zola > float.Parse(tablesData[3].text) * 0.095)
                {
                    float n = water_Zola / 2;
                    water_Zola -= n * Time.deltaTime;
                }

                foreach (var e in Water_CO_enter)
                {
                    e.text = water_CO_enter.ToString("0.");
                }
                if (water_CO_enter > float.Parse(tablesData[4].text) * 0.025)
                {
                    float n = water_CO_enter / 2;
                    water_CO_enter -= n * Time.deltaTime;
                }

                foreach (var e in Water_NO_enter)
                {
                    e.text = water_NO_enter.ToString("0.");
                }
                if (water_NO_enter > float.Parse(tablesData[5].text) * 0.023)
                {
                    float n = water_NO_enter / 2;
                    water_NO_enter -= n * Time.deltaTime;
                }

                foreach (var e in Water_NO2_enter)
                {
                    e.text = water_NO2_enter.ToString("0.");
                }
                if (water_NO2_enter > float.Parse(tablesData[6].text) * 0.023)
                {
                    float n = water_NO2_enter / 2;
                    water_NO2_enter -= n * Time.deltaTime;
                }
            }
            if (delay < 0)
            {
                foreach (var e in Water_Temperature_exit)
                {
                    e.text = water_Temperature_exit.ToString("0.");
                }
                if (water_Temperature_exit > float.Parse(tablesData[0].text) * 0.107)
                {
                    float n = water_Temperature_exit / 2;
                    water_Temperature_exit -= n * Time.deltaTime;
                }

                foreach (var e in Water_Dust_exit)
                {
                    e.text = water_Dust_exit.ToString("0.");
                }
                if (water_Dust_exit > float.Parse(tablesData[1].text) * 0.01)
                {
                    float n = water_Dust_exit / 2;
                    water_Dust_exit -= n * Time.deltaTime;
                }

                foreach (var e in Water_SolidParticle_exit)
                {
                    e.text = water_SolidParticles_exit.ToString("0.");
                }
                if (water_SolidParticles_exit > float.Parse(tablesData[2].text) * 0.0093)
                {
                    float n = water_SolidParticles_exit / 2;
                    water_SolidParticles_exit -= n * Time.deltaTime;
                }

                foreach (var e in Water_Zola_exit)
                {
                    e.text = water_Zola_exit.ToString("0.");
                }
                if (water_Zola_exit > float.Parse(tablesData[3].text) * 0.0238)
                {
                    float n = water_Zola_exit / 2;
                    water_Zola_exit -= n * Time.deltaTime;
                }

                foreach (var e in Water_CO_exit)
                {
                    e.text = water_CO_exit.ToString("0.");
                }
                if (water_CO_exit > float.Parse(tablesData[4].text) * 0.025)
                {
                    float n = water_CO_exit / 2;
                    water_CO_exit -= n * Time.deltaTime;
                }

                foreach (var e in Water_NO_exit)
                {
                    e.text = water_NO_exit.ToString("0.");
                }
                if (water_NO_exit > float.Parse(tablesData[5].text) * 0.0237)
                {
                    float n = water_NO_exit / 2;
                    water_NO_exit -= n * Time.deltaTime;
                }

                foreach (var e in Water_NO2_exit)
                {
                    e.text = water_NO2_exit.ToString("0.");
                }
                if (water_NO2_exit > float.Parse(tablesData[6].text) * 0.0235)
                {
                    float n = water_NO2_exit / 2;
                    water_NO2_exit -= n * Time.deltaTime;
                }
                ///////////////////////////////////////////////////////////////////
                foreach (var e in Water_CO2)
                {
                    e.text = water_CO2.ToString("0.");
                }
                if (water_CO2 > float.Parse(tablesData[7].text) * 0.918)
                {
                    float n = water_CO2 / 2;
                    water_CO2 -= n * Time.deltaTime;
                }

                foreach (var e in Water_SO2)
                {
                    e.text = water_SO2.ToString("0.");
                }
                if (water_SO2 > float.Parse(tablesData[8].text) * 0.043)
                {
                    float n = water_SO2 / 2;
                    water_SO2 -= n * Time.deltaTime;
                }

                foreach (var e in Water_CH4)
                {
                    e.text = water_CH4.ToString("0.");
                }
                if (water_CH4 > float.Parse(tablesData[9].text) * 0.294)
                {
                    float n = water_CH4 / 2;
                    water_CH4 -= n * Time.deltaTime;
                }

                foreach (var e in Water_H2S)
                {
                    e.text = water_H2S.ToString("0.");
                }
                if (water_H2S > float.Parse(tablesData[10].text) * 0.04)
                {
                    float n = water_H2S / 2;
                    water_H2S -= n * Time.deltaTime;
                }
            }
        }

    }
}
