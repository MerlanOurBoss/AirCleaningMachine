using UnityEngine;

public class GeneralManagerForElectroFilter : ModuleManagerBase<MathModuleForElectro>
{
    [Header("ElectroFilter")]
    [SerializeField] private ParticleSystem[] electroFilterSmokes;
    [SerializeField] private Animator electroFilterAnimation;

    protected override void OnMathModuleReady(MathModuleForElectro module)
    {
        module._electroFilterAnimator = electroFilterAnimation;
        module._smokeParticles = electroFilterSmokes;
    }

    protected override bool TryCalculateRow(ParameterRowUI row, float originalValue)
    {
        switch (row.id)
        {
            case "Температура":
                float newTemperature = originalValue - (0.5f * (float)MathModule.length * 4);
                row.valueTextOut.text = Mathf.Ceil(newTemperature).ToString();
                return true;

            case "Твердые частицы":
                row.valueTextOut.text = Mathf.Ceil(originalValue * 0.3f).ToString(); // 30%
                return true;

            default:
                return false;
        }
    }

    protected override void OnStartModule()
    {
        if (electroFilterSmokes != null)
            foreach (var smoke in electroFilterSmokes)
                if (smoke != null) smoke.Play();

        if (electroFilterAnimation != null)
            electroFilterAnimation.Play("NewColecAnim");
    }

    protected override void OnStopModule()
    {
        if (electroFilterSmokes != null)
            foreach (var smoke in electroFilterSmokes)
                if (smoke != null) smoke.Stop();

        if (electroFilterAnimation != null)
            electroFilterAnimation.Play("NewColecAnimStop");
    }
}