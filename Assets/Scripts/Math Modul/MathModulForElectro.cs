using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MathModuleForElectro : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem[] _smokeParticles;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField _densityInput;
    [SerializeField] private TMP_InputField _speedInput;
    [SerializeField] private TMP_InputField _radiusInput;
    [SerializeField] private TMP_InputField _chargeInput;
    [SerializeField] private TMP_InputField _gasFlow;
    [SerializeField] private TextMeshProUGUI _temperature;
    [SerializeField] private TextMeshProUGUI _solidParticle;

    [Header("Visual Effects")]
    [SerializeField] private Animator _electroFilterAnimator;

    [Header("Output Text Fields")]
    [SerializeField] private TextMeshProUGUI _potentialText;
    [SerializeField] private TextMeshProUGUI _fieldText;
    [SerializeField] private TextMeshProUGUI _sizeText;

    [Header("Other Components")]
    [SerializeField] private Translator _translator;

    // Physical properties
    private float _electricPotential = 0;
    private float _electricField = 0;
    private double _specificPower = 0;

    // Constants
    private const float GRAVITY = 9.8f;
    private const float ELECTRIC_CONSTANT = 8.85f;
    private const float VISCOSITY = 1.8f;

    //Габариты
    private double length = 0;
    private double height = 0;
    private double width = 0;

    private double area = 0;

    //Расходники
    private double electro;
    private double consumables;
    private double ashFormation;


    private void Start()
    {
        InitializeInputs();
        var col = GameObject.FindGameObjectWithTag("Collec");

        _electroFilterAnimator = col.GetComponent<Animator>();
        if (_electroFilterAnimator != null)
        {
            Debug.LogError("Can not find electrofilter animator on scene");
        }
    }

    private void InitializeInputs()
    {
        _densityInput.text = "0,1 мА/см²";
        _speedInput.text = "2 м/с";
        _radiusInput.text = "0,5 мм";
        _chargeInput.text = "3 Кл";
    }

    private void Update()
    {
        CalculatePhysics();
        UpdateUI();
        UpdateVisualEffects();
    }

    private void CalculatePhysics()
    {
        float density = ParseInputValue(_densityInput.text);
        float speed = ParseInputValue(_speedInput.text);
        float radius = ParseInputValue(_radiusInput.text);
        float charge = ParseInputValue(_chargeInput.text);

        // Electric calculations
        _electricPotential = -density / -ELECTRIC_CONSTANT;
        _electricField = -1 * _electricPotential;


        string inputTemperature = _temperature.text;
        string numberTemperature = inputTemperature.Split(' ')[0];
        double valueTemperature  = double.Parse(numberTemperature);

        string inputSolidParticle = _solidParticle.text;
        string numberSolidParticle = inputSolidParticle.Split(' ')[0];
        double valueSolidParticle = double.Parse(numberSolidParticle);


        _specificPower = 35.7 * Math.Exp(0.015 * (((valueTemperature + (valueTemperature - 0.5 * length * 4)) / 2.0) - 150.0)) * Math.Log(1.0 / (1.0 - 0.7));

        string inputGasFlow = _gasFlow.text;
        string numberGasFlow = inputGasFlow.Split(' ')[0];
        double valueGasFlow = double.Parse(numberGasFlow);

        area = (Math.Log(1 / (1 - 0.7)) / 0.1 / 3600) * valueGasFlow;

        height = Math.Ceiling(Math.Sqrt(area / 6.0));
        length = Math.Ceiling(area / 4.0f / height);
        width = Math.Ceiling(valueGasFlow / 3600.0 / 1.2 / height);

        double countElectro = (valueGasFlow * _specificPower) / 1000000;
        electro = countElectro * 38.85 * 24;
        consumables = 0.7058 * valueGasFlow / 365;
        ashFormation = valueGasFlow * (valueSolidParticle - (1 - valueSolidParticle)) / 1000000000 * 24;
    }

    private float ParseInputValue(string inputText)
    {
        string numericPart = inputText.Substring(0, inputText.IndexOf(' '));
        numericPart = numericPart.Replace(',', '.'); 
        return float.Parse(numericPart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
    }

    private void UpdateUI()
    {
        switch (_translator.currentLanguage)
        {
            case Translator.Language.Russian:
                _potentialText.text = $"Потенциал: \n\t\t{_electricPotential:0.000} Дж/Кл";
                _fieldText.text = $"Электрическое поле: \n\t\t{_electricField:0.000} Н/Кл";
                _sizeText.text = $"Длина ЭФ: {length:0.0} м \n" +
                                    $"Ширина ЭФ: {width:0.0} м \n" +
                                        $"Высота ЭФ: {height:0.0} м \n" +
                                         $"Расходники ЭФ: {electro + consumables: 0.0} тг";
                break;
            case Translator.Language.Kazakh:
                _potentialText.text = $"Потенциал: {_electricPotential:0.000} Дж/Кл";
                _fieldText.text = $"Электр өрісі: {_electricField:0.000} Н/Кл";
                _sizeText.text = $"ЭФ Ұзындығы: {length:0.0} м \n" +
                                    $"ЭФ Ені: {width:0.0} м \n" +
                                        $"ЭФ Биіктігі: {height:0.0} м";
                break;
            default:
                _potentialText.text = $"Potential: {_electricPotential:0.000} J/Kl";
                _fieldText.text = $"Electric field: {_electricField:0.000} N/Kl";
                _sizeText.text = $"EF Length: {length:0.0} m \n" +
                                    $"EF Width: {width:0.0} m \n" +
                                        $"EF Height: {height:0.0} m";
                break;
        }
    }


    private void UpdateVisualEffects()
    {
        UpdateFilterSpeed();
        UpdateSmokeOpacity();
    }

    private void UpdateFilterSpeed()
    {
        float speed = ParseInputValue(_speedInput.text);
        _electroFilterAnimator.speed = speed switch
        {
            0.5f => 0.7f,
            1.5f => 0.8f,
            _ => 1f
        };
    }

    private void UpdateSmokeOpacity()
    {
        float density = ParseInputValue(_densityInput.text);
        float targetAlpha = density switch
        {
            0.01f => 0.4f,
            0.05f => 0.7f,
            _ => 1.0f
        };

        foreach (var smoke in _smokeParticles)
        {
            var colorModule = smoke.colorOverLifetime;
            var gradient = colorModule.color.gradient;

            var newGradient = new Gradient();
            newGradient.SetKeys(
                gradient.colorKeys,
                new[] { new GradientAlphaKey(targetAlpha, 0f), new GradientAlphaKey(targetAlpha, 1f) }
            );

            colorModule.color = new ParticleSystem.MinMaxGradient(newGradient);
        }
    }
}