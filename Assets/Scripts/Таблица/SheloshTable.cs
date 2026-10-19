using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SheloshTable : MonoBehaviour
{
    public Slider Shelosh_Temperature_enter;
    public Slider Shelosh_Dust_enter;
    public Slider Shelosh_SolidParticle_enter;
    public Slider Shelosh_Zola_enter;

    public Slider Shelosh_CO_enter;
    public Slider Shelosh_NO_enter;
    public Slider Shelosh_NO2_enter;

    public Slider Shelosh_CO2_enter;
    public Slider Shelosh_SO2_enter;
    public Slider Shelosh_CH4_enter;
    public Slider Shelosh_H2S_enter;
    public Slider Shelosh_O2_enter;
    public Slider Shelosh_N2_enter;

    public TextMeshProUGUI Shelosh_Temperature_exit;
    public TextMeshProUGUI Shelosh_Dust_exit;
    public TextMeshProUGUI Shelosh_SolidParticle_exit;
    public TextMeshProUGUI Shelosh_Zola_exit;

    public TextMeshProUGUI Shelosh_CO_exit;
    public TextMeshProUGUI Shelosh_NO_exit;
    public TextMeshProUGUI Shelosh_NO2_exit;

    public TextMeshProUGUI Shelosh_CO2_exit;
    public TextMeshProUGUI Shelosh_SO2_exit;
    public TextMeshProUGUI Shelosh_CH4_exit;
    public TextMeshProUGUI Shelosh_H2S_exit;
    public TextMeshProUGUI Shelosh_O2_exit;
    public TextMeshProUGUI Shelosh_N2_exit;



    private float shelosh_Temperature;
    private float shelosh_Dust;
    private float shelosh_SolidParticles;
    private float shelosh_Zola;

    private float shelosh_CO_enter;
    private float shelosh_NO_enter;
    private float shelosh_NO2_enter;

    private float shelosh_CO2_enter;
    private float shelosh_SO2_enter;
    private float shelosh_CH4_enter;
    private float shelosh_H2S_enter;
    private float shelosh_O2_enter;
    private float shelosh_N2_enter;

    private float shelosh_Temperature_exit;
    private float shelosh_Dust_exit;
    private float shelosh_SolidParticles_exit;
    private float shelosh_Zola_exit;

    private float shelosh_CO_exit;
    private float shelosh_NO_exit;
    private float shelosh_NO2_exit;

    private float shelosh_CO2_exit;
    private float shelosh_SO2_exit;
    private float shelosh_CH4_exit;
    private float shelosh_H2S_exit;

    public float delay;
    private float delayPrivate;

    public TextMeshProUGUI[] tablesData;
    public ParticleSystem[] smokes;
    public bool isEnable = false;
    void Start()
    {
        delayPrivate = delay;
        shelosh_Temperature = ParseValue(tablesData[0].text);
        shelosh_Dust = ParseValue(tablesData[1].text);
        shelosh_SolidParticles = ParseValue(tablesData[2].text);
        shelosh_Zola = ParseValue(tablesData[3].text);

        shelosh_CO_enter = ParseValue(tablesData[4].text);
        shelosh_CO2_enter = ParseValue(tablesData[5].text);
        shelosh_NO_enter = ParseValue(tablesData[6].text);
        shelosh_NO2_enter = ParseValue(tablesData[7].text);
        
        shelosh_SO2_enter = ParseValue(tablesData[8].text);
        shelosh_CH4_enter = ParseValue(tablesData[9].text);
        shelosh_H2S_enter = ParseValue(tablesData[10].text);
        shelosh_O2_enter = ParseValue(tablesData[11].text);
        shelosh_N2_enter = ParseValue(tablesData[12].text);

        shelosh_Temperature_exit = ParseValue(tablesData[0].text);
        shelosh_Dust_exit = ParseValue(tablesData[1].text);
        shelosh_SolidParticles_exit = ParseValue(tablesData[2].text);
        shelosh_Zola_exit = ParseValue(tablesData[3].text);

        shelosh_CO_exit = ParseValue(tablesData[4].text);
        shelosh_CO2_exit = ParseValue(tablesData[5].text);
        shelosh_NO_exit = ParseValue(tablesData[6].text);
        shelosh_NO2_exit = ParseValue(tablesData[7].text);

        shelosh_SO2_exit = ParseValue(tablesData[8].text);
        shelosh_CH4_exit = ParseValue(tablesData[9].text);
        shelosh_H2S_exit = ParseValue(tablesData[10].text); 
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;
            if (delay < 2)
            {

                if (Shelosh_SolidParticle_enter.value > 0 && Shelosh_SolidParticle_enter.value <= 20)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.5f);
                    }
                }
                else if (Shelosh_SolidParticle_enter.value >= 20 && Shelosh_SolidParticle_enter.value <= 50)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 1f);
                    }
                }

                Shelosh_Temperature_enter.value = shelosh_Temperature;
                Shelosh_Dust_enter.value = shelosh_Dust;
                Shelosh_SolidParticle_enter.value = shelosh_SolidParticles;
                Shelosh_Zola_enter.value = shelosh_Zola;
                Shelosh_CO_enter.value = shelosh_CO_enter;
                Shelosh_NO_enter.value = shelosh_NO_enter;
                Shelosh_NO2_enter.value = shelosh_NO2_enter;

                Shelosh_CO2_enter.value = shelosh_CO2_enter;
                Shelosh_SO2_enter.value = shelosh_SO2_enter;
                Shelosh_CH4_enter.value = shelosh_CH4_enter;
                Shelosh_H2S_enter.value = shelosh_H2S_enter;
                Shelosh_O2_enter.value = shelosh_O2_enter;
                Shelosh_N2_enter.value = shelosh_N2_enter;

                shelosh_Temperature = Mathf.Lerp(shelosh_Temperature, ParseValue(tablesData[0].text), 2 * Time.deltaTime);
                shelosh_Dust = Mathf.Lerp(shelosh_Dust, ParseValue(tablesData[1].text), 2 * Time.deltaTime);
                shelosh_SolidParticles = Mathf.Lerp(shelosh_SolidParticles, ParseValue(tablesData[2].text), 2 * Time.deltaTime);
                shelosh_Zola = Mathf.Lerp(shelosh_Zola, ParseValue(tablesData[3].text), 2 * Time.deltaTime);
                shelosh_CO_enter = Mathf.Lerp(shelosh_CO_enter, ParseValue(tablesData[4].text), 2 * Time.deltaTime);
                shelosh_CO2_enter = Mathf.Lerp(shelosh_CO2_enter, ParseValue(tablesData[5].text), 2 * Time.deltaTime);
                shelosh_NO_enter = Mathf.Lerp(shelosh_NO_enter, ParseValue(tablesData[6].text), 2 * Time.deltaTime);
                shelosh_NO2_enter = Mathf.Lerp(shelosh_NO2_enter, ParseValue(tablesData[7].text), 2 * Time.deltaTime);

                shelosh_SO2_enter = Mathf.Lerp(shelosh_SO2_enter, ParseValue(tablesData[8].text), 2 * Time.deltaTime);
                shelosh_CH4_enter = Mathf.Lerp(shelosh_CH4_enter, ParseValue(tablesData[9].text), 2 * Time.deltaTime);
                shelosh_H2S_enter = Mathf.Lerp(shelosh_H2S_enter, ParseValue(tablesData[10].text), 2 * Time.deltaTime);
                shelosh_O2_enter = Mathf.Lerp(shelosh_O2_enter, ParseValue(tablesData[11].text), 2 * Time.deltaTime);
                shelosh_N2_enter = Mathf.Lerp(shelosh_N2_enter, ParseValue(tablesData[12].text), 2 * Time.deltaTime);
            }
            if (delay < 0)
            {
                Shelosh_Temperature_exit.text = shelosh_Temperature_exit.ToString("0.");
                Shelosh_Dust_exit.text = shelosh_Dust_exit.ToString("0.");
                Shelosh_SolidParticle_exit.text = shelosh_SolidParticles_exit.ToString("0.");
                Shelosh_Zola_exit.text = shelosh_Zola_exit.ToString("0.");

                Shelosh_CO_exit.text = shelosh_CO_exit.ToString("0.");
                Shelosh_NO_exit.text = shelosh_NO_exit.ToString("0.");
                Shelosh_NO2_exit.text = shelosh_NO2_exit.ToString("0.");

                Shelosh_CO2_exit.text = shelosh_CO2_exit.ToString("0.");
                Shelosh_SO2_exit.text = shelosh_SO2_exit.ToString("0.");
                Shelosh_CH4_exit.text = shelosh_CH4_exit.ToString("0.");
                Shelosh_H2S_exit.text = shelosh_H2S_exit.ToString("0.");
                Shelosh_O2_exit.text = shelosh_O2_enter.ToString("0.");
                Shelosh_N2_exit.text = shelosh_N2_enter.ToString("0.");

                shelosh_Temperature_exit = Mathf.Lerp(shelosh_Temperature_exit, ParseValue(tablesData[0].text) * 0.12f, 2 * Time.deltaTime);
                shelosh_Dust_exit = Mathf.Lerp(shelosh_Dust_exit, ParseValue(tablesData[1].text) * 0.1f, 2 * Time.deltaTime);
                shelosh_SolidParticles_exit = Mathf.Lerp(shelosh_SolidParticles_exit, ParseValue(tablesData[2].text) * 0.1f, 2 * Time.deltaTime);
                shelosh_Zola_exit = Mathf.Lerp(shelosh_Zola_exit, ParseValue(tablesData[3].text) * 0.25f, 2 * Time.deltaTime);

                shelosh_CO_exit = Mathf.Lerp(shelosh_CO_exit, ParseValue(tablesData[4].text) , 2 * Time.deltaTime);
                shelosh_CO2_exit = Mathf.Lerp(shelosh_CO2_exit, ParseValue(tablesData[5].text) * 0.922f, 2 * Time.deltaTime);
                shelosh_NO_exit = Mathf.Lerp(shelosh_NO_exit, ParseValue(tablesData[6].text) , 2 * Time.deltaTime);
                shelosh_NO2_exit = Mathf.Lerp(shelosh_NO2_exit, ParseValue(tablesData[7].text) , 2 * Time.deltaTime);

                
                shelosh_SO2_exit = Mathf.Lerp(shelosh_SO2_exit, ParseValue(tablesData[8].text) * 0.043f, 2 * Time.deltaTime);
                shelosh_CH4_exit = Mathf.Lerp(shelosh_CH4_exit, ParseValue(tablesData[9].text) * 0.294f, 2 * Time.deltaTime);
                shelosh_H2S_exit = Mathf.Lerp(shelosh_H2S_exit, ParseValue(tablesData[10].text) * 0.04f, 2 * Time.deltaTime);
                shelosh_O2_enter = Mathf.Lerp(shelosh_O2_enter, ParseValue(tablesData[11].text) , 2 * Time.deltaTime);
                shelosh_N2_enter = Mathf.Lerp(shelosh_N2_enter, ParseValue(tablesData[12].text) , 2 * Time.deltaTime);

            }

            // Раньше было "else if" после "if (delay < 0)" — ветка никогда не срабатывала,
            // и таблица пересчитывалась бесконечно.
            if (delay < -10)
            {
                isEnable = false;

            }
        }
    }
    public void RepeatCalculateShelosh()
    {
        delay = delayPrivate;
        isEnable = true;
        shelosh_Temperature = ParseValue(tablesData[0].text);
        shelosh_Dust = ParseValue(tablesData[1].text);
        shelosh_SolidParticles = ParseValue(tablesData[2].text);
        shelosh_Zola = ParseValue(tablesData[3].text);
        shelosh_CO_enter = ParseValue(tablesData[4].text);
        shelosh_CO2_enter = ParseValue(tablesData[5].text);
        shelosh_NO_enter = ParseValue(tablesData[6].text);
        shelosh_NO2_enter = ParseValue(tablesData[7].text);
        
        shelosh_SO2_enter = ParseValue(tablesData[8].text);
        shelosh_CH4_enter = ParseValue(tablesData[9].text);
        shelosh_H2S_enter = ParseValue(tablesData[10].text);
        shelosh_O2_enter = ParseValue(tablesData[11].text);
        shelosh_N2_enter = ParseValue(tablesData[12].text);
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
