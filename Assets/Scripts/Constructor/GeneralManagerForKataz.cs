using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class GeneralManagerForKataz : ModuleManagerBase<MathModulForKataz>
{
    [Header("Kataz")]
    [SerializeField] private ParticleSystem[] katazSmokes;
    [SerializeField] private TemperatureCatalizator temperatureCatalizator;

    private float _temp;
    private double _coValue;
    private double _coValueResult;
    private double _noValue;

    // Геометрия и константы — по таблице расчёта (Лист1, столбцы X–Z и AF–AH).
    private const double BlockDiameter = 0.9;     // м, Y19 / AG19
    private const double BlockLength = 0.09;      // м
    private const double NoToNo2Share = 0.102;    // доля NO, окисляемая в NO2 (Y8, Y9)
    private const double So2Share = 0.353;        // доля SO2, связываемая на катализаторе P (Y10)

    protected override void OnMathModuleReady(MathModulForKataz module)
    {
        module._smokes = katazSmokes;
    }

    // Строки обрабатываются по порядку: Температура → CO → NO → NO2 → SO2,
    // поэтому NO/NO2/SO2 используют конверсию CO, посчитанную в этом же проходе.
    //
    // Таблица описывает два разных катализатора:
    //  • «с драгметаллами» (P): окисляет CO; часть NO переходит в NO2; снижает SO2.
    //  • «без драгметаллов» (V): CO не трогает; восстанавливает NO и NO2.
    protected override bool TryCalculateRow(ParameterRowUI row, float originalValue)
    {
        double katazCount = SafeParse(MathModule._katazBlockCount.text);
        double flow = MathModule.valueGasFlow; // м³/ч
        bool precious = MathModule.HasPreciousMetals;
        bool nonPrecious = MathModule.HasNoPreciousMetals;

        switch (row.id)
        {
            case "Температура":
            {
                MathModule._tempBefore = originalValue;
                float roundedTemp = Mathf.Ceil(originalValue * 4);
                _temp = roundedTemp;
                MathModule._tempAfter = roundedTemp;
                row.valueTextOut.text = roundedTemp.ToString();
                return true;
            }
            case "CO":
            {
                double inputCO = SafeParse(row.valueTextIn.text);
                double result = inputCO;

                // Y6: CO_вых = CO_вх · exp(−k_CO · 3600 · V / Q). Деление на расход — внутри экспоненты.
                if (precious)
                    result = inputCO * Decay(RateConstantCO(), katazCount, flow);

                _coValue = inputCO;
                _coValueResult = result;
                row.valueTextOut.text = FormatValue(result);
                return true;
            }
            case "NO":
            {
                double inputNO = SafeParse(row.valueTextIn.text);
                _noValue = inputNO;
                double result = inputNO;

                if (precious)
                    result = inputNO - CoConversion() * inputNO * NoToNo2Share;          // Y8
                else if (nonPrecious)
                    result = inputNO * Decay(RateConstantNO(), katazCount, flow);        // AG8

                row.valueTextOut.text = FormatValue(result);
                return true;
            }
            case "NO2":
            {
                double inputNO2 = SafeParse(row.valueTextIn.text);
                double result = inputNO2;

                if (precious)
                    result = inputNO2 + CoConversion() * _noValue * NoToNo2Share;        // Y9
                else if (nonPrecious)
                    result = inputNO2 * Decay(RateConstantNO(), katazCount, flow);       // AG9

                row.valueTextOut.text = FormatValue(result);
                return true;
            }
            case "SO2":
            {
                if (!precious) return false; // катализатор V пропускает SO2 без изменений (AG10)

                double inputSO2 = SafeParse(row.valueTextIn.text);
                double result = inputSO2 - CoConversion() * inputSO2 * So2Share;          // Y10
                row.valueTextOut.text = FormatValue(result);
                return true;
            }
            default:
                return false;
        }
    }

    // Y23: k_CO = 10^6.48 · exp(−39700 / R / T), с⁻¹
    private double RateConstantCO() => Math.Pow(10, 6.48) * Math.Exp(-39700.0 / 8.314 / (_temp + 273.15));

    // AG23: k_NO = 10^6.72 · exp(−43900 / R / T), с⁻¹
    private double RateConstantNO() => Math.Pow(10, 6.72) * Math.Exp(-43900.0 / 8.314 / (_temp + 273.15));

    // exp(−k · 3600 · V / Q), V = π·D²·N·0.09/4 — объём катализатора, Q — расход газа, м³/ч.
    // При нулевом расходе газ через катализатор не идёт — концентрация не меняется.
    private static double Decay(double k, double blockCount, double flow)
    {
        if (flow <= 0) return 1.0;
        double volume = Math.PI * BlockDiameter * BlockDiameter * blockCount * BlockLength / 4.0;
        return Math.Exp(-k * 3600.0 * volume / flow);
    }

    // Доля прореагировавшего CO; при нулевом входном CO — 0 (раньше получалось NaN).
    private double CoConversion() => _coValue != 0 ? (_coValue - _coValueResult) / _coValue : 0;

    // Малые концентрации (после катализатора CO ≈ 0.02 ppm) показываем с двумя знаками,
    // иначе Ceil превращал их в «1». Точка — инвариантная, следующий модуль её разбирает.
    private static string FormatValue(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) value = 0;
        if (value < 0) value = 0;
        return value < 10
            ? value.ToString("0.##", CultureInfo.InvariantCulture)
            : Math.Ceiling(value).ToString(CultureInfo.InvariantCulture);
    }

    private static double SafeParse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return 0;

        s = s.Trim()
            .Replace(" ", "")
            .Replace("\n", "")
            .Replace("\t", "")
            .Replace(",", ".")
            .Replace("блок", "")
            .Replace("шт", "");

        s = new string(s.Where(ch => char.IsDigit(ch) || ch == '.' || ch == '-').ToArray());

        return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double value) ? value : 0;
    }

    protected override void OnStartModule()
    {
        if (katazSmokes == null || temperatureCatalizator == null) return;

        foreach (var smoke in katazSmokes)
            if (smoke != null) smoke.Play();

        temperatureCatalizator.StartSimulation();
    }

    protected override void OnStopModule()
    {
        if (katazSmokes == null || temperatureCatalizator == null) return;

        foreach (var smoke in katazSmokes)
            if (smoke != null) smoke.Stop();

        temperatureCatalizator.StopSimulation();
    }
}