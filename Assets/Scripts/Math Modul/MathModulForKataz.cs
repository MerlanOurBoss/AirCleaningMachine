using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MathModulForKataz : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _smokes;
    [SerializeField] private TMP_InputField _temperatureText;
    [SerializeField] private TMP_InputField _pressureText;
    [SerializeField] private TMP_InputField _flowRateText;
    [SerializeField] private TMP_InputField _katazBlockCount;
    [SerializeField] private TMP_InputField _katazBlockType;
    [SerializeField] private TMP_InputField _gasFlowMain;
    [SerializeField] private TMP_InputField _gasSource;
    [SerializeField] private TextMeshProUGUI _temperatureBeforeReact;
    [SerializeField] private TextMeshProUGUI _temperatureAfterKataz;

    [SerializeField] private TextMeshProUGUI gasVelocity;
    [SerializeField] private TextMeshProUGUI gasDensitie;
    [SerializeField] private TextMeshProUGUI gasmassFlow;
    [SerializeField] private TextMeshProUGUI coGaz;
    [SerializeField] private TextMeshProUGUI _sizeText;

    [SerializeField] private Translator translator;

    private bool isProcessed = false;
    private bool isProcessed1 = false;
    private bool isProcessed2 = false;
    private int a = 0;
    private int b = 0;
    private float velocity;
    private float density;
    private float massFlow;
    private float CO_Gaz_Out;


    private float deametr = 0;

    private float CO_Gaz_In = 0.1f;
    private float k_CO = 0.02f;
    const float R = 8.314f;
    const double Pi = Math.PI;
    const float molarMass = 0.029f;

    //Расходники
    private double electro;
    private double electroAdditional;
    private double consumption;
    private double catalyzator;
    private double waterConsumables;
    private double reagentConsumables;

    private void Start()
    {
        _katazBlockCount.text = "4";
        _katazBlockType.text = "с драгметаллами";
        _temperatureText.text = "25 °C";
        _pressureText.text = "101325 Па";
        _flowRateText.text = "1 м³/с";
        _gasSource.text = "Угольный";
        _katazBlockCount.onValueChanged.AddListener(ChangeBlockNumber);
    }

    [Obsolete]
    private void Update()
    {
        if (translator.currentLanguage == Translator.Language.Russian)
        {
            gasVelocity.text = "Скорость газа: " + "\n" +
                "		   " + velocity.ToString("0.000") + " м/с";
            gasDensitie.text = "Плотность газа: " + "\n" +
                "		   " + density.ToString("0.000") + " кг/м³";
            gasmassFlow.text = "Массовый расход газа:: " + "\n" +
                "		   " + massFlow.ToString("0.000") + " кг/с";
            coGaz.text = "Вых. концентрация CO: " + "\n" +
                "		   " + CO_Gaz_Out.ToString("0.000") + " моль/м³";
            _sizeText.text = $"Диаметр блока: {deametr:0.0} м \n" +
                                $"Расходники ЭФ: {((electro + electroAdditional) * 38.85 * 24) + (waterConsumables * 59.84) + (reagentConsumables * 71.56) + (catalyzator) + (consumption): 0.0} тг";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            gasVelocity.text = "Газдың жылдамдығы: " + "\n" +
                "		   " + velocity.ToString("0.000") + " м/с";
            gasDensitie.text = "Газдың тығыздығы: " + "\n" +
                "		   " + density.ToString("0.000") + " кг/м³";
            gasmassFlow.text = "Газдың массалық шығыны: " + "\n" +
                "		   " + massFlow.ToString("0.000") + " кг/с";
            coGaz.text = "Шығар. СО концентрациясы: " + "\n" +
                "		   " + CO_Gaz_Out.ToString("0.000") + " моль/м³";
            _sizeText.text = $"Блок диаметрі: {deametr:0.0} м \n";
        }
        else
        {
            gasVelocity.text = "Gas velocity: " + "\n" +
                "		   " + velocity.ToString("0.000") + " m/s";
            gasDensitie.text = "Gas Density: " + "\n" +
                "		   " + density.ToString("0.000") + " kg/m³";
            gasmassFlow.text = "Mass gas consumption: " + "\n" +
                "		   " + massFlow.ToString("0.000") + " kg/s";
            coGaz.text = "CO output concentration: " + "\n" +
                "		   " + CO_Gaz_Out.ToString("0.000") + " mol/m³";
            _sizeText.text = $"Block diameter: {deametr:0.0} m \n";
        }

        float crossSectionArea = (float)((float)Pi * Math.Pow(0.5 / 2, 2));

        CO_Gaz_Out = (float)(CO_Gaz_In * Math.Pow(Math.E, k_CO * 1 * (1 / velocity)));
        density = (float.Parse(_pressureText.text[.._pressureText.text.IndexOf(" ")].ToString()) * molarMass) / (R * float.Parse(_temperatureText.text[.._temperatureText.text.IndexOf(" ")].ToString()));
        velocity = float.Parse(_flowRateText.text[.._flowRateText.text.IndexOf(" ")].ToString()) / crossSectionArea;
        massFlow = density * velocity * crossSectionArea;

        string inputGasFlow = _gasFlowMain.text;
        string numberGasFlow = inputGasFlow.Split(' ')[0];
        double valueGasFlow = double.Parse(numberGasFlow);

        deametr = (float)Math.Ceiling(
                        Math.Sqrt((4.0 * valueGasFlow) / Math.PI / 3600.0 / 2.9)
                        );

        double _tempBefore = double.Parse(_temperatureBeforeReact.text);
        double _tempAfter = double.Parse(_temperatureAfterKataz.text);
        double _catalyzBlock = double.Parse(_katazBlockCount.text);

        electro = (1.06 * (_tempAfter - _tempBefore) * 29.68 / 22.4) * valueGasFlow / 3600;

        if (_gasSource.text == "Угольный")
        {
            consumption = (0.429 * valueGasFlow) / 0.85 * 1000;
        }
        else if (_gasSource.text == "Газ")
        {
            consumption = 0.429 * valueGasFlow / (3400 - 1600) * 1000 * 24 * 71.56;
        }
        else
        {
            consumption = 0;
        }

        catalyzator = 28.4 * (deametr * deametr) * _catalyzBlock * 2;

        waterConsumables = 0.4 * 450 / 1000 * 24 / 34 * (valueGasFlow / 3600);
        reagentConsumables = 135 * 24 / 1000 / 34 * (valueGasFlow / 3600);

        electroAdditional = Mathf.Ceil((float)(0.12f * (450f / 1000f) / 5f / 34f * (valueGasFlow / 3600) * 10f)) / 10f * 5f;

        if (_katazBlockType.text == "с драгметаллами")
        {
            catalyzator = catalyzator * 25076 / 365;
        }
        else if (_katazBlockType.text == "без драгметаллов")
        {
            catalyzator = (catalyzator * 25076 + catalyzator * 0.2 * 4200) / 365;
        }

        if (_flowRateText.text == "1,5 м³/с" && !isProcessed)
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
        else if (_flowRateText.text == "1 м³/с" && !isProcessed1)
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
        else if (_flowRateText.text == "2 м³/с" && !isProcessed2)
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

    public void ChangeBlockNumber(string text)
    {
        KatazBlockCountManager[] managers = FindObjectsOfType<KatazBlockCountManager>();
        foreach (KatazBlockCountManager manager in managers)
        {
            manager.ChangeBlocks(int.Parse(text));
        }
    }
}
