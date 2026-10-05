using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GasTableManager : MonoBehaviour
{
    public enum ModuleType
    {
        Electrofilter,
        Emulsifier,
        Catalyst,
        CO2Collector
    }
    [Serializable]
    public class ParameterRowUI
    {
        public string id;
        public Slider slider;
        public TextMeshProUGUI valueText;
    }

    [Serializable]
    public class ModuleData
    {
        public ModuleType type;
        public List<float> values;
    }

    [SerializeField] private List<ParameterRowUI> rows = new List<ParameterRowUI>();
    [SerializeField] private List<ModuleData> modules = new List<ModuleData>();

    [Header("Текущий модуль (по очереди)")]
    [SerializeField] private int currentModuleIndex = 0;

    private void Start()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            int paramIndex = i;
            if (rows[i].slider != null)
            {
                rows[i].slider.onValueChanged.AddListener(
                    value => OnSliderChanged(paramIndex, value));
            }
        }

        ApplyModule(currentModuleIndex);
    }
    
    public void SetModule(ModuleType type)
    {
        int index = modules.FindIndex(m => m.type == type);
        if (index >= 0)
        {
            currentModuleIndex = index;
            ApplyModule(currentModuleIndex);
        }
        else
        {
            Debug.LogWarning($"GasTableManager: модуль {type} не найден в списке modules.");
        }
    }
    public void NextModule()
    {
        currentModuleIndex++;
        if (currentModuleIndex >= modules.Count)
            currentModuleIndex = 0;

        ApplyModule(currentModuleIndex);
    }

    private void ApplyModule(int moduleIndex)
    {
        if (moduleIndex < 0 || moduleIndex >= modules.Count)
        {
            Debug.LogWarning("GasTableManager: неверный индекс модуля.");
            return;
        }

        ModuleData module = modules[moduleIndex];

        if (module.values.Count < rows.Count)
        {
            Debug.LogWarning("GasTableManager: в ModuleData не хватает значений для всех параметров.");
        }

        for (int i = 0; i < rows.Count; i++)
        {
            float value = (i < module.values.Count) ? module.values[i] : 0f;

            if (rows[i].slider != null)
            {
                rows[i].slider.SetValueWithoutNotify(value);
            }

            if (rows[i].valueText != null)
            {
                rows[i].valueText.text = Mathf.RoundToInt(value).ToString();
            }
        }
    }
    private void OnSliderChanged(int paramIndex, float value)
    {
        if (paramIndex < 0 || paramIndex >= rows.Count)
            return;

        if (rows[paramIndex].valueText != null)
            rows[paramIndex].valueText.text = Mathf.RoundToInt(value).ToString();

        ModuleData currentModule = modules[currentModuleIndex];
        
        while (currentModule.values.Count <= paramIndex)
            currentModule.values.Add(0f);

        currentModule.values[paramIndex] = value;
    }
}
