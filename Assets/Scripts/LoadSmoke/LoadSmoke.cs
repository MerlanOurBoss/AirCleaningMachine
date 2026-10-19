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

        // Порядок цепочки = порядок объектов в иерархии (тот же, по которому PipeConnector соединяет трубы).
        // Раньше брался порядок FindObjectsOfType + Reverse(), который Unity не гарантирует.
        _modules.Sort((a, b) => CompareHierarchyOrder(a.transform, b.transform));
        
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

        // 1) mainTable -> входные значения первого модуля
        var firstRows = _parameterModules[0].Rows;
        for (int i = 0; i < mainTable.Count; i++)
        {
            var source = mainTable[i];
            var target = FindTargetRow(firstRows, source.id, i);
            if (source.valueTextIn != null && target != null && target.valueTextIn != null)
                target.valueTextIn.text = source.valueTextIn.text;
        }

        // 2) цепочка: OUT текущего -> IN следующего
        for (int m = 0; m < _parameterModules.Count - 1; m++)
        {
            var currentRows = _parameterModules[m].Rows;
            var nextRows    = _parameterModules[m + 1].Rows;

            for (int i = 0; i < currentRows.Count; i++)
            {
                var source = currentRows[i];
                var target = FindTargetRow(nextRows, source.id, i);
                if (target == null || source.valueTextOut == null || target.valueTextIn == null)
                    continue;

                target.valueTextIn.text = source.valueTextOut.text;

                if (source.slider != null && target.slider != null)
                    target.slider.value = source.slider.value;
            }
        }
    }

    /// <summary>
    /// Строка-получатель: по совпадению id (если он задан), иначе по тому же индексу.
    /// Раньше строки сопоставлялись только по индексу, и при разном наборе строк
    /// в модулях значение уходило в чужой параметр.
    /// </summary>
    private static ParameterRowUI FindTargetRow(List<ParameterRowUI> rows, string id, int index)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            foreach (var row in rows)
                if (row != null && string.Equals(row.id?.Trim(), id.Trim(), StringComparison.OrdinalIgnoreCase))
                    return row;
        }
        return index < rows.Count ? rows[index] : null;
    }

    private static int CompareHierarchyOrder(Transform a, Transform b)
    {
        var pathA = GetHierarchyPath(a);
        var pathB = GetHierarchyPath(b);
        int n = Mathf.Min(pathA.Count, pathB.Count);
        for (int i = 0; i < n; i++)
        {
            int cmp = pathA[i].CompareTo(pathB[i]);
            if (cmp != 0) return cmp;
        }
        return pathA.Count.CompareTo(pathB.Count);
    }

    private static List<int> GetHierarchyPath(Transform t)
    {
        var path = new List<int>();
        for (; t != null; t = t.parent)
            path.Insert(0, t.GetSiblingIndex());
        return path;
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
