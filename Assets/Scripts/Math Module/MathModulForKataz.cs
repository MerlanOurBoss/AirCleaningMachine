using System;
using Math_Module;
using TMPro;
using UnityEngine;

public class MathModulForKataz : MathModuleBase
{
    public ParticleSystem[] _smokes;
    [SerializeField] private TMP_InputField _temperatureText;
    [SerializeField] private TMP_InputField _pressureText;
    [SerializeField] private TMP_InputField _flowRateText;
    public TMP_InputField _katazBlockCount;
    public TMP_InputField _katazBlockType;
    public TMP_InputField _gasFlowMain;
    [SerializeField] private TMP_InputField _gasSource;
    public double valueGasFlow = 0;
    [SerializeField] private TextMeshProUGUI gasVelocity;
    [SerializeField] private TextMeshProUGUI gasDensitie;
    [SerializeField] private TextMeshProUGUI gasmassFlow;
    [SerializeField] private TextMeshProUGUI coGaz;
    [SerializeField] private TextMeshProUGUI _sizeText;

    public double _tempBefore;
    public double _tempAfter;

    // Уровень эффекта дыма (0/1/2), соответствует старым isProcessed/isProcessed1/isProcessed2.
    private int _smokeLevel = -1;
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

    // Расходники
    private double electro;
    private double electroAdditional;
    private double consumption;
    private double catalyzator;
    private double waterConsumables;
    private double reagentConsumables;

    protected override string BuildDirtySnapshot() =>
        $"{_temperatureText.text}|{_pressureText.text}|{_flowRateText.text}|{_gasFlowMain.text}|" +
        $"{_katazBlockCount.text}|{_katazBlockType.text}|{_gasSource.text}";

    protected override void OnLanguageChanged(Translator.Language lang)
    {
        switch (lang)
        {
            case Translator.Language.Russian:
                _katazBlockCount.text = "4";
                _katazBlockType.text = "с драгметаллами";
                _temperatureText.text = "25 °C";
                _pressureText.text = "101325 Па";
                _flowRateText.text = "1 м³/с";
                _gasSource.text = "Угольный";
                break;

            case Translator.Language.Kazakh:
                _katazBlockCount.text = "4";
                _katazBlockType.text = "с драгметаллами";
                _temperatureText.text = "25 °C";
                _pressureText.text = "101325 Па";
                _flowRateText.text = "1 м³/с";
                _gasSource.text = "Көміртек";
                break;

            case Translator.Language.English:
                _katazBlockCount.text = "4";
                _katazBlockType.text = "with precious metals";
                _temperatureText.text = "25 °C";
                _pressureText.text = "101325 Pa";
                _flowRateText.text = "1 m³/s";
                _gasSource.text = "Coal";
                break;
        }
    }

    protected override void Start()
    {
        base.Start();
        // onValueChanged в оригинале назначался ЗАНОВО при каждой смене языка
        // (внутри OnLanguageChanged) — это копило дублирующиеся подписки при
        // повторных сменах языка. Подписываемся один раз здесь.
        _katazBlockCount.onValueChanged.AddListener(ChangeBlockNumber);
    }

    protected override void Recalculate()
    {
        UpdateDisplayTexts();

        float crossSectionArea = (float)(Pi * Math.Pow(0.5 / 2, 2));

        CO_Gaz_Out = (float)(CO_Gaz_In * Math.Pow(Math.E, k_CO * 1 * (1 / velocity)));
        density = (ParseLeadingNumber(_pressureText.text) * molarMass) / (R * ParseLeadingNumber(_temperatureText.text));
        velocity = ParseLeadingNumber(_flowRateText.text) / crossSectionArea;
        massFlow = density * velocity * crossSectionArea;

        valueGasFlow = ParseLeadingNumber(_gasFlowMain.text);

        deametr = (float)Math.Ceiling(Math.Sqrt((4.0 * valueGasFlow) / Math.PI / 3600.0 / 2.9));

        double _catalyzBlock = ParseLeadingNumber(_katazBlockCount.text);

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

        catalyzator = 28.4 * (deametr * deametr) * _catalyzBlock;

        waterConsumables = 0.4 * 450 / 1000 / 34 * (valueGasFlow / 3600);
        reagentConsumables = 135 / 1000 / 34 * (valueGasFlow / 3600);

        electroAdditional = Mathf.Ceil((float)(0.12f * (450f / 1000f) / 5f / 34f * (valueGasFlow / 3600) * 10f)) / 10f * 5f;

        if (_katazBlockType.text == "с драгметаллами")
        {
            catalyzator = catalyzator * 25076 * 2 / 365 / 24;
        }
        else if (_katazBlockType.text == "без драгметаллов")
        {
            catalyzator = (catalyzator * 25076 + catalyzator * 0.2 * 4200) / 365 / 24;
        }

        ApplySmokeLevelForFlowRate();
    }

