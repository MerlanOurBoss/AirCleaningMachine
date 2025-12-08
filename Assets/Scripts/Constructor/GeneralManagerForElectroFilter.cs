using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManagerForElectroFilter : GeneralManagerBase, IParameterModule
{
    [Header("ElectroFilter")]
    [Space]

    [SerializeField] private ParticleSystem[] _electroFilterSmokes;
    [SerializeField] private List<ParameterRowUI> rows = new List<ParameterRowUI>();
    public List<ParameterRowUI> Rows => rows;
    [SerializeField] private Animator _electroFilterAnimation;
    [SerializeField] private Camera _componentCamera;
    [SerializeField] private CameraSequenceController _cameraSequenceController;
    
    [SerializeField] private Button exitButton;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button switchButton;

    private Coroutine updateRoutine;
    private float updateInterval = 5f;
    public MathModuleForElectro _collec;
    private static int globalCounterElectro = 0;
    public int instanceID;

    private void Start()
    {
        globalCounterElectro++;
        instanceID = globalCounterElectro;

        Debug.Log($"[Electro] Назначен instanceID = {instanceID} для {gameObject.name}");
        
        MathModuleForElectro[] modules = FindObjectsOfType<MathModuleForElectro>();

        foreach (var m in modules)
        {
            if (m.instanceID == instanceID)
            {
                _collec = m;
                break;
            }
        }

        if (_collec != null)
        {
            _collec._electroFilterAnimator = _electroFilterAnimation;
            _collec._smokeParticles = _electroFilterSmokes;
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
                Debug.LogWarning($"[Electrofilter] Невозможно преобразовать valueTextIn у id={row.id}");
                continue;
            }
            
            if (row.id == "Температура")
            {
                float newTemperature = originalValue - (0.5f * (float)_collec.length * 4);   // -0.5 * 12 * 4
                float roundedTemp = Mathf.Ceil(newTemperature);

                row.valueTextOut.text = roundedTemp.ToString();
            }
            
            else if (row.id == "Твердые частицы")
            {
                float newDust = originalValue * 0.3f; // 30%
                float roundedDust = Mathf.Ceil(newDust);

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
        if (_electroFilterSmokes != null)
        {
            foreach (ParticleSystem smoke in _electroFilterSmokes)
            {
                if (smoke != null)
                    smoke.Play();
            }
        }

        if (_electroFilterAnimation != null)
            _electroFilterAnimation.Play("NewColecAnim");
    }

    protected override void OnStopModule()
    {
        if (_electroFilterSmokes != null)
        {
            foreach (ParticleSystem smoke in _electroFilterSmokes)
            {
                if (smoke != null)
                    smoke.Stop();
            }
        }

        if (_electroFilterAnimation != null)
            _electroFilterAnimation.Play("NewColecAnimStop");
    }
    
    private void OnDisable()
    {
        // Сбрасываем счётчик только если сцена выгружается
        globalCounterElectro = 0;
    }
}