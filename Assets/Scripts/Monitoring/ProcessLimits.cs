using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Нормы параметров для ProcessMonitor. Значения по умолчанию — ЗАГЛУШКИ: их нужно заменить
/// реальными нормативами (нормы выбросов, ПДК, паспорта аппаратов) в инспекторе ассета
/// Assets/Resources/ProcessLimits.asset (меню Tools/Process Monitor/Select Limits).
/// </summary>
[CreateAssetMenu(fileName = "ProcessLimits", menuName = "Air Cleaning/Process Limits")]
public class ProcessLimits : ScriptableObject
{
    /// <summary>Путь для Resources.Load.</summary>
    public const string ResourcesPath = "ProcessLimits";

    [Serializable]
    public class Rule
    {
        [Tooltip("Только для читаемости в инспекторе, в логике не участвует.")]
        public string label;
        public bool enabled = true;

        [Tooltip("SystemOutlet — выход последнего аппарата цепочки, т.е. то, что уходит в трубу.")]
        public ProcessStage stage = ProcessStage.SystemOutlet;

        [Tooltip("Вход или выход аппарата. Для SystemOutlet не используется (всегда выход).")]
        public CheckPoint point = CheckPoint.Out;

        [Tooltip("id строки ParameterRowUI: Температура, Твердые частицы, CO, NO, NO2, SO2, H2S, CO2, CH4, O2, N2.")]
        public string parameterId;

        [Tooltip("Только для показа в панели тревог.")]
        public string unit;

        [Header("Верхняя граница")]
        public bool checkMax = true;
        public float warningMax;
        public float criticalMax;

        [Header("Нижняя граница")]
        public bool checkMin;
        public float warningMin;
        public float criticalMin;
    }

    [Tooltip("Как часто проверять значения, с. Сами значения по цепочке обновляются раз в updateInterval модулей (5 с).")]
    [Min(0.1f)] public float checkInterval = 1f;

    [Tooltip("Запас от порога (доля), на который значение должно вернуться, чтобы тревога снялась, — " +
             "иначе на границе нормы тревога «мигает».")]
    [Range(0f, 0.5f)] public float hysteresis = 0.05f;

    [Tooltip("Наибольшее число блоков катализатора, которое можно выставить (варианты выпадающего списка — 2, 3, 4).")]
    [Min(1)] public int catalystMaxBlocks = 4;

    public List<Rule> rules = CreateDefaultRules();

    /// <summary>Нормы-заглушки, подобранные под диапазоны слайдеров главной таблицы.</summary>
    public static List<Rule> CreateDefaultRules() => new List<Rule>
    {
        Max("Выход: твердые частицы", ProcessStage.SystemOutlet, CheckPoint.Out, "Твердые частицы", "мг/м³", 20, 50),
        Max("Выход: CO", ProcessStage.SystemOutlet, CheckPoint.Out, "CO", "мг/м³", 50, 100),
        Max("Выход: NO", ProcessStage.SystemOutlet, CheckPoint.Out, "NO", "мг/м³", 200, 300),
        Max("Выход: NO2", ProcessStage.SystemOutlet, CheckPoint.Out, "NO2", "мг/м³", 50, 85),
        Max("Выход: SO2", ProcessStage.SystemOutlet, CheckPoint.Out, "SO2", "мг/м³", 100, 200),
        Max("Выход: H2S", ProcessStage.SystemOutlet, CheckPoint.Out, "H2S", "мг/м³", 5, 10),
        Max("Выход: CO2", ProcessStage.SystemOutlet, CheckPoint.Out, "CO2", "%", 3, 5),

        // Пыль отравляет катализатор — её должны снять аппараты перед ним.
        Max("Катализатор, вход: твердые частицы", ProcessStage.Catalyst, CheckPoint.In, "Твердые частицы", "мг/м³", 10, 30),

        // Шаблоны: температуры по цепочке зависят от расходов, включите после подбора порогов.
        new Rule
        {
            label = "(шаблон) Катализатор, вход: температура не ниже", enabled = false,
            stage = ProcessStage.Catalyst, point = CheckPoint.In, parameterId = "Температура", unit = "°C",
            checkMax = false, checkMin = true, warningMin = 60, criticalMin = 40
        },
        Max("(шаблон) Сбор CO2, вход: температура", ProcessStage.CO2Collector, CheckPoint.In, "Температура", "°C", 80, 120, false),
    };

    private static Rule Max(string label, ProcessStage stage, CheckPoint point, string id, string unit,
        float warning, float critical, bool enabled = true) => new Rule
    {
        label = label, enabled = enabled, stage = stage, point = point, parameterId = id, unit = unit,
        checkMax = true, warningMax = warning, criticalMax = critical
    };
}
