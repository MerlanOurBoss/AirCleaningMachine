using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaterTable : MonoBehaviour
{
    public Slider Water_Temperature_enter;
    public Slider Water_Dust_enter;
    public Slider Water_SolidParticle_enter;
    public Slider Water_Zola_enter;

    public Slider Water_CO_enter;
    public Slider Water_NO_enter;
    public Slider Water_NO2_enter;

    public Slider Water_CO2_enter;
    public Slider Water_SO2_enter;
    public Slider Water_CH4_enter;
    public Slider Water_H2S_enter;
    public Slider Water_O2_enter;
    public Slider Water_N2_enter;

    public TextMeshProUGUI Water_Temperature_exit;
    public TextMeshProUGUI Water_Dust_exit;
    public TextMeshProUGUI Water_SolidParticle_exit;
    public TextMeshProUGUI Water_Zola_exit;

    public TextMeshProUGUI Water_CO_exit;
    public TextMeshProUGUI Water_NO_exit;
    public TextMeshProUGUI Water_NO2_exit;

    public TextMeshProUGUI Water_CO2_exit;
    public TextMeshProUGUI Water_SO2_exit;
    public TextMeshProUGUI Water_CH4_exit;
    public TextMeshProUGUI Water_H2S_exit;
    public TextMeshProUGUI Water_O2_exit;
    public TextMeshProUGUI Water_N2_exit;



    private float water_Temperature;
    private float water_Dust;
    private float water_SolidParticles;
    private float water_Zola;

    private float water_CO_enter;
    private float water_NO_enter;
    private float water_NO2_enter;

    private float water_CO2_enter;
    private float water_SO2_enter;
    private float water_CH4_enter;
    private float water_H2S_enter;
    private float water_O2_enter;
    private float water_N2_enter;

    private float water_Temperature_exit;
    private float water_Dust_exit;
    private float water_SolidParticles_exit;
    private float water_Zola_exit;

    private float water_CO_exit;
    private float water_NO_exit;
    private float water_NO2_exit;

    private float water_CO2_exit;
    private float water_SO2_exit;
    private float water_CH4_exit;
    private float water_H2S_exit;

    private float delay = 20f;

    public TextMeshProUGUI[] tablesData;
    public ParticleSystem[] smokes;
    public bool isEnable = false;
    void Start()
    {
        water_Temperature = float.Parse(tablesData[0].text);
        water_Dust = float.Parse(tablesData[1].text);
        water_SolidParticles = float.Parse(tablesData[2].text);
        water_Zola = float.Parse(tablesData[3].text);

        water_CO_enter = float.Parse(tablesData[4].text);
        water_CO2_enter = float.Parse(tablesData[5].text);
        water_NO_enter = float.Parse(tablesData[6].text);
        water_NO2_enter = float.Parse(tablesData[7].text);
        
        water_SO2_enter = float.Parse(tablesData[8].text);
        water_CH4_enter = float.Parse(tablesData[9].text);
        water_H2S_enter = float.Parse(tablesData[10].text);
        water_O2_enter = float.Parse(tablesData[11].text);
        water_N2_enter = float.Parse(tablesData[12].text);

        water_Temperature_exit = float.Parse(tablesData[0].text);
        water_Dust_exit = float.Parse(tablesData[1].text);
        water_SolidParticles_exit = float.Parse(tablesData[2].text);
        water_Zola_exit = float.Parse(tablesData[3].text);

        water_CO_exit = float.Parse(tablesData[4].text);
        water_CO2_exit = float.Parse(tablesData[5].text);
        water_NO_exit = float.Parse(tablesData[6].text);
        water_NO2_exit = float.Parse(tablesData[7].text);

        water_SO2_exit = float.Parse(tablesData[8].text);
        water_CH4_exit = float.Parse(tablesData[9].text);
        water_H2S_exit = float.Parse(tablesData[10].text); 
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;
            if (delay < 2)
            {

                if (Water_SolidParticle_enter.value > 0 && Water_SolidParticle_enter.value <= 20)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.5f);
                    }
                }
                else if (Water_SolidParticle_enter.value > 20 && Water_SolidParticle_enter.value < 50)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 1f);
                    }
                }

                Water_Temperature_enter.value = water_Temperature;
                Water_Dust_enter.value = water_Dust;
                Water_SolidParticle_enter.value = water_SolidParticles;
                Water_Zola_enter.value = water_Zola;
                Water_CO_enter.value = water_CO_enter;
                Water_NO_enter.value = water_NO_enter;
                Water_NO2_enter.value = water_NO2_enter;

                Water_CO2_enter.value = water_CO2_enter;
                Water_SO2_enter.value = water_SO2_enter;
                Water_CH4_enter.value = water_CH4_enter;
                Water_H2S_enter.value = water_H2S_enter;
                Water_O2_enter.value = water_O2_enter;
                Water_N2_enter.value = water_N2_enter;

                water_Temperature = Mathf.Lerp(water_Temperature, float.Parse(tablesData[0].text), 2 * Time.deltaTime);
                water_Dust = Mathf.Lerp(water_Dust, float.Parse(tablesData[1].text), 2 * Time.deltaTime);
                water_SolidParticles = Mathf.Lerp(water_SolidParticles, float.Parse(tablesData[2].text), 2 * Time.deltaTime);
                water_Zola = Mathf.Lerp(water_Zola, float.Parse(tablesData[3].text), 2 * Time.deltaTime);
                water_CO_enter = Mathf.Lerp(water_CO_enter, float.Parse(tablesData[4].text), 2 * Time.deltaTime);
                water_CO2_enter = Mathf.Lerp(water_CO2_enter, float.Parse(tablesData[5].text), 2 * Time.deltaTime);
                water_NO_enter = Mathf.Lerp(water_NO_enter, float.Parse(tablesData[6].text), 2 * Time.deltaTime);
                water_NO2_enter = Mathf.Lerp(water_NO2_enter, float.Parse(tablesData[7].text), 2 * Time.deltaTime);

                water_SO2_enter = Mathf.Lerp(water_SO2_enter, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                water_CH4_enter = Mathf.Lerp(water_CH4_enter, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                water_H2S_enter = Mathf.Lerp(water_H2S_enter, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                water_O2_enter = Mathf.Lerp(water_O2_enter, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                water_N2_enter = Mathf.Lerp(water_N2_enter, float.Parse(tablesData[12].text), 2 * Time.deltaTime);
            }
            if (delay < 0)
            {
                Water_Temperature_exit.text = water_Temperature_exit.ToString("0.");
                Water_Dust_exit.text = water_Dust_exit.ToString("0.");
                Water_SolidParticle_exit.text = water_SolidParticles_exit.ToString("0.");
                Water_Zola_exit.text = water_Zola_exit.ToString("0.");

                Water_CO_exit.text = water_CO_exit.ToString("0.");
                Water_NO_exit.text = water_NO_exit.ToString("0.");
                Water_NO2_exit.text = water_NO2_exit.ToString("0.");

                Water_CO2_exit.text = water_CO2_exit.ToString("0.");
                Water_SO2_exit.text = water_SO2_exit.ToString("0.");
                Water_CH4_exit.text = water_CH4_exit.ToString("0.");
                Water_H2S_exit.text = water_H2S_exit.ToString("0.");
                Water_O2_exit.text = water_O2_enter.ToString("0.");
                Water_N2_exit.text = water_N2_enter.ToString("0.");

                water_Temperature_exit = Mathf.Lerp(water_Temperature_exit, float.Parse(tablesData[0].text) * 0.12f, 2 * Time.deltaTime);
                water_Dust_exit = Mathf.Lerp(water_Dust_exit, float.Parse(tablesData[1].text) * 0.1f, 2 * Time.deltaTime);
                water_SolidParticles_exit = Mathf.Lerp(water_SolidParticles_exit, float.Parse(tablesData[2].text) * 0.1f, 2 * Time.deltaTime);
                water_Zola_exit = Mathf.Lerp(water_Zola_exit, float.Parse(tablesData[3].text) * 0.25f, 2 * Time.deltaTime);

                water_CO_exit = Mathf.Lerp(water_CO_exit, float.Parse(tablesData[4].text) , 2 * Time.deltaTime);
                water_CO2_exit = Mathf.Lerp(water_CO2_exit, float.Parse(tablesData[5].text) * 0.922f, 2 * Time.deltaTime);
                water_NO_exit = Mathf.Lerp(water_NO_exit, float.Parse(tablesData[6].text) , 2 * Time.deltaTime);
                water_NO2_exit = Mathf.Lerp(water_NO2_exit, float.Parse(tablesData[7].text) , 2 * Time.deltaTime);

                
                water_SO2_exit = Mathf.Lerp(water_SO2_exit, float.Parse(tablesData[8].text) * 0.043f, 2 * Time.deltaTime);
                water_CH4_exit = Mathf.Lerp(water_CH4_exit, float.Parse(tablesData[9].text) * 0.294f, 2 * Time.deltaTime);
                water_H2S_exit = Mathf.Lerp(water_H2S_exit, float.Parse(tablesData[10].text) * 0.04f, 2 * Time.deltaTime);
                water_O2_enter = Mathf.Lerp(water_O2_enter, float.Parse(tablesData[11].text) , 2 * Time.deltaTime);
                water_N2_enter = Mathf.Lerp(water_N2_enter, float.Parse(tablesData[12].text) , 2 * Time.deltaTime);

            }
            else if (delay < -10)
            {
                isEnable = false;

            }
        }
    }
    public void RepeatCalculateWater()
    {
        delay = 19f;
        isEnable = true;
        water_Temperature = float.Parse(tablesData[0].text);
        water_Dust = float.Parse(tablesData[1].text);
        water_SolidParticles = float.Parse(tablesData[2].text);
        water_Zola = float.Parse(tablesData[3].text);
        water_CO_enter = float.Parse(tablesData[4].text);
        water_CO2_enter = float.Parse(tablesData[5].text);
        water_NO_enter = float.Parse(tablesData[6].text);
        water_NO2_enter = float.Parse(tablesData[7].text);
        
        water_SO2_enter = float.Parse(tablesData[8].text);
        water_CH4_enter = float.Parse(tablesData[9].text);
        water_H2S_enter = float.Parse(tablesData[10].text);
        water_O2_enter = float.Parse(tablesData[11].text);
        water_N2_enter = float.Parse(tablesData[12].text);
    }
}
