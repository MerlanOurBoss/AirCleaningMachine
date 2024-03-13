using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReactTable : MonoBehaviour
{
    public TextMeshProUGUI[] React_Temperature_enter;
    public TextMeshProUGUI[] React_Dust_enter;
    public TextMeshProUGUI[] React_SolidParticle_enter;
    public TextMeshProUGUI[] React_Zola_enter;

    public TextMeshProUGUI[] React_Temperature_exit;
    public TextMeshProUGUI[] React_Dust_exit;
    public TextMeshProUGUI[] React_SolidParticle_exit;
    public TextMeshProUGUI[] React_Zola_exit;

    public TextMeshProUGUI[] React_CO_enter;
    public TextMeshProUGUI[] React_NO_enter;
    public TextMeshProUGUI[] React_NO2_enter;

    public TextMeshProUGUI[] React_CO_exit;
    public TextMeshProUGUI[] React_NO_exit;
    public TextMeshProUGUI[] React_NO2_exit;

    public TextMeshProUGUI[] React_CO2;
    public TextMeshProUGUI[] React_SO2;
    public TextMeshProUGUI[] React_CH4;
    public TextMeshProUGUI[] React_H2S;

    public TextMeshProUGUI[] React_CO2_exit;
    public TextMeshProUGUI[] React_SO2_exit;
    public TextMeshProUGUI[] React_CH4_exit;
    public TextMeshProUGUI[] React_H2S_exit;

    private float react_Temperature;
    private float react_Dust;
    private float react_SolidParticles;
    private float react_Zola;

    private float react_Temperature_exit;
    private float react_Dust_exit;
    private float react_SolidParticles_exit;
    private float react_Zola_exit;
    private float react_CO_enter;
    private float react_NO_enter;
    private float react_NO2_enter;

    private float react_CO_exit;
    private float react_NO_exit;
    private float react_NO2_exit;

    private float react_CO2;
    private float react_SO2;
    private float react_CH4;
    private float react_H2S;

    private float react_CO2_exit;
    private float react_SO2_exit;
    private float react_CH4_exit;
    private float react_H2S_exit;

    private float delay = 38f;

    public TextMeshProUGUI[] tablesData;

    public bool isEnable = false;
    void Start()
    {
        react_Temperature = float.Parse(tablesData[0].text);
        react_Dust = float.Parse(tablesData[1].text); 
        react_SolidParticles = float.Parse(tablesData[2].text); 
        react_Zola = float.Parse(tablesData[3].text); 

        react_Temperature_exit = float.Parse(tablesData[0].text); 
        react_Dust_exit = float.Parse(tablesData[1].text); 
        react_SolidParticles_exit = float.Parse(tablesData[2].text); 
        react_Zola_exit = float.Parse(tablesData[3].text); 

        react_CO_enter = float.Parse(tablesData[4].text); 
        react_NO_enter = float.Parse(tablesData[5].text); 
        react_NO2_enter = float.Parse(tablesData[6].text); 

        react_CO_exit = float.Parse(tablesData[4].text);
        react_NO_exit = float.Parse(tablesData[5].text);
        react_NO2_exit = float.Parse(tablesData[6].text);

        react_CO2 = float.Parse(tablesData[7].text);
        react_SO2 = float.Parse(tablesData[8].text);
        react_CH4 = float.Parse(tablesData[9].text);
        react_H2S = float.Parse(tablesData[10].text);

        react_CO2_exit = float.Parse(tablesData[7].text);
        react_SO2_exit = float.Parse(tablesData[8].text);
        react_CH4_exit = float.Parse(tablesData[9].text);
        react_H2S_exit = float.Parse(tablesData[10].text);
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnable)
        {
            delay -= 1 * Time.deltaTime;
            if (delay < 2)
            {
                foreach (var e in React_Temperature_enter)
                {
                    e.text = react_Temperature.ToString("0.");
                }
                if (react_Temperature > float.Parse(tablesData[0].text) * 0.107)
                {
                    float n = react_Temperature / 2;
                    react_Temperature -= n * Time.deltaTime;
                }

                foreach (var e in React_Dust_enter)
                {
                    e.text = react_Dust.ToString("0.");
                }
                if (react_Dust > float.Parse(tablesData[1].text) * 0.01)
                {
                    float n = react_Dust / 2;
                    react_Dust -= n * Time.deltaTime;
                }

                foreach (var e in React_SolidParticle_enter)
                {
                    e.text = react_SolidParticles.ToString("0.");
                }
                if (react_SolidParticles > float.Parse(tablesData[2].text) * 0.0093)
                {
                    float n = react_SolidParticles / 2;
                    react_SolidParticles -= n * Time.deltaTime;
                }

                foreach (var e in React_Zola_enter)
                {
                    e.text = react_Zola.ToString("0.");
                }
                if (react_Zola > float.Parse(tablesData[3].text) * 0.0238)
                {
                    float n = react_Zola / 2;
                    react_Zola -= n * Time.deltaTime;
                }

                foreach (var e in React_CO_enter)
                {
                    e.text = react_CO_enter.ToString("0.");
                }
                if (react_CO_enter > float.Parse(tablesData[4].text) * 0.025)
                {
                    float n = react_CO_enter / 2;
                    react_CO_enter -= n * Time.deltaTime;
                }

                foreach (var e in React_NO_enter)
                {
                    e.text = react_NO_enter.ToString("0.");
                }
                if (react_NO_enter > float.Parse(tablesData[5].text) * 0.0237)
                {
                    float n = react_NO_enter / 2;
                    react_NO_enter -= n * Time.deltaTime;
                }

                foreach (var e in React_NO2_enter)
                {
                    e.text = react_NO2_enter.ToString("0.");
                }
                if (react_NO2_enter > float.Parse(tablesData[6].text) * 0.0235)
                {
                    float n = react_NO2_enter / 2;
                    react_NO2_enter -= n * Time.deltaTime;
                }
                foreach (var e in React_CO2)
                {
                    e.text = react_CO2.ToString("0.");
                }
                if (react_CO2 > float.Parse(tablesData[7].text) * 0.918)
                {
                    float n = react_CO2 / 2;
                    react_CO2 -= n * Time.deltaTime;
                }

                foreach (var e in React_SO2)
                {
                    e.text = react_SO2.ToString("0.");
                }
                if (react_SO2 > float.Parse(tablesData[8].text) * 0.043)
                {
                    float n = react_SO2 / 2;
                    react_SO2 -= n * Time.deltaTime;
                }

                foreach (var e in React_CH4)
                {
                    e.text = react_CH4.ToString("0.");
                }
                if (react_CH4 > float.Parse(tablesData[9].text) * 0.294)
                {
                    float n = react_CH4 / 2;
                    react_CH4 -= n * Time.deltaTime;
                }

                foreach (var e in React_H2S)
                {
                    e.text = react_H2S.ToString("0.");
                }
                if (react_H2S > float.Parse(tablesData[10].text) * 0.04)
                {
                    float n = react_H2S / 2;
                    react_H2S -= n * Time.deltaTime;
                }
            }
            if (delay < 0)
            {
                foreach (var e in React_Temperature_exit)
                {
                    e.text = react_Temperature_exit.ToString("0.");
                }
                if (react_Temperature_exit > float.Parse(tablesData[0].text) * 0.09)
                {
                    float n = react_Temperature_exit / 2;
                    react_Temperature_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_Dust_exit)
                {
                    e.text = react_Dust_exit.ToString("0.");
                }
                if (react_Dust_exit > float.Parse(tablesData[1].text) * 0)
                {
                    float n = react_Dust_exit / 2;
                    react_Dust_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_SolidParticle_exit)
                {
                    e.text = react_SolidParticles_exit.ToString("0.");
                }
                if (react_SolidParticles_exit > float.Parse(tablesData[2].text) * 0.0047)
                {
                    float n = react_SolidParticles_exit / 2;
                    react_SolidParticles_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_Zola_exit)
                {
                    e.text = react_Zola_exit.ToString("0.");
                }
                if (react_Zola_exit > float.Parse(tablesData[3].text) * 0)
                {
                    float n = react_Zola_exit / 2;
                    react_Zola_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_CO_exit)
                {
                    e.text = react_CO_exit.ToString("0.");
                }
                if (react_CO_exit > float.Parse(tablesData[4].text) * 0.0125)
                {
                    float n = react_CO_exit / 2;
                    react_CO_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_NO_exit)
                {
                    e.text = react_NO_exit.ToString("0.");
                }
                if (react_NO_exit > float.Parse(tablesData[5].text) * 0.0108)
                {
                    float n = react_NO_exit / 2;
                    react_NO_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_NO2_exit)
                {
                    e.text = react_NO2_exit.ToString("0.");
                }
                if (react_NO2_exit > float.Parse(tablesData[6].text) * 0.0118)
                {
                    float n = react_NO2_exit / 2;
                    react_NO2_exit -= n * Time.deltaTime;
                }
                ///////////////////////////////////////////////////////////////////
                
                ///////////////////////////////////////////////////////////////
                foreach (var e in React_CO2_exit)
                {
                    e.text = react_CO2_exit.ToString("0.");
                }
                if (react_CO2_exit > float.Parse(tablesData[7].text) * 0.462)
                {
                    float n = react_CO2_exit / 2;
                    react_CO2_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_SO2_exit)
                {
                    e.text = react_SO2_exit.ToString("0.");
                }
                if (react_SO2_exit > float.Parse(tablesData[8].text) * 0)
                {
                    float n = react_SO2_exit / 2;
                    react_SO2_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_CH4_exit)
                {
                    e.text = react_CH4_exit.ToString("0.");
                }
                if (react_CH4_exit > float.Parse(tablesData[9].text) * 0.15)
                {
                    float n = react_CH4_exit / 2;
                    react_CH4_exit -= n * Time.deltaTime;
                }

                foreach (var e in React_H2S_exit)
                {
                    e.text = react_H2S_exit.ToString("0.");
                }
                if (react_H2S_exit > float.Parse(tablesData[10].text) * 0 )
                {
                    float n = react_H2S_exit / 2;
                    react_H2S_exit -= n * Time.deltaTime;
                }
            }
        }
    }
}
