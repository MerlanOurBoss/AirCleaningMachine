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

    //[Header("Связь со слайдерами и текстом в таблице")]
    [Serializable]
    public class ParameterRowUI
    {
        public string id;                 // Например: "Температура", "Пыль", "CO", ...
        public Slider slider;             // Слайдер справа
        public TextMeshProUGUI valueText; // Значение в таблице (столбец 'Вход')
    }

    //[Header("Настройки модулей")]
    [Serializable]
    public class ModuleData
    {
        public ModuleType type;           // Какой модуль
        public List<float> values;        // Значения по всем параметрам (по порядку, как в списке rows)
    }

    [SerializeField] private List<ParameterRowUI> rows = new List<ParameterRowUI>();
    [SerializeField] private List<ModuleData> modules = new List<ModuleData>();

    [Header("Текущий модуль (по очереди)")]
    [SerializeField] private int currentModuleIndex = 0;

    private void Start()
    {
        // Подписываемся на изменение каждого слайдера
        for (int i = 0; i < rows.Count; i++)
        {
            int paramIndex = i; // важно сделать копию индекса
            if (rows[i].slider != null)
            {
                rows[i].slider.onValueChanged.AddListener(
                    value => OnSliderChanged(paramIndex, value));
            }
        }

        ApplyModule(currentModuleIndex);
    }

    /// <summary>
    /// Вызывается извне, когда вы по логике перешли к следующему модулю
    /// (например из скрипта камер или кнопки "Далее").
    /// </summary>
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

    /// <summary>
    /// Переключение по очереди: 0→1→2→3→0 ...
    /// Можно повесить на кнопку "Следующий компонент".
    /// </summary>
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

        // На всякий случай проверяем, что количество значений совпадает
        if (module.values.Count < rows.Count)
        {
            Debug.LogWarning("GasTableManager: в ModuleData не хватает значений для всех параметров.");
        }

        for (int i = 0; i < rows.Count; i++)
        {
            float value = (i < module.values.Count) ? module.values[i] : 0f;

            if (rows[i].slider != null)
            {
                rows[i].slider.SetValueWithoutNotify(value); // чтобы не вызвать лишний OnValueChanged
            }

            if (rows[i].valueText != null)
            {
                rows[i].valueText.text = Mathf.RoundToInt(value).ToString();
            }
        }
    }

    // Вызывается, когда пользователь двигает слайдер
    private void OnSliderChanged(int paramIndex, float value)
    {
        // Обновляем текст в таблице
        if (paramIndex < 0 || paramIndex >= rows.Count)
            return;

        if (rows[paramIndex].valueText != null)
            rows[paramIndex].valueText.text = Mathf.RoundToInt(value).ToString();

        // Сохраняем значение в данных текущего модуля
        ModuleData currentModule = modules[currentModuleIndex];

        // Убедимся, что в списке есть место
        while (currentModule.values.Count <= paramIndex)
            currentModule.values.Add(0f);

        currentModule.values[paramIndex] = value;

        // Здесь можно добавить свою логику:
        // например, пересчёт "Выхода" и влияния компонента на следующую ступень
    }
}
