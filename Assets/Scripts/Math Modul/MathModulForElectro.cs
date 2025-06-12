using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathModuleForElectro : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem[] _smokeParticles;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField _densityInput;
    [SerializeField] private TMP_InputField _speedInput;
    [SerializeField] private TMP_InputField _radiusInput;
    [SerializeField] private TMP_InputField _chargeInput;

    [Header("Visual Effects")]
    [SerializeField] private Animator _electroFilterAnimator;

    [Header("Output Text Fields")]
    [SerializeField] private TextMeshProUGUI _massFlowText;
    [SerializeField] private TextMeshProUGUI _potentialText;
    [SerializeField] private TextMeshProUGUI _fieldText;
    [SerializeField] private TextMeshProUGUI _accelerationText;

    [Header("Other Components")]
    [SerializeField] private Translator _translator;

    // Physical properties
    private float _massFlow = 0;
    private float _electricPotential = 0;
    private float _electricField = 0;
    private float _particleAcceleration = 0;

    // Constants
    private const float GRAVITY = 9.8f;
    private const float ELECTRIC_CONSTANT = 8.85f;
    private const float VISCOSITY = 1.8f;

    private void Start()
    {
        InitializeInputs();
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

        // Mass flow calculation
        _massFlow = density * speed * 0.05f;

        // Electric calculations
        _electricPotential = -density / -ELECTRIC_CONSTANT;
        _electricField = -1 * _electricPotential;

        // Force calculations
        float dragForce = -6 * Mathf.PI * VISCOSITY * radius * speed;
        float gravityForce = _massFlow * GRAVITY;
        float electricForce = charge * _electricField;

        // Acceleration calculation
        _particleAcceleration = (1 / _massFlow) * (dragForce * gravityForce * electricForce);
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
                _massFlowText.text = $"Массовый поток газа: \n\t\t{_massFlow:0.000} кг/с";
                _potentialText.text = $"Потенциал: \n\t\t{_electricPotential:0.000} Дж/Кл";
                _fieldText.text = $"Электрическое поле: \n\t\t{_electricField:0.000} Н/Кл";
                _accelerationText.text = $"Ускорение частиц: \n\t\t{_particleAcceleration:0.000} м/с²";
                break;
            case Translator.Language.Kazakh:
                _massFlowText.text = $"Газдың массалық ағыны: {_massFlow:0.000} кг/с";
                _potentialText.text = $"Потенциал: {_electricPotential:0.000} Дж/Кл";
                _fieldText.text = $"Электр өрісі: {_electricField:0.000} Н/Кл";
                _accelerationText.text = $"Бөлшектердің үдеуі: {_particleAcceleration:0.000} м/с²";
                break;
            default:
                _massFlowText.text = $"Mass flow of gas: {_massFlow:0.000} kg/s";
                _potentialText.text = $"Potential: {_electricPotential:0.000} J/Kl";
                _fieldText.text = $"Electric field: {_electricField:0.000} N/Kl";
                _accelerationText.text = $"Particle acceleration: {_particleAcceleration:0.000} m/s²";
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