using System;
using System.Collections.Generic;
using System.Globalization;

public enum AlertLevel { Normal, Warning, Critical }

/// <summary>Где проверяется параметр: на выходе всей установки или на конкретном аппарате.</summary>
public enum ProcessStage { SystemOutlet, Electrofilter, Emulsifier, Catalyst, CO2Collector }

public enum CheckPoint { In, Out }

/// <summary>Совет оператору. Apply == null — только текст: в модели нет настройки, которая это исправит.</summary>
public class Recommendation
{
    public readonly string Text;
    public readonly Action Apply;
    public bool CanApply => Apply != null;

    public Recommendation(string text, Action apply = null)
    {
        Text = text;
        Apply = apply;
    }
}

/// <summary>Нарушение нормы на одном аппарате (или на выходе установки) с рекомендациями.</summary>
public class ProcessAlert
{
    public ProcessLimits.Rule Rule;
    public GeneralManagerBase Module;   // аппарат, на чьей строке нарушена норма
    public ProcessStage Stage;          // что показывать оператору: аппарат или «выход установки»
    public CheckPoint Point;
    public AlertLevel Level;
    public float Value;
    public bool AboveMax;               // false — значение ниже нижней границы
    public int Order;                   // порядок правил — чтобы список не прыгал при сортировке по уровню
    public readonly List<Recommendation> Recommendations = new List<Recommendation>();
}

/// <summary>Тексты мониторинга на трёх языках интерфейса.</summary>
public static class MonitoringText
{
    public static string Pick(Translator.Language lang, string ru, string kk, string en) =>
        lang == Translator.Language.Kazakh ? kk : lang == Translator.Language.English ? en : ru;

    public static string StageName(ProcessStage stage, Translator.Language lang)
    {
        switch (stage)
        {
            case ProcessStage.Electrofilter: return Pick(lang, "Электрофильтр", "Электрофильтр", "Electrofilter");
            case ProcessStage.Emulsifier: return Pick(lang, "Эмульгатор", "Эмульгатор", "Emulsifier");
            case ProcessStage.Catalyst: return Pick(lang, "Катализатор", "Катализатор", "Catalyst");
            case ProcessStage.CO2Collector: return Pick(lang, "Сбор CO2", "CO2 жинау", "CO2 Collection");
            default: return Pick(lang, "Выход установки", "Қондырғы шығысы", "System outlet");
        }
    }

    /// <summary>Название параметра по id строки (газы — как есть).</summary>
    public static string ParameterName(string id, Translator.Language lang)
    {
        string t = id?.Trim() ?? string.Empty;
        if (string.Equals(t, "Температура", StringComparison.OrdinalIgnoreCase))
            return Pick(lang, "Температура", "Температура", "Temperature");
        if (string.Equals(t, "Твердые частицы", StringComparison.OrdinalIgnoreCase))
            return Pick(lang, "Твердые частицы", "Қатты бөлшектер", "Solid particles");
        return t;
    }

    public static string PointName(CheckPoint point, Translator.Language lang) =>
        point == CheckPoint.In ? Pick(lang, "вход", "кіріс", "inlet") : Pick(lang, "выход", "шығыс", "outlet");

    public static string LevelName(AlertLevel level, Translator.Language lang) =>
        level == AlertLevel.Critical ? Pick(lang, "АВАРИЯ", "АПАТ", "CRITICAL")
                                     : Pick(lang, "ВНИМАНИЕ", "ЕСКЕРТУ", "WARNING");

    public static string Badge(AlertLevel worst, int count, Translator.Language lang)
    {
        switch (worst)
        {
            case AlertLevel.Critical: return Pick(lang, "Авария", "Апат", "Critical") + ": " + count;
            case AlertLevel.Warning: return Pick(lang, "Внимание", "Ескерту", "Warnings") + ": " + count;
            default: return Pick(lang, "Параметры в норме", "Параметрлер қалыпты", "All parameters normal");
        }
    }

    /// <summary>«АВАРИЯ · Катализатор, выход · CO: 132 мг/м³ (норма ≤ 50)».</summary>
    public static string AlertTitle(ProcessAlert alert, Translator.Language lang)
    {
        var rule = alert.Rule;
        string where = StageName(alert.Stage, lang);
        if (alert.Stage != ProcessStage.SystemOutlet)
            where += ", " + PointName(alert.Point, lang);

        string unit = string.IsNullOrWhiteSpace(rule.unit) ? string.Empty : " " + rule.unit.Trim();
        string norm = alert.AboveMax
            ? $"≤ {Number(rule.warningMax)}"
            : $"≥ {Number(rule.warningMin)}";

        return $"{LevelName(alert.Level, lang)} · {where} · {ParameterName(rule.parameterId, lang)}: " +
               $"{Number(alert.Value)}{unit} ({Pick(lang, "норма", "норма", "limit")} {norm})";
    }

    /// <summary>Малые значения — с двумя знаками, остальные — целыми (как в таблицах аппаратов).</summary>
    public static string Number(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return "—";
        return Math.Abs(value) < 10
            ? value.ToString("0.##", CultureInfo.InvariantCulture)
            : Math.Round(value).ToString("0", CultureInfo.InvariantCulture);
    }
}
