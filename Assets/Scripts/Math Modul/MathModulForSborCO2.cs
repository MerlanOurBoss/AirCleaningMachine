using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MathModulForSborCO2 : MonoBehaviour
{
    public ParticleSystem[] _smokes;
    [SerializeField] private TMP_InputField _sorbentTypeText;
    [SerializeField] private TMP_InputField _gasVolumeText;
    [SerializeField] private TMP_InputField _adsorptionTempText;
    [SerializeField] private TMP_InputField _desorptionTempText;
    [SerializeField] private TMP_InputField _diametrSborText;
    [SerializeField] private TMP_InputField _fanSpeedSborText;
    public TMP_InputField _gasFlowMain;

    [SerializeField] private TextMeshProUGUI _adsorptionTimeText;
    [SerializeField] private TextMeshProUGUI _desorptionTimeText;
    [SerializeField] private TextMeshProUGUI _capturedCO2Text;
    [SerializeField] private TextMeshProUGUI _desorbedCO2Text;
    [SerializeField] private TextMeshProUGUI _effectiveFlowRate;
    [SerializeField] private TextMeshProUGUI _massCO2;
    [SerializeField] private TextMeshProUGUI _totalCapacity;
    [SerializeField] private TextMeshProUGUI _co2FlowRate;
    [SerializeField] private TextMeshProUGUI _sizeText;
    [SerializeField] private Translator translator;

    [Header("Sbor Script")]
    public NewSborScript newSbor;

    [Header("Fan Animation")]
    public Animator[] fansAnim;

    private string sorbentType;
    private float gasVolume;
    private float adsorptionTemp;
    private float desorptionTemp;
    private bool isProcessed = false;
    private bool isProcessed1 = false;
    private bool isProcessed2 = false;
    private int a = 0;
    private int b = 0;

    private float massCO2;
    private float sorbentCapacity; 
    private float sorbentEfficiency; 
    public float capturedCO2; 
    public float co2FlowRate; 
    private float totalCapacity; 
    private float effectiveFlowRate; 
    private float sorbentMass; 
    private float desorptionTime = 0;

    private const float CO2Concentration = 9f; 
    private const float molarMassCO2 = 44f; 
    private const float oneMolarVolume = 22.41f;

    //Габариты
    private double length = 0;
    private double height = 0;

    //Расходники
    private double electro;
    private double sorbentConsumables;
    private double parDesorbentConsumption;
    
    private static int globalCounterSbor = 0;
    public int instanceID;
    
    private void Awake()
    {
        globalCounterSbor++;
        instanceID = globalCounterSbor;

        Debug.Log($"[SborCO2] Назначен instanceID = {instanceID} для {gameObject.name}");
    }
    private void Start()
    {
        UpdateSorbentProperties();
        _sorbentTypeText.text = "Аминокислотные";
        _gasVolumeText.text = "100 м³/ч";
        _adsorptionTempText.text = "50 °C";
        _desorptionTempText.text = "150 °C";
        _diametrSborText.text = "0,1 м";
        _fanSpeedSborText.text = "500 об/мин";
        
        var translate = GameObject.FindGameObjectWithTag("Translator");
        translator =  translate.GetComponent<Translator>();
    }

    [System.Obsolete]
    private void Update()
    {
        UpdateSorbentProperties();

        switch (_fanSpeedSborText.text)
        {
            case "500 об/мин":
                fansAnim[0].speed = 1;
                fansAnim[1].speed = 1;
                break;
            case "1000 об/мин":
                fansAnim[0].speed = 3;
                fansAnim[1].speed = 3;
                break;
            case "1500 об/мин":
                fansAnim[0].speed = 5;
                fansAnim[1].speed = 5;
                break;
        }

        float adsorptionTime = CalculateAdsorptionTime();
        float desorbedCO2 = CalculateDesorption();

        electro = (sorbentMass * (desorptionTemp - adsorptionTemp) * (1.1 + 0.86) + sorbentMass * 0.962 * 75) / 0.8 * 1.05 / 3600 * 1.05;
        sorbentConsumables = sorbentMass / 1000 * (1 + 0.015 * 365) / 365 / 24 * 1000;
        parDesorbentConsumption = (sorbentMass * 1.1 * (desorptionTemp - adsorptionTemp) + massCO2 * 0.868 * (desorptionTemp - adsorptionTemp) + 129 * massCO2 / 0.044) * 1.1 / (2100 + 4.184 * (170 - desorptionTemp)) / 24;

        if (translator.currentLanguage == Translator.Language.Russian)
        {
            _adsorptionTimeText.text = $"Время адсорбции:\n           {adsorptionTime:0.00} ч";
            _desorptionTimeText.text = $"Время десорбции:\n           {desorptionTime:0.00} ч";
            _capturedCO2Text.text = $"Захваченный CO2:\n           {capturedCO2:0.00} моль";
            _co2FlowRate.text = $"Скорость потока CO2:\n           {co2FlowRate} кмоль/ч";
            _totalCapacity.text = $"Общая производительность:\n           {totalCapacity:0.00} кмоль";
            _massCO2.text = $"Масса CO2:\n           {massCO2:0.00} кг";
            _effectiveFlowRate.text = $"Эффективная скорость потока:\n           {effectiveFlowRate} моль/ч";
            _sizeText.text = $"Длина аппарата: {length:0.0} м \n" +
                                $"Высота аппарата: {height:0.0} м \n " +
                                         $"Расходники: {(electro * 38.85) + (sorbentConsumables * 535 * 193) + (parDesorbentConsumption* 71.56/1000): 0.0} тг";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            _adsorptionTimeText.text = $"Адсорбция уақыты:\n           {adsorptionTime:0.00} cағ";
            _capturedCO2Text.text = $"Ұсталғаң CO2:\n           {capturedCO2:0.00} моль";
            _co2FlowRate.text = $"Ағын жылдамдығы CO2:\n           {co2FlowRate} кмоль/cағ";
            _totalCapacity.text = $"Жалпы өнімділік:\n           {totalCapacity:0.00} кмоль";
            _massCO2.text = $"CO2 Салмағы:\n           {massCO2:0.00} кг";
            _effectiveFlowRate.text = $"Тиімді ағын жылдамдығы:\n           {effectiveFlowRate} моль/cағ";
            _sizeText.text = $"Құрылғының ұзындығы: {length:0.0} м \n" +
                    $"Құрылғының биіктігі: {height:0.0} м"+
                        $"Шығын материалдар: {(electro * 38.85) + (sorbentConsumables * 535 * 193) + (parDesorbentConsumption* 71.56/1000): 0.0} тг";
        }
        else
        {
            _adsorptionTimeText.text = $"Adsorption time:\n           {adsorptionTime:0.00} h";
            _capturedCO2Text.text = $"Captured CO2:\n           {capturedCO2:0.00} mol";
            _co2FlowRate.text = $"Flow rate CO2:\n           {co2FlowRate} kmol/h";
            _totalCapacity.text = $"Overall performance:\n           {totalCapacity:0.00} kmol";
            _massCO2.text = $"CO2 Mass:\n           {massCO2:0.00} кг";
            _effectiveFlowRate.text = $"Effective flow rate:\n           {effectiveFlowRate} mol/h";
            _sizeText.text = $"Length of device: {length:0.0} m \n" +
                    $"Height of device: {height:0.0} m" +
                        $"Consumables: {(electro * 38.85) + (sorbentConsumables * 535 * 193) + (parDesorbentConsumption* 71.56/1000): 0.0} tg";
        }

        if (_gasVolumeText.text == "150 м³/ч" && !isProcessed)
        {
            a++;
            newSbor.timingDelay = 170f;
            foreach (ParticleSystem smoke in _smokes)
            {
                smoke.startSpeed = smoke.startSpeed + 0.2f - (0.4f * b);

                ParticleSystem.ColorOverLifetimeModule colorModul = smoke.colorOverLifetime;

                Gradient currentGradient = colorModul.color.gradient;

                Gradient newGradient = new Gradient();
                newGradient.SetKeys(
                    currentGradient.colorKeys,
                    new GradientAlphaKey[] {
                    new GradientAlphaKey(0.7f, 0.0f), 
                    new GradientAlphaKey(0.7f, 1.0f) 
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
            newSbor.timingDelay = 150f;
            foreach (ParticleSystem smoke in _smokes)
            {
                if (smoke == null)
                    continue;

                ParticleSystem.MainModule mainModule = smoke.main;
                mainModule.startSpeed = mainModule.startSpeed.constant - (0.2f * a) - (0.4f * b);

                ParticleSystem.ColorOverLifetimeModule colorModul = smoke.colorOverLifetime;

                Gradient currentGradient = colorModul.color.gradient;

                Gradient newGradient = new Gradient();
                newGradient.SetKeys(
                    currentGradient.colorKeys,
                    new GradientAlphaKey[] {
                    new GradientAlphaKey(0.3f, 0.0f), 
                    new GradientAlphaKey(0.3f, 1.0f) 
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
            newSbor.timingDelay = 200f;
            foreach (ParticleSystem smoke in _smokes)
            {
                smoke.startSpeed = smoke.startSpeed + 0.4f - (0.2f * a);

                ParticleSystem.ColorOverLifetimeModule colorModul = smoke.colorOverLifetime;

                Gradient currentGradient = colorModul.color.gradient;

                Gradient newGradient = new Gradient();
                newGradient.SetKeys(
                    currentGradient.colorKeys,
                    new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f), 
                    new GradientAlphaKey(1.0f, 1.0f) 
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
        sorbentType = _sorbentTypeText.text;
        switch (sorbentType.ToLower())
        {
            case "цеолитовые":
                sorbentCapacity = 0.7f;
                sorbentEfficiency = 0.9f;
                break;
            case "цирконат лития":
                sorbentCapacity = 1f;
                sorbentEfficiency = 0.85f;
                break;
            case "аминокислотные":
                sorbentCapacity = 0.92f;
                sorbentEfficiency = 0.92f;
                break;
            default:
                sorbentCapacity = 0.8f;
                sorbentEfficiency = 0.8f;
                break;
        }
    }

    private float CalculateAdsorptionTime()
    {
        string inputGasFlow = _gasFlowMain.text;
        string numberGasFlow = inputGasFlow.Split(' ')[0];
        double valueGasFlow = double.Parse(numberGasFlow);

        massCO2 = 44 * (CO2Concentration / 100) * ((float)valueGasFlow / 22.4f) * 0.7f;
        sorbentMass = (massCO2 / sorbentCapacity) * 1.2f;
        gasVolume = ParseInput(_gasVolumeText.text);
        adsorptionTemp = ParseInput(_adsorptionTempText.text);
        co2FlowRate = ((float)valueGasFlow * CO2Concentration / 100) / oneMolarVolume;
        totalCapacity = sorbentMass * (sorbentCapacity / 1000) * 4;
        capturedCO2 = massCO2 / 44;
        effectiveFlowRate = co2FlowRate * sorbentEfficiency;

        length = Math.Ceiling(
                    Math.Sqrt((4.0 * (valueGasFlow / 2.0)) / Math.PI / 3600.0 / 0.5)
                    );

        height = Math.Ceiling(
                    sorbentMass / (1.0 - 0.37) / 670.0 / (valueGasFlow / 3600.0 / 0.5) + 1.5 * 2.0
                    );

        return capturedCO2 / totalCapacity;
    }

    private float CalculateDesorption()
    {
        desorptionTemp = ParseInput(_desorptionTempText.text);
        float desorptionEfficiency = Mathf.Clamp((desorptionTemp - 100f) / 30f, 0f, 1f);
        float desorbedCO2 = capturedCO2 * desorptionEfficiency;
        desorptionTime = capturedCO2 / Mathf.Max(desorbedCO2, 0.001f);
        return desorbedCO2;
    }

    private float ParseInput(string input)
    {
        if (float.TryParse(input.Split(' ')[0], out float value))
            return value;
        return 0f;
    }
}
