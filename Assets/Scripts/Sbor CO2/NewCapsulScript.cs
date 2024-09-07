using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewCapsulScript : MonoBehaviour
{
    public bool isTriggerFromOven = false;

    [Header("Particle Systems")]
    public ParticleSystem smokeInCapsul;
    public ParticleSystem smokeOutCapsul;
    public ParticleSystem smokeOutCapsulSecond;
    public ParticleSystem smokeParInCapsul;

    [Header("UI Elements")]
    public TextMeshProUGUI display;
    public TextMeshProUGUI display2;

    [Header("Materials")]
    public Material absent;
    public Material absent2;
    public Color targetColor;

    [Header("Gate Configuration")]
    public GameObject gate;
    public GameObject[] gates;

    private float fillingCount = 0;
    private int state = 0;

    private float displayValue = 0f;
    private float displayValue2 = 0f;

    private bool isFilling = true;
    private bool isProcessActive = false;
    private bool isDelayActive = false;

    public float timingDelay = 150f;

    void Start()
    {
        InitializeGates();
        absent.color = Color.white;
        absent2.color = Color.white;
    }

    void Update()
    {
        UpdateDisplay();
        HandleDelay();
        HandleFillingProcess();
        HandleUnfillingProcess();
    }

    private void InitializeGates()
    {
        SetGatesState(new[] { true, false, false, true, true, false, false, true });
    }

    private void UpdateDisplay()
    {
        display.text = displayValue.ToString("0.");
        display2.text = displayValue2.ToString("0.");
    }

    private void HandleDelay()
    {
        if (isTriggerFromOven)
        {
            isProcessActive = true;
            isDelayActive = false;
        }
    }

    private void HandleFillingProcess()
    {
        if (isProcessActive && state == 0 && isFilling)
        {
            fillingCount += 5 * Time.deltaTime;
            PlayFillingEffects();
            UpdateFillingDisplay();

            if (fillingCount >= timingDelay)
            {
                TransitionToUnfilling();
            }
        }
    }

    private void HandleUnfillingProcess()
    {
        if (isProcessActive && state == 1 && !isFilling)
        {
            fillingCount += 5 * Time.deltaTime;
            PlayUnfillingEffects();
            UpdateUnfillingDisplay();

            if (fillingCount >= timingDelay)
            {
                TransitionToFilling();
            }
        }
    }

    private void PlayFillingEffects()
    {
        if (fillingCount < 1)
        {
            smokeInCapsul.Play();
            smokeOutCapsul.Play();
            smokeParInCapsul.Stop();
            smokeOutCapsulSecond.Stop();
        }

        if (fillingCount >= ((timingDelay / 2) - 15) && fillingCount < ((timingDelay / 2) + 5))
        {
            displayValue = Mathf.Lerp(displayValue, 100f, 2 * Time.deltaTime);
            displayValue2 = Mathf.Lerp(displayValue2, 0f, 2 * Time.deltaTime);
        }

        AbsentOn();
        AbsentOff2();
        ActivateGatesForFilling();
    }

    private void ActivateGatesForFilling()
    {
        SetGatesState(new[] { false, true, false, true, true, false, true, false });
    }

    private void UpdateFillingDisplay()
    {
        if (fillingCount >= ((timingDelay / 2) - 15) && fillingCount < ((timingDelay / 2) + 5))
        {
            displayValue = Mathf.Lerp(displayValue, 100f, 2 * Time.deltaTime);
            displayValue2 = Mathf.Lerp(displayValue2, 0f, 2 * Time.deltaTime);
        }
    }

    private void PlayUnfillingEffects()
    {
        gate.SetActive(true);
        smokeParInCapsul.Play();
        smokeOutCapsulSecond.Play();
        smokeInCapsul.Stop();
        smokeOutCapsul.Stop();

        if (fillingCount >= ((timingDelay / 2) - 15) && fillingCount < ((timingDelay / 2) + 5))
        {
            displayValue = Mathf.Lerp(displayValue, 0f, 2 * Time.deltaTime);
            displayValue2 = Mathf.Lerp(displayValue2, 100f, 2 * Time.deltaTime);
        }

        AbsentOff();
        AbsentOn2();
        ActivateGatesForUnfilling();
    }

    private void ActivateGatesForUnfilling()
    {
        SetGatesState(new[] { true, false, true, false, false, true, false, true });
    }

    private void UpdateUnfillingDisplay()
    {
        if (fillingCount >= ((timingDelay / 2) - 15) && fillingCount < ((timingDelay / 2) + 5))
        {
            displayValue = Mathf.Lerp(displayValue, 0f, 2 * Time.deltaTime);
            displayValue2 = Mathf.Lerp(displayValue2, 100f, 2 * Time.deltaTime);
        }
    }

    private void TransitionToUnfilling()
    {
        isFilling = false;
        state = 1;
        fillingCount = 0;
        //SetGatesState(new[] { false, true, false, true, true, false, false, true });
    }

    private void TransitionToFilling()
    {
        isFilling = true;
        state = 0;
        fillingCount = 0;
        //SetGatesState(new[] { true, false, true, false, false, true, true, false });
    }

    private void SetGatesState(bool[] states)
    {
        for (int i = 0; i < gates.Length; i++)
        {
            gates[i].SetActive(states[i]);
        }
    }

    public void AbsentOn()
    {
        absent.color = Color.Lerp(absent.color, targetColor, 1f * Time.fixedDeltaTime);
    }

    public void AbsentOff()
    {
        absent.color = Color.Lerp(absent.color, Color.white, .15f * Time.fixedDeltaTime);
    }

    public void AbsentOn2()
    {
        absent2.color = Color.Lerp(absent2.color, targetColor, 1f * Time.fixedDeltaTime);
    }

    public void AbsentOff2()
    {
        absent2.color = Color.Lerp(absent2.color, Color.white, .15f * Time.fixedDeltaTime);
    }

    public void StopColumnProcess()
    {
        isProcessActive = false;
        isDelayActive = false;
        fillingCount = 0;
        gate.SetActive(true);

        smokeOutCapsul.Stop();
        smokeInCapsul.Stop();

        AbsentOff();
        AbsentOff2();
        displayValue = 0f;
        displayValue2 = 0f;
    }
}
