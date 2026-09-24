using System;
using Math_Module;
using TMPro;
using UnityEngine;

public class MathModuleForEmul : MathModuleBase
{
    public ParticleSystem[] _mySmokes;
    [SerializeField] private TMP_InputField _temperature;
    [SerializeField] private TMP_InputField _gasFlow;
    [SerializeField] private TMP_InputField _waterFlow;
    public TMP_InputField _fluidType;
    public TMP_InputField _gasFlowMain;
    public string fluid;

    [SerializeField] private TextMeshProUGUI gasSpeed;
    [SerializeField] private TextMeshProUGUI waterSpeed;
    [SerializeField] private TextMeshProUGUI gasMassFlow;
    [SerializeField] private TextMeshProUGUI waterMassFlow;
    [SerializeField] private TextMeshProUGUI massTransfer;
    [SerializeField] private TextMeshProUGUI gasСonsumption;
    [SerializeField] private TextMeshProUGUI sizeText;

    private float _gasSpeed = 0;
    private float _waterSpeed = 0;
    private float _gasMassFlow = 0;
    private float _waterMassFlow = 0;
    private float _reynoldsNumber = 0;
    private float _massTransfer = 0;
    public float _gasСonsumption = 0;
    public float сonsumption = 0;

    private readonly float empiricalConstantsA = 0.5f;
    private readonly float empiricalConstantsB = 0.8f;

    private readonly float deametrDroplet = 1;
    private readonly float waterDensity = 1000f;
    private readonly float causticSodaDensity = 1100f;
    private readonly float sodaDensity = 1050f;

    private readonly float waterDynamicViscosity = 0.001f;
    private readonly float causticSodaDynamicViscosity = 0.0012f;
    private readonly float sodaDynamicViscosity = 0.0013f;

    // Габариты
    private float deametr = 0;
    private double height = 0;

    // Расходники
    private double electro;
    private double waterConsumables;
    private double dryReagentConsumptionCount;
    private double dryReagentConsumption;
    private double otherConsumables;

    protected override string BuildDirtySnapshot() =>
        $"{_fluidType.text}|{_gasFlow.text}|{_waterFlow.text}|{_gasFlowMain.text}|{_temperature.text}";

    protected override void OnLanguageChanged(Translator.Language lang)
    {
        switch (lang)
        {
            case Translator.Language.Russian:
                _temperature.text = "15 °C";
                _gasFlow.text = "14 м³/с";
                _waterFlow.text = "0,1 м³/с";
                _fluidType.text = fluid;
                if (_fluidType.text == "Water" || _fluidType.text == "Су" || _fluidType.text == "Вода")
                {
                    _fluidType.text = "Вода";
                    fluid = "Вода";
                }
                else if (_fluidType.text == "Caustic soda" || _fluidType.text == "Каустикалық сода" || _fluidType.text == "Едкий натрий")
                {
                    _fluidType.text = "Едкий натрий";
                    fluid = "Едкий натрий";
                }
                else if (_fluidType.text == "Soda" || _fluidType.text == "Сода")
                {
                    _fluidType.text = "Сода";
                    fluid = "Сода";
                }
                break;

            case Translator.Language.Kazakh:
                _temperature.text = "15 °C";
                _gasFlow.text = "14 м³/с";
                _waterFlow.text = "0,1 м³/с";
                _fluidType.text = fluid;
                if (_fluidType.text == "Water" || _fluidType.text == "Су" || _fluidType.text == "Вода")
                {
                    _fluidType.text = "Су";
                    fluid = "Су";
                }
                else if (_fluidType.text == "Caustic soda" || _fluidType.text == "Каустикалық сода" || _fluidType.text == "Едкий натрий")
                {
                    _fluidType.text = "Каустикалық сода";
                    fluid = "Каустикалық сода";
                }
                else if (_fluidType.text == "Soda" || _fluidType.text == "Сода")
                {
                    _fluidType.text = "Сода";
                    fluid = "Сода";
                }
                break;

            case Translator.Language.English:
                _temperature.text = "15 °C";
                _gasFlow.text = "14 m³/s";
                _waterFlow.text = "0,1 m³/s";
                _fluidType.text = fluid;
                if (_fluidType.text == "Water" || _fluidType.text == "Су" || _fluidType.text == "Вода")
                {
                    _fluidType.text = "Water";
                    fluid = "Water";
                }
                else if (_fluidType.text == "Caustic soda" || _fluidType.text == "Каустикалық сода" || _fluidType.text == "Едкий натрий")
                {
                    _fluidType.text = "Caustic soda";
                    fluid = "Caustic soda";
                }
                else if (_fluidType.text == "Soda" || _fluidType.text == "Сода")
                {
                    _fluidType.text = "Soda";
                    fluid = "Soda";
                }
                break;
        }
    }

