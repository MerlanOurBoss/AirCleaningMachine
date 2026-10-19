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

    public float delay;
    private float delayPrivate;

    public TextMeshProUGUI[] tablesData;
    public ParticleSystem[] smokes;

    public bool isEnable = false;
    void Start()
    {
        delayPrivate = delay;
        kata_Temperature = ParseValue(tablesData[0].text);
        kata_Dust = ParseValue(tablesData[1].text);
        kata_SolidParticles = ParseValue(tablesData[2].text);
        kata_Zola = ParseValue(tablesData[3].text);

        kata_CO = ParseValue(tablesData[4].text);
        kata_NO = ParseValue(tablesData[6].text);
        kata_NO2 = ParseValue(tablesData[7].text);

        kata_CO2 = ParseValue(tablesData[5].text);
        kata_SO2 = ParseValue(tablesData[8].text);
        kata_CH4 = ParseValue(tablesData[9].text);
        kata_H2S = ParseValue(tablesData[10].text);
        kata_O2 = ParseValue(tablesData[11].text);
        kata_N2 = ParseValue(tablesData[12].text);

        kata_Temperature_exit = ParseValue(tablesData[0].text);
        kata_Dust_exit = ParseValue(tablesData[1].text);
        kata_SolidParticles_exit = ParseValue(tablesData[2].text);
        kata_Zola_exit = ParseValue(tablesData[3].text);

        kata_CO_exit = ParseValue(tablesData[4].text);
        kata_NO_exit = ParseValue(tablesData[6].text);
        kata_NO2_exit = ParseValue(tablesData[7].text);
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
                else if (Katalizator_Dust_enter.value >= 10 && Katalizator_Dust_enter.value < 20)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.9f, 0.9f, 0.9f, 0.3f);
                    }
                }
                else if (Katalizator_Dust_enter.value >= 20 && Katalizator_Dust_enter.value <= 30)
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
                else if (Katalizator_Zola_enter.value >= 5 && Katalizator_Zola_enter.value < 10)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
                    }
                }
                else if (Katalizator_Zola_enter.value >= 10 && Katalizator_Zola_enter.value <= 20)
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

                kata_Temperature = Mathf.Lerp(kata_Temperature, ParseValue(tablesData[0].text), 2 * Time.deltaTime);
                kata_Dust = Mathf.Lerp(kata_Dust, ParseValue(tablesData[1].text), 2 * Time.deltaTime);
                kata_SolidParticles = Mathf.Lerp(kata_SolidParticles, ParseValue(tablesData[2].text), 2 * Time.deltaTime);
                kata_Zola = Mathf.Lerp(kata_Zola, ParseValue(tablesData[3].text), 2 * Time.deltaTime);

                kata_CO = Mathf.Lerp(kata_CO, ParseValue(tablesData[4].text), 2 * Time.deltaTime);
                kata_CO2 = Mathf.Lerp(kata_CO2, ParseValue(tablesData[5].text), 2 * Time.deltaTime);

                kata_NO = Mathf.Lerp(kata_NO, ParseValue(tablesData[6].text), 2 * Time.deltaTime);
                kata_NO2 = Mathf.Lerp(kata_NO2, ParseValue(tablesData[7].text), 2 * Time.deltaTime);
                kata_SO2 = Mathf.Lerp(kata_SO2, ParseValue(tablesData[8].text), 2 * Time.deltaTime);
                kata_CH4 = Mathf.Lerp(kata_CH4, ParseValue(tablesData[9].text), 2 * Time.deltaTime);
                kata_H2S = Mathf.Lerp(kata_H2S, ParseValue(tablesData[10].text), 2 * Time.deltaTime);
                kata_O2 = Mathf.Lerp(kata_O2, ParseValue(tablesData[11].text), 2 * Time.deltaTime);
                kata_N2 = Mathf.Lerp(kata_N2, ParseValue(tablesData[12].text), 2 * Time.deltaTime);
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

                kata_Temperature_exit = Mathf.Lerp(kata_Temperature_exit, ParseValue(tablesData[0].text) * 4.17f, 2 * Time.deltaTime);
                kata_Dust_exit = Mathf.Lerp(kata_Dust_exit, ParseValue(tablesData[1].text), 2 * Time.deltaTime);
                kata_SolidParticles_exit = Mathf.Lerp(kata_SolidParticles_exit, ParseValue(tablesData[2].text), 2 * Time.deltaTime);
                kata_Zola_exit = Mathf.Lerp(kata_Zola_exit, ParseValue(tablesData[3].text), 2 * Time.deltaTime);

                kata_CO_exit = Mathf.Lerp(kata_CO_exit, ParseValue(tablesData[4].text) * 0.025f, 2 * Time.deltaTime);
                kata_CO2 = Mathf.Lerp(kata_CO2, ParseValue(tablesData[5].text), 2 * Time.deltaTime);
                kata_NO_exit = Mathf.Lerp(kata_NO_exit, ParseValue(tablesData[6].text) * 0.023f, 2 * Time.deltaTime);
                kata_NO2_exit = Mathf.Lerp(kata_NO2_exit, ParseValue(tablesData[7].text) * 0.023f, 2 * Time.deltaTime);

                kata_SO2 = Mathf.Lerp(kata_SO2, ParseValue(tablesData[8].text), 2 * Time.deltaTime);
                kata_CH4 = Mathf.Lerp(kata_CH4, ParseValue(tablesData[9].text), 2 * Time.deltaTime);
                kata_H2S = Mathf.Lerp(kata_H2S, ParseValue(tablesData[10].text), 2 * Time.deltaTime);
                kata_O2 = Mathf.Lerp(kata_O2, ParseValue(tablesData[11].text), 2 * Time.deltaTime);
                kata_N2 = Mathf.Lerp(kata_N2, ParseValue(tablesData[12].text), 2 * Time.deltaTime);
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
        delay = delayPrivate;
        isEnable = true;
        kata_Temperature = ParseValue(tablesData[0].text);
        kata_Dust = ParseValue(tablesData[1].text);
        kata_SolidParticles = ParseValue(tablesData[2].text);
        kata_Zola = ParseValue(tablesData[3].text);

        kata_CO = ParseValue(tablesData[4].text);
        kata_NO = ParseValue(tablesData[6].text);
        kata_NO2 = ParseValue(tablesData[7].text);

        kata_CO2 = ParseValue(tablesData[5].text);
        kata_SO2 = ParseValue(tablesData[8].text);
        kata_CH4 = ParseValue(tablesData[9].text);
        kata_H2S = ParseValue(tablesData[10].text);
        kata_O2 = ParseValue(tablesData[11].text);
        kata_N2 = ParseValue(tablesData[12].text);

    }

    // Разбор числа из текста таблицы без зависимости от языка Windows ("12.5" и "12,5").
    // Раньше float.Parse бросал исключение каждый кадр, если формат не совпадал с локалью.
    private static float ParseValue(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0f;
        return float.TryParse(text.Trim().Replace(',', '.'),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out float value) ? value : 0f;
    }
}
