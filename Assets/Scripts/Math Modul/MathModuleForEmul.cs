using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathModuleForEmul : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private DropSpawner[] _drops;
    [SerializeField] private TMP_InputField _temperature;
    [SerializeField] private TMP_InputField _gasFlow;
    [SerializeField] private TMP_InputField _waterFlow;
    [SerializeField] private TMP_InputField _fluidType;
    [SerializeField] private string fluid;

    [SerializeField] private TextMeshProUGUI gasSpeed;
    [SerializeField] private TextMeshProUGUI waterSpeed;
    [SerializeField] private TextMeshProUGUI gasMassFlow;
    [SerializeField] private TextMeshProUGUI waterMassFlow;
    [SerializeField] private TextMeshProUGUI massTransfer;
    [SerializeField] private Translator translator;

    private float _gasSpeed = 0;
    private float _waterSpeed = 0;
    private float _gasMassFlow = 0;
    private float _waterMassFlow = 0;
    private float _reynoldsNumber = 0;
    private float _massTransfer = 0;

    private readonly float deametr = 2f;

    private readonly float empiricalConstantsA = 0.5f;
    private readonly float empiricalConstantsB = 0.8f;

    private readonly float deametrDroplet = 1;
    private readonly float waterDensity = 1000f;
    private readonly float causticSodaDensity = 1100f;
    private readonly float sodaDensity = 1050f;

    private readonly float waterDynamicViscosity = 0.001f;
    private readonly float causticSodaDynamicViscosity = 0.0012f;
    private readonly float sodaDynamicViscosity = 0.0013f;


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
            gasSpeed.text = "Скорость газа: " + _gasSpeed.ToString("0.000") + " м/с";
            waterSpeed.text = "Скорость воды: " + _waterSpeed.ToString("0.000") + " м/с";
            gasMassFlow.text = "Массовый поток газа: " + _gasMassFlow.ToString("0.000") + " м³/с";
            waterMassFlow.text = "Массовый поток жидкости: " + _waterMassFlow.ToString("0.000") + " м³/с";
            massTransfer.text = "Коэф. массового переноса: " + _massTransfer.ToString("0.0") + " м/с";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            gasSpeed.text = "Газ жылдамдығы: " + _gasSpeed.ToString("0.000") + " м/с";
            waterSpeed.text = "Су жылдамдығы: " + _waterSpeed.ToString("0.000") + " м/с";
            gasMassFlow.text = "Газ массасы ағыны: " + _gasMassFlow.ToString("0.000") + " м³/с";
            waterMassFlow.text = "Сұйық масса ағыны: " + _waterMassFlow.ToString("0.000") + " м³/с";
            massTransfer.text = "Масса тасымалдау коэфф.: " + _massTransfer.ToString("0.0") + " м/с";
        }
        else
        {
            gasSpeed.text = "Gas Speed: " + _gasSpeed.ToString("0.000") + " m/s";
            waterSpeed.text = "Water Speed: " + _waterSpeed.ToString("0.000") + " m/s";
            gasMassFlow.text = "Gas Mass Flow: " + _gasMassFlow.ToString("0.000") + " m³/s";
            waterMassFlow.text = "Liquid mass flow: " + _waterMassFlow.ToString("0.000") + " m³/s";
            massTransfer.text = "Mass transfer coefficient: " + _massTransfer.ToString("0.0") + " m/s";
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

        _massTransfer = empiricalConstantsA * Mathf.Pow((_reynoldsNumber / deametrDroplet), empiricalConstantsB);
    }
}
