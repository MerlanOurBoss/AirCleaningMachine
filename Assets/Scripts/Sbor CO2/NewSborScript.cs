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
    public TextMeshProUGUI display4;
    public TextMeshProUGUI display5;

    [Header("Materials")]
    public Material absent;
    public Material absent2;
    public Material materialSbor;
    public Color targetColor;

    [Header("Gate Configuration")]
    public GameObject gate;
    public GameObject[] gates;
    public GameObject[] gazAnalyz;

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
    private float displayValue4 = 0f;
    private float displayValue5 = 0f;

    private bool isFilling = true;
    private bool isProcessActive = false;
    private bool isDelayActive = false;
    private bool isPaused = false; // Флаг паузы

    public float timingDelay = 150f;
    private bool isUpdated = false;
    private bool isFirstFilling = true;
    private int? randomValue = null;

    public float yellowBlinkDuration = 2f; // Время мигания желтой лампочки
    public float yellowBlinkInterval = 0.3f; // Интервал мигания желтой лампочки
    void Start()
    {
        absent.color = Color.white;
        absent2.color = Color.white;
        gazAnalyz[0].SetActive(true);
        gazAnalyz[1].SetActive(false);
        gazAnalyz[2].SetActive(true);
        gazAnalyz[3].SetActive(false);
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

    public void StartRedSequence(Transform valve)
    {
        GameObject greenLamp = valve.Find("Green Light")?.gameObject;
        GameObject yellowLamp = valve.Find("Yellow Light")?.gameObject;
        GameObject redLamp = valve.Find("Red Light")?.gameObject;
        AudioSource audio = valve.GetComponent<AudioSource>();

        StartCoroutine(RedGateSequence(greenLamp, yellowLamp, redLamp, audio));
    }

    public void StartGreenSequence(Transform valve)
    {
        GameObject greenLamp = valve.Find("Green Light")?.gameObject;
        GameObject yellowLamp = valve.Find("Yellow Light")?.gameObject;
        GameObject redLamp = valve.Find("Red Light")?.gameObject;
        AudioSource audio = valve.GetComponent<AudioSource>();

        StartCoroutine(GreenGateSequence(greenLamp, yellowLamp, redLamp, audio));
    }

    IEnumerator RedGateSequence(GameObject greenLamp, GameObject yellowLamp, GameObject redLamp, AudioSource audio)
    {
         // Выключаем зеленую лампочку
        yield return StartCoroutine(BlinkYellowLamp(yellowLamp));
        if (yellowLamp != null) yellowLamp.SetActive(false); // Выключаем желтую лампочку
        if (audio != null) audio.Play();
        if (redLamp != null) redLamp.SetActive(true);
        if (greenLamp != null) greenLamp.SetActive(false);// Включаем красную лампочку
    }

    IEnumerator GreenGateSequence(GameObject greenLamp, GameObject yellowLamp, GameObject redLamp, AudioSource audio)
    {
         // Выключаем красную лампочку
        yield return StartCoroutine(BlinkYellowLamp(yellowLamp));
        if (yellowLamp != null) yellowLamp.SetActive(false); // Выключаем желтую лампочку
        if (audio != null) audio.Play();
        if (greenLamp != null) greenLamp.SetActive(true);
        if (redLamp != null) redLamp.SetActive(false); // Включаем зеленую лампочку
    }

    IEnumerator BlinkYellowLamp(GameObject yellowLamp)
    {
        float elapsedTime = 0f;
        bool isLampOn = false;
        
        while (elapsedTime < yellowBlinkDuration)
        {
            isLampOn = !isLampOn;
            if (yellowLamp != null) yellowLamp.SetActive(isLampOn);
            yield return new WaitForSeconds(yellowBlinkInterval);
            elapsedTime += yellowBlinkInterval;
        }
        yellowLamp.SetActive(false);
    }

    private void UpdateDisplay()
    {
        display.text = displayValue.ToString("0.");
        display2.text = displayValue2.ToString("0.");
        display3.text = displayValue3.ToString("0.");
        display4.text = displayValue4.ToString("0." + " °C");
        display5.text = displayValue5.ToString("0." + " °C");
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
            else if (delay >= 3 && delay <= 5)
            {
                displayValue4 = Mathf.Lerp(displayValue4, 540f, 8 * Time.deltaTime);
            }
            else if (delay >= 1 && delay <= 3)
            {
                displayValue5 = Mathf.Lerp(displayValue5, 40f, 8 * Time.deltaTime);
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
        bool isStarted = false;

        if (fillingCount >= 0 && fillingCount <= 2)
        {
            StartGreenSequence(gates[0].transform);
            StartGreenSequence(gates[1].transform);
        }
        if (fillingCount >= 15 && fillingCount <= 17)
        {
            isStarted = true;
        }
        if (isStarted)
        {
            smokeInCapsul.Play();
            AbsentOn();
        }

        if (fillingCount >= 120 && fillingCount <= 135)
        {
            gazAnalyz[3].GetComponentInParent<AudioSource>().Play();
        }

        if (fillingCount >= 135 && fillingCount <= 150)
        {
            gazAnalyz[2].SetActive(false);
            gazAnalyz[3].SetActive(true);
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
                StartRedSequence(gates[6].transform);
                gazAnalyz[4].SetActive(true);
                gazAnalyz[5].SetActive(false);
                displayValue3 = Mathf.Lerp(displayValue3, 50f, 30 * Time.deltaTime);
                parInStraightPipe.Play();

                if (fillingCount >= 135 && fillingCount <= 140)
                {
                    gazAnalyz[5].GetComponentInParent<AudioSource>().Play();
                }

                if (fillingCount >= 148 && fillingCount <= 150)
                {
                    StartGreenSequence(gates[6].transform);
                    gazAnalyz[5].SetActive(true);
                    
                    gazAnalyz[4].SetActive(false);
                }
            }
        }
        else
        {
            displayValue3 = Mathf.Lerp(displayValue3, 0f, 30 * Time.deltaTime);
            randomValue = null; // Сбрасываем число, если не в диапазоне
        }


        if (fillingCount >= 134 && fillingCount <= 136)
        {
            StartRedSequence(gates[0].transform);
            StartRedSequence(gates[1].transform);
        }
    }
    private void PlayFillingEffects()
    {
        bool isStarted = false;
        if (fillingCount >= 0 && fillingCount <= 2)
        {
            StartGreenSequence(gates[3].transform);
            StartGreenSequence(gates[0].transform);
            StartGreenSequence(gates[1].transform);
            StartGreenSequence(gates[4].transform);
        }
        if (fillingCount >= 2 && fillingCount <= 8)
        {
            gazAnalyz[0].SetActive(true);
            gazAnalyz[1].SetActive(false);
            displayValue2 = Mathf.Lerp(displayValue2, 0, 4f * Time.deltaTime);
            parInCapsul1.Stop();
            parInCapsul2.Stop();
            parInStraightPipe.Stop();
        }
        if (fillingCount >= 15 && fillingCount <= 17)
        {
            isStarted = true;
        }
        if (isStarted)
        {
            smokeParInCapsul2.Play();
            smokeOutCapsul.Stop();
            smokeParInCapsul.Stop();

            smokeInCapsul.Play();
            smokeInCapsul2.Stop();

            AbsentOn();
            AbsentOff2();
        }

        if (fillingCount >= 25 && fillingCount < 27)
        {
            smokeOutCapsulSecond.Play();
        }

        if (fillingCount >= 45 && fillingCount < 50)
        {
            parInCapsul1.Play();
        }
        if (fillingCount >= 120 && fillingCount <= 135)
        {
            gazAnalyz[3].GetComponentInParent<AudioSource>().Play();
        }

        if (fillingCount >= 135 && fillingCount <= 150)
        {
            
            gazAnalyz[2].SetActive(false);
            gazAnalyz[3].SetActive(true);
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
                StartRedSequence(gates[6].transform);
                gazAnalyz[4].SetActive(true);
                gazAnalyz[5].SetActive(false);
                displayValue3 = Mathf.Lerp(displayValue3, 50f, 30 * Time.deltaTime);
                parInStraightPipe.Play();

                if (fillingCount >= 135 && fillingCount <= 140)
                {
                    gazAnalyz[5].GetComponentInParent<AudioSource>().Play();
                }

                if (fillingCount >= 148 && fillingCount <= 150)
                {
                    StartGreenSequence(gates[6].transform);
                    gazAnalyz[5].SetActive(true);
                    gazAnalyz[4].SetActive(false);
                }
            }
        }
        else
        {
            displayValue3 = Mathf.Lerp(displayValue3, 0f, 30 * Time.deltaTime);

            randomValue = null; // Сбрасываем число, если не в диапазоне
        }
        
        if (fillingCount >= 134 && fillingCount <= 136)
        {
            StartRedSequence(gates[3].transform);
            StartRedSequence(gates[0].transform);
            StartRedSequence(gates[1].transform);
            StartRedSequence(gates[4].transform);

        }
    }

    private void PlayUnfillingEffects()
    {
        bool isStarted = false;
        if (fillingCount >= 0 && fillingCount <= 2)
        {
            StartGreenSequence(gates[1].transform);
            StartGreenSequence(gates[2].transform);
            StartGreenSequence(gates[3].transform);
            StartGreenSequence(gates[5].transform);
        }

        if (fillingCount >= 2 && fillingCount <= 8)
        {
            gazAnalyz[2].SetActive(true);
            gazAnalyz[3].SetActive(false);
            displayValue = Mathf.Lerp(displayValue, 0, 4f * Time.deltaTime);
            parInCapsul2.Stop();
            parInCapsul1.Stop();
            parInStraightPipe.Stop();
        }
        gate.SetActive(true);
        if (fillingCount >= 15 && fillingCount <= 17)
        {
            isStarted = true;
        }
        if (isStarted)
        {
            smokeParInCapsul.Play();
            smokeOutCapsulSecond.Stop();
            smokeParInCapsul2.Stop();
            smokeInCapsul2.Play();
            smokeInCapsul.Stop();
            AbsentOff();
            AbsentOn2();
        }
            
        if (fillingCount >= 25 && fillingCount < 27)
        {
            smokeOutCapsul.Play();
        }

        if (fillingCount >= 45 && fillingCount < 50)
        {
            parInCapsul2.Play();
        }
        if (fillingCount >= 120 && fillingCount <= 135)
        {
            gazAnalyz[1].GetComponentInParent<AudioSource>().Play();
        }

        if (fillingCount >= 135 && fillingCount <= 150)
        {
            gazAnalyz[0].SetActive(false);
            gazAnalyz[1].SetActive(true);
            
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
                StartRedSequence(gates[6].transform);
                gazAnalyz[4].SetActive(true);
                gazAnalyz[5].SetActive(false);
                displayValue3 = Mathf.Lerp(displayValue3, 50f, 30 * Time.deltaTime);
                parInStraightPipe.Play();

                if (fillingCount >= 135 && fillingCount <= 140)
                {
                    gazAnalyz[5].GetComponentInParent<AudioSource>().Play();
                }

                if (fillingCount >= 148 && fillingCount <= 150)
                {
                    StartGreenSequence(gates[6].transform);
                    gazAnalyz[5].SetActive(true);
                    gazAnalyz[4].SetActive(false);
                }
            }
        }
        else
        {
            displayValue3 = Mathf.Lerp(displayValue3, 0f, 30 * Time.deltaTime);

            randomValue = null; // Сбрасываем число, если не в диапазоне
        }

        if (fillingCount >= 134 && fillingCount <= 136)
        {
            StartRedSequence(gates[1].transform);
            StartRedSequence(gates[2].transform);
            StartRedSequence(gates[3].transform);
            StartRedSequence(gates[5].transform);
        }
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
        materialSbor.SetFloat("_Filling", -31f);
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
