using TMPro;
using UnityEngine;

/// <summary>
/// Строка состояния под заголовком панели «Настройка системы»:
/// «● Ожидает запуска» → «● Идёт симуляция · 02:15» → «● Остановлено».
/// Симуляция идёт с нажатия «Симулировать» до «Остановить» (LoadSmoke.IsRunning), таймер — от нажатия.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class SimulationStatusLabel : MonoBehaviour
{
    [Tooltip("Цепочка аппаратов, которую запускают кнопки «Симулировать» / «Остановить». Пусто — ищется на сцене.")]
    [SerializeField] private LoadSmoke chain;

    private TextMeshProUGUI _text;
    private string _last;
    private bool _started; // «Остановлено» — только после первого запуска, до него «Ожидает запуска»

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        if (chain == null) chain = FindAnyObjectByType<LoadSmoke>();
    }

    private void Update()
    {
        bool running = chain != null && chain.IsRunning;
        if (running) _started = true;

        var lang = ProcessMonitor.Instance != null ? ProcessMonitor.Instance.Language : Translator.Language.Russian;
        string text;
        if (running)
        {
            int seconds = Mathf.FloorToInt(Time.time - chain.RunningSince);
            string label = MonitoringText.Pick(lang, "Идёт симуляция", "Симуляция жүріп жатыр", "Simulation running");
            text = $"<color=#{ColorUtility.ToHtmlStringRGB(UITheme.Accent)}>●</color> {label} · {seconds / 60:00}:{seconds % 60:00}";
        }
        else
        {
            string label = _started
                ? MonitoringText.Pick(lang, "Остановлено", "Тоқтатылды", "Stopped")
                : MonitoringText.Pick(lang, "Ожидает запуска", "Іске қосуды күтуде", "Waiting to start");
            text = $"<color=#{ColorUtility.ToHtmlStringRGB(UITheme.Muted)}>●</color> {label}";
        }

        if (text == _last) return;
        _last = text;
        _text.text = text;
    }
}