    private void UpdateDisplayTexts()
    {
        double consumablesTotal = ((electro + electroAdditional) * 38.85) + (waterConsumables * 59.84) + (reagentConsumables * 71.56) + catalyzator + consumption;

        if (translator.currentLanguage == Translator.Language.Russian)
        {
            gasVelocity.text = "Скорость газа: \n\t\t   " + velocity.ToString("0.000") + " м/с";
            gasDensitie.text = "Плотность газа: \n\t\t   " + density.ToString("0.000") + " кг/м³";
            gasmassFlow.text = "Массовый расход газа:: \n\t\t   " + massFlow.ToString("0.000") + " кг/с";
            coGaz.text = "Вых. концентрация CO: \n\t\t   " + CO_Gaz_Out.ToString("0.000") + " моль/м³";
            _sizeText.text = $"Диаметр блока: {deametr:0.0} м \n" +
                                $"Расходники: {consumablesTotal: 0.0} тг";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            gasVelocity.text = "Газдың жылдамдығы: \n\t\t   " + velocity.ToString("0.000") + " м/с";
            gasDensitie.text = "Газдың тығыздығы: \n\t\t   " + density.ToString("0.000") + " кг/м³";
            gasmassFlow.text = "Газдың массалық шығыны: \n\t\t   " + massFlow.ToString("0.000") + " кг/с";
            coGaz.text = "Шығар. СО концентрациясы: \n\t\t   " + CO_Gaz_Out.ToString("0.000") + " моль/м³";
            _sizeText.text = $"Блок диаметрі: {deametr:0.0} м \n" +
                             $"Шығын материалдар: {consumablesTotal: 0.0} тг";
        }
        else
        {
            gasVelocity.text = "Gas velocity: \n\t\t   " + velocity.ToString("0.000") + " m/s";
            gasDensitie.text = "Gas Density: \n\t\t   " + density.ToString("0.000") + " kg/m³";
            gasmassFlow.text = "Mass gas consumption: \n\t\t   " + massFlow.ToString("0.000") + " kg/s";
            coGaz.text = "CO output concentration: \n\t\t   " + CO_Gaz_Out.ToString("0.000") + " mol/m³";
            _sizeText.text = $"Block diameter: {deametr:0.0} m \n" +
                             $"Consumables: {consumablesTotal: 0.0} тг";
        }
    }

    /// <summary>
    /// Было три идентичных блока (isProcessed/isProcessed1/isProcessed2 + a/b).
    /// ВНИМАНИЕ: сравнение по полной локализованной строке ("1,5 м³/с") сохранено
    /// как в оригинале — принципиально стоило бы сравнивать по числу
    /// (см. ParseLeadingNumber), но замена изменила бы то, при каких именно
    /// строках срабатывает эффект, поэтому оставлено как есть, а не "исправлено"
    /// самовольно. Аддитивный дрейф startSpeed (+0.2f - 0.4f*b) — тоже из
    /// оригинала, не идемпотентен при частом переключении пресетов; похоже на
    /// баг дизайна, но здесь не трогаю — это к тому, кто настраивал эффект.
    /// </summary>
    private void ApplySmokeLevelForFlowRate()
    {
        if (_flowRateText.text == "1,5 м³/с" && _smokeLevel != 0)
        {
            a++;
            ApplySmokeSpeedAndAlpha(0.2f - (0.4f * b), 0.7f);
            _smokeLevel = 0;
            b = 0;
        }
        else if (_flowRateText.text == "1 м³/с" && _smokeLevel != 1)
        {
            ApplySmokeSpeedAndAlpha(-(0.2f * a) - (0.4f * b), 0.3f);
            _smokeLevel = 1;
            a = 0;
            b = 0;
        }
        else if (_flowRateText.text == "2 м³/с" && _smokeLevel != 2)
        {
            b++;
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

    public void ChangeBlockNumber(string text)
    {
        KatazBlockCountManager[] managers = FindObjectsOfType<KatazBlockCountManager>();
        foreach (KatazBlockCountManager manager in managers)
        {
            manager.ChangeBlocks(int.Parse(text));
        }
    }
}