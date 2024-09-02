using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemperatureCatalizator : MonoBehaviour
{
    public Material firstMaterial;
    public Material secondMaterial;
    public Material thirdMaterial;
    [SerializeField] private ParticleSystem[] fire;
    public Animator anim;
    public TextMeshProUGUI texTemp1;
    public TextMeshProUGUI texTemp2;

    public Color originColor;
    public KatalizatorScript kataz;

    private float delay = 6f;
    private float temp1 = 26f;
    private float temp2 = 26f;
    private bool canTemp1 = false;
    private bool canTemp2 = false;
    private bool canStart = false;

    private void Start()
    {
        ResetMaterials();
    }

    private void Update()
    {
        if (!canStart) return;

        delay -= Time.deltaTime;
        UpdateTemperatureDisplay();

        if (!kataz.isSecond)
        {
            UpdateMaterialColorsAndTemperatures(firstMaterial, secondMaterial, thirdMaterial, 500f, 320f);
        }
        else
        {
            UpdateMaterialColorsAndTemperatures(firstMaterial, secondMaterial, thirdMaterial, 0f, 320f, isSecondMode: true);
        }

        HandleTemperaturePhases();
    }

    private void UpdateTemperatureDisplay()
    {
        texTemp1.text = $"{temp1:0}°C";
        texTemp2.text = $"{temp2:0}°C";
    }

    private void UpdateMaterialColorsAndTemperatures(Material firstMat, Material secondMat, Material thirdMat, float targetTemp1, float targetTemp2, bool isSecondMode = false)
    {
        if (canTemp1)
        {
            thirdMat.color = Color.Lerp(thirdMat.color, Color.white, Time.deltaTime);
            if (!isSecondMode) temp1 = Mathf.Lerp(temp1, targetTemp1, 1.5f * Time.deltaTime);
        }

        if (canTemp2)
        {
            firstMat.color = Color.Lerp(firstMat.color, Color.white, Time.deltaTime);
            secondMat.color = Color.Lerp(secondMat.color, Color.white, Time.deltaTime);
            temp2 = Mathf.Lerp(temp2, targetTemp2, 1.5f * Time.deltaTime);
        }
    }

    private void HandleTemperaturePhases()
    {
        if (delay < 5f && !canTemp1)
        {
            temp2 = Mathf.Lerp(temp2, 160f, 1.5f * Time.deltaTime);
        }
        if (delay < 3f && !canTemp2)
        {
            temp1 = Mathf.Lerp(temp1, 160f, 1.5f * Time.deltaTime);
        }
        if (delay < 0f)
        {
            canTemp1 = true;
        }
        if (delay < -2f)
        {
            canTemp2 = true;
        }
    }

    public void StartSimulation()
    {
        anim.Play("Catalizaotr_Anim");
        canStart = true;
        PlayParticles();
    }

    public void StopSimulation()
    {
        StopParticles();
        ResetSimulationState();
    }

    private void ResetSimulationState()
    {
        delay = 6f;
        canStart = false;
        canTemp1 = false;
        canTemp2 = false;
        ResetMaterials();
        anim.StopPlayback();
    }

    private void ResetMaterials()
    {
        firstMaterial.color = originColor;
        secondMaterial.color = originColor;
        thirdMaterial.color = originColor;
    }

    private void PlayParticles()
    {
        foreach (var item in fire)
        {
            item.Play();
        }
    }

    private void StopParticles()
    {
        foreach (var item in fire)
        {
            item.Stop();
        }
    }
}