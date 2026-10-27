using UnityEngine;
using UnityEngine.UI;

public enum UISoundType
{
    Select,     // обычная кнопка: языки, разрешения, списки
    Navigate,   // переход между разделами и окнами
    Confirm,    // запуск, сохранение, применение
    Cancel,     // остановка, удаление, выход
    Toggle,     // включение/выключение
    None        // без звука
}

/// <summary>
/// Звуки интерфейса. Подключает звук ко всем кнопкам и переключателям сцены:
/// тип берётся из компонента ButtonSound на кнопке, без него — Select (для Toggle — Toggle).
/// </summary>
public class SoundForButtons : MonoBehaviour
{
    [Tooltip("Устарело: звук подключается ко всем кнопкам сцены автоматически. Оставлено для совместимости.")]
    public Button[] buttons;
    public AudioSource audi;

    [Header("Звуки по типам кнопок (несколько вариантов — выбирается случайный)")]
    public AudioClip[] selectClips;
    public AudioClip[] navigateClips;
    public AudioClip[] confirmClips;
    public AudioClip[] cancelClips;
    public AudioClip[] toggleClips;

    [Tooltip("Случайный разброс высоты звука, чтобы частые нажатия не звучали одинаково.")]
    [Range(0f, 0.1f)] public float pitchVariation = 0.03f;

    public static SoundForButtons Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Start()
    {
        // Кнопки с ButtonSound подключаются сами (в своём Awake), здесь — все остальные.
        foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (button.GetComponent<ButtonSound>() == null)
                button.onClick.AddListener(() => Play(UISoundType.Select));
        }

        foreach (Toggle toggle in FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (toggle.GetComponent<ButtonSound>() == null)
                toggle.onValueChanged.AddListener(_ => Play(UISoundType.Toggle));
        }
    }

    public void Play(UISoundType type)
    {
        if (type == UISoundType.None || audi == null) return;

        AudioClip[] clips = type switch
        {
            UISoundType.Navigate => navigateClips,
            UISoundType.Confirm => confirmClips,
            UISoundType.Cancel => cancelClips,
            UISoundType.Toggle => toggleClips,
            _ => selectClips
        };
        AudioClip clip = Pick(clips) ?? Pick(selectClips);
        if (clip == null) return;

        audi.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audi.PlayOneShot(clip);
    }

    private static AudioClip Pick(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
