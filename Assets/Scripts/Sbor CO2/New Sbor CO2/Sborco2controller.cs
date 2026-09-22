using TMPro;
using UnityEngine;

public class SborCO2Controller : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private SborProcessStateMachine stateMachine;
    [SerializeField] private SborDisplayUI display; // channel 0..4 == displayValue..displayValue5

    [Header("Valves (индексы совпадают с оригинальным gates[])")]
    [SerializeField] private ValveLampController[] gates = new ValveLampController[7];

    [Header("Gas analyzers (индексы совпадают с оригинальным gazAnalyz[])")]
    [SerializeField] private GameObject[] gazAnalyz = new GameObject[6];

    [Header("Particle systems")]
    [SerializeField] private ParticleSystem smokeParInCapsul;
    [SerializeField] private ParticleSystem smokeParInCapsul2;
    [SerializeField] private ParticleSystem smokeOutCapsul;
    [SerializeField] private ParticleSystem smokeOutCapsulSecond;
    [SerializeField] private ParticleSystem smokeInCapsul0_1;
    [SerializeField] private ParticleSystem smokeInCapsul0_2;
    [SerializeField] private ParticleSystem smokeInCapsul1end;
    [SerializeField] private ParticleSystem smokeInCapsul2end;
    [SerializeField] private ParticleSystem parInCapsul1;
    [SerializeField] private ParticleSystem parInCapsul2;
    [SerializeField] private ParticleSystem parInStraightPipe;

    [Header("Materials")]
    [SerializeField] private MaterialFadeAnimator absentFade;
    [SerializeField] private MaterialFadeAnimator absent2Fade;
    [SerializeField] private Material materialSbor;
    [SerializeField] private Renderer[] objectsWithMaterial;  

    [Header("Animators")]
    [SerializeField] private Animator uroven1;
    [SerializeField] private Animator uroven2;

    [Header("Misc")]
    [SerializeField] private GameObject gate;     
    [SerializeField] private float defaultDelay = 6f;

    private int _fillCount;
    private bool _materialSborUpdatedThisPhase;
    private int? _leakRoll;

    private void Awake()
    {
        stateMachine.OnStageChanged += HandleStageChanged;
        stateMachine.OnTick += HandleTick;
    }

    private void OnDestroy()
    {
        stateMachine.OnStageChanged -= HandleStageChanged;
        stateMachine.OnTick -= HandleTick;
    }

    private void Start()
    {
        var mainCameraListener = Camera.main != null ? Camera.main.GetComponent<AudioListener>() : null;
        if (mainCameraListener != null)
            mainCameraListener.enabled = true;

        gazAnalyz[0].SetActive(true);
        gazAnalyz[1].SetActive(false);
        gazAnalyz[2].SetActive(true);
        gazAnalyz[3].SetActive(false);
    }

    // ---------- Публичный API ----------

    public void StartColumnProcess()
    {
        stateMachine.StartWithDelay(defaultDelay);
    }

    public void StopColumnProcess()
    {
        stateMachine.Stop();

        uroven1.Play("Idle");
        uroven2.Play("Idle");
        gate.SetActive(true);
        materialSbor.SetFloat("_Filling", -31f);
        absentFade.FadeTo(0.5f);
        absent2Fade.FadeTo(0.5f);

        smokeOutCapsul.Stop();
        smokeOutCapsulSecond.Stop();
        smokeParInCapsul.Stop();
        smokeParInCapsul2.Stop();
        parInCapsul1.Stop();
        parInCapsul2.Stop();
        parInStraightPipe.Stop();

        display.ResetAll();

        for (int i = 0; i < gates.Length; i++)
            gates[i].Close();

        gazAnalyz[0].SetActive(false);
        gazAnalyz[1].SetActive(true);
        gazAnalyz[2].SetActive(false);
        gazAnalyz[3].SetActive(true);
        gazAnalyz[4].SetActive(false);
        gazAnalyz[5].SetActive(true);

        _fillCount = 0;
        _leakRoll = null;
        _materialSborUpdatedThisPhase = false;
    }

    public void PauseProcess() => stateMachine.Pause();

    public void ResumeProcess() => stateMachine.ResumeAfterDelay();

    // ---------- Реакция на автомат состояний ----------

    private void HandleStageChanged(SborProcessStateMachine.Stage stage)
    {
        if (stage == SborProcessStateMachine.Stage.Filling || stage == SborProcessStateMachine.Stage.Unfilling)
        {
            _materialSborUpdatedThisPhase = false;
            _urovenRaised = false;
            _leakRoll = null;
        }
    }

    private void HandleTick(SborProcessStateMachine.Stage stage, float elapsed)
    {
        switch (stage)
        {
            case SborProcessStateMachine.Stage.FirstFilling:
                PlayFirstFillingEffects(elapsed);
                break;
            case SborProcessStateMachine.Stage.Filling:
                PlayFillingEffects(elapsed);
                break;
            case SborProcessStateMachine.Stage.Unfilling:
                PlayUnfillingEffects(elapsed);
                break;
        }
    }

    // ---------- Эффекты (буквальная транскрипция таймингов оригинала) ----------

    private void PlayFirstFillingEffects(float t)
    {
        if (t is >= 0 and <= 2)
        {
            gates[0].Open();
            gates[1].Open();
        }

        bool isStarted = t is >= 15 and <= 17;
        if (isStarted)
            smokeInCapsul0_1.Play();

        if (t is >= 15 and <= 16)
            absentFade.FadeTo(2f);

        if (t is >= 120 and <= 135)
        {
            var src = gazAnalyz[3].GetComponentInParent<AudioSource>();
            if (src != null) src.Play();
            smokeInCapsul1end.Play();
        }

        if (t is >= 135 and <= 150)
        {
            gazAnalyz[2].SetActive(false);
            gazAnalyz[3].SetActive(true);
            display.SetTarget(0, 50f, speedOverride: 1f);
        }

        if (t is >= 134 and <= 136)
        {
            gates[0].Close();
            gates[1].Close();
        }
    }

    private void PlayFillingEffects(float t)
    {
        if (t is >= 0 and <= 2)
        {
            gates[3].Open();
            gates[0].Open();
            gates[1].Open();
            gates[4].Open();
        }

        if (t is >= 2 and <= 8)
        {
            gazAnalyz[0].SetActive(true);
            gazAnalyz[1].SetActive(false);
            display.SetTarget(0, 0f, speedOverride: 4f);
            display.SetTarget(1, 0f, speedOverride: 4f);
            parInCapsul1.Stop();
            parInCapsul2.Stop();
            smokeInCapsul1end.Stop();
            smokeInCapsul2end.Stop();
            parInStraightPipe.Stop();
            smokeInCapsul0_2.Stop();
        }

        bool isStarted = false;
        if (t is >= 15 and <= 17)
        {
            isStarted = true;
            gazAnalyz[4].SetActive(false);
            gazAnalyz[5].SetActive(true);
            display.SetTarget(2, 0f, speedOverride: 30f);
            gates[6].Close();
        }

        if (isStarted)
        {
            smokeParInCapsul2.Play();
            smokeOutCapsul.Stop();
            smokeParInCapsul.Stop();
            smokeInCapsul0_1.Play();
        }

        if (t is >= 15 and <= 16)
        {
            absentFade.FadeTo(2f);
            absent2Fade.FadeTo(0.5f);
        }

        if (t is >= 25 and < 30)
        {
            smokeOutCapsulSecond.Play();
            smokeInCapsul2end.Play();
            display.SetTarget(1, 50f, speedOverride: 4.2f);
        }

        if (t is >= 30 and <= 40)
        {
            gazAnalyz[4].SetActive(true);
            gazAnalyz[5].SetActive(false);
            display.SetTarget(2, 50f, speedOverride: 30f);
            gates[6].Open();
        }

        if (t is >= 45 and < 50)
            parInCapsul1.Play();

        if (t is >= 120 and <= 135)
        {
            var src = gazAnalyz[3].GetComponentInParent<AudioSource>();
            if (src != null) src.Play();
            smokeInCapsul1end.Play();
        }

        if (t is >= 135 and <= 150)
        {
            gazAnalyz[2].SetActive(false);
            gazAnalyz[3].SetActive(true);
            display.SetTarget(0, 50f, speedOverride: 2f);
            ApplyMaterialSborRampOnce();
        }

        MaybeTriggerLeakBranch(t);

        if (t is >= 145 and <= 150)
            IncreaseFillingOnce();

        if (t is >= 134 and <= 136)
        {
            gates[3].Close();
            gates[0].Close();
            gates[1].Close();
            gates[4].Close();
        }
    }

    private void PlayUnfillingEffects(float t)
    {
        if (t is >= 0 and <= 2)
        {
            gates[1].Open();
            gates[2].Open();
            gates[3].Open();
            gates[5].Open();
        }

        if (t is >= 2 and <= 8)
        {
            gazAnalyz[2].SetActive(true);
            gazAnalyz[3].SetActive(false);
            display.SetTarget(1, 0f, speedOverride: 4f);
            display.SetTarget(0, 0f, speedOverride: 4f);
            parInCapsul2.Stop();
            parInCapsul1.Stop();
            smokeInCapsul1end.Stop();
            smokeInCapsul2end.Stop();
            parInStraightPipe.Stop();
            smokeInCapsul0_1.Stop();
        }
        
        gate.SetActive(true);

        bool isStarted = false;
        if (t is >= 15 and <= 17)
        {
            isStarted = true;
            gazAnalyz[4].SetActive(false);
            gazAnalyz[5].SetActive(true);
            display.SetTarget(2, 0f, speedOverride: 30f);
            gates[6].Close();
        }

        if (isStarted)
        {
            smokeParInCapsul.Play();
            smokeOutCapsulSecond.Stop();
            smokeParInCapsul2.Stop();
            smokeInCapsul0_2.Play();
        }

        if (t is >= 15 and <= 16)
        {
            absentFade.FadeTo(0.5f);
            absent2Fade.FadeTo(2f);
        }

        if (t is >= 25 and < 30)
        {
            smokeOutCapsul.Play();
            smokeInCapsul1end.Play();
            display.SetTarget(0, 50f, speedOverride: 4.2f);
        }

        if (t is >= 30 and <= 40)
        {
            gazAnalyz[4].SetActive(true);
            gazAnalyz[5].SetActive(false);
            display.SetTarget(2, 50f, speedOverride: 30f);
            gates[6].Open();
        }

        if (t is >= 45 and < 50)
            parInCapsul2.Play();

        if (t is >= 120 and <= 135)
        {
            var src = gazAnalyz[1].GetComponentInParent<AudioSource>();
            if (src != null) src.Play();
            smokeInCapsul2end.Play();
        }

        if (t is >= 135 and <= 150)
        {
            gazAnalyz[0].SetActive(false);
            gazAnalyz[1].SetActive(true);
            display.SetTarget(1, 50f, speedOverride: 2f);
            ApplyMaterialSborRampOnce();
        }

        if (t is >= 145 and <= 150)
            IncreaseFillingOnce();

        MaybeTriggerLeakBranch(t);

        if (t is >= 134 and <= 136)
        {
            gates[1].Close();
            gates[2].Close();
            gates[3].Close();
            gates[5].Close();
        }
    }

    // ---------- Общая логика, ранее дословно продублированная в двух методах оригинала ----------

    private void ApplyMaterialSborRampOnce()
    {
        if (_materialSborUpdatedThisPhase) return;

        float current = materialSbor.GetFloat("_Filling");
        if (current < -21f)
            materialSbor.SetFloat("_Filling", current + 10f);

        _materialSborUpdatedThisPhase = true;
    }

    private bool _urovenRaised;

    private void IncreaseFillingOnce()
    {
        if (_urovenRaised) return;
        IncreaseFilling();
        _urovenRaised = true;
    }
    
    private void IncreaseFilling()
    {
        _fillCount = Mathf.Min(_fillCount + 1, objectsWithMaterial.Length);

        float currentFilling1 = objectsWithMaterial[2].material.GetFloat("_Filling");
        if (currentFilling1 >= 19f)
        {
            foreach (var ren in objectsWithMaterial)
                ren.material.SetFloat("_Filling", -19f);
        }

        for (int i = 0; i < _fillCount; i++)
        {
            var mat = objectsWithMaterial[i].material;
            float newFilling = mat.GetFloat("_Filling") + 2f;
            mat.SetFloat("_Filling", newFilling);
        }
    }

    private void MaybeTriggerLeakBranch(float t)
    {
        if (t is >= 135 and <= 150)
        {
            _leakRoll ??= Random.Range(1, 10);

            if (_leakRoll == 9)
            {
                gates[6].Close();
                gazAnalyz[4].SetActive(false);
                gazAnalyz[5].SetActive(true);
                display.SetTarget(2, 0f, speedOverride: 30f);
                parInStraightPipe.Play();

                if (t is >= 135 and <= 140)
                {
                    var src = gazAnalyz[5].GetComponentInParent<AudioSource>();
                    if (src != null) src.Play();
                }

                if (t is >= 148 and <= 150)
                {
                    gates[6].Open();
                    gazAnalyz[5].SetActive(true);
                    gazAnalyz[4].SetActive(false);
                }
            }
        }
        else
        {
            _leakRoll = null;
        }
    }
}