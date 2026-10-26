using System;
using System.Collections;
using UnityEngine;

public class SborProcessStateMachine : MonoBehaviour
{
    public enum Stage
    {
        Idle,
        Delay,
        FirstFilling,
        Filling,
        Unfilling
    }

    [SerializeField] private float fillSpeed = 5f;      
    [SerializeField] private float timingDelay = 150f; 
    [SerializeField] private float resumeDelay = 5f;

    public Stage CurrentStage { get; private set; } = Stage.Idle;
    public float Elapsed { get; private set; } 
    public bool IsPaused { get; private set; }

    /// <summary>Вызывается каждый раз, когда elapsed обновился (Stage, elapsed).</summary>
    public event Action<Stage, float> OnTick;

    /// <summary>Вызывается один раз при смене стадии.</summary>
    public event Action<Stage> OnStageChanged;

    /// <summary>Тик обратного отсчёта перед стартом процесса, remaining — оставшееся время.</summary>
    public event Action<float> OnDelayTick;

    private float _delayRemaining;
    private bool _wasFirstFilling = true;

    /// <summary>Запуск процесса с задержкой — аналог StartColumnProcess().</summary>
    public void StartWithDelay(float delaySeconds)
    {
        _delayRemaining = delaySeconds;
        SetStage(Stage.Delay);
    }

    /// <summary>Полная остановка и сброс — аналог StopColumnProcess() (без визуальных эффектов, это забота контроллера).</summary>
    public void Stop()
    {
        Elapsed = 0f;
        IsPaused = false;
        SetStage(Stage.Idle);
    }

    public void Pause() => IsPaused = true;

    /// <summary>Аналог ResumeProcess(): снятие паузы через фиксированную задержку.</summary>
    public void ResumeAfterDelay()
    {
        StartCoroutine(ResumeRoutine());
    }

    private IEnumerator ResumeRoutine()
    {
        yield return new WaitForSeconds(resumeDelay);
        IsPaused = false;
    }

    private void Update()
    {
        if (IsPaused) return;

        switch (CurrentStage)
        {
            case Stage.Delay:
                TickDelay();
                break;

            case Stage.FirstFilling:
            case Stage.Filling:
                TickFilling();
                break;

            case Stage.Unfilling:
                TickUnfilling();
                break;
        }
    }

    private void TickDelay()
    {
        _delayRemaining -= Time.deltaTime;
        OnDelayTick?.Invoke(_delayRemaining);

        if (_delayRemaining <= 0f)
        {
            SetStage(_wasFirstFilling ? Stage.FirstFilling : Stage.Filling);
        }
    }

    private void TickFilling()
    {
        Elapsed += fillSpeed * Time.deltaTime;
        OnTick?.Invoke(CurrentStage, Elapsed);

        if (Elapsed >= timingDelay)
        {
            _wasFirstFilling = false;
            Elapsed = 0f;
            SetStage(Stage.Unfilling);
        }
    }

    private void TickUnfilling()
    {
        Elapsed += fillSpeed * Time.deltaTime;
        OnTick?.Invoke(CurrentStage, Elapsed);

        if (Elapsed >= timingDelay)
        {
            Elapsed = 0f;
            SetStage(Stage.Filling);
        }
    }

    private void SetStage(Stage next)
    {
        CurrentStage = next;
        OnStageChanged?.Invoke(next);
    }
}