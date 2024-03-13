using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KatalizatorTable : MonoBehaviour
{
    public TextMeshProUGUI[] Katalizator_Temperature_enter;

    public TextMeshProUGUI[] Katalizator_Dust_enter;
    public TextMeshProUGUI[] Katalizator_SolidParticle_enter;
    public TextMeshProUGUI[] Katalizator_Zola_enter;

    public TextMeshProUGUI[] Katalizator_Temperature_exit;

    public TextMeshProUGUI[] Katalizator_Dust_exit;
    public TextMeshProUGUI[] Katalizator_SolidParticle_exit;
    public TextMeshProUGUI[] Katalizator_Zola_exit;

    public TextMeshProUGUI[] Katalizator_CO;
    public TextMeshProUGUI[] Katalizator_NO;
    public TextMeshProUGUI[] Katalizator_NO2;

    private float kata_Temperature;
    private float kata_Dust;
    private float kata_SolidParticles;
    private float kata_Zola;

    private float kata_Temperature_exit;
    private float kata_Dust_exit;
    private float kata_SolidParticles_exit;
    private float kata_Zola_exit;
    private float kata_CO;
    private float kata_NO;
    private float kata_NO2;

    private float delay = 10f;

    public TextMeshProUGUI[] tablesData;

    public bool isEnable = false;
    void Start()
    {
        kata_Temperature = float.Parse(tablesData[0].text);
        kata_Dust = float.Parse(tablesData[1].text);
        kata_SolidParticles = float.Parse(tablesData[2].text);
        kata_Zola = float.Parse(tablesData[3].text);

        kata_Temperature_exit = float.Parse(tablesData[0].text);
        kata_Dust_exit = float.Parse(tablesData[1].text);
        kata_SolidParticles_exit = float.Parse(tablesData[2].text);
        kata_Zola_exit = float.Parse(tablesData[3].text);

        kata_CO = float.Parse(tablesData[4].text); 
        kata_NO = float.Parse(tablesData[5].text); 
        kata_NO2 = float.Parse(tablesData[6].text);
    }

    void Update()
    {
        
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;
            if (delay < 2)
            {
                foreach (var e in Katalizator_Temperature_enter)
                {
                    e.text = kata_Temperature.ToString("0.");
                }
                if (kata_Temperature > float.Parse(tablesData[0].text) * 0.215)
                {
                    float n = kata_Temperature / 2;
                    kata_Temperature -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_Dust_enter)
                {
                    e.text = kata_Dust.ToString("0.");
                }
                if (kata_Dust > float.Parse(tablesData[1].text) * 0.1)
                {
                    float n = kata_Dust / 2;
                    kata_Dust -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_SolidParticle_enter)
                {
                    e.text = kata_SolidParticles.ToString("0.");
                }
                if (kata_SolidParticles > float.Parse(tablesData[2].text) * 0.093)
                {
                    float n = kata_SolidParticles / 2;
                    kata_SolidParticles -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_Zola_enter)
                {
                    e.text = kata_Zola.ToString("0.");
                }
                if (kata_Zola > float.Parse(tablesData[3].text) * 0.095)
                {
                    float n = kata_Zola / 2;
                    kata_Zola -= n * Time.deltaTime;
                }


            }
            if (delay < 0)
            {
                foreach (var e in Katalizator_Temperature_exit)
                {
                    e.text = kata_Temperature_exit.ToString("0.");
                }      
                if (kata_Temperature_exit > float.Parse(tablesData[0].text) * 0.895)
                {
                    float n = kata_Temperature_exit / 2;
                    kata_Temperature_exit -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_Dust_exit)
                {
                    e.text = kata_Dust_exit.ToString("0.");
                }
                if (kata_Dust_exit > float.Parse(tablesData[1].text) * 0.1)
                {
                    float n = kata_Dust_exit / 2;
                    kata_Dust_exit -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_SolidParticle_exit)
                {
                    e.text = kata_SolidParticles_exit.ToString("0.");
                }
                if (kata_SolidParticles_exit > float.Parse(tablesData[2].text) * 0.093)
                {
                    float n = kata_SolidParticles_exit / 2;
                    kata_SolidParticles_exit -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_Zola_exit)
                {
                    e.text = kata_Zola_exit.ToString("0.");
                }
                if (kata_Zola_exit > float.Parse(tablesData[3].text) * 0.095)
                {
                    float n = kata_Zola_exit / 2;
                    kata_Zola_exit -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_CO)
                {
                    e.text = kata_CO.ToString("0.");
                }
                if (kata_CO > float.Parse(tablesData[4].text) * 0.025)
                {
                    float n = kata_CO / 2;
                    kata_CO -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_NO)
                {
                    e.text = kata_NO.ToString("0.");
                }
                if (kata_NO > float.Parse(tablesData[5].text) * 0.023)
                {
                    float n = kata_NO / 2;
                    kata_NO -= n * Time.deltaTime;
                }

                foreach (var e in Katalizator_NO2)
                {
                    e.text = kata_NO2.ToString("0.");
                }
                if (kata_NO2 > float.Parse(tablesData[6].text) * 0.023)
                {
                    float n = kata_NO / 2;
                    kata_NO2 -= n * Time.deltaTime;
                }
            }
        }

    }
}
