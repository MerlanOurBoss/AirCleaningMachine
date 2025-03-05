using System.Collections;
using TMPro;
using UnityEngine;

public class NewSborScript : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem smokeParInCapsul;
    public ParticleSystem smokeParInCapsul2;
    public ParticleSystem smokeOutCapsul;
    public ParticleSystem smokeOutCapsulSecond;
    public ParticleSystem smokeInCapsul;
    public ParticleSystem smokeInCapsul2;
    public ParticleSystem parInCapsul1;
    public ParticleSystem parInCapsul2;
    public ParticleSystem parInStraightPipe;

    [Header("UI Elements")]
    public TextMeshProUGUI display;
    public TextMeshProUGUI display2;
    public TextMeshProUGUI display3;

    [Header("Materials")]
    public Material absent;
    public Material absent2;
    public Material materialSbor;
    public Color targetColor;

    [Header("Gate Configuration")]
    public GameObject gate;
    public GameObject[] gates;

    [Header("Timing")]
    public float delay;
    private float delayUpdate;

    [Header("Uroven Animation")]
    public Animator uroven1;
    public Animator uroven2;

    private float fillingCount = 0;
    private int state = 0;

    private float displayValue = 0f;
    private float displayValue2 = 0f;
    private float displayValue3 = 0f;

    private bool isFilling = true;
    private bool isProcessActive = false;
    private bool isDelayActive = false;
    private bool isPaused = false; // Флаг паузы

    public float timingDelay = 150f;
    private bool isUpdated = false;
    private bool isFirstFilling = true;
    private int? randomValue = null;

    void Start()
    {
        InitializeGates();
        absent.color = Color.white;
        absent2.color = Color.white;
    }

    void Update()
    {
        if (isPaused) return; // Если пауза активна, не выполняем обновления

        UpdateDisplay();
        HandleDelay();

        if (isFirstFilling)
        {
            HandleFirstFillingProcess(); // Отдельный метод для первого заполнения
        }
        else
        {
            HandleFillingProcess();
            HandleUnfillingProcess();
        }
    }

    private void InitializeGates()
    {
        SetGatesState(new[] { false, true, false, true, false, true, false, true, false, true, false , true});
    }

    private void UpdateDisplay()
    {
        display.text = displayValue.ToString("0.");
        display2.text = displayValue2.ToString("0.");
        display3.text = displayValue3.ToString("0.");
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

    private void HandleFirstFillingProcess()
    {
        if (isProcessActive && state == 0 && isFilling)
        {
            fillingCount += 5 * Time.deltaTime;
            PlayFirstFillingEffects();

            if (fillingCount >= timingDelay)
            {
                isFirstFilling = false; // После первой половины переключаемся на обычный процесс
            }
        }
    }

    private void HandleFillingProcess()
    {
        if (isProcessActive && state == 0 && isFilling)
        {
            fillingCount += 5 * Time.deltaTime;

            if (isFirstFilling)
            {
                PlayFirstFillingEffects(); // Запускаем первый процесс заполнения
                if (fillingCount >= timingDelay)
                {
                    isFirstFilling = false; // После первой половины переключаемся
                }
            }
            else
            {
                PlayFillingEffects();
            }

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

            if (fillingCount >= timingDelay)
            {
                TransitionToFilling();
            }
        }
    }

    private void PlayFirstFillingEffects()
    {
        if (fillingCount >= 135 && fillingCount <= 150)
        {
            displayValue = Mathf.Lerp(displayValue, 50f, 1 * Time.deltaTime);
        }

        if (fillingCount >= 135 && fillingCount <= 150)
        {
            if (randomValue == null) // Генерируем случайное число только один раз
            {
                randomValue = Random.Range(1, 10);
            }

            if (randomValue == 9)
            {
                displayValue3 = Mathf.Lerp(displayValue3, 50f, 30 * Time.deltaTime);
                gates[12].SetActive(false);
                gates[13].SetActive(true);
                parInStraightPipe.Play();
            }
        }
        else
        {
            displayValue3 = Mathf.Lerp(displayValue3, 0f, 30 * Time.deltaTime);
            gates[12].SetActive(true);
            gates[13].SetActive(false);
            randomValue = null; // Сбрасываем число, если не в диапазоне
        }

        smokeInCapsul.Play();
        AbsentOn();
        ActivateGatesForFirstFilling();
    }
    private void PlayFillingEffects()
    {
        if (fillingCount >= 2 && fillingCount <= 8)
        {
            displayValue2 = Mathf.Lerp(displayValue2, 0, 4f * Time.deltaTime);
            parInCapsul1.Stop();
            parInCapsul2.Stop();
            parInStraightPipe.Stop();
        }
        
        smokeParInCapsul2.Play();
        smokeOutCapsul.Stop();
        smokeParInCapsul.Stop();

        if (fillingCount >= 25 && fillingCount < 27)
        {
            smokeOutCapsulSecond.Play();
        }

        if (fillingCount >= 45 && fillingCount < 50)
        {
            parInCapsul1.Play();
        }

        if (fillingCount >= 135 && fillingCount <= 150)
        {
            displayValue = Mathf.Lerp(displayValue, 50f, 2f * Time.deltaTime);

            if (!isUpdated)
            {
                float a = materialSbor.GetFloat("_Filling");
                materialSbor.SetFloat("_Filling", a + 10f);
                isUpdated = true;
            }
        }
        if (fillingCount >= 135 && fillingCount <= 150)
        {
            if (randomValue == null) // Генерируем случайное число только один раз
            {
                randomValue = Random.Range(1, 10);
            }

            if (randomValue == 9)
            {
                displayValue3 = Mathf.Lerp(displayValue3, 50f, 30 * Time.deltaTime);
                gates[12].SetActive(false);
                gates[13].SetActive(true);
                parInStraightPipe.Play();
            }
        }
        else
        {
            displayValue3 = Mathf.Lerp(displayValue3, 0f, 30 * Time.deltaTime);
            gates[12].SetActive(true);
            gates[13].SetActive(false);
            randomValue = null; // Сбрасываем число, если не в диапазоне
        }


        smokeInCapsul.Play();
        smokeInCapsul2.Stop();

        AbsentOn();
        AbsentOff2();
        ActivateGatesForFilling();
    }

    private void ActivateGatesForFirstFilling()
    {
        SetGatesState(new[] { true, false, true, false, false, true, false, true, true, false, false, true });
    }

    private void ActivateGatesForFilling()
    {
        SetGatesState(new[] { true, false, true, false, false, true, true, false, true, false, false, true });
    }

    private void PlayUnfillingEffects()
    {
        if (fillingCount >= 2 && fillingCount <= 8)
        {
            displayValue = Mathf.Lerp(displayValue, 0, 4f * Time.deltaTime);
            parInCapsul2.Stop();
            parInCapsul1.Stop();
            parInStraightPipe.Stop();
        }
        gate.SetActive(true);
        smokeParInCapsul.Play();
        smokeOutCapsulSecond.Stop();
        smokeParInCapsul2.Stop();

        if (fillingCount >= 25 && fillingCount < 27)
        {
            smokeOutCapsul.Play();
        }

        if (fillingCount >= 45 && fillingCount < 50)
        {
            parInCapsul2.Play();
        }

        if (fillingCount >= 135 && fillingCount <= 150)
        {
            displayValue2 = Mathf.Lerp(displayValue2, 50f, 2f * Time.deltaTime);
            
            if (!isUpdated)
            {
                float a = materialSbor.GetFloat("_Filling");
                materialSbor.SetFloat("_Filling", a + 10f);
                isUpdated = true;
            }
        }

        if (fillingCount >= 135 && fillingCount <= 150)
        {
            if (randomValue == null) // Генерируем случайное число только один раз
            {
                randomValue = Random.Range(1, 10);
            }

            if (randomValue == 9)
            {
                displayValue3 = Mathf.Lerp(displayValue3, 50f, 30 * Time.deltaTime);
                gates[12].SetActive(false);
                gates[13].SetActive(true);
                parInStraightPipe.Play();
            }
        }
        else
        {
            displayValue3 = Mathf.Lerp(displayValue3, 0f, 30 * Time.deltaTime);
            gates[12].SetActive(true);
            gates[13].SetActive(false);
            randomValue = null; // Сбрасываем число, если не в диапазоне
        }

        smokeInCapsul2.Play();
        smokeInCapsul.Stop();
        AbsentOff();
        AbsentOn2();
        ActivateGatesForUnfilling();
    }

    private void ActivateGatesForUnfilling()
    {
        SetGatesState(new[] { true, false, false, true, true, false, true, false, false, true, true, false });
    }

    private void TransitionToUnfilling()
    {
        isFilling = false;
        state = 1;
        fillingCount = 0;
        isUpdated = false;

        smokeInCapsul.Stop();
        smokeInCapsul2.Stop();
    }

    private void TransitionToFilling()
    {
        isFilling = true;
        state = 0;
        fillingCount = 0;
        isUpdated = false;
    }

    private void SetGatesState(bool[] states)
    {
        for (int i = 0; i < gates.Length-2; i++)
        {
            gates[i].SetActive(states[i]);
        }
    }

    public void AbsentOn()
    {
        absent.color = Color.Lerp(absent.color, targetColor, .30f * Time.fixedDeltaTime);
        uroven1.Play("Open");
    }

    public void AbsentOff()
    {
        absent.color = Color.Lerp(absent.color, Color.white, .15f * Time.fixedDeltaTime);
        uroven1.Play("Close");
    }

    public void AbsentOn2()
    {
        absent2.color = Color.Lerp(absent2.color, targetColor, .30f * Time.fixedDeltaTime);
        uroven2.Play("Open");
    }

    public void AbsentOff2()
    {
        absent2.color = Color.Lerp(absent2.color, Color.white, .15f * Time.fixedDeltaTime);
        uroven2.Play("Close");
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
        materialSbor.SetFloat("_Filling", -30f);
        delay = 6;

        smokeOutCapsul.Stop();
        smokeParInCapsul2.Stop();
        smokeInCapsul.Stop();
        smokeInCapsul2.Stop();

        absent.color = Color.white;
        absent2.color = Color.white;
        displayValue = 0f;
        displayValue2 = 0f;
    }

    public void PauseProcess()
    {
        isPaused = true;
    }

    public void ResumeProcess()
    {
        StartCoroutine(ResumeAfterDelay());
    }

    private IEnumerator ResumeAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        isPaused = false;
    }
}
