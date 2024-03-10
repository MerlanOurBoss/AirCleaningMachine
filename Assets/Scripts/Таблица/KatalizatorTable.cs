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

    void Start()
    {
        kata_Temperature = 560;
        kata_Dust = 100;
        kata_SolidParticles = 215;
        kata_Zola = 42;

        kata_Temperature_exit = 560;
        kata_Dust_exit = 100;
        kata_SolidParticles_exit = 215;
        kata_Zola_exit = 42;

        kata_CO = 120;
        kata_NO = 23;
        kata_NO2 = 21;
    }

    void Update()
    {
        delay -= 1 * Time.deltaTime;
        if (delay < 2)
        {
            foreach (var e in Katalizator_Temperature_enter)
            {
                e.text = kata_Temperature.ToString("0.");
            }
            if (kata_Temperature > 120)
            {
                float n = kata_Temperature / 2;
                kata_Temperature -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_Dust_enter)
            {
                e.text = kata_Dust.ToString("0.");
            }
            if (kata_Dust > 10)
            {
                float n = kata_Dust / 2;
                kata_Dust -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_SolidParticle_enter)
            {
                e.text = kata_SolidParticles.ToString("0.");
            }
            if (kata_SolidParticles > 20)
            {
                float n = kata_SolidParticles / 2;
                kata_SolidParticles -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_Zola_enter)
            {
                e.text = kata_Zola.ToString("0.");
            }
            if (kata_Zola > 4)
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
            if (kata_Temperature_exit > 120)
            {
                float n = kata_Temperature_exit / 2;
                kata_Temperature_exit -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_Dust_exit)
            {
                e.text = kata_Dust_exit.ToString("0.");
            }
            if (kata_Dust_exit > 10)
            {
                float n = kata_Dust_exit / 2;
                kata_Dust_exit -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_SolidParticle_exit)
            {
                e.text = kata_SolidParticles_exit.ToString("0.");
            }
            if (kata_SolidParticles_exit > 20)
            {
                float n = kata_SolidParticles_exit / 2;
                kata_SolidParticles_exit -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_Zola_exit)
            {
                e.text = kata_Zola_exit.ToString("0.");
            }
            if (kata_Zola_exit > 4)
            {
                float n = kata_Zola_exit / 2;
                kata_Zola_exit -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_CO)
            {
                e.text = kata_CO.ToString("0.");   
            }
            if (kata_CO > 60)
            {
                float n = kata_CO / 2;
                kata_CO -= n * Time.deltaTime;
            }
    
            foreach (var e in Katalizator_NO)
            {
                e.text = kata_NO.ToString("0.");
            }
            if (kata_NO > 11.5)
            {
                float n = kata_NO / 2;
                kata_NO -= n * Time.deltaTime;
            }

            foreach (var e in Katalizator_NO2)
            {
                e.text = kata_NO2.ToString("0.");
            }
            if (kata_NO2 > 10.5)
            {
                float n = kata_NO / 2;
                kata_NO2 -= n * Time.deltaTime;
            }
        }
    }
}
