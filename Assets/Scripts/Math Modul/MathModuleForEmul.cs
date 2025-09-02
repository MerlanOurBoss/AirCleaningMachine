using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MathModuleForEmul : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private DropSpawner[] _drops;
    [SerializeField] private TMP_InputField _temperature;
    [SerializeField] private TMP_InputField _gasFlow;
    [SerializeField] private TMP_InputField _waterFlow;
    [SerializeField] private TMP_InputField _fluidType;
    [SerializeField] private TMP_InputField _gasFlowMain;
    [SerializeField] private string fluid;

    [SerializeField] private TextMeshProUGUI gasSpeed;
    [SerializeField] private TextMeshProUGUI waterSpeed;
    [SerializeField] private TextMeshProUGUI gasMassFlow;
    [SerializeField] private TextMeshProUGUI waterMassFlow;
    [SerializeField] private TextMeshProUGUI massTransfer;
    [SerializeField] private TextMeshProUGUI gasСonsumption;
    [SerializeField] private Translator translator; 
    [SerializeField] private TextMeshProUGUI sizeText;

    private float _gasSpeed = 0;
    private float _waterSpeed = 0;
    private float _gasMassFlow = 0;
    private float _waterMassFlow = 0;
    private float _reynoldsNumber = 0;
    private float _massTransfer = 0;
    private float _gasСonsumption = 0;

    private readonly float empiricalConstantsA = 0.5f;
    private readonly float empiricalConstantsB = 0.8f;

    private readonly float deametrDroplet = 1;
    private readonly float waterDensity = 1000f;
    private readonly float causticSodaDensity = 1100f;
    private readonly float sodaDensity = 1050f;

    private readonly float waterDynamicViscosity = 0.001f;
    private readonly float causticSodaDynamicViscosity = 0.0012f;
    private readonly float sodaDynamicViscosity = 0.0013f;

    private readonly float сonsumption = 34f;

    //Габариты
    private float deametr = 0;
    private double height = 0;


    void Start()
    {
        _temperature.text = "15 °C";
        _gasFlow.text = "14 м³/с";
        _waterFlow.text = "0,1 м³/с";
        _fluidType.text = fluid;
    }

    [System.Obsolete]
    void Update()
    {
        if (translator.currentLanguage == Translator.Language.Russian)
        {
            gasSpeed.text = "Скорость газа: " + "\n" +
                "		   " + _gasSpeed.ToString("0.000") + " м/с";
            waterSpeed.text = "Скорость воды: " + "\n" +
                "		   " + _waterSpeed.ToString("0.000") + " м/с";
            gasMassFlow.text = "Массовый поток газа: " + "\n" +
                "		   " + _gasMassFlow.ToString("0.000") + " м³/с";
            waterMassFlow.text = "Массовый поток жидкости: " + "\n" +
                "		   " + _waterMassFlow.ToString("0.000") + " м³/с";
            massTransfer.text = "Коэф. массового переноса: " + "\n" +
                "		   " + _massTransfer.ToString("0.0") + " м/с";
            gasСonsumption.text = "Расход жидкости: " + "\n" +
                "		   " + _gasСonsumption.ToString() + " м³/с";
            sizeText.text = $"Диаметр аппарата: {deametr:0.0} м \n" +
                        $"Высота аппарата: {height:0.0} м";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            gasSpeed.text = "Газ жылдамдығы: " + "\n" +
                "		   " + _gasSpeed.ToString("0.000") + " м/с";
            waterSpeed.text = "Су жылдамдығы: " + "\n" +
                "		   " + _waterSpeed.ToString("0.000") + " м/с";
            gasMassFlow.text = "Газ массасы ағыны: " + "\n" +
                "		   " + _gasMassFlow.ToString("0.000") + " м³/с";
            waterMassFlow.text = "Сұйық масса ағыны: " + "\n" +
                "		   " + _waterMassFlow.ToString("0.000") + " м³/с";
            massTransfer.text = "Масса тасымалдау коэфф.: " + "\n" +
                "		   " + _massTransfer.ToString("0.0") + " м/с";
            gasСonsumption.text = "Сұйықтықты тұтыну: " + "\n" +
                "		   " + _gasСonsumption.ToString() + " м³/с";
            sizeText.text = $"Құрылғының диаметрі: {deametr:0.0} м \n" +
                                $"Құрылғының биіктігі: {height:0.0} м";
        }
        else
        {
            gasSpeed.text = "Gas Speed: " + "\n" +
                "		   " + _gasSpeed.ToString("0.000") + " m/s";
            waterSpeed.text = "Water Speed: " + "\n" +
                "		   " + _waterSpeed.ToString("0.000") + " m/s";
            gasMassFlow.text = "Gas Mass Flow: " + "\n" +
                "		   " + _gasMassFlow.ToString("0.000") + " m³/s";
            waterMassFlow.text = "Liquid mass flow: " + "\n" +
                "		   " + _waterMassFlow.ToString("0.000") + " m³/s";
            massTransfer.text = "Mass transfer coefficient: " + "\n" +
                "		   " + _massTransfer.ToString("0.0") + " m/s";
            gasСonsumption.text = "Gas Сonsumption: " + "\n" +
                "		   " + _gasСonsumption.ToString() + " м³/с";
            sizeText.text = $"Diameter of device: {deametr:0.0} m \n" +
                                $"Height of device: {height:0.0} m";
        }

        if (_gasFlow.text == "10 м³/с")
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
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

        }
        else if (_gasFlow.text == "12 м³/с")
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
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
            
        }
        else
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
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
 
        }

        if (_fluidType.text == "Вода")
        {
            _reynoldsNumber = (waterDensity * _waterSpeed * deametrDroplet) / waterDynamicViscosity;
        }
        else if (_fluidType.text == "Едкий натрий")
        {
            _reynoldsNumber = (causticSodaDensity * _waterSpeed * deametrDroplet) / causticSodaDynamicViscosity;
        }
        else
        {
            _reynoldsNumber = (sodaDensity * _waterSpeed * deametrDroplet) / sodaDynamicViscosity;
        }

        _gasSpeed = (4 * float.Parse(_gasFlow.text[.._gasFlow.text.IndexOf(" ")].ToString())) / (3.14159f * Mathf.Pow(deametr, 2));
            
        _waterSpeed = (4 * float.Parse(_waterFlow.text[.._waterFlow.text.IndexOf(" ")].ToString())) / (3.14159f * Mathf.Pow(deametr, 2));

        _gasMassFlow = (_gasSpeed * 3.14159f * Mathf.Pow(deametr, 2)) / 4;

        _waterMassFlow = (_waterSpeed * 3.14159f * Mathf.Pow(deametr, 2)) / 4;

        _gasСonsumption = (0.22f * сonsumption) / 1000; 

        _massTransfer = empiricalConstantsA * Mathf.Pow((_reynoldsNumber / deametrDroplet), empiricalConstantsB);

        string inputGasFlow = _gasFlowMain.text;
        string numberGasFlow = inputGasFlow.Split(' ')[0];
        double valueGasFlow = double.Parse(numberGasFlow);

        deametr = (float)Math.Ceiling(
            Math.Sqrt((4.0 * valueGasFlow) / Math.PI / 3600.0 / 2.9)
        );
        height = Math.Ceiling(((0.8 + 18 * 0.15 + 0.35 * deametr + 0.6) / 5.0) * 10.0) / 10.0 * 5.0;
    }
}
