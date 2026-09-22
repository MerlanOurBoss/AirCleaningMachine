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

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>Открыть клапан (зелёная лампа) — было StartGreenSequence(transform).</summary>
    public void Open()
    {
        if (greenLamp != null && greenLamp.activeSelf)
            return;

        Restart(Sequence(openClip, openWhenDone: true));
    }

    /// <summary>Закрыть клапан (красная лампа) — было StartRedSequence(transform).</summary>
    public void Close()
    {
        if (redLamp != null && redLamp.activeSelf)
            return;

        Restart(Sequence(closeClip, openWhenDone: false));
    }

    private void Restart(IEnumerator routine)
    {
        if (_running != null)
            StopCoroutine(_running);
        _running = StartCoroutine(routine);
    }

    private IEnumerator Sequence(AudioClip clip, bool openWhenDone)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        yield return StartCoroutine(BlinkYellow());

        if (yellowLamp != null) yellowLamp.SetActive(false);
        if (greenLamp != null) greenLamp.SetActive(openWhenDone);
        if (redLamp != null) redLamp.SetActive(!openWhenDone);
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