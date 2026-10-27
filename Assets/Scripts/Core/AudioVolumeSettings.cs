using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Громкость групп микшера (Master / UI / SFX): применяет сохранённые значения при запуске сцены
/// и, если назначены ползунки, связывает их с микшером. Значения хранятся в PlayerPrefs.
/// </summary>
public class AudioVolumeSettings : MonoBehaviour
{
    public const string MasterParam = "MasterVolume";
    public const string UIParam = "UIVolume";
    public const string SFXParam = "SFXVolume";

    [SerializeField] private AudioMixer mixer;

    [Header("Ползунки (необязательно, диапазон 0..1)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        Bind(masterSlider, MasterParam);
        Bind(uiSlider, UIParam);
        Bind(sfxSlider, SFXParam);
    }

    private void Bind(Slider slider, string param)
    {
        float value = PlayerPrefs.GetFloat(param, 1f);
        Apply(param, value);

        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(v => SetVolume(param, v));
    }

    public void SetMasterVolume(float value) => SetVolume(MasterParam, value);
    public void SetUIVolume(float value) => SetVolume(UIParam, value);
    public void SetSFXVolume(float value) => SetVolume(SFXParam, value);

    public void SetVolume(string param, float value)
    {
        value = Mathf.Clamp01(value);
        Apply(param, value);
        PlayerPrefs.SetFloat(param, value);
    }

    private void Apply(string param, float linear)
    {
        if (mixer == null) return;
        // Линейное значение 0..1 → децибелы микшера; 0 — полная тишина (-80 дБ).
        float db = linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;
        mixer.SetFloat(param, db);
    }
}
