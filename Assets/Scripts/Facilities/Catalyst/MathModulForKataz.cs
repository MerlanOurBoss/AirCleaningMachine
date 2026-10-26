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
    private const float BlockDiameter = 0.9f; // м — как в GeneralManagerForKataz

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

    // Варианты из выпадающих списков на русском, казахском и английском (префаб New Katalizator).
    private static readonly string[] WithPreciousMetals = { "с драгметаллами", "бағалы металдармен", "with precious metals" };
    private static readonly string[] WithoutPreciousMetals = { "без драгметаллов", "бағалы металдарсыз", "without precious metals" };
    private static readonly string[] CoalSource = { "Угольный", "Көміртек", "Coal" };
    private static readonly string[] GasSource = { "Газ", "Gas" };

    /// <summary>Тип блока «с драгметаллами» — на любом языке интерфейса.</summary>
    public bool HasPreciousMetals => MatchesAny(_katazBlockType.text, WithPreciousMetals);

    /// <summary>Тип блока «без драгметаллов» — на любом языке интерфейса.</summary>
    public bool HasNoPreciousMetals => MatchesAny(_katazBlockType.text, WithoutPreciousMetals);

    public int BlockCount => Mathf.RoundToInt(ParseLeadingNumber(_katazBlockCount.text));

    /// <summary>Выставить число блоков (как выбор в выпадающем списке): пересчёт и показ блоков — через onValueChanged.</summary>
    public void SetBlockCount(int blocks) => _katazBlockCount.text = blocks.ToString();

    /// <summary>Выставить тип блоков названием на текущем языке интерфейса.</summary>
    public void SetBlockType(bool preciousMetals)
    {
        var names = preciousMetals ? WithPreciousMetals : WithoutPreciousMetals;
        var lang = translator != null ? translator.currentLanguage : Translator.Language.Russian;
        _katazBlockType.text = lang == Translator.Language.Kazakh ? names[1]
                             : lang == Translator.Language.English ? names[2] : names[0];
    }

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
        float crossSectionArea = (float)(Pi * Math.Pow(0.5 / 2, 2));

        // Порядок исправлен: сначала скорость и плотность, потом зависящие от них величины.
        // Раньше CO_Gaz_Out считался по скорости с прошлого шага (в первый раз — делением на 0).
        // Уравнение состояния ρ = P·M / (R·T) — температура в кельвинах (в поле она в °C).
        // Раньше подставлялись °C: при 25 °C выходило 14 кг/м³ вместо 1.18, при 0 °C — деление на ноль.
        float temperatureK = ParseLeadingNumber(_temperatureText.text) + 273.15f;
        density = temperatureK > 0f ? (ParseLeadingNumber(_pressureText.text) * molarMass) / (R * temperatureK) : 0f;
        velocity = ParseLeadingNumber(_flowRateText.text) / crossSectionArea;
        massFlow = density * velocity * crossSectionArea;
        // Окисление CO на катализаторе: C_вых = C_вх · exp(−k·L/v), L = 1 м. Раньше знак в экспоненте
        // был «+», и на выходе CO получалось больше, чем на входе. Без потока газа концентрация не меняется.
        CO_Gaz_Out = velocity > 0f ? (float)(CO_Gaz_In * Math.Exp(-k_CO * 1 / velocity)) : CO_Gaz_In;

        valueGasFlow = ParseLeadingNumber(_gasFlowMain.text);

        // Диаметр каталитического блока фиксированный — 0.9 м (таблица: Y19, Y56).
        // Раньше здесь стояла формула диаметра эмульгатора √(4Q/π/3600/2.9), и стоимость
        // катализатора 28.4·D²·N выходила примерно в 20 раз больше.
        deametr = BlockDiameter;

        double _catalyzBlock = ParseLeadingNumber(_katazBlockCount.text);

        // Подогрев: при угле или газе считается расход топлива (U51, U52),
        // электроэнергия на нагрев — только при остальных источниках, т.е. «Ресурсы ТЭЦ» (U50).
        electro = 0;
        if (MatchesAny(_gasSource.text, CoalSource))
        {
            consumption = (0.429 * valueGasFlow) / 0.85 * 1000;
        }
        else if (MatchesAny(_gasSource.text, GasSource))
        {
            consumption = 0.429 * valueGasFlow / (3400 - 1600) * 1000 * 24 * 71.56;
        }
        else
        {
            consumption = 0;
            electro = (1.06 * (_tempAfter - _tempBefore) * 29.68 / 22.4) * valueGasFlow / 3600;
        }

        catalyzator = 28.4 * (deametr * deametr) * _catalyzBlock;

        waterConsumables = 0.4 * 450 / 1000 / 34 * (valueGasFlow / 3600);
        // Было 135 / 1000 / 34 — целочисленное деление, результат всегда 0.
        reagentConsumables = 135.0 / 1000 / 34 * (valueGasFlow / 3600);

        electroAdditional = Mathf.Ceil((float)(0.12f * (450f / 1000f) / 5f / 34f * (valueGasFlow / 3600) * 10f)) / 10f * 5f;

        if (HasPreciousMetals)
        {
            catalyzator = catalyzator * 25076 * 2 / 365 / 24;
        }
        else if (HasNoPreciousMetals)
        {
            catalyzator = (catalyzator * 25076 + catalyzator * 0.2 * 4200) / 365 / 24;
        }

        // Вывод — после расчёта (раньше на экране были значения с прошлого шага).
        UpdateDisplayTexts();
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
        // Сравнение по числу, а не по строке — работает на любом языке и с точкой или запятой.
        if (NumberEquals(_flowRateText.text, 1.5f) && _smokeLevel != 0)
        {
            a++;
            ApplySmokeSpeedAndAlpha(0.2f - (0.4f * b), 0.7f);
            _smokeLevel = 0;
            b = 0;
        }
        else if (NumberEquals(_flowRateText.text, 1f) && _smokeLevel != 1)
        {
            ApplySmokeSpeedAndAlpha(-(0.2f * a) - (0.4f * b), 0.3f);
            _smokeLevel = 1;
            a = 0;
            b = 0;
        }
        else if (NumberEquals(_flowRateText.text, 2f) && _smokeLevel != 2)
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
        // Раньше int.Parse бросал исключение, пока поле пустое или редактируется.
        int blocks = Mathf.RoundToInt(ParseLeadingNumber(text));
        if (blocks <= 0) return;

        KatazBlockCountManager[] managers = FindObjectsOfType<KatazBlockCountManager>();
        foreach (KatazBlockCountManager manager in managers)
        {
            manager.ChangeBlocks(blocks);
        }
    }
}