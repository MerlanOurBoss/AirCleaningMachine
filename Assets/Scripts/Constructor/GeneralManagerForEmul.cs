using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class GeneralManagerForEmul : GeneralManagerBase, IParameterModule
{
    [Header("Emul")]
    [Space]

    [SerializeField] private List<ParameterRowUI> rows = new List<ParameterRowUI>();
    public List<ParameterRowUI> Rows => rows;
    [SerializeField] private ParticleSystem[] _emulSmokes;
    [SerializeField] private PlayableDirector[] _emulFluid;
    [SerializeField] private DropSpawner[] dropSpawners;
    [SerializeField] private Camera _componentCamera;
    [SerializeField] private CameraSequenceController _cameraSequenceController;

    [SerializeField] private Button exitButton;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button switchButton;

    private bool _isPlaying = false;
    private Coroutine updateRoutine;
    private float updateInterval = 5f;
    private ChamgingEmul emulType;
    [SerializeField] private MathModuleForEmul _emul;
    [SerializeField] private TextMeshProUGUI _fluidTest;
    
    private static int globalCounter = 0;
    public int instanceID;
    private bool isStart = false;
    private void Start()
    {
        globalCounter++;
        instanceID = globalCounter;

        Debug.Log($"[Emul] Назначен instanceID = {instanceID} для {gameObject.name}");
        MathModuleForEmul[] modules = FindObjectsOfType<MathModuleForEmul>();

        foreach (var m in modules)
        {
            if (m.instanceID == instanceID)
            {
                _emul = m;
                break;
            }
        }
        
        if (_emul != null)
        {
            _emul._mySmokes = _emulSmokes;
        }

        _cameraSequenceController = GameObject.FindFirstObjectByType<CameraSequenceController>();

        if (_cameraSequenceController != null)
        {
            if (exitButton != null)
                exitButton.onClick.AddListener(_cameraSequenceController.RestoreWallFromReveal);

            if (switchButton != null)
                switchButton.onClick.AddListener(_cameraSequenceController.ShowOverview);

            if (enterButton != null && _componentCamera != null)
                enterButton.onClick.AddListener(() => _cameraSequenceController.Reveal(_componentCamera));
        }
        updateRoutine = StartCoroutine(UpdateValuesRoutine());

        emulType = gameObject.GetComponent<ChamgingEmul>();
        
        if (emulType.countWater == 1)
        {
            _emul.fluid = "Вода";
            Debug.Log("water");
        }
        else if (emulType.countWater == 2)
        {
            _emul.fluid = "Едкий натрий";
            Debug.Log("no water");
        }
        else
        {
            _emul.fluid = "Сода";
        }
        
        GameObject testText = GameObject.FindGameObjectWithTag("TestText");
        _fluidTest = testText.GetComponent<TextMeshProUGUI>();
    }

    
    private IEnumerator UpdateValuesRoutine()
    {
        while (true)
        {
            UpdateCalculatedParameters();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void Update()
    {
        if (_isPlaying)
        {
            if (_emulFluid != null)
            {
                foreach (PlayableDirector fluid in _emulFluid)
                {
                    if (fluid)
                    {
                        fluid.Play();
                        DiagnoseTimeline(fluid);
                    }

                }
            }
        }
        else
        {
            if (_emulFluid != null)
            {
                foreach (PlayableDirector fluid in _emulFluid)
                {
                    if (fluid)
                        fluid.Stop();
                }
            }
        }
    }
    public void DiagnoseTimeline(PlayableDirector dir)
    {
        Debug.Log("===== DIAGNOSTICS FOR PLAYABLE DIRECTOR =====");

        if (dir == null)
        {
            Debug.LogError("❌ PlayableDirector = NULL");
            return;
        }

        Debug.Log("Object: " + dir.gameObject.name);

        Debug.Log("Scene: '" + dir.gameObject.scene.name + "'");
        if (dir.gameObject.scene.name == null || dir.gameObject.scene.name == "")
            Debug.LogWarning("⚠ Объект находится в prefab scene (НЕ в активной сцене!)");
        if (dir.gameObject.scene != SceneManager.GetActiveScene())
            Debug.LogWarning("⚠ Объект НЕ в активной сцене. Это может ломать Timeline.");

        if (dir.playableAsset == null)
            Debug.LogError(" Timeline asset (playableAsset) = NULL — в BUILD asset НЕ сохранился.");

        // 3. Проверка графа
        Debug.Log("Root Playables: " + dir.playableGraph.GetRootPlayableCount());
        if (!dir.playableGraph.IsValid())
            Debug.LogWarning("⚠ PlayableGraph = INVALID. RebuildGraph() требуется.");

        // 4. Проверка активен ли объект
        if (!dir.gameObject.activeInHierarchy)
            Debug.LogWarning("⚠ Объект не активен. Timeline не будет играть.");

        // 5. Проверка bindings (очень важно, если Timeline использует AnimationTrack)
        var timeline = dir.playableAsset as TimelineAsset;
        if (timeline != null)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                var binding = dir.GetGenericBinding(track);

                if (binding == null)
                    Debug.LogWarning("⚠ TRACK '" + track.name + "' потерял binding! (частая причина в динамически загруженных префабах)");
            }
        }
        else
        {
            Debug.LogWarning("⚠ TimelineAsset не является TimelineAsset (null или другой тип?).");
        }

        Debug.Log("===== END OF DIAGNOSTICS =====");
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateCalculatedParameters()
    {
        foreach (var row in rows)
        {
            if (!row.valueTextIn)
                continue;
            
            if (!float.TryParse(row.valueTextIn.text, out float originalValue))
            {
                Debug.LogWarning($"[Emul] Невозможно преобразовать valueTextIn у id={row.id}");
                continue;
            }
            
            if (row.id == "Температура")
            {
                float newTemperature = originalValue - (140000 *(_emul._gasСonsumption/_emul.сonsumption)); 
                float roundedTemp = Mathf.Ceil(newTemperature);
                
                Debug.Log(roundedTemp);
                row.valueTextOut.text = roundedTemp.ToString();
            }
            
            else if (row.id == "Твердые частицы")
            {
                float newDust = originalValue * 0.005f;
                float roundedDust = Mathf.Ceil(newDust);

                row.valueTextOut.text = roundedDust.ToString();
            }
            else if (row.id == "NO2")
            {
                float num = 0;
                if (_emul._fluidType.text == "Вода")
                {
                    num = 0.8f;
                }
                else
                {
                    num = 0.05f;
                }
                float no2 = originalValue * num; // 30%
                float roundedDust = Mathf.Ceil(no2);

                row.valueTextOut.text = roundedDust.ToString();
            }
            
            else if (row.id == "SO2")
            {
                float num = 0;
                if (_emul._fluidType.text == "Вода")
                {
                    num = 0.8f;
                }
                else
                {
                    num = 0.05f;
                }
                float so2 = originalValue * num; // 30%
                float roundedDust = Mathf.Ceil(so2);

                row.valueTextOut.text = roundedDust.ToString();
            }
            
            else if (row.id == "H2S")
            {
                float num = 0;
                if (_emul._fluidType.text == "Вода")
                {
                    num = 0.8f;
                }
                else
                {
                    num = 0.05f;
                }
                float h2s = originalValue * num; // 30%
                float roundedDust = Mathf.Ceil(h2s);

                row.valueTextOut.text = roundedDust.ToString();
            }
            else
            {
                row.valueTextOut.text = row.valueTextIn.text;
            }
        }
    }
    
    protected override void OnStartModule()
    {
        if (_isPlaying)
            return;

        if (_emulSmokes != null)
        {
            foreach (ParticleSystem smoke in _emulSmokes)
            {
                if (smoke != null)
                    smoke.Play();
            }
        }

        if (dropSpawners != null)
        {
            foreach (DropSpawner spawner in dropSpawners)
            {
                spawner.startCor();
            }
        }

        _isPlaying = true;
    }
    
    protected override void OnStopModule()
    {
        if (_emulSmokes != null)
        {
            foreach (ParticleSystem smoke in _emulSmokes)
            {
                if (smoke != null)
                    smoke.Stop();
            }
        }
        
        if (dropSpawners != null)
        {
            foreach (DropSpawner spawner in dropSpawners)
            {
                spawner.stopCor();
            }
        }

        _isPlaying = false;
    }
        
    private void OnDisable()
    {
        globalCounter = 0;
    }
}
