using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathModuleForEmul : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private DropSpawner[] _drops;
    [SerializeField] private TMP_InputField _gasFlow;
    [SerializeField] private TMP_InputField _waterFlow;
    [SerializeField] private TMP_InputField _fluidType;
    [SerializeField] private string fluid;

    [SerializeField] private TextMeshProUGUI gasSpeed;
    [SerializeField] private TextMeshProUGUI waterSpeed;
    [SerializeField] private TextMeshProUGUI gasMassFlow;
    [SerializeField] private TextMeshProUGUI waterMassFlow;
    [SerializeField] private TextMeshProUGUI massTransfer;

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
        _gasFlow.text = "14 м³/с";
        _waterFlow.text = "0,1 м³/с";
        _fluidType.text = fluid;
    }

    [System.Obsolete]
    void Update()
    {
        gasSpeed.text = "Скорость газа: " + _gasSpeed.ToString("0.000") + " м/с";
        waterSpeed.text = "Скорость воды: " + _waterSpeed.ToString("0.000") + " м/с";
        gasMassFlow.text = "Массовый поток газа: " + _gasMassFlow.ToString("0.000") + " м³/с";
        waterMassFlow.text = "Массовый поток жидкости: " + _waterMassFlow.ToString("0.000") + " м³/с";
        massTransfer.text = "Коэф. массового переноса: " + _massTransfer.ToString("0.0") + " м/с";

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
            Debug.Log("0.7");
        }
        else if (_gasFlow.text == "1,5 м/с")
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
            Debug.Log("0.9");
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
            Debug.Log("1");
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
