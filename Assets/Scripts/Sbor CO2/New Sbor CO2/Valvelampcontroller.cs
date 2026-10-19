using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ValveLampController : MonoBehaviour
{
    [Header("Lamps")]
    [SerializeField] private GameObject greenLamp;
    [SerializeField] private GameObject yellowLamp;
    [SerializeField] private GameObject redLamp;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    [Header("Timing")]
    [SerializeField] private float yellowBlinkDuration = 2f;
    [SerializeField] private float yellowBlinkInterval = 0.3f;

    /// <summary>true, если клапан сейчас открыт (горит зелёная лампа).</summary>
    public bool IsOpen => greenLamp != null && greenLamp.activeSelf;

    private Coroutine _running;
    private bool _runningToOpen; // куда ведёт текущая последовательность (имеет смысл, пока _running != null)

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // При выключении объекта Unity останавливает корутины — сбрасываем признак «идёт переключение».
    private void OnDisable()
    {
        _running = null;
    }

    /// <summary>Открыть клапан (зелёная лампа) — было StartGreenSequence(transform).</summary>
    public void Open()
    {
        // Повторный вызов, пока клапан уже открывается (или уже открыт), ничего не делает.
        // SborCO2Controller вызывает Open каждый кадр в своём временном окне, и раньше мигание
        // и звук перезапускались каждый кадр, пока не загорится зелёная лампа.
        if (_running != null ? _runningToOpen : IsOpen)
            return;

        Restart(openClip, toOpen: true);
    }

    /// <summary>Закрыть клапан (красная лампа) — было StartRedSequence(transform).</summary>
    public void Close()
    {
        bool isClosed = redLamp != null && redLamp.activeSelf;
        if (_running != null ? !_runningToOpen : isClosed)
            return;

        Restart(closeClip, toOpen: false);
    }

    private void Restart(AudioClip clip, bool toOpen)
    {
        if (_running != null)
            StopCoroutine(_running);
        _running = null;

        if (!isActiveAndEnabled) return; // на выключенном объекте корутину запустить нельзя

        _runningToOpen = toOpen;
        _running = StartCoroutine(Sequence(clip, toOpen));
    }

    private IEnumerator Sequence(AudioClip clip, bool openWhenDone)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        // Вложенный перечислитель вместо отдельной корутины: при перезапуске StopCoroutine
        // останавливает и мигание (раньше старое мигание продолжалось параллельно с новым).
        yield return BlinkYellow();

        if (yellowLamp != null) yellowLamp.SetActive(false);
        if (greenLamp != null) greenLamp.SetActive(openWhenDone);
        if (redLamp != null) redLamp.SetActive(!openWhenDone);

        _running = null;
    }

    private IEnumerator BlinkYellow()
    {
        if (yellowLamp == null) yield break;

        float elapsed = 0f;
        bool isOn = false;
        while (elapsed < yellowBlinkDuration)
        {
            isOn = !isOn;
            yellowLamp.SetActive(isOn);
            yield return new WaitForSeconds(yellowBlinkInterval);
            elapsed += yellowBlinkInterval;
        }
        yellowLamp.SetActive(false);
    }
}