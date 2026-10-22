using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class GeneralManagerForEmul : ModuleManagerBase<MathModuleForEmul>
{
    [Header("Emul")]
    [SerializeField] private ParticleSystem[] emulSmokes;
    [SerializeField] private PlayableDirector[] emulFluid;
    [SerializeField] private DropSpawner[] dropSpawners;
    [SerializeField] private TextMeshProUGUI fluidTest;

    // Доля NO2/SO2/H2S, остающаяся в газе после эмульгатора: на воде и на реагенте (едкий натрий, сода).
    public const float WaterPassRatio = 0.8f;
    public const float ReagentPassRatio = 0.05f;

    /// <summary>Номера жидкостей в ChamgingEmul.countWater.</summary>
    public const int FluidWater = 1, FluidCausticSoda = 2, FluidSoda = 3;

    private bool _isPlaying;
    private ChamgingEmul _emulType;
    private int _appliedFluidType = -1;

    /// <summary>
    /// Сменить жидкость (FluidWater / FluidCausticSoda / FluidSoda). Через ChamgingEmul, как клавиша T
    /// в конструкторе, — меняется и цвет жидкости на модели, а матмодуль подхватит тип в SyncFluidType.
    /// </summary>
    public void SelectFluid(int type)
    {
        if (_emulType != null) _emulType.countWater = type;
        else if (MathModule != null) MathModule.SetFluidType(type);
    }

    protected override void OnMathModuleReady(MathModuleForEmul module)
    {
        module._mySmokes = emulSmokes;

        _emulType = GetComponent<ChamgingEmul>();
        if (_emulType == null)
        {
            Debug.LogError($"{name}: не найден компонент ChamgingEmul на этом объекте.");
        }
        else
        {
            _appliedFluidType = _emulType.countWater;
            module.SetFluidType(_appliedFluidType);
        }

        GameObject testText = GameObject.FindGameObjectWithTag(Tags.TestText);
        if (testText == null)
            Debug.LogError("GeneralManagerForEmul: объект с тегом 'TestText' не найден на сцене.");
        else
            fluidTest = testText.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        SyncFluidType();

        if (emulFluid == null) return;

        foreach (var fluid in emulFluid)
        {
            if (fluid == null) continue;

            // Play()/Stop() только при расхождении состояния, а не каждый кадр. Поведение то же:
            // во время работы Timeline перезапускается, если закончился; в простое — остановлен.
            bool isDirectorPlaying = fluid.state == PlayState.Playing;

            if (_isPlaying && !isDirectorPlaying)
                fluid.Play();
            else if (!_isPlaying && isDirectorPlaying)
                fluid.Stop();
        }
    }

    // Смена жидкости в конструкторе (клавиша T) теперь доходит до матмодуля — раньше тип читался один раз.
    private void SyncFluidType()
    {
        if (_emulType == null || MathModule == null) return;

        int current = _emulType.countWater;
        if (current < 1 || current > 3 || current == _appliedFluidType) return;

        _appliedFluidType = current;
        MathModule.SetFluidType(current);
    }

    private static void DiagnoseTimeline(PlayableDirector dir)
    {
        if (dir == null) return;

        if (string.IsNullOrEmpty(dir.gameObject.scene.name))
            Debug.LogWarning("Объект находится в prefab scene (не в активной сцене!)");
        if (dir.gameObject.scene != SceneManager.GetActiveScene())
            Debug.LogWarning("Объект не в активной сцене. Это может ломать Timeline.");
        if (dir.playableAsset == null)
            Debug.LogError("Timeline asset (playableAsset) = NULL — в BUILD asset не сохранился.");
        if (!dir.playableGraph.IsValid())
            Debug.LogWarning("PlayableGraph = INVALID. RebuildGraph() требуется.");
        if (!dir.gameObject.activeInHierarchy)
            Debug.LogWarning("Объект не активен. Timeline не будет играть.");

        if (dir.playableAsset is TimelineAsset timeline)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                if (dir.GetGenericBinding(track) == null)
                    Debug.LogWarning($"TRACK '{track.name}' потерял binding! (частая причина — в динамически загруженных префабах)");
            }
        }
        else
        {
            Debug.LogWarning("TimelineAsset не является TimelineAsset (null или другой тип?).");
        }
    }

    protected override bool TryCalculateRow(ParameterRowUI row, float originalValue)
    {
        switch (row.id)
        {
            case "Температура":
                float newTemperature = MathModule.сonsumption != 0
                    ? originalValue - (140000 * (MathModule._gasСonsumption / MathModule.сonsumption))
                    : originalValue;
                row.valueTextOut.text = Mathf.Ceil(newTemperature).ToString();
                return true;

            case "Твердые частицы":
                row.valueTextOut.text = Mathf.Ceil(originalValue * 0.005f).ToString();
                return true;

            case "NO2":
            case "SO2":
            case "H2S":
                // Было сравнение с "Вода" — на казахском и английском всегда бралось 0.05.
                float ratio = MathModule.IsWater ? WaterPassRatio : ReagentPassRatio;
                row.valueTextOut.text = Mathf.Ceil(originalValue * ratio).ToString();
                return true;

            default:
                return false;
        }
    }

    protected override void OnStartModule()
    {
        if (_isPlaying) return;

        if (emulSmokes != null)
            foreach (var smoke in emulSmokes)
                if (smoke != null) smoke.Play();

        if (dropSpawners != null)
            foreach (var spawner in dropSpawners)
                if (spawner != null) spawner.startCor();

        // Диагностика Timeline — один раз при запуске (раньше писалась в лог каждый кадр).
        if (emulFluid != null)
            foreach (var fluid in emulFluid)
                if (fluid != null) DiagnoseTimeline(fluid);

        _isPlaying = true;
    }

    protected override void OnStopModule()
    {
        if (emulSmokes != null)
            foreach (var smoke in emulSmokes)
                if (smoke != null) smoke.Stop();

        if (dropSpawners != null)
            foreach (var spawner in dropSpawners)
                if (spawner != null) spawner.stopCor();

        _isPlaying = false;
    }
}