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

    private bool _isPlaying;

    protected override void OnMathModuleReady(MathModuleForEmul module)
    {
        module._mySmokes = emulSmokes;

        var emulType = GetComponent<ChamgingEmul>();
        if (emulType == null)
        {
            Debug.LogError($"{name}: не найден компонент ChamgingEmul на этом объекте.");
        }
        else
        {
            module.fluid = emulType.countWater switch
            {
                1 => "Вода",
                2 => "Едкий натрий",
                _ => "Сода"
            };
        }

        GameObject testText = GameObject.FindGameObjectWithTag("TestText");
        if (testText == null)
            Debug.LogError("GeneralManagerForEmul: объект с тегом 'TestText' не найден на сцене.");
        else
            fluidTest = testText.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (emulFluid == null) return;

        foreach (var fluid in emulFluid)
        {
            if (fluid == null) continue;

            if (_isPlaying)
            {
                fluid.Play();
                DiagnoseTimeline(fluid);
            }
            else
            {
                fluid.Stop();
            }
        }
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
                float newTemperature = originalValue - (140000 * (MathModule._gasСonsumption / MathModule.сonsumption));
                row.valueTextOut.text = Mathf.Ceil(newTemperature).ToString();
                return true;

            case "Твердые частицы":
                row.valueTextOut.text = Mathf.Ceil(originalValue * 0.005f).ToString();
                return true;

            case "NO2":
            case "SO2":
            case "H2S":
                float ratio = MathModule._fluidType.text == "Вода" ? 0.8f : 0.05f;
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
                spawner.startCor();

        _isPlaying = true;
    }

    protected override void OnStopModule()
    {
        if (emulSmokes != null)
            foreach (var smoke in emulSmokes)
                if (smoke != null) smoke.Stop();

        if (dropSpawners != null)
            foreach (var spawner in dropSpawners)
                spawner.stopCor();

        _isPlaying = false;
    }
}