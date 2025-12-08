using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManagerForSborCO2 : GeneralManagerBase, IParameterModule
{ 
    [Header("SborC02")]
    [Space]

    [SerializeField] private ParticleSystem[] _SborSmokes;
    [SerializeField] private List<ParameterRowUI> rows = new List<ParameterRowUI>();
    public List<ParameterRowUI> Rows => rows;
    [SerializeField] private Animator[] _SborFans;
    [SerializeField] private NewSborScript _SborScript;

    [SerializeField] private Camera _componentCamera;
    [SerializeField] private CameraSequenceController _cameraSequenceController;

    [SerializeField] private Button exitButton;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button switchButton;
    
    private Coroutine updateRoutine;
    private float updateInterval = 5f;
    private MathModulForSborCO2 sbor;
    private static int globalCounterSbor = 0;
    public int instanceID;

    private void Start()
    {
        globalCounterSbor++;
        instanceID = globalCounterSbor;

        Debug.Log($"[SborCO2] Назначен instanceID = {instanceID} для {gameObject.name}");
        
        MathModulForSborCO2[] modules = FindObjectsOfType<MathModulForSborCO2>();

        foreach (var m in modules)
        {
            if (m.instanceID == instanceID)
            {
                sbor = m;
                break;
            }
        }

        if (sbor != null)
        {
            sbor._smokes = _SborSmokes;
            sbor.fansAnim = _SborFans;
            sbor.newSbor = _SborScript;
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
        foreach (var row in rows)
        {
            if (!row.valueTextIn)
                continue;
            
            if (!float.TryParse(row.valueTextIn.text, out float originalValue))
            {
                Debug.LogWarning($"[Emul] Невозможно преобразовать valueTextIn у id={row.id}");
                continue;
            }
            
            if (row.id == "Температура")
            {
                float newTemperature = originalValue * 0.7f; 
                float roundedTemp = Mathf.Ceil(newTemperature);

                row.valueTextOut.text = roundedTemp.ToString();
            }
            
            else if (row.id == "CO2")
            {
                float co2 = originalValue * (sbor.co2FlowRate - sbor.capturedCO2)/sbor.co2FlowRate;
                float roundedDust = Mathf.Ceil(co2);

                row.valueTextOut.text = roundedDust.ToString();
            }
            else
            {
                row.valueTextOut.text = row.valueTextIn.text;
            }
        }
    }
    protected override void OnStartModule()
    {
        if (_SborScript != null)
        {
            _SborScript.StartColumnProcess();
        }
        
        if (_SborFans != null)
        {
            foreach (Animator fan in _SborFans)
            {
                if (fan != null)
                    fan.Play("BigFan");
            }
        }
    }

    protected override void OnStopModule()
    {
        if (_SborScript != null)
        {
            _SborScript.StopColumnProcess();
        }
        
        if (_SborFans != null)
        {
            foreach (Animator fan in _SborFans)
            {
                if (fan != null)
                    fan.Play("Stop");
            }
        }
    }
    
    private void OnDisable()
    {
        // Сбрасываем счётчик только если сцена выгружается
        globalCounterSbor = 0;
    }
}