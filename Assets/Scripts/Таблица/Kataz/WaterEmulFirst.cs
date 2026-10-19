using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterEmulFirst : MonoBehaviour
{
    public Slider Water_Temperature_enter;
    public Slider Water_Dust_enter;
    public Slider Water_SolidParticles_enter;
    public Slider Water_Zola_enter;
    public Slider Water_CO_enter;
    public Slider Water_CO2_enter;
    public Slider Water_NO_enter;
    public Slider Water_NO2_enter;
    public Slider Water_SO2_enter;
    public Slider Water_CH4_enter;
    public Slider Water_H2S_enter;
    public Slider Water_O2_enter;
    public Slider Water_N2_enter;

    public TextMeshProUGUI Water_Temperature;
    public TextMeshProUGUI Water_Dust;
    public TextMeshProUGUI Water_SolidParticles;
    public TextMeshProUGUI Water_Zola;
    public TextMeshProUGUI Water_CO;
    public TextMeshProUGUI Water_CO2;
    public TextMeshProUGUI Water_NO;
    public TextMeshProUGUI Water_NO2;
    public TextMeshProUGUI Water_SO2;
    public TextMeshProUGUI Water_CH4;
    public TextMeshProUGUI Water_H2S;
    public TextMeshProUGUI Water_O2;
    public TextMeshProUGUI Water_N2;
    public ParticleSystem[] smokes;

    private float water_Temperature;
    private float water_Dust;
    private float water_SolidParticles;
    private float water_Zola;
    private float water_CO;
    private float water_CO2;
    private float water_NO;
    private float water_NO2;
    private float water_SO2;
    private float water_CH4;
    private float water_H2S;
    private float water_O2;
    private float water_N2;

    private float delay = 2f;
    public TextMeshProUGUI[] tablesData;
    public TextMeshProUGUI[] originTablesData;

    public bool isEnable = false;
    private void Start()
    {
        Water_Temperature_enter.value = ParseValue(originTablesData[0].text);
        Water_Dust_enter.value = ParseValue(originTablesData[1].text);
        Water_SolidParticles_enter.value = ParseValue(originTablesData[2].text);
        Water_Zola_enter.value = ParseValue(originTablesData[3].text);
        Water_CO_enter.value = ParseValue(originTablesData[4].text);
        Water_CO2_enter.value = ParseValue(originTablesData[5].text);
        Water_NO_enter.value = ParseValue(originTablesData[6].text);
        Water_NO2_enter.value = ParseValue(originTablesData[7].text);
        Water_SO2_enter.value = ParseValue(originTablesData[8].text);
        Water_CH4_enter.value = ParseValue(originTablesData[9].text);
        Water_H2S_enter.value = ParseValue(originTablesData[10].text);
        Water_O2_enter.value = ParseValue(originTablesData[11].text);
        Water_N2_enter.value = ParseValue(originTablesData[12].text);

        water_Temperature = ParseValue(tablesData[0].text);
        water_Dust = ParseValue(tablesData[1].text);
        water_SolidParticles = ParseValue(tablesData[2].text);
        water_Zola = ParseValue(tablesData[3].text);

        water_CO = ParseValue(tablesData[4].text);
        water_CO2 = ParseValue(tablesData[5].text);
        water_NO = ParseValue(tablesData[6].text);
        water_NO2 = ParseValue(tablesData[7].text);
        water_SO2 = ParseValue(tablesData[8].text);
        water_CH4 = ParseValue(tablesData[9].text);
        water_H2S = ParseValue(tablesData[10].text);
        water_O2 = ParseValue(tablesData[11].text);
        water_N2 = ParseValue(tablesData[12].text);
    }

    [System.Obsolete]
    void Update()
    {
        if (isEnable)
        {
            delay -= 1 * Time.deltaTime;

            if (delay < 0)
            {

                if (Water_SolidParticles_enter.value > 0 && Water_SolidParticles_enter.value < 300)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(1f, 1f, 1f, 0.3f);
                    }
                }
                else if (Water_SolidParticles_enter.value >= 300 && Water_SolidParticles_enter.value < 400)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
                    }
                }
                else if (Water_SolidParticles_enter.value >= 400 && Water_SolidParticles_enter.value <= 500)
                {
                    foreach (ParticleSystem item in smokes)
                    {
                        item.startColor = new Color(0.7f, 0.7f, 0.7f, 0.3f);
                    }
                }


                Water_Temperature.text = water_Temperature.ToString("0.");
                Water_Dust.text = water_Dust.ToString("0.");
                Water_SolidParticles.text = water_SolidParticles.ToString("0.");
                Water_Zola.text = water_Zola.ToString("0.");

                Water_CO.text = water_CO.ToString("0.");
                Water_CO2.text = water_CO2.ToString("0.");
                Water_NO.text = water_NO.ToString("0.");
                Water_NO2.text = water_NO2.ToString("0.");
                Water_SO2.text = water_SO2.ToString("0.");
                Water_CH4.text = water_CH4.ToString("0.");
                Water_H2S.text = water_H2S.ToString("0.");
                Water_O2.text = water_O2.ToString("0.");
                Water_N2.text = water_N2.ToString("0.");

                water_Temperature = Mathf.Lerp(water_Temperature, ParseValue(tablesData[0].text) * 0.12f, 2 * Time.deltaTime);
                water_Dust = Mathf.Lerp(water_Dust, ParseValue(tablesData[1].text) * 0.1f, 2 * Time.deltaTime);
                water_SolidParticles = Mathf.Lerp(water_SolidParticles, ParseValue(tablesData[2].text) * 0.1f, 2 * Time.deltaTime);
                water_Zola = Mathf.Lerp(water_Zola, ParseValue(tablesData[3].text) * 0.25f, 2 * Time.deltaTime);

                water_CO = Mathf.Lerp(water_CO, ParseValue(tablesData[4].text), 2 * Time.deltaTime);
                water_CO2 = Mathf.Lerp(water_CO2, ParseValue(tablesData[5].text) * 0.922f, 2 * Time.deltaTime);
                water_NO = Mathf.Lerp(water_NO, ParseValue(tablesData[6].text) , 2 * Time.deltaTime);
                water_NO2 = Mathf.Lerp(water_NO2, ParseValue(tablesData[7].text) , 2 * Time.deltaTime);

                water_SO2 = Mathf.Lerp(water_SO2, ParseValue(tablesData[8].text) * 0.043f, 2 * Time.deltaTime);
                water_CH4 = Mathf.Lerp(water_CH4, ParseValue(tablesData[9].text) * 0.294f, 2 * Time.deltaTime);
                water_H2S = Mathf.Lerp(water_H2S, ParseValue(tablesData[10].text) * 0.04f, 2 * Time.deltaTime);
                water_O2 = Mathf.Lerp(water_O2, ParseValue(tablesData[11].text), 2 * Time.deltaTime);
                water_N2 = Mathf.Lerp(water_N2, ParseValue(tablesData[12].text), 2 * Time.deltaTime);

            }

            // Раньше было "else if" после "if (delay < 0)" — ветка никогда не срабатывала,
            // и таблица пересчитывалась бесконечно.
            if (delay < -5)
            {
                isEnable = false;
            }

        }
    }

    public void RecalculateData()
    {
        Water_Temperature_enter.value = ParseValue(originTablesData[0].text);
        Water_Dust_enter.value = ParseValue(originTablesData[1].text);
        Water_SolidParticles_enter.value = ParseValue(originTablesData[2].text);
        Water_Zola_enter.value = ParseValue(originTablesData[3].text);
        Water_CO_enter.value = ParseValue(originTablesData[4].text);
        Water_CO2_enter.value = ParseValue(originTablesData[5].text);
        Water_NO_enter.value = ParseValue(originTablesData[6].text);
        Water_NO2_enter.value = ParseValue(originTablesData[7].text);
        Water_SO2_enter.value = ParseValue(originTablesData[8].text);
        Water_CH4_enter.value = ParseValue(originTablesData[9].text);
        Water_H2S_enter.value = ParseValue(originTablesData[10].text);
        Water_O2_enter.value = ParseValue(originTablesData[11].text);
        Water_N2_enter.value = ParseValue(originTablesData[12].text);
    }
    public void RepeatCalculate()
    {
        delay = 5f;
        isEnable = true;
        water_Temperature = ParseValue(tablesData[0].text);
        water_Dust = ParseValue(tablesData[1].text);
        water_SolidParticles = ParseValue(tablesData[2].text);
        water_Zola = ParseValue(tablesData[3].text);

        water_CO = ParseValue(tablesData[4].text);
        water_CO2 = ParseValue(tablesData[5].text);
        water_NO = ParseValue(tablesData[6].text);
        water_NO2 = ParseValue(tablesData[7].text);
        water_SO2 = ParseValue(tablesData[8].text);
        water_CH4 = ParseValue(tablesData[9].text);
        water_H2S = ParseValue(tablesData[10].text);
        water_O2 = ParseValue(tablesData[11].text);
        water_N2 = ParseValue(tablesData[12].text);
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
