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
    private double _no2Value;
                               
    protected override void OnMathModuleReady(MathModulForKataz module)
    {
        module._smokes = katazSmokes;
    }

    protected override bool TryCalculateRow(ParameterRowUI row, float originalValue)
    {
        double katazCount = SafeParse(MathModule._katazBlockCount.text);
        double flow = SafeParse(MathModule.valueGasFlow.ToString());

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
                double constCO = Math.Pow(10, 6.48) * Math.Exp(-39700.0 / 8.314 / (_temp + 273.15));
                double inputValueCO = SafeParse(row.valueTextIn.text);

                double result = inputValueCO * Math.Exp(
                    -constCO * 3600.0 *
                    (Math.PI * Math.Pow(0.9, 2) * katazCount * 0.09 / 4.0)
                ) / flow;

                _coValue = double.Parse(row.valueTextIn.text);
                _coValueResult = result;
                row.valueTextOut.text = Mathf.Ceil((float)result).ToString();
                return true;
            }
            case "NO":
            {
                double inputValueNO = SafeParse(row.valueTextIn.text);
                _noValue = double.Parse(row.valueTextIn.text);

                if (MathModule._katazBlockType.text == "с драгметаллами")
                {
                    double result = inputValueNO - (_coValue - _coValueResult) / _coValue * inputValueNO * 0.102;
                    double constNO = Math.Pow(10, 6.72) * Math.Exp(-43900.0 / 8.314 / (_temp + 273.15));
                    double resultFinal = result * Math.Exp(-constNO * 3600.0 * (Math.PI * Math.Pow(0.9, 2) * katazCount * 0.09 / 4.0) / flow);
                    row.valueTextOut.text = Mathf.Ceil((float)resultFinal).ToString();
                }
                else if (MathModule._katazBlockType.text == "без драгметаллов")
                {
                    double result = Math.Pow(10, 6.72) * Math.Exp(-43900.0 / 8.314 / (_temp + 273.15));
                    row.valueTextOut.text = Mathf.Ceil((float)result).ToString();
                }
                return true;
            }
            case "NO2":
            {
                double inputValueNO2 = SafeParse(row.valueTextIn.text);

                if (MathModule._katazBlockType.text == "с драгметаллами")
                {
                    double result = inputValueNO2 + (_coValue - _coValueResult) / _coValue * _noValue * 0.102;
                    double constNO = Math.Pow(10, 6.72) * Math.Exp(-43900.0 / 8.314 / (_temp + 273.15));
                    double resultFinal = result * Math.Exp(-constNO * 3600.0 * (Math.PI * Math.Pow(0.9, 2) * katazCount * 0.09 / 4.0) / flow);
                    row.valueTextOut.text = Mathf.Ceil((float)resultFinal).ToString();
                }
                else if (MathModule._katazBlockType.text == "без драгметаллов")
                {
                    double result = double.Parse(row.valueTextIn.text) + (_coValue - _coValueResult) / _coValue * _no2Value * 0.102;
                    row.valueTextOut.text = Mathf.Ceil((float)result).ToString();
                }
                return true;
            }
            default:
                return false;
        }
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