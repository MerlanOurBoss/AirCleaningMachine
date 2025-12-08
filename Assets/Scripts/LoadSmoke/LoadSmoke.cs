using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSmoke : MonoBehaviour
{
    [Serializable]
    public class MainTableRowUI
    {
        public string id;
        public Slider slider;
        public TextMeshProUGUI valueTextIn;
    }
    
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private List<GeneralManagerBase> _modules;
    [SerializeField] private List<MainTableRowUI> mainTable = new List<MainTableRowUI>();

    private List<IParameterModule> _parameterModules = new List<IParameterModule>();
    
    [SerializeField] private float updateInterval = 5f;
    private void Start()
    {
        startButton.onClick.AddListener(StartSimulation);
        stopButton.onClick.AddListener(StopSimulation);
        
        var found = FindObjectsOfType<GeneralManagerBase>(true);
        _modules = new List<GeneralManagerBase>(found);
        _modules.Reverse();
        
        CacheParameterModules();
        StartCoroutine(UpdateChainRoutine());
    }
    private IEnumerator UpdateChainRoutine()
    {
        while (true)
        {
            UpdateValuesChain();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void CacheParameterModules()
    {
        _parameterModules.Clear();

        foreach (var m in _modules)
        {
            if (m is IParameterModule pm)
                _parameterModules.Add(pm);
        }

        if (_parameterModules.Count == 0)
        {
            Debug.LogWarning("LoadSmoke: ни один из GeneralManagerBase не реализует IParameterModule");
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateValuesChain()
    {
        if (_parameterModules.Count == 0)
            return;

        // 1) mainTable -> первый модуль обновление IN
        var firstRows = _parameterModules[0].Rows;

        int count0 = Mathf.Min(mainTable.Count, firstRows.Count);

        for (int i = 0; i < count0; i++)
        {
            // обновляем входные значения первого модуля
            firstRows[i].valueTextIn.text = mainTable[i].valueTextIn.text;
        }

        // 2) цепочка: OUT текущего -> IN следующего
        for (int m = 0; m < _parameterModules.Count - 1; m++)
        {
            var currentRows = _parameterModules[m].Rows;
            var nextRows    = _parameterModules[m + 1].Rows;

            int count = Mathf.Min(currentRows.Count, nextRows.Count);

            for (int i = 0; i < count; i++)
            {
                if (currentRows[i].valueTextOut != null && nextRows[i].valueTextIn != null)
                {
                    nextRows[i].valueTextIn.text = currentRows[i].valueTextOut.text;
                    nextRows[i].slider.value = currentRows[i].slider.value;
                }
            }
        }

        // INFO FOR DEBUG
        Debug.Log("Chain values updated at " + DateTime.Now.ToLongTimeString());
    }
    
    private void StartSimulation()
    {        
        if (_modules == null || _modules.Count == 0)
        {
            Debug.LogWarning("AllModulesStarter: на сцене не найдено ни одного GeneralManagerBase");
            return;
        }
        
        foreach (var module in _modules)
        {
            if (module != null)
                module.StartModule();
        }
    }

    private void StopSimulation()
    {
        foreach (var module in _modules)
        {
            if (module != null)
                module.StopModule();
        }
    }
}
