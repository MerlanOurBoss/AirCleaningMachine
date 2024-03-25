using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReactTable : MonoBehaviour
{
    public Slider React_Temperature_enter;
    public Slider React_Dust_enter;
    public Slider React_SolidParticle_enter;
    public Slider React_Zola_enter;

    public Slider React_CO_enter;
    public Slider React_NO_enter;
    public Slider React_NO2_enter;

    public Slider React_CO2;
    public Slider React_SO2;
    public Slider React_CH4;
    public Slider React_H2S;

    public Slider React_O2;
    public Slider React_N2;

    public TextMeshProUGUI React_Temperature_exit;
    public TextMeshProUGUI React_Dust_exit;
    public TextMeshProUGUI React_SolidParticle_exit;
    public TextMeshProUGUI React_Zola_exit;

    public TextMeshProUGUI React_CO_exit;
    public TextMeshProUGUI React_NO_exit;
    public TextMeshProUGUI React_NO2_exit;

    public TextMeshProUGUI React_CO2_exit;
    public TextMeshProUGUI React_SO2_exit;
    public TextMeshProUGUI React_CH4_exit;
    public TextMeshProUGUI React_H2S_exit;

    public TextMeshProUGUI React_O2_exit;
    public TextMeshProUGUI React_N2_exit;

    private float react_Temperature;
    private float react_Dust;
    private float react_SolidParticles;
    private float react_Zola;

    private float react_CO_enter;
    private float react_NO_enter;
    private float react_NO2_enter;

    private float react_CO2_enter;
    private float react_SO2_enter;
    private float react_CH4_enter;
    private float react_H2S_enter;
    private float react_O2_enter;
    private float react_N2_enter;


    private float react_Temperature_exit;
    private float react_Dust_exit;
    private float react_SolidParticles_exit;
    private float react_Zola_exit;

    private float react_CO_exit;
    private float react_NO_exit;
    private float react_NO2_exit;

    private float react_CO2_exit;
    private float react_SO2_exit;
    private float react_CH4_exit;
    private float react_H2S_exit;

    private float delay = 38f;

    public TextMeshProUGUI[] tablesData;
    public ParticleSystem[] smokes;

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
        react_NO_enter = float.Parse(tablesData[6].text); 
        react_NO2_enter = float.Parse(tablesData[7].text);

        react_CO2_enter = float.Parse(tablesData[5].text);
        react_SO2_enter = float.Parse(tablesData[8].text);
        react_CH4_enter = float.Parse(tablesData[9].text);
        react_H2S_enter = float.Parse(tablesData[10].text);
        react_O2_enter = float.Parse(tablesData[10].text);
        react_N2_enter = float.Parse(tablesData[10].text);

        react_CO_exit = float.Parse(tablesData[4].text);
        react_NO_exit = float.Parse(tablesData[6].text);
        react_NO2_exit = float.Parse(tablesData[7].text);

        react_CO2_exit = float.Parse(tablesData[5].text);
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
                if (React_SolidParticle_enter.value == 1 || React_SolidParticle_enter.value == 2)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.2f);
                    }
                }
                else if (React_SolidParticle_enter.value > 2 && React_SolidParticle_enter.value < 5)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.4f);
                    }
                }

                React_Temperature_enter.value = react_Temperature;
                React_Dust_enter.value = react_Dust;
                React_SolidParticle_enter.value = react_SolidParticles;
                React_Zola_enter.value = react_Zola;
                React_CO_enter.value = react_CO_enter;
                React_NO_enter.value = react_NO_enter;
                React_NO2_enter.value = react_NO2_enter;
                React_CO2.value = react_CO2_enter;
                React_SO2.value = react_SO2_enter;
                React_CH4.value = react_CH4_enter;
                React_H2S.value = react_H2S_enter;
                React_O2.value = react_O2_enter;
                React_N2.value = react_N2_enter;

                react_Temperature = Mathf.Lerp(react_Temperature, float.Parse(tablesData[0].text), 2 * Time.deltaTime);
                react_Dust = Mathf.Lerp(react_Dust, float.Parse(tablesData[1].text), 2 * Time.deltaTime);
                react_SolidParticles = Mathf.Lerp(react_SolidParticles, float.Parse(tablesData[2].text), 2 * Time.deltaTime);
                react_Zola = Mathf.Lerp(react_Zola, float.Parse(tablesData[3].text), 2 * Time.deltaTime);
                react_CO_enter = Mathf.Lerp(react_CO_enter, float.Parse(tablesData[4].text), 2 * Time.deltaTime);
                react_NO_enter = Mathf.Lerp(react_NO_enter, float.Parse(tablesData[6].text), 2 * Time.deltaTime);
                react_NO2_enter = Mathf.Lerp(react_NO2_enter, float.Parse(tablesData[7].text), 2 * Time.deltaTime);
                react_CO2_enter = Mathf.Lerp(react_CO2_enter, float.Parse(tablesData[5].text), 2 * Time.deltaTime);
                react_SO2_enter = Mathf.Lerp(react_SO2_enter, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                react_CH4_enter = Mathf.Lerp(react_CH4_enter, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                react_H2S_enter = Mathf.Lerp(react_H2S_enter, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                react_O2_enter = Mathf.Lerp(react_O2_enter, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                react_N2_enter = Mathf.Lerp(react_N2_enter, float.Parse(tablesData[12].text), 2 * Time.deltaTime);
            }
            if (delay < 0)
            {
                React_Temperature_exit.text = react_Temperature_exit.ToString("0.");
                React_Dust_exit.text = react_Dust_exit.ToString("0.");
                React_SolidParticle_exit.text = react_SolidParticles_exit.ToString("0.");
                React_Zola_exit.text = react_Zola_exit.ToString("0.");
                React_CO_exit.text = react_CO_exit.ToString("0.");
                React_NO_exit.text = react_NO_exit.ToString("0.");
                React_NO2_exit.text = react_NO2_exit.ToString("0.");

                React_CO2_exit.text = react_CO2_exit.ToString("0.");
                React_SO2_exit.text = react_SO2_exit.ToString("0.");
                React_CH4_exit.text = react_CH4_exit.ToString("0.");
                React_H2S_exit.text = react_H2S_exit.ToString("0.");
                React_O2_exit.text = react_O2_enter.ToString("0.");
                React_N2_exit.text = react_N2_enter.ToString("0.");

                react_Temperature_exit = Mathf.Lerp(react_Temperature_exit, float.Parse(tablesData[0].text) * 0.84f, 2 * Time.deltaTime);
                react_Dust_exit = Mathf.Lerp(react_Dust_exit, float.Parse(tablesData[1].text) * 0f, 2 * Time.deltaTime);
                react_SolidParticles_exit = Mathf.Lerp(react_SolidParticles_exit, float.Parse(tablesData[2].text) * 0.5f, 2 * Time.deltaTime);
                react_Zola_exit = Mathf.Lerp(react_Zola_exit, float.Parse(tablesData[3].text) * 0f, 2 * Time.deltaTime);
                react_CO_exit = Mathf.Lerp(react_CO_exit, float.Parse(tablesData[4].text) * 0.5f, 2 * Time.deltaTime);
                react_NO_exit = Mathf.Lerp(react_NO_exit, float.Parse(tablesData[6].text) * 0.45f, 2 * Time.deltaTime);
                react_NO2_exit = Mathf.Lerp(react_NO2_exit, float.Parse(tablesData[7].text) * 0.5f, 2 * Time.deltaTime);
                react_CO2_exit = Mathf.Lerp(react_CO2_exit, float.Parse(tablesData[5].text) * 0.5f, 2 * Time.deltaTime);
                react_SO2_exit = Mathf.Lerp(react_SO2_exit, float.Parse(tablesData[8].text) * 0f, 2 * Time.deltaTime);
                react_CH4_exit = Mathf.Lerp(react_CH4_exit, float.Parse(tablesData[9].text) * 0.5f, 2 * Time.deltaTime);
                react_H2S_exit = Mathf.Lerp(react_H2S_exit, float.Parse(tablesData[10].text) * 0f, 2 * Time.deltaTime);
                react_O2_enter = Mathf.Lerp(react_O2_enter, float.Parse(tablesData[11].text) , 2 * Time.deltaTime);
                react_N2_enter = Mathf.Lerp(react_N2_enter, float.Parse(tablesData[12].text) , 2 * Time.deltaTime);

            }
            else if (delay < -10)
            {
                isEnable = false;

            }
        }
    }

    public void RepeatCalculateReact()
    {
        delay = 19f;
        isEnable = true;
        react_Temperature = float.Parse(tablesData[0].text);
        react_Dust = float.Parse(tablesData[1].text);
        react_SolidParticles = float.Parse(tablesData[2].text);
        react_Zola = float.Parse(tablesData[3].text);
        react_CO_enter = float.Parse(tablesData[4].text);
        react_NO_enter = float.Parse(tablesData[5].text);
        react_NO2_enter = float.Parse(tablesData[6].text);
        react_CO2_enter = float.Parse(tablesData[7].text);
        react_SO2_enter = float.Parse(tablesData[8].text);
        react_CH4_enter = float.Parse(tablesData[9].text);
        react_H2S_enter = float.Parse(tablesData[10].text);
        react_O2_enter = float.Parse(tablesData[11].text);
        react_N2_enter = float.Parse(tablesData[12].text);
    }
}
