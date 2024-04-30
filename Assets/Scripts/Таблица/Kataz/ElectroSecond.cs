using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElectroSecond : MonoBehaviour
{
    public Slider Electrofilter_Temperature_enter;
    public Slider Electrofilter_Dust_enter;
    public Slider Electrofilter_SolidParticle_enter;
    public Slider Electrofilter_Zola_enter;

    public Slider Electrofilter_CO;
    public Slider Electrofilter_NO;
    public Slider Electrofilter_NO2;
    public Slider Electrofilter_CO2;
    public Slider Electrofilter_SO2;
    public Slider Electrofilter_CH4;
    public Slider Electrofilter_H2S;
    public Slider Electrofilter_O2;
    public Slider Electrofilter_N2;

    public TextMeshProUGUI Electrofilter_Temperature_exit;
    public TextMeshProUGUI Electrofilter_Dust_exit;
    public TextMeshProUGUI Electrofilter_SolidParticle_exit;
    public TextMeshProUGUI Electrofilter_Zola_exit;

    public TextMeshProUGUI Electrofilter_CO_exit;
    public TextMeshProUGUI Electrofilter_NO_exit;
    public TextMeshProUGUI Electrofilter_NO2_exit;
    public TextMeshProUGUI Electrofilter_CO2_exit;
    public TextMeshProUGUI Electrofilter_SO2_exit;
    public TextMeshProUGUI Electrofilter_CH4_exit;
    public TextMeshProUGUI Electrofilter_H2S_exit;
    public TextMeshProUGUI Electrofilter_O2_exit;
    public TextMeshProUGUI Electrofilter_N2_exit;


    private float elec_Temperature;
    private float elec_Dust;
    private float elec_SolidParticle;
    private float elec_Zola;

    private float elec_CO;
    private float elec_NO;
    private float elec_NO2;

    private float elec_Temperature_exit;
    private float elec_Dust_exit;
    private float elec_SolidParticle_exit;
    private float elec_Zola_exit;

    private float elec_CO_exit;
    private float elec_NO_exit;
    private float elec_NO2_exit;

    private float elec_CO2;
    private float elec_SO2;
    private float elec_CH4;
    private float elec_H2S;
    private float elec_O2;
    private float elec_N2;

    private float delay = 13f;

    public TextMeshProUGUI[] tablesData;
    public ParticleSystem[] smokes;

    public bool isEnable = false;
    void Start()
    {
        elec_Temperature = float.Parse(tablesData[0].text);
        elec_Dust = float.Parse(tablesData[1].text);
        elec_SolidParticle = float.Parse(tablesData[2].text);
        elec_Zola = float.Parse(tablesData[3].text);

        elec_CO = float.Parse(tablesData[4].text);
        elec_NO = float.Parse(tablesData[6].text);
        elec_NO2 = float.Parse(tablesData[7].text);

        elec_CO2 = float.Parse(tablesData[5].text);
        elec_SO2 = float.Parse(tablesData[8].text);
        elec_CH4 = float.Parse(tablesData[9].text);
        elec_H2S = float.Parse(tablesData[10].text);
        elec_O2 = float.Parse(tablesData[11].text);
        elec_N2 = float.Parse(tablesData[12].text);

        elec_Temperature_exit = float.Parse(tablesData[0].text);
        elec_Dust_exit = float.Parse(tablesData[1].text);
        elec_SolidParticle_exit = float.Parse(tablesData[2].text);
        elec_Zola_exit = float.Parse(tablesData[3].text);

        elec_CO_exit = float.Parse(tablesData[4].text);
        elec_NO_exit = float.Parse(tablesData[6].text);
        elec_NO2_exit = float.Parse(tablesData[7].text);
    }

    void Update()
    {

        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;

            if (delay < 2)
            {
                if (Electrofilter_Dust_enter.value > 0 && Electrofilter_Dust_enter.value < 100)
                {
                    smokes[0].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[1].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[2].startColor = new Color(1f, 1f, 1f, 1f);
                }
                else if (Electrofilter_Dust_enter.value > 100 && Electrofilter_Dust_enter.value < 200)
                {
                    smokes[0].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    smokes[1].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    smokes[2].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                else if (Electrofilter_Dust_enter.value > 200 && Electrofilter_Dust_enter.value < 300)
                {
                    smokes[0].startColor = new Color(0f, 0f, 0f, 1f);
                    smokes[1].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    smokes[2].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                }

                if (Electrofilter_SolidParticle_enter.value > 0 && Electrofilter_SolidParticle_enter.value < 300)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.emissionRate = 200f;
                        item.startSpeed = 5f;
                    }
                }
                else if (Electrofilter_SolidParticle_enter.value > 300 && Electrofilter_SolidParticle_enter.value < 400)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.emissionRate = 250f;
                        item.startSpeed = 4.5f;
                    }
                }
                else if (Electrofilter_SolidParticle_enter.value > 400 && Electrofilter_SolidParticle_enter.value < 500)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.emissionRate = 300f;
                        item.startSpeed = 4f;
                    }
                }


                if (Electrofilter_Zola_enter.value > 0 && Electrofilter_Zola_enter.value < 50)
                {
                    smokes[0].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[1].startColor = new Color(1f, 1f, 1f, 1f);
                    smokes[2].startColor = new Color(1f, 1f, 1f, 1f);
                }
                else if (Electrofilter_Zola_enter.value > 50 && Electrofilter_Zola_enter.value < 100)
                {
                    smokes[0].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    smokes[1].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    smokes[2].startColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                else if (Electrofilter_Zola_enter.value > 100 && Electrofilter_Zola_enter.value < 200)
                {
                    smokes[0].startColor = new Color(0f, 0f, 0f, 1f);
                    smokes[1].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    smokes[2].startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                }

                Electrofilter_Temperature_enter.value = elec_Temperature;
                Electrofilter_Dust_enter.value = elec_Dust;
                Electrofilter_SolidParticle_enter.value = elec_SolidParticle;
                Electrofilter_Zola_enter.value = elec_Zola;

                Electrofilter_CO.value = elec_CO;
                Electrofilter_CO2.value = elec_CO2;
                Electrofilter_NO.value = elec_NO;
                Electrofilter_NO2.value = elec_NO2;

                Electrofilter_SO2.value = elec_SO2;
                Electrofilter_CH4.value = elec_CH4;
                Electrofilter_H2S.value = elec_H2S;
                Electrofilter_O2.value = elec_O2;
                Electrofilter_N2.value = elec_N2;

                elec_Temperature = Mathf.Lerp(elec_Temperature, float.Parse(tablesData[0].text), 2 * Time.deltaTime);
                elec_Dust = Mathf.Lerp(elec_Dust, float.Parse(tablesData[1].text), 2 * Time.deltaTime);
                elec_SolidParticle = Mathf.Lerp(elec_SolidParticle, float.Parse(tablesData[2].text), 2 * Time.deltaTime);
                elec_Zola = Mathf.Lerp(elec_Zola, float.Parse(tablesData[3].text), 2 * Time.deltaTime);

                elec_CO = Mathf.Lerp(elec_CO, float.Parse(tablesData[4].text), 2 * Time.deltaTime);
                elec_CO2 = Mathf.Lerp(elec_CO2, float.Parse(tablesData[5].text), 2 * Time.deltaTime);

                elec_NO = Mathf.Lerp(elec_NO, float.Parse(tablesData[6].text), 2 * Time.deltaTime);
                elec_NO2 = Mathf.Lerp(elec_NO2, float.Parse(tablesData[7].text), 2 * Time.deltaTime);
                elec_SO2 = Mathf.Lerp(elec_SO2, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                elec_CH4 = Mathf.Lerp(elec_CH4, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                elec_H2S = Mathf.Lerp(elec_H2S, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                elec_O2 = Mathf.Lerp(elec_O2, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                elec_N2 = Mathf.Lerp(elec_N2, float.Parse(tablesData[12].text), 2 * Time.deltaTime);
            }
            if (delay < 0)
            {
                Electrofilter_Temperature_exit.text = elec_Temperature_exit.ToString("0.");
                Electrofilter_Dust_exit.text = elec_Dust_exit.ToString("0.");
                Electrofilter_SolidParticle_exit.text = elec_SolidParticle_exit.ToString("0.");
                Electrofilter_Zola_exit.text = elec_Zola_exit.ToString("0.");
                Electrofilter_CO_exit.text = elec_CO_exit.ToString("0.");
                Electrofilter_NO_exit.text = elec_NO_exit.ToString("0.");
                Electrofilter_NO2_exit.text = elec_NO2_exit.ToString("0.");
                Electrofilter_CO2_exit.text = elec_CO2.ToString("0.");
                Electrofilter_SO2_exit.text = elec_SO2.ToString("0.");
                Electrofilter_CH4_exit.text = elec_CH4.ToString("0.");
                Electrofilter_H2S_exit.text = elec_H2S.ToString("0.");
                Electrofilter_O2_exit.text = elec_O2.ToString("0.");
                Electrofilter_N2_exit.text = elec_N2.ToString("0.");

                elec_Temperature_exit = Mathf.Lerp(elec_Temperature_exit, float.Parse(tablesData[0].text) * 0.215f, 2 * Time.deltaTime);
                elec_Dust_exit = Mathf.Lerp(elec_Dust_exit, float.Parse(tablesData[1].text) * 0.1f, 2 * Time.deltaTime);
                elec_SolidParticle_exit = Mathf.Lerp(elec_SolidParticle_exit, float.Parse(tablesData[2].text) * 0.093f, 2 * Time.deltaTime);
                elec_Zola_exit = Mathf.Lerp(elec_Zola_exit, float.Parse(tablesData[3].text) * 0.095f, 2 * Time.deltaTime);

                elec_CO_exit = Mathf.Lerp(elec_CO_exit, float.Parse(tablesData[4].text), 2 * Time.deltaTime);
                elec_CO2 = Mathf.Lerp(elec_CO2, float.Parse(tablesData[5].text), 2 * Time.deltaTime);
                elec_NO_exit = Mathf.Lerp(elec_NO_exit, float.Parse(tablesData[6].text), 2 * Time.deltaTime);
                elec_NO2_exit = Mathf.Lerp(elec_NO2_exit, float.Parse(tablesData[7].text), 2 * Time.deltaTime);

                elec_SO2 = Mathf.Lerp(elec_SO2, float.Parse(tablesData[8].text), 2 * Time.deltaTime);
                elec_CH4 = Mathf.Lerp(elec_CH4, float.Parse(tablesData[9].text), 2 * Time.deltaTime);
                elec_H2S = Mathf.Lerp(elec_H2S, float.Parse(tablesData[10].text), 2 * Time.deltaTime);
                elec_O2 = Mathf.Lerp(elec_O2, float.Parse(tablesData[11].text), 2 * Time.deltaTime);
                elec_N2 = Mathf.Lerp(elec_N2, float.Parse(tablesData[12].text), 2 * Time.deltaTime);
            }
            if (delay < -10)
            {
                isEnable = false;
                //Electrofilter_Temperature_enter.onValueChanged.AddListener(delegate { RepeatCalculateelecz(); });
            }

        }
    }

    public void RepeatCalculateelecz()
    {
        //Electrofilter_Temperature_enter.onValueChanged.RemoveListener(delegate { RepeatCalculateelecz(); });
        delay = 12f;
        isEnable = true;
        elec_Temperature = float.Parse(tablesData[0].text);
        elec_Dust = float.Parse(tablesData[1].text);
        elec_SolidParticle = float.Parse(tablesData[2].text);
        elec_Zola = float.Parse(tablesData[3].text);

        elec_CO = float.Parse(tablesData[4].text);
        elec_NO = float.Parse(tablesData[6].text);
        elec_NO2 = float.Parse(tablesData[7].text);

        elec_CO2 = float.Parse(tablesData[5].text);
        elec_SO2 = float.Parse(tablesData[8].text);
        elec_CH4 = float.Parse(tablesData[9].text);
        elec_H2S = float.Parse(tablesData[10].text);
        elec_O2 = float.Parse(tablesData[11].text);
        elec_N2 = float.Parse(tablesData[12].text);

    }
}
