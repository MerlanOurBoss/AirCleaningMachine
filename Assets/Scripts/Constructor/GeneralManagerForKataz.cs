using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManagerForKataz : GeneralManagerBase, IParameterModule
{
    [Header("Kataz")] [Space] [SerializeField]
    private ParticleSystem[] _katazSmokes;
    [SerializeField] private List<ParameterRowUI> rows = new List<ParameterRowUI>();
    public List<ParameterRowUI> Rows => rows;
    [SerializeField] private Camera _componentCamera;
    [SerializeField] private CameraSequenceController _cameraSequenceController;
    [SerializeField] private TemperatureCatalizator _temperatureCatalizator;

    [SerializeField] private Button exitButton;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button switchButton;

    private MathModulForKataz kataz;
    private Coroutine updateRoutine;
    private float updateInterval = 5f;
    private static int globalCounterKataz = 0;
    public int instanceID;

    private void Start()
    {
        globalCounterKataz++;
        instanceID = globalCounterKataz;

        Debug.Log($"[Kataz] Назначен instanceID = {instanceID} для {gameObject.name}");
        
        MathModulForKataz[] modules = FindObjectsOfType<MathModulForKataz>();

        foreach (var m in modules)
        {
            if (m.instanceID == instanceID)
            {
                kataz = m;
                break;
            }
        }
        if (kataz != null)
        {
            kataz._smokes = _katazSmokes;
        }

        _cameraSequenceController = GameObject.FindFirstObjectByType<CameraSequenceController>();

        if (_cameraSequenceController != null)
        {
            if (exitButton != null)
                exitButton.onClick.AddListener(_cameraSequenceController.RestoreWallFromReveal);

            if (switchButton != null)
                switchButton.onClick.AddListener(_cameraSequenceController.ShowOverview);

            if (enterButton != null && _componentCamera != null)
                enterButton.onClick.AddListener(() => _cameraSequenceController.Reveal(_componentCamera));
        }
        updateRoutine = StartCoroutine(UpdateValuesRoutine());
    }
    private IEnumerator UpdateValuesRoutine()
    {
        while (true)
        {
            UpdateCalculatedParameters();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateCalculatedParameters()
    {
        float temp = 0f;
        double coValue = 0f;
        double coValueResult = 0f;
        double noValue = 0f;
        double no2Value = 0f;
        
        double katazCount = SafeParse(kataz._katazBlockCount.text);
        double flow = SafeParse(kataz.valueGasFlow.ToString());
        
        foreach (var row in rows)
        {
            if (!row.valueTextIn)
                continue;
            
            if (!float.TryParse(row.valueTextIn.text, out float originalValue))
            {
                Debug.LogWarning($"[Kataz] Невозможно преобразовать valueTextIn у id={row.id}");
                continue;
            }
            
            if (row.id == "Температура")
            {
                kataz._tempBefore = originalValue;
                float newTemperature = originalValue * 4; 
                float roundedTemp = Mathf.Ceil(newTemperature);
                temp = roundedTemp;
                kataz._tempAfter = roundedTemp;
                row.valueTextOut.text = roundedTemp.ToString();
            }
            
            else if (row.id == "CO")
            {
                double constCO = Math.Pow(10, 6.48) * Math.Exp(-39700.0 / 8.314 / (temp + 273.15));
                double inputValueCO = SafeParse(row.valueTextIn.text);
                
                double result = inputValueCO * Math.Exp(
                    -constCO * 3600.0 * 
                    (Math.PI * Math.Pow(0.9, 2) * katazCount * 0.09 / 4.0)
                ) / flow;
                
                coValue = double.Parse(row.valueTextIn.text);
                coValueResult = result;
                double roundedCO = Mathf.Ceil((float)result);
                row.valueTextOut.text = roundedCO.ToString();
            }
            else if (row.id == "NO")
            {
                double inputValueNO = SafeParse(row.valueTextIn.text);
                noValue = double.Parse(row.valueTextIn.text);
                
                if (kataz._katazBlockType.text == "с драгметаллами")
                {
                    double result = inputValueNO -
                                    (coValue - coValueResult) / coValue * inputValueNO * 0.102;
                    
                    double constNO = Math.Pow(10, 6.72) * Math.Exp(-43900.0 / 8.314 / (temp + 273.15));
                    
                    double resultFinal = result * Math.Exp(-constNO * 3600.0 * (Math.PI * Math.Pow(0.9, 2) * katazCount * 0.09 / 4.0) / flow);
                
                    double roundedNO = Mathf.Ceil((float)resultFinal);
                    row.valueTextOut.text = roundedNO.ToString();
                }
                else if (kataz._katazBlockType.text == "без драгметаллов")
                {
                    double result = Math.Pow(10, 6.72) * 
                             Math.Exp(-43900.0 / 8.314 / (temp + 273.15));
                    
                    double roundedNO = Mathf.Ceil((float)result);
                    row.valueTextOut.text = roundedNO.ToString();
                    Debug.Log("sasas");
                }
            }
            else if (row.id == "NO2")
            {
                double inputValueNO2 = SafeParse(row.valueTextIn.text);

                if (kataz._katazBlockType.text == "с драгметаллами")
                {
                    double result = inputValueNO2 +
                                    (coValue - coValueResult) / coValue * noValue * 0.102;
                    
                    double constNO = Math.Pow(10, 6.72) * Math.Exp(-43900.0 / 8.314 / (temp + 273.15));
                    
                    double resultFinal = result * Math.Exp(-constNO * 3600.0 * (Math.PI * Math.Pow(0.9, 2) * katazCount * 0.09 / 4.0) / flow);
                    
                    double roundedNO2 = Mathf.Ceil((float)resultFinal);
                    row.valueTextOut.text = roundedNO2.ToString();
                }
                else if (kataz._katazBlockType.text == "без драгметаллов")
                {
                    double result = double.Parse(row.valueTextIn.text) +
                                    (coValue - coValueResult) / coValue * no2Value * 0.102;

                    double roundedNO2 = Mathf.Ceil((float)result);
                    row.valueTextOut.text = roundedNO2.ToString();
                }
            }
            else
            {
                row.valueTextOut.text = row.valueTextIn.text;
            }
        }
    }
    double SafeParse(string s)
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

        double value;
        if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            return 0;

        return value;
    }
    protected override void OnStartModule()
    {
        if (_katazSmokes == null)
            return;
        if (_temperatureCatalizator == null)
            return;
        
        foreach (ParticleSystem smoke in _katazSmokes)
        {
            if (smoke != null)
                smoke.Play();
        }

        _temperatureCatalizator.StartSimulation();
    }

    protected override void OnStopModule()
    {
        if (_katazSmokes == null)
            return;
        if (_temperatureCatalizator == null)
            return;
        
        foreach (ParticleSystem smoke in _katazSmokes)
        {
            if (smoke != null)
                smoke.Stop();
        }

        _temperatureCatalizator.StopSimulation();
    }
    
    private void OnDisable()
    {
        globalCounterKataz = 0;
    }
}