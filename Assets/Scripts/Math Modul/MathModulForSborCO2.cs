using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathModulForSborCO2 : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _smokes;
    [SerializeField] private TMP_InputField _sorbentTypeText;
    [SerializeField] private TMP_InputField _gasVolumeText;
    [SerializeField] private TMP_InputField _adsorptionTempText;
    [SerializeField] private TMP_InputField _desorptionTempText;

    [SerializeField] private TextMeshProUGUI _adsorptionTimeText;
    [SerializeField] private TextMeshProUGUI _capturedCO2Text;
    [SerializeField] private TextMeshProUGUI _desorbedCO2Text;

    private string sorbentType;
    private float gasVolume;
    private float adsorptionTemp;
    private float desorptionTemp;
    private bool isProcessed = false;
    private bool isProcessed1 = false;
    private bool isProcessed2 = false;
    private int a = 0;
    private int b = 0;

    private float sorbentCapacity; // моль CO₂/кг
    private float sorbentEfficiency; // Коэффициент эффективности
    private float capturedCO2; // Захваченный CO₂ (моль)
    private const float CO2Concentration = 0.1f; // Концентрация CO₂ в газе (10%)
    private const float molarMassCO2 = 44.01f; // Молярная масса CO₂
    private const float sorbentMass = 10f;

    private void Start()
    {
        UpdateSorbentProperties();
    }

    [System.Obsolete]
    private void Update()
    {
        UpdateSorbentProperties();
        // Вывод параметров
        _adsorptionTimeText.text = "Время адсорбции: " + CalculateAdsorptionTime().ToString("0.00") + " ч";
        _capturedCO2Text.text = "Захваченный CO2: " + capturedCO2.ToString("0.00") + " моль";
        _desorbedCO2Text.text = "Десорбированный CO2: " + CalculateDesorption().ToString("0.00") + " моль";

        if (_gasVolumeText.text == "150 м³/ч" && !isProcessed)
        {
            a++;
            foreach (ParticleSystem smoke in _smokes)
            {
                smoke.startSpeed = smoke.startSpeed + 0.2f - (0.4f * b);

                ParticleSystem.ColorOverLifetimeModule colorModul = smoke.colorOverLifetime;

                Gradient currentGradient = colorModul.color.gradient;

                Gradient newGradient = new Gradient();
                newGradient.SetKeys(
                    currentGradient.colorKeys,
                    new GradientAlphaKey[] {
                    new GradientAlphaKey(0.7f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(0.7f, 1.0f) // Конечная альфа
                    }
                );
                colorModul.color = new ParticleSystem.MinMaxGradient(newGradient);
            }
            isProcessed = true;
            isProcessed1 = false;
            isProcessed2 = false;
            b = 0;
        }
        else if (_gasVolumeText.text == "100 м³/ч" && !isProcessed1)
        {
            foreach (ParticleSystem smoke in _smokes)
            {
                smoke.startSpeed = smoke.startSpeed - (0.2f * a) - (0.4f * b);

                ParticleSystem.ColorOverLifetimeModule colorModul = smoke.colorOverLifetime;

                Gradient currentGradient = colorModul.color.gradient;

                Gradient newGradient = new Gradient();
                newGradient.SetKeys(
                    currentGradient.colorKeys,
                    new GradientAlphaKey[] {
                    new GradientAlphaKey(0.3f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(0.3f, 1.0f) // Конечная альфа
                    }
                );
                colorModul.color = new ParticleSystem.MinMaxGradient(newGradient);
            }
            isProcessed = false;
            isProcessed1 = true;
            isProcessed2 = false;
            a = 0;
            b = 0;
        }
        else if (_gasVolumeText.text == "200 м³/ч" && !isProcessed2)
        {
            b++;
            foreach (ParticleSystem smoke in _smokes)
            {
                smoke.startSpeed = smoke.startSpeed + 0.4f - (0.2f * a);

                ParticleSystem.ColorOverLifetimeModule colorModul = smoke.colorOverLifetime;

                Gradient currentGradient = colorModul.color.gradient;

                Gradient newGradient = new Gradient();
                newGradient.SetKeys(
                    currentGradient.colorKeys,
                    new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(1.0f, 1.0f) // Конечная альфа
                    }
                );
                colorModul.color = new ParticleSystem.MinMaxGradient(newGradient);
            }
            isProcessed = false;
            isProcessed1 = false;
            isProcessed2 = true;
            a = 0;
        }
    }

    private void UpdateSorbentProperties()
    {
        // Определение характеристик сорбента
        sorbentType = _sorbentTypeText.text;
        switch (sorbentType.ToLower())
        {
            case "цеолит":
                sorbentCapacity = 2.0f;
                sorbentEfficiency = 0.9f;
                break;
            case "сода":
                sorbentCapacity = 1.5f;
                sorbentEfficiency = 0.85f;
                break;
            case "модифицированный сорбент":
                sorbentCapacity = 2.5f;
                sorbentEfficiency = 0.92f;
                break;
            default:
                sorbentCapacity = 1.0f;
                sorbentEfficiency = 0.8f;
                break;
        }
    }

    private float CalculateAdsorptionTime()
    {
        // Расчет времени адсорбции
        gasVolume = ParseInput(_gasVolumeText.text);
        float co2FlowRate = gasVolume * CO2Concentration * molarMassCO2 / 22.4f; // Поток CO₂ (моль/ч)
        float totalCapacity = sorbentMass * sorbentCapacity;
        float effectiveFlowRate = co2FlowRate * sorbentEfficiency;
        capturedCO2 = Mathf.Min(totalCapacity, effectiveFlowRate);
        return totalCapacity / effectiveFlowRate;
    }

    private float CalculateDesorption()
    {
        // Расчет десорбции
        desorptionTemp = ParseInput(_desorptionTempText.text);
        float desorptionEfficiency = Mathf.Clamp((desorptionTemp - 100f) / 30f, 0f, 1f);
        return capturedCO2 * desorptionEfficiency;
    }

    private float ParseInput(string input)
    {
        // Извлечение числа из текста (до пробела)
        if (float.TryParse(input.Split(' ')[0], out float value))
        {
            return value;
        }
        return 0f;
    }
}
