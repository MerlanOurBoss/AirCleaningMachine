using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Задаёт тип звука для кнопки или переключателя. Без этого компонента кнопка звучит как Select.
/// </summary>
[DisallowMultipleComponent]
public class ButtonSound : MonoBehaviour
{
    public UISoundType type = UISoundType.Select;

    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(Play);

        var toggle = GetComponent<Toggle>();
        if (toggle != null) toggle.onValueChanged.AddListener(_ => Play());
    }

    private void Play()
    {
        if (SoundForButtons.Instance != null) SoundForButtons.Instance.Play(type);
    }
}
