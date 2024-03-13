using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SborTable : MonoBehaviour
{
    public TextMeshProUGUI[] Sbor_Temperature_enter;
    public TextMeshProUGUI[] Sbor_Dust_enter;
    public TextMeshProUGUI[] Sbor_SolidParticle_enter;
    public TextMeshProUGUI[] Sbor_Zola_enter;

    public TextMeshProUGUI[] Sbor_Temperature_exit;
    public TextMeshProUGUI[] Sbor_Dust_exit;
    public TextMeshProUGUI[] Sbor_SolidParticle_exit;
    public TextMeshProUGUI[] Sbor_Zola_exit;

    public TextMeshProUGUI[] Sbor_CO_enter;
    public TextMeshProUGUI[] Sbor_NO_enter;
    public TextMeshProUGUI[] Sbor_NO2_enter;

    public TextMeshProUGUI[] Sbor_CO_exit;
    public TextMeshProUGUI[] Sbor_NO_exit;
    public TextMeshProUGUI[] Sbor_NO2_exit;

    public TextMeshProUGUI[] Sbor_CO2;
    public TextMeshProUGUI[] Sbor_SO2;
    public TextMeshProUGUI[] Sbor_CH4;
    public TextMeshProUGUI[] Sbor_H2S;

    public TextMeshProUGUI[] Sbor_CO2_exit;
    public TextMeshProUGUI[] Sbor_SO2_exit;
    public TextMeshProUGUI[] Sbor_CH4_exit;
    public TextMeshProUGUI[] Sbor_H2S_exit;

    private float sbor_Temperature;
    private float sbor_Dust;
    private float sbor_SolidParticles;
    private float sbor_Zola;

    private float sbor_Temperature_exit;
    private float sbor_Dust_exit;
    private float sbor_SolidParticles_exit;
    private float sbor_Zola_exit;
    private float sbor_CO_enter;
    private float sbor_NO_enter;
    private float sbor_NO2_enter;

    private float sbor_CO_exit;
    private float sbor_NO_exit;
    private float sbor_NO2_exit;

    private float sbor_CO2;
    private float sbor_SO2;
    private float sbor_CH4;
    private float sbor_H2S;

    private float sbor_CO2_exit;
    private float sbor_SO2_exit;
    private float sbor_CH4_exit;
    private float sbor_H2S_exit;

    private float delay = 60f;

    public TextMeshProUGUI[] tablesData;

    public bool isEnable = false;

    void Start()
    {
        sbor_Temperature = float.Parse(tablesData[0].text);
        sbor_Dust = float.Parse(tablesData[1].text);
        sbor_SolidParticles = float.Parse(tablesData[2].text);
        sbor_Zola = float.Parse(tablesData[3].text);

        sbor_Temperature_exit = float.Parse(tablesData[0].text);
        sbor_Dust_exit = float.Parse(tablesData[1].text);
        sbor_SolidParticles_exit = float.Parse(tablesData[2].text);
        sbor_Zola_exit = float.Parse(tablesData[3].text);

        sbor_CO_enter = float.Parse(tablesData[4].text);
        sbor_NO_enter = float.Parse(tablesData[5].text);
        sbor_NO2_enter = float.Parse(tablesData[6].text);

        sbor_CO_exit = float.Parse(tablesData[4].text);
        sbor_NO_exit = float.Parse(tablesData[5].text);
        sbor_NO2_exit = float.Parse(tablesData[6].text);

        sbor_CO2 = float.Parse(tablesData[7].text);
        sbor_SO2 = float.Parse(tablesData[8].text);
        sbor_CH4 = float.Parse(tablesData[9].text);
        sbor_H2S = float.Parse(tablesData[10].text);

        sbor_CO2_exit = float.Parse(tablesData[7].text);
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
                foreach (var e in Sbor_Temperature_enter)
                {
                    e.text = sbor_Temperature.ToString("0.");
                }
                if (sbor_Temperature > float.Parse(tablesData[0].text) * 0.09)
                {
                    float n = sbor_Temperature / 2;
                    sbor_Temperature -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_Dust_enter)
                {
                    e.text = sbor_Dust.ToString("0.");
                }
                if (sbor_Dust > float.Parse(tablesData[1].text) * 0)
                {
                    float n = sbor_Dust / 2;
                    sbor_Dust -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_SolidParticle_enter)
                {
                    e.text = sbor_SolidParticles.ToString("0.");
                }
                if (sbor_SolidParticles > float.Parse(tablesData[2].text) * 0.0047)
                {
                    float n = sbor_SolidParticles / 2;
                    sbor_SolidParticles -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_Zola_enter)
                {
                    e.text = sbor_Zola.ToString("0.");
                }
                if (sbor_Zola > float.Parse(tablesData[3].text) * 0)
                {
                    float n = sbor_Zola / 2;
                    sbor_Zola -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_CO_enter)
                {
                    e.text = sbor_CO_enter.ToString("0.");
                }
                if (sbor_CO_enter > float.Parse(tablesData[4].text) * 0.0125)
                {
                    float n = sbor_CO_enter / 2;
                    sbor_CO_enter -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_NO_enter)
                {
                    e.text = sbor_NO_enter.ToString("0.");
                }
                if (sbor_NO_enter > float.Parse(tablesData[5].text) * 0.0108)
                {
                    float n = sbor_NO_enter / 2;
                    sbor_NO_enter -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_NO2_enter)
                {
                    e.text = sbor_NO2_enter.ToString("0.");
                }
                if (sbor_NO2_enter > float.Parse(tablesData[6].text) * 0.0118)
                {
                    float n = sbor_NO2_enter / 2;
                    sbor_NO2_enter -= n * Time.deltaTime;
                }
                foreach (var e in Sbor_CO2)
                {
                    e.text = sbor_CO2.ToString("0.");
                }
                if (sbor_CO2 > float.Parse(tablesData[7].text) * 0.462)
                {
                    float n = sbor_CO2 / 2;
                    sbor_CO2 -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_SO2)
                {
                    e.text = sbor_SO2.ToString("0.");
                }
                if (sbor_SO2 > float.Parse(tablesData[8].text) * 0)
                {
                    float n = sbor_SO2 / 2;
                    sbor_SO2 -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_CH4)
                {
                    e.text = sbor_CH4.ToString("0.");
                }
                if (sbor_CH4 > float.Parse(tablesData[9].text) * 0.15)
                {
                    float n = sbor_CH4 / 2;
                    sbor_CH4 -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_H2S)
                {
                    e.text = sbor_H2S.ToString("0.");
                }
                if (sbor_H2S > float.Parse(tablesData[10].text) * 0)
                {
                    float n = sbor_H2S / 2;
                    sbor_H2S -= n * Time.deltaTime;
                }
            }
            if (delay < 0)
            {
                foreach (var e in Sbor_Temperature_exit)
                {
                    e.text = sbor_Temperature_exit.ToString("0.");
                }
                if (sbor_Temperature_exit > float.Parse(tablesData[0].text) * 0.054)
                {
                    float n = sbor_Temperature_exit / 2;
                    sbor_Temperature_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_Dust_exit)
                {
                    e.text = sbor_Dust_exit.ToString("0.");
                }
                if (sbor_Dust_exit > float.Parse(tablesData[1].text) * 0)
                {
                    float n = sbor_Dust_exit / 2;
                    sbor_Dust_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_SolidParticle_exit)
                {
                    e.text = sbor_SolidParticles_exit.ToString("0.");
                }
                if (sbor_SolidParticles_exit > float.Parse(tablesData[2].text) * 0.0047)
                {
                    float n = sbor_SolidParticles_exit / 2;
                    sbor_SolidParticles_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_Zola_exit)
                {
                    e.text = sbor_Zola_exit.ToString("0.");
                }
                if (sbor_Zola_exit > float.Parse(tablesData[3].text) * 0)
                {
                    float n = sbor_Zola_exit / 2;
                    sbor_Zola_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_CO_exit)
                {
                    e.text = sbor_CO_exit.ToString("0.");
                }
                if (sbor_CO_exit > float.Parse(tablesData[4].text) * 0.0125)
                {
                    float n = sbor_CO_exit / 2;
                    sbor_CO_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_NO_exit)
                {
                    e.text = sbor_NO_exit.ToString("0.");
                }
                if (sbor_NO_exit > float.Parse(tablesData[5].text) * 0.0108)
                {
                    float n = sbor_NO_exit / 2;
                    sbor_NO_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_NO2_exit)
                {
                    e.text = sbor_NO2_exit.ToString("0.");
                }
                if (sbor_NO2_exit > float.Parse(tablesData[6].text) * 0.0118)
                {
                    float n = sbor_NO2_exit / 2;
                    sbor_NO2_exit -= n * Time.deltaTime;
                }
                ///////////////////////////////////////////////////////////////////

                ///////////////////////////////////////////////////////////////
                foreach (var e in Sbor_CO2_exit)
                {
                    e.text = sbor_CO2_exit.ToString("0.");
                }
                if (sbor_CO2_exit > float.Parse(tablesData[7].text) * 0.0037)
                {
                    float n = sbor_CO2_exit / 2;
                    sbor_CO2_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_SO2_exit)
                {
                    e.text = sbor_SO2_exit.ToString("0.");
                }
                if (sbor_SO2_exit > float.Parse(tablesData[8].text) * 0)
                {
                    float n = sbor_SO2_exit / 2;
                    sbor_SO2_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_CH4_exit)
                {
                    e.text = sbor_CH4_exit.ToString("0.");
                }
                if (sbor_CH4_exit > float.Parse(tablesData[9].text) * 0.15)
                {
                    float n = sbor_CH4_exit / 2;
                    sbor_CH4_exit -= n * Time.deltaTime;
                }

                foreach (var e in Sbor_H2S_exit)
                {
                    e.text = sbor_H2S_exit.ToString("0.");
                }
                if (sbor_H2S_exit > float.Parse(tablesData[10].text) * 0)
                {
                    float n = sbor_H2S_exit / 2;
                    sbor_H2S_exit -= n * Time.deltaTime;
                }
            }
        }
    }
}
