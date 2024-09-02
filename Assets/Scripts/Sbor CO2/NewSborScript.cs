using TMPro;
using UnityEngine;

public class NewSborScript : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem smokeInCapsul;
    public ParticleSystem smokeOutCapsul;
    public ParticleSystem smokeOutCapsulSecond;
    public ParticleSystem smokeParInCapsul;

    [Header("UI Elements")]
    public TextMeshProUGUI display;

    [Header("Materials")]
    public Material absent;
    public Color targetColor;

    [Header("Gate Configuration")]
    public GameObject gate;
    public GameObject[] gates;

    [Header("Timing")]
    public float delay;
    private float delayUpdate;

    private float fillingCount = 0;
    private int state = 0;

    private float displayValue = 0f;

    private bool isFilling = true;
    private bool isProcessActive = false;
    private bool isDelayActive = false;

    public float timingDelay = 150f;

    void Start()
    {
        InitializeGates();
        absent.color = Color.white;
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
        SetGatesState(new[] { true, false, true, false, true, false });
    }

    private void UpdateDisplay()
    {
        display.text = displayValue.ToString("0.");
    }

    private void HandleDelay()
    {
        if (isDelayActive)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                isProcessActive = true;
                isDelayActive = false;
            }
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

        if (fillingCount >= ((timingDelay/2) - 15) && fillingCount < ((timingDelay / 2) + 5))
        {
            displayValue = Mathf.Lerp(displayValue, 100f, 2 * Time.deltaTime);
        }

        AbsentOn();
        Invoke("ActivateGatesForFilling", 5f);
    }

    private void ActivateGatesForFilling()
    {
        SetGatesState(new[] { true, false, true, false, false, true });
    }

    private void UpdateFillingDisplay()
    {
        if (fillingCount >= ((timingDelay / 2) - 15) && fillingCount < ((timingDelay / 2) + 5))
        {
            displayValue = Mathf.Lerp(displayValue, 100f, 2 * Time.deltaTime);
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
        }

        AbsentOff();
        Invoke("ActivateGatesForUnfilling", 5f);
    }

    private void ActivateGatesForUnfilling()
    {
        SetGatesState(new[] { false, true, false, true, true, false });
    }

    private void UpdateUnfillingDisplay()
    {
        if (fillingCount >= ((timingDelay / 2) - 15) && fillingCount < ((timingDelay / 2) + 5))
        {
            displayValue = Mathf.Lerp(displayValue, 0f, 2 * Time.deltaTime);
        }
    }

    private void TransitionToUnfilling()
    {
        isFilling = false;
        state = 1;
        fillingCount = 0;
        SetGatesState(new[] { false, true, false, true, true, false });
    }

    private void TransitionToFilling()
    {
        isFilling = true;
        state = 0;
        fillingCount = 0;
        SetGatesState(new[] { true, false, true, false, false, true });
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

    public void StartColumnProcess()
    {
        isDelayActive = true;
        delayUpdate = delay;
    }

    public void StopColumnProcess()
    {
        isProcessActive = false;
        isDelayActive = false;
        fillingCount = 0;
        gate.SetActive(true);

        delay = delayUpdate;

        smokeOutCapsul.Stop();
        smokeInCapsul.Stop();

        AbsentOff();
        displayValue = 0f;
    }
}
