using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class ModuleManagerBase<TMathModule> : GeneralManagerBase, IParameterModule
    where TMathModule : MonoBehaviour, IFacilityIdentified
{
    [Header("Math module")]
    [Tooltip("Прямая ссылка на связанный MathModule этого экземпляра. Если оставить пустым — " +
             "будет предпринят поиск по facilityId (см. ниже).")]
    [SerializeField] protected TMathModule mathModule;

    [Tooltip("Используется только если mathModule выше не назначен напрямую: ищется " +
             "MathModule на сцене с таким же facilityId. Должно совпадать с полем " +
             "facilityId на связанном MathModuleForXxx.")]
    [SerializeField] protected string facilityId;

    [Header("Parameters")]
    [SerializeField] protected List<ParameterRowUI> rows = new List<ParameterRowUI>();
    public List<ParameterRowUI> Rows => rows;
    [SerializeField] protected float updateInterval = 5f;

    [Header("Camera reveal")]
    [SerializeField] protected Camera componentCamera;
    [SerializeField] protected CameraSequenceController cameraSequenceController;
    [SerializeField] protected Button exitButton;
    [SerializeField] protected Button enterButton;
    [SerializeField] protected Button switchButton;

    protected TMathModule MathModule => mathModule;

    private Coroutine _updateRoutine;

    protected virtual void Start()
    {
        if (mathModule == null && FacilityPairingRegistry.Instance != null)
            mathModule = FacilityPairingRegistry.Instance.Resolve<TMathModule>(this);

        if (mathModule == null)
            ResolveMathModuleByFacilityId();

        if (mathModule != null)
        {
            ApplyMathModule(mathModule);
        }
        else
        {
            Debug.Log($"{GetType().Name} ({gameObject.name}): mathModule пока не найден " +
                      "(ни напрямую, ни через реестр/facilityId) — ожидаю явного вызова " +
                      "SetMathModule() от кода, который его создаёт.");
        }

        if (cameraSequenceController == null)
            cameraSequenceController = FindFirstObjectByType<CameraSequenceController>();

        WireCameraButtons();

        _updateRoutine = StartCoroutine(UpdateValuesRoutine());
    }

    public void SetMathModule(TMathModule module)
    {
        if (module == null)
        {
            Debug.LogError($"{GetType().Name} ({gameObject.name}): SetMathModule(null) — так делать не нужно.");
            return;
        }

        if (mathModule != null && mathModule != module)
        {
            Debug.LogWarning($"{GetType().Name} ({gameObject.name}): mathModule переназначен " +
                              $"с '{mathModule.name}' на '{module.name}'.");
        }

        ApplyMathModule(module);
    }

    private void ApplyMathModule(TMathModule module)
    {
        mathModule = module;
        OnMathModuleReady(mathModule);
    }

    protected virtual void OnDestroy()
    {
        if (cameraSequenceController != null)
            cameraSequenceController.OnRevealStateChanged -= HandleRevealState;
    }

    private void ResolveMathModuleByFacilityId()
    {
        if (string.IsNullOrWhiteSpace(facilityId))
            return;

        var candidates = FindObjectsOfType<TMathModule>(includeInactive: true)
            .Where(c => c.FacilityId == facilityId)
            .ToList();

        if (candidates.Count > 1)
        {
            Debug.LogError($"{GetType().Name} ({gameObject.name}): найдено {candidates.Count} объектов " +
                            $"MathModule с одинаковым facilityId='{facilityId}' " +
                            $"({string.Join(", ", candidates.Select(c => c.name))}). " +
                            "Похоже, один из объектов был задублирован без смены ID. Привязка к первому " +
                            "найденному — временно, до исправления ID на дубликате.");
        }

        if (candidates.Count > 0)
            mathModule = candidates[0];
    }

    protected virtual void OnMathModuleReady(TMathModule module) { }

    protected abstract bool TryCalculateRow(ParameterRowUI row, float originalValue);

    private void WireCameraButtons()
    {
        if (cameraSequenceController == null) return;

        cameraSequenceController.OnRevealStateChanged += HandleRevealState;

        if (exitButton != null)
            exitButton.onClick.AddListener(cameraSequenceController.RestoreWallFromReveal);

        if (switchButton != null)
            switchButton.onClick.AddListener(cameraSequenceController.ShowOverview);

        if (enterButton != null && componentCamera != null)
            enterButton.onClick.AddListener(() => cameraSequenceController.Reveal(componentCamera));
    }

    private void HandleRevealState(bool revealed)
    {
        if (enterButton != null) enterButton.gameObject.SetActive(!revealed);
        if (exitButton != null) exitButton.gameObject.SetActive(revealed);
    }

    private IEnumerator UpdateValuesRoutine()
    {
        while (true)
        {
            UpdateCalculatedParameters();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateCalculatedParameters()
    {
        if (mathModule == null) return;

        foreach (var row in rows)
        {
            if (!row.valueTextIn) continue;

            if (!float.TryParse(row.valueTextIn.text, out float originalValue))
            {
                Debug.LogWarning($"[{GetType().Name}] Невозможно преобразовать valueTextIn у id={row.id}");
                continue;
            }

            if (!TryCalculateRow(row, originalValue))
                row.valueTextOut.text = row.valueTextIn.text;
        }
    }
}