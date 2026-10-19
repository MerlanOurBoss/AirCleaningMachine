using System;
using Math_Module;
using TMPro;
using UnityEngine;

public class MathModulForSborCO2 : MathModuleBase
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

    [Header("Sbor Script")]
    public SborCO2Controller newSbor; // или SborCO2Controller, если уже мигрировали

    [Header("Fan Animation")]
    public Animator[] fansAnim;

    private string sorbentType;
    private float gasVolume;
    private float adsorptionTemp;
    private float desorptionTemp;

    private int _smokeLevel = -1;
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

    // Габариты
    private double length = 0;
    private double height = 0;

    // Расходники
    private double electro;
    private double sorbentConsumables;
    private double parDesorbentConsumption;

    protected override string BuildDirtySnapshot() =>
        $"{_sorbentTypeText.text}|{_gasVolumeText.text}|{_adsorptionTempText.text}|" +
        $"{_desorptionTempText.text}|{_fanSpeedSborText.text}|{_gasFlowMain.text}";

    protected override void Start()
    {
        _sorbentTypeText.text = "Аминокислотные";
        _gasVolumeText.text = "100 м³/ч";
        _adsorptionTempText.text = "50 °C";
        _desorptionTempText.text = "150 °C";
        _diametrSborText.text = "0,1 м";
        _fanSpeedSborText.text = "500 об/мин";

        base.Start();
    }

    protected override void OnLanguageChanged(Translator.Language lang)
    {
        switch (lang)
        {
            case Translator.Language.Russian:
                _sorbentTypeText.text = "Аминокислотные";
                _gasVolumeText.text = "100 м³/ч";
                _adsorptionTempText.text = "50 °C";
                _desorptionTempText.text = "150 °C";
                _diametrSborText.text = "0,1 м";
                _fanSpeedSborText.text = "500 об/мин";
                break;

            case Translator.Language.Kazakh:
                _sorbentTypeText.text = "Аминқышқылдары";
                _gasVolumeText.text = "100 м³/ч";
                _adsorptionTempText.text = "50 °C";
                _desorptionTempText.text = "150 °C";
                _diametrSborText.text = "0,1 м";
                _fanSpeedSborText.text = "500 айн/мин";
                break;

            case Translator.Language.English:
                _sorbentTypeText.text = "Amino acids";
                _gasVolumeText.text = "100 m³/h";
                _adsorptionTempText.text = "50 °C";
                _desorptionTempText.text = "150 °C";
                _diametrSborText.text = "0,1 m";
                _fanSpeedSborText.text = "500 rpm";
                break;
        }
    }

    protected override void Recalculate()
    {
        UpdateSorbentProperties();

        // По числу оборотов, а не по строке "об/мин" — работает и для "айн/мин", и для "rpm".
        if (NumberEquals(_fanSpeedSborText.text, 500f)) SetFansSpeed(1);
        else if (NumberEquals(_fanSpeedSborText.text, 1000f)) SetFansSpeed(3);
        else if (NumberEquals(_fanSpeedSborText.text, 1500f)) SetFansSpeed(5);

        float adsorptionTime = CalculateAdsorptionTime();
        float desorbedCO2 = CalculateDesorption();

        electro = (sorbentMass * (desorptionTemp - adsorptionTemp) * (1.1 + 0.86) + sorbentMass * 0.962 * 75) / 0.8 * 1.05 / 3600 * 1.05;
        sorbentConsumables = sorbentMass / 1000 * (1 + 0.015 * 365) / 365 / 24 * 1000;
        parDesorbentConsumption = (sorbentMass * 1.1 * (desorptionTemp - adsorptionTemp) + massCO2 * 0.868 * (desorptionTemp - adsorptionTemp) + 129 * massCO2 / 0.044) * 1.1 / (2100 + 4.184 * (170 - desorptionTemp)) / 24;

        UpdateDisplayTexts(adsorptionTime, desorbedCO2);
        ApplySmokeLevelForGasVolume();
    }

    // fansAnim назначает GeneralManagerForSborCO2; до этого массив может быть пустым.
    private void SetFansSpeed(float speed)
    {
        if (fansAnim == null) return;
        foreach (var fan in fansAnim)
            if (fan != null) fan.speed = speed;
    }

    private void UpdateDisplayTexts(float adsorptionTime, float desorbedCO2)
    {
        double consumablesTotal = (electro * 38.85) + (sorbentConsumables * 535 * 193) + (parDesorbentConsumption * 71.56 / 1000);

        if (translator.currentLanguage == Translator.Language.Russian)
        {
            _adsorptionTimeText.text = $"Время адсорбции:\n           {adsorptionTime:0.00} ч";
            _desorptionTimeText.text = $"Время десорбции:\n           {desorptionTime:0.00} ч";
            _capturedCO2Text.text = $"Захваченный CO2:\n           {capturedCO2:0.00} моль";
            _co2FlowRate.text = $"Скорость потока CO2:\n           {co2FlowRate} кмоль/ч";
            _totalCapacity.text = $"Общая производительность:\n           {totalCapacity:0.00} кмоль";
            _massCO2.text = $"Масса CO2:\n           {massCO2:0.00} кг";
            _effectiveFlowRate.text = $"Эффек. скорость потока:\n           {effectiveFlowRate:0.000} моль/ч";
            _sizeText.text = $"Длина аппарата: {length:0.0} м \n" +
                                $"Высота аппарата: {height:0.0} м \n " +
                                         $"Расходники: {consumablesTotal: 0.0} тг";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            _adsorptionTimeText.text = $"Адсорбция уақыты:\n           {adsorptionTime:0.00} cағ";
            _desorptionTimeText.text = $"Десорбция уақыты:\n           {desorptionTime:0.00} cағ";
            _capturedCO2Text.text = $"Ұсталған CO2:\n           {capturedCO2:0.00} моль";
            _co2FlowRate.text = $"Ағын жылдамдығы CO2:\n           {co2FlowRate} кмоль/cағ";
            _totalCapacity.text = $"Жалпы өнімділік:\n           {totalCapacity:0.00} кмоль";
            _massCO2.text = $"CO2 Салмағы:\n           {massCO2:0.00} кг";
            _effectiveFlowRate.text = $"Тиімді ағын жылдамдығы:\n           {effectiveFlowRate:0.000} моль/cағ";
            _sizeText.text = $"Құрылғының ұзындығы: {length:0.0} м \n" +
                    $"Құрылғының биіктігі: {height:0.0} м \n" +
                        $"Шығын материалдар: {consumablesTotal: 0.0} тг";
        }
        else
        {
            _adsorptionTimeText.text = $"Adsorption time:\n           {adsorptionTime:0.00} h";
            _desorptionTimeText.text = $"Desorption time:\n           {desorptionTime:0.00} h";
            _capturedCO2Text.text = $"Captured CO2:\n           {capturedCO2:0.00} mol";
            _co2FlowRate.text = $"Flow rate CO2:\n           {co2FlowRate} kmol/h";
            _totalCapacity.text = $"Overall performance:\n           {totalCapacity:0.00} kmol";
            _massCO2.text = $"CO2 Mass:\n           {massCO2:0.00} kg";
            _effectiveFlowRate.text = $"Effective flow rate:\n           {effectiveFlowRate:0.000} mol/h";
            _sizeText.text = $"Length of device: {length:0.0} m \n" +
                    $"Height of device: {height:0.0} m \n" +
                        $"Consumables: {consumablesTotal: 0.0} tg";
        }
    }

    private void ApplySmokeLevelForGasVolume()
    {
        // По числу, а не по строке "м³/ч" — работает и для "м³/сағ", и для "m³/h".
        if (NumberEquals(_gasVolumeText.text, 150f) && _smokeLevel != 0)
        {
            a++;
            //newSbor.timingDelay = 170f;
            ApplySmokeSpeedAndAlpha(0.2f - (0.4f * b), 0.7f);
            _smokeLevel = 0;
            b = 0;
        }
        else if (NumberEquals(_gasVolumeText.text, 100f) && _smokeLevel != 1)
        {
            //newSbor.timingDelay = 150f;
            ApplySmokeSpeedAndAlpha(-(0.2f * a) - (0.4f * b), 0.3f);
            _smokeLevel = 1;
            a = 0;
            b = 0;
        }
        else if (NumberEquals(_gasVolumeText.text, 200f) && _smokeLevel != 2)
        {
            b++;
            //newSbor.timingDelay = 200f;
            ApplySmokeSpeedAndAlpha(0.4f - (0.2f * a), 1.0f);
            _smokeLevel = 2;
            a = 0;
        }
    }

    private void ApplySmokeSpeedAndAlpha(float speedDelta, float alpha)
    {
        foreach (ParticleSystem smoke in _smokes)
        {
            if (smoke == null) continue;
            smoke.startSpeed += speedDelta;
            ParticleFxUtility.SetOverLifetimeAlpha(smoke, alpha);
        }
    }

    // Названия сорбентов на русском, казахском и английском (префаб New Sbor).
    private static readonly string[] ZeoliteNames = { "Цеолитовые", "Zeolite" };
    private static readonly string[] LithiumZirconateNames = { "Цирконат лития", "Литий цирконаты", "Lithium zirconate" };
    private static readonly string[] AminoAcidNames = { "Аминокислотные", "Аминқышқылдары", "Amino acids" };

    private void UpdateSorbentProperties()
    {
        sorbentType = _sorbentTypeText.text;

        // Раньше сравнивались только русские названия: на казахском и английском
        // сорбент всегда уходил в значения по умолчанию, и цифры менялись при смене языка.
        if (MatchesAny(sorbentType, ZeoliteNames))
        {
            sorbentCapacity = 0.7f;
            sorbentEfficiency = 0.9f;
        }
        else if (MatchesAny(sorbentType, LithiumZirconateNames))
        {
            sorbentCapacity = 1f;
            sorbentEfficiency = 0.85f;
        }
        else if (MatchesAny(sorbentType, AminoAcidNames))
        {
            sorbentCapacity = 0.92f;
            sorbentEfficiency = 0.92f;
        }
        else
        {
            sorbentCapacity = 0.8f;
            sorbentEfficiency = 0.8f;
        }
    }

    private float CalculateAdsorptionTime()
    {
        double valueGasFlow = ParseLeadingNumber(_gasFlowMain.text);

        massCO2 = 44 * (CO2Concentration / 100) * ((float)valueGasFlow / 22.4f) * 0.7f;
        sorbentMass = (massCO2 / sorbentCapacity) * 1.2f;
        gasVolume = ParseLeadingNumber(_gasVolumeText.text);
        adsorptionTemp = ParseLeadingNumber(_adsorptionTempText.text);
        co2FlowRate = ((float)valueGasFlow * CO2Concentration / 100) * sorbentCapacity / oneMolarVolume;
        totalCapacity = sorbentMass * (sorbentCapacity / 1000) * 4;
        capturedCO2 = massCO2 * sorbentCapacity / 44;
        effectiveFlowRate = co2FlowRate * sorbentEfficiency / sorbentCapacity;

        length = Math.Ceiling(Math.Sqrt((4.0 * (valueGasFlow / 2.0)) / Math.PI / 3600.0 / 0.5));
        height = Math.Ceiling(sorbentMass / (1.0 - 0.37) / 670.0 / (valueGasFlow / 3600.0 / 0.5) + 1.5 * 2.0);

        return totalCapacity > 0f ? capturedCO2 / totalCapacity : 0f;
    }

    private float CalculateDesorption()
    {
        desorptionTemp = ParseLeadingNumber(_desorptionTempText.text);
        float desorptionEfficiency = Mathf.Clamp((desorptionTemp - 100f) / 30f, 0f, 1f);
        float desorbedCO2 = capturedCO2 * desorptionEfficiency;
        desorptionTime = capturedCO2 / Mathf.Max(desorbedCO2, 0.001f);
        return desorbedCO2;
    }
}