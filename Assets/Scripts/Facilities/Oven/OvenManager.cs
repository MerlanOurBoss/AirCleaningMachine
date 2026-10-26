using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvenManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Toggle[] toggles;

    [Header("Particle System")]
    public ParticleSystem smoke;

    private readonly Color defaultColor = new Color(0.6075471f, 0.6075471f, 0.6075471f);
    private readonly Color electrofilterColor = new Color(0.6235294f, 0.4130386f, 0);
    private readonly Color katalizatorColor = new Color(0.7450981f, 0.8078431f, 0.7176471f);
    private readonly Color emulgatorColor = new Color(0.5686275f, 0.7529412f, 0.8078431f);
    private readonly Color sborCO2Color = new Color(0.7764706f, 0.9215686f, 1f);

    void Update()
    {
        UpdateSmokeColor();
    }

    private void UpdateSmokeColor()
    {
        if (!AnyToggleOn())
        {
            smoke.startColor = defaultColor;
            return;
        }

        if (toggles[0].isOn)
        {
            smoke.startColor = electrofilterColor;
        }
        else if (toggles[1].isOn)
        {
            smoke.startColor = katalizatorColor;
        }
        else if (toggles[2].isOn || toggles[3].isOn || toggles[4].isOn)
        {
            smoke.startColor = emulgatorColor;
        }
        else if (toggles[5].isOn)
        {
            smoke.startColor = sborCO2Color;
        }
    }

    private bool AnyToggleOn()
    {
        foreach (var toggle in toggles)
        {
            if (toggle.isOn) return true;
        }
        return false;
    }
}
