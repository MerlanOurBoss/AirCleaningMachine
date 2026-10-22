using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Рекомендации к тревогам ProcessMonitor. Перебирает аппараты, стоящие до точки контроля, и по формулам
/// самих модулей считает, какая настройка вернёт параметр в норму (кнопка «Применить»). Если в модели нет
/// настройки, которая влияет на параметр, — совет текстом: добавить аппарат в схему или снизить параметр на входе.
/// </summary>
public static class ProcessAdvisor
{
    // Какие аппараты снижают параметр — для совета «добавьте аппарат в схему».
    private static readonly Dictionary<string, ProcessStage[]> Reducers =
        new Dictionary<string, ProcessStage[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "Твердые частицы", new[] { ProcessStage.Electrofilter, ProcessStage.Emulsifier } },
            { "SO2", new[] { ProcessStage.Emulsifier } },
            { "H2S", new[] { ProcessStage.Emulsifier } },
            { "NO2", new[] { ProcessStage.Emulsifier, ProcessStage.Catalyst } },
            { "NO", new[] { ProcessStage.Catalyst } },
            { "CO", new[] { ProcessStage.Catalyst } },
            { "CO2", new[] { ProcessStage.CO2Collector } },
        };

    public static void Advise(ProcessAlert alert, IReadOnlyList<GeneralManagerBase> chain, ProcessLimits limits,
        Translator.Language lang)
    {
        var recs = alert.Recommendations;
        string id = alert.Rule.parameterId.Trim();
        string name = MonitoringText.ParameterName(id, lang);

        if (!alert.AboveMax)
        {
            recs.Add(new Recommendation(MonitoringText.Pick(lang,
                $"{name} ниже нормы. Проверьте режим аппаратов перед точкой контроля.",
                $"{name} нормадан төмен. Бақылау нүктесіне дейінгі аппараттардың режимін тексеріңіз.",
                $"{name} is below the limit. Check the apparatus upstream of this point.")));
            return;
        }

        // Цель — ниже порога предупреждения с запасом на гистерезис, чтобы после «Применить» тревога снялась.
        float warning = alert.Rule.warningMax;
        float target = warning - Mathf.Max(2f * limits.hysteresis, 0.05f) * Mathf.Abs(warning);

        // На точку контроля влияют аппараты до неё; на выходе аппарата — и он сам.
        int index = IndexOf(chain, alert.Module);
        int last = alert.Point == CheckPoint.Out ? index : index - 1;
        var present = new HashSet<ProcessStage>();

        for (int i = 0; i <= last; i++)
        {
            var module = chain[i];
            var stage = ProcessMonitor.StageOf(module);
            if (stage.HasValue) present.Add(stage.Value);

            var row = ProcessMonitor.FindRow(module, id);
            if (row == null
                || !ProcessMonitor.TryReadValue(row.valueTextIn, out float cIn)
                || !ProcessMonitor.TryReadValue(row.valueTextOut, out float cOut)
                || cOut <= 0f || alert.Value <= 0f || target <= 0f)
                continue;

            // Аппараты после этого меняют параметр пропорционально, поэтому цель на его выходе = target · cOut / Value.
            float moduleTarget = target * cOut / alert.Value;

            switch (module)
            {
                case GeneralManagerForKataz kataz:
                    AdviseCatalyst(kataz, id, cIn, moduleTarget, limits, lang, recs);
                    break;
                case GeneralManagerForEmul emul:
                    AdviseEmulsifier(emul, id, cOut, lang, recs);
                    break;
            }
        }

        if (recs.Count > 0) return;

        if (Reducers.TryGetValue(id, out var reducers) && !Array.Exists(reducers, present.Contains))
        {
            string names = string.Join(" / ", Array.ConvertAll(reducers, s => MonitoringText.StageName(s, lang)));
            recs.Add(new Recommendation(MonitoringText.Pick(lang,
                $"В схеме до точки контроля нет аппарата, который снижает {name}. Добавьте в конструкторе: {names}.",
                $"Бақылау нүктесіне дейінгі сызбада {name} төмендететін аппарат жоқ. Конструкторда қосыңыз: {names}.",
                $"No apparatus before this point reduces {name}. Add in the constructor: {names}.")));
            return;
        }

        recs.Add(new Recommendation(MonitoringText.Pick(lang,
            $"Аппараты схемы уже на пределе модели — настроек, которые снизят {name}, нет. Снизьте {name} на входе установки.",
            $"Сызбадағы аппараттар модель шегінде — {name} төмендететін баптау жоқ. Қондырғы кірісінде {name} азайтыңыз.",
            $"The apparatus are at the limit of the model — no setting lowers {name}. Reduce {name} at the system inlet.")));
    }

    // ================= катализатор =================

    // CO окисляют только блоки «с драгметаллами», NO и NO2 восстанавливают только блоки «без драгметаллов»
    // (см. GeneralManagerForKataz.TryCalculateRow). Число блоков и температура — из той же формулы Decay.
    private static void AdviseCatalyst(GeneralManagerForKataz kataz, string id, float cIn, float target,
        ProcessLimits limits, Translator.Language lang, List<Recommendation> recs)
    {
        var math = kataz.MathModule;
        if (math == null) return;

        bool co = IsId(id, "CO");
        bool nox = IsId(id, "NO") || IsId(id, "NO2");
        if (!co && !nox) return;

        double flow = math.valueGasFlow;
        if (flow <= 0 || cIn <= 0) return;

        double temp = kataz.CatalystTemperature;
        double k = co ? GeneralManagerForKataz.RateConstantCO(temp) : GeneralManagerForKataz.RateConstantNO(temp);
        int current = math.BlockCount;
        int max = Mathf.Max(limits.catalystMaxBlocks, 1);
        double needed = GeneralManagerForKataz.BlocksRequired(k, cIn, target, flow);
        int blocks = needed > max ? max : Mathf.Max(Mathf.CeilToInt((float)needed), 1);
        string name = MonitoringText.ParameterName(id, lang);

        bool rightType = co ? math.HasPreciousMetals : math.HasNoPreciousMetals;
        if (!rightType)
        {
            int n = Mathf.Clamp(Mathf.Max(blocks, current), 1, max);
            string after = MonitoringText.Number(GeneralManagerForKataz.ConcentrationAfter(k, cIn, n, flow));
            string text = co
                ? MonitoringText.Pick(lang,
                    $"Сменить тип блоков на «с драгметаллами» ({n} бл.): CO окисляется только на них. CO на выходе катализатора ≈ {after}.",
                    $"Блок түрін «бағалы металдармен» етіп ауыстыру ({n} блок): CO тек соларда тотығады. Катализатор шығысында CO ≈ {after}.",
                    $"Switch blocks to \"with precious metals\" ({n} blocks): only they oxidise CO. CO at the catalyst outlet ≈ {after}.")
                : MonitoringText.Pick(lang,
                    $"Сменить тип блоков на «без драгметаллов» ({n} бл.): {name} на выходе катализатора ≈ {after}. " +
                    "CO на таких блоках не окисляется — при высоком CO нужен второй катализатор с драгметаллами.",
                    $"Блок түрін «бағалы металдарсыз» етіп ауыстыру ({n} блок): катализатор шығысында {name} ≈ {after}. " +
                    "Мұндай блоктарда CO тотықпайды — CO жоғары болса, бағалы металдары бар екінші катализатор қажет.",
                    $"Switch blocks to \"without precious metals\" ({n} blocks): {name} at the catalyst outlet ≈ {after}. " +
                    "These blocks do not oxidise CO — if CO is high, add a second catalyst with precious metals.");

            recs.Add(new Recommendation(text, () =>
            {
                math.SetBlockType(co);
                math.SetBlockCount(n);
            }));

            if (needed > max)
                AddTemperatureHint(co, cIn, target, max, flow, temp, lang, recs);
            return;
        }

        // Тип блоков подходит — не хватает объёма катализатора или температуры.
        if (needed <= current) return;

        if (current < max)
        {
            int n = blocks;
            string after = MonitoringText.Number(GeneralManagerForKataz.ConcentrationAfter(k, cIn, n, flow));
            recs.Add(new Recommendation(MonitoringText.Pick(lang,
                $"Увеличить число блоков катализатора: {current} → {n}. {name} на выходе катализатора ≈ {after}.",
                $"Катализатор блоктарының санын көбейту: {current} → {n}. Катализатор шығысында {name} ≈ {after}.",
                $"Increase catalyst blocks: {current} → {n}. {name} at the catalyst outlet ≈ {after}."),
                () => math.SetBlockCount(n)));
        }

        if (needed > max)
            AddTemperatureHint(co, cIn, target, max, flow, temp, lang, recs);
    }

    // Блоков не хватит даже при максимуме: какая нужна температура (k = A·exp(−E/RT), решённое относительно T).
    private static void AddTemperatureHint(bool co, float cIn, float target, int blocks, double flow, double currentTemp,
        Translator.Language lang, List<Recommendation> recs)
    {
        double rate = GeneralManagerForKataz.RateRequired(cIn, target, blocks, flow);
        double temp = co
            ? GeneralManagerForKataz.TemperatureForRateCO(rate)
            : GeneralManagerForKataz.TemperatureForRateNO(rate);

        if (double.IsNaN(temp))
        {
            recs.Add(new Recommendation(MonitoringText.Pick(lang,
                $"Даже {blocks} блоков не хватит ни при какой температуре: снизьте расход газа или поставьте второй катализатор.",
                $"{blocks} блок кез келген температурада да жеткіліксіз: газ шығынын азайтыңыз немесе екінші катализатор орнатыңыз.",
                $"Even {blocks} blocks are not enough at any temperature: reduce the gas flow or add a second catalyst.")));
            return;
        }

        string now = Degrees(currentTemp);
        string need = Degrees(temp);
        string inlet = Degrees(temp / GeneralManagerForKataz.HeatingFactor);
        recs.Add(new Recommendation(MonitoringText.Pick(lang,
            $"При {now} °C в катализаторе даже {blocks} блоков мало. Нужно ≈ {need} °C — подогрейте газ на входе катализатора до ≈ {inlet} °C.",
            $"Катализаторда {now} °C болғанда {blocks} блок та аз. ≈ {need} °C қажет — катализатор кірісіндегі газды ≈ {inlet} °C дейін қыздырыңыз.",
            $"At {now} °C in the catalyst even {blocks} blocks are not enough. About {need} °C is needed — heat the gas at the catalyst inlet to ≈ {inlet} °C.")));
    }

    // ================= эмульгатор =================

    // Из газов эмульгатор поглощает NO2, SO2 и H2S; реагент — намного сильнее воды (GeneralManagerForEmul.TryCalculateRow).
    private static void AdviseEmulsifier(GeneralManagerForEmul emul, string id, float cOut, Translator.Language lang,
        List<Recommendation> recs)
    {
        bool absorbed = IsId(id, "SO2") || IsId(id, "NO2") || IsId(id, "H2S");
        var math = emul.MathModule;
        if (!absorbed || math == null || !math.IsWater) return;

        string after = MonitoringText.Number(cOut * GeneralManagerForEmul.ReagentPassRatio / GeneralManagerForEmul.WaterPassRatio);
        string reagent = Percent(1f - GeneralManagerForEmul.ReagentPassRatio);
        string water = Percent(1f - GeneralManagerForEmul.WaterPassRatio);

        recs.Add(new Recommendation(MonitoringText.Pick(lang,
            $"Заменить воду на едкий натрий: {id} на выходе эмульгатора ≈ {after} (реагент поглощает {reagent}%, вода — {water}%).",
            $"Суды каустикалық содаға ауыстыру: эмульгатор шығысында {id} ≈ {after} (реагент {reagent}%, су {water}% сіңіреді).",
            $"Replace water with caustic soda: {id} at the emulsifier outlet ≈ {after} (reagent absorbs {reagent}%, water {water}%)."),
            () => emul.SelectFluid(GeneralManagerForEmul.FluidCausticSoda)));
    }

    // ================= помощники =================

    private static int IndexOf(IReadOnlyList<GeneralManagerBase> chain, GeneralManagerBase module)
    {
        for (int i = 0; i < chain.Count; i++)
            if (chain[i] == module) return i;
        return -1;
    }

    private static bool IsId(string id, string expected) => string.Equals(id, expected, StringComparison.OrdinalIgnoreCase);

    private static string Degrees(double value) => Math.Round(value).ToString("0", CultureInfo.InvariantCulture);

    private static string Percent(float share) => Mathf.RoundToInt(share * 100f).ToString(CultureInfo.InvariantCulture);
}