    protected override void Recalculate()
    {
        fluid = _fluidType.text;
        UpdateDisplayTexts();
        UpdateSmokeAlphaForGasFlow();

        if (_fluidType.text == "Вода" || _fluidType.text == "Water" || _fluidType.text == "Су")
        {
            _reynoldsNumber = (waterDensity * _waterSpeed * deametrDroplet) / waterDynamicViscosity;
        }
        else if (_fluidType.text == "Едкий натрий" || _fluidType.text == "Caustic soda" || _fluidType.text == "Каустикалық сода")
        {
            _reynoldsNumber = (causticSodaDensity * _waterSpeed * deametrDroplet) / causticSodaDynamicViscosity;
        }
        else
        {
            _reynoldsNumber = (sodaDensity * _waterSpeed * deametrDroplet) / sodaDynamicViscosity;
        }

        _gasSpeed = (4 * ParseLeadingNumber(_gasFlow.text)) / (3.14159f * Mathf.Pow(deametr, 2));
        _waterSpeed = (4 * ParseLeadingNumber(_waterFlow.text)) / (3.14159f * Mathf.Pow(deametr, 2));
        _gasMassFlow = (_gasSpeed * 3.14159f * Mathf.Pow(deametr, 2)) / 4;
        _waterMassFlow = (_waterSpeed * 3.14159f * Mathf.Pow(deametr, 2)) / 4;
        _massTransfer = empiricalConstantsA * Mathf.Pow((_reynoldsNumber / deametrDroplet), empiricalConstantsB);

        double valueGasFlow = ParseLeadingNumber(_gasFlowMain.text);

        сonsumption = (float)valueGasFlow / 3600;
        _gasСonsumption = (0.22f * сonsumption) / 1000;

        deametr = (float)Math.Ceiling(Math.Sqrt((4.0 * valueGasFlow) / Math.PI / 3600.0 / 2.9));
        height = Math.Ceiling(((0.8 + 18 * 0.15 + 0.35 * deametr + 0.6) / 5.0) * 10.0) / 10.0 * 5.0;

        electro = 0.12 * (_gasСonsumption * 3600);
        waterConsumables = 0.005 * (valueGasFlow * 1000);
        dryReagentConsumptionCount = (10 / 100.0) * (1100 / 1000.0) * (valueGasFlow / 1000.0) / 0.95 / 1000;

        float fluidCon;
        if (fluid == "Едкий натрий" || fluid == "Каустикалық сода" || fluid == "Caustic soda")
        {
            fluidCon = 200000;
        }
        else if (fluid == "Сода" || fluid == "Soda")
        {
            fluidCon = 125000;
        }
        else
        {
            fluidCon = 1;
        }

        dryReagentConsumption = dryReagentConsumptionCount * fluidCon;
        otherConsumables = (8090 + 0.036 * valueGasFlow) / 365 / 24;
    }

    private void UpdateDisplayTexts()
    {
        double consumablesTotal = (electro * 38.85) + (waterConsumables * 59.84) + dryReagentConsumption + otherConsumables;

        if (translator.currentLanguage == Translator.Language.Russian)
        {
            gasSpeed.text = "Скорость газа: \n\t\t   " + _gasSpeed.ToString("0.000") + " м/с";
            waterSpeed.text = "Скорость воды: \n\t\t   " + _waterSpeed.ToString("0.000") + " м/с";
            gasMassFlow.text = "Массовый поток газа: \n\t\t   " + _gasMassFlow.ToString("0.000") + " м³/с";
            waterMassFlow.text = "Массовый поток жидкости: \n\t\t   " + _waterMassFlow.ToString("0.000") + " м³/с";
            massTransfer.text = "Коэф. массового переноса: \n\t\t   " + _massTransfer.ToString("0.0") + " м/с";
            gasСonsumption.text = "Расход жидкости: \n\t\t   " + _gasСonsumption.ToString("0.000") + " м³/с";
            sizeText.text = $"Диаметр аппарата: {deametr:0.0} м \n" +
                        $"Высота аппарата: {height:0.0} м \n" +
                         $"Расходники: {consumablesTotal: 0.0} тг";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            gasSpeed.text = "Газ жылдамдығы: \n\t\t   " + _gasSpeed.ToString("0.000") + " м/с";
            waterSpeed.text = "Су жылдамдығы: \n\t\t   " + _waterSpeed.ToString("0.000") + " м/с";
            gasMassFlow.text = "Газ массасы ағыны: \n\t\t   " + _gasMassFlow.ToString("0.000") + " м³/с";
            waterMassFlow.text = "Сұйық масса ағыны: \n\t\t   " + _waterMassFlow.ToString("0.000") + " м³/с";
            massTransfer.text = "Масса тасымалдау коэфф.: \n\t\t   " + _massTransfer.ToString("0.0") + " м/с";
            gasСonsumption.text = "Сұйықтықты тұтыну: \n\t\t   " + _gasСonsumption.ToString("0.000") + " м³/с";
            sizeText.text = $"Құрылғының диаметрі: {deametr:0.0} м \n" +
                                $"Құрылғының биіктігі: {height:0.0} м \n" +
                                    $"Шығын материалдар: {consumablesTotal: 0.0} тг";
        }
        else
        {
            gasSpeed.text = "Gas Speed: \n\t\t   " + _gasSpeed.ToString("0.000") + " m/s";
            waterSpeed.text = "Water Speed: \n\t\t   " + _waterSpeed.ToString("0.000") + " m/s";
            gasMassFlow.text = "Gas Mass Flow: \n\t\t   " + _gasMassFlow.ToString("0.000") + " m³/s";
            waterMassFlow.text = "Liquid mass flow: \n\t\t   " + _waterMassFlow.ToString("0.000") + " m³/s";
            massTransfer.text = "Mass transfer coefficient: \n\t\t   " + _massTransfer.ToString("0.0") + " m/s";
            gasСonsumption.text = "Gas Сonsumption: \n\t\t   " + _gasСonsumption.ToString("0.000") + " м³/с";
            sizeText.text = $"Diameter of device: {deametr:0.0} m \n" +
                                $"Height of device: {height:0.0} m \n" +
                                    $"Consumables: {consumablesTotal: 0.0} tg";
        }
    }

    private void UpdateSmokeAlphaForGasFlow()
    {
        float alpha = _gasFlow.text switch
        {
            "10 м³/с" => 0.3f,
            "12 м³/с" => 0.7f,
            _ => 1.0f
        };

        ParticleFxUtility.SetOverLifetimeAlpha(_mySmokes, alpha);
    }
}