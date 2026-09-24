using UnityEngine;

public class GeneralManagerForSborCO2 : ModuleManagerBase<MathModulForSborCO2>
{
    [Header("SborCO2")]
    [SerializeField] private ParticleSystem[] sborSmokes;
    [SerializeField] private Animator[] sborFans;
    [SerializeField] private SborCO2Controller sborScript;

    protected override void OnMathModuleReady(MathModulForSborCO2 module)
    {
        module._smokes = sborSmokes;
        module.fansAnim = sborFans;
        module.newSbor = sborScript;
    }

    protected override bool TryCalculateRow(ParameterRowUI row, float originalValue)
    {
        switch (row.id)
        {
            case "Температура":
                row.valueTextOut.text = Mathf.Ceil(originalValue * 0.7f).ToString();
                return true;

            case "CO2":
                float co2 = originalValue * (MathModule.co2FlowRate - MathModule.capturedCO2) / MathModule.co2FlowRate;
                row.valueTextOut.text = Mathf.Ceil(co2).ToString();
                return true;

            default:
                return false;
        }
    }

    protected override void OnStartModule()
    {
        sborScript?.StartColumnProcess();

        if (sborFans == null) return;
        foreach (var fan in sborFans)
            if (fan != null) fan.Play("BigFan");
    }

    protected override void OnStopModule()
    {
        sborScript?.StopColumnProcess();

        if (sborFans == null) return;
        foreach (var fan in sborFans)
            if (fan != null) fan.Play("Stop");
    }
}