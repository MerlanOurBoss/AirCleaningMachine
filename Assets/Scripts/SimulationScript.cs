using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SimulationScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _smokes;
    [SerializeField] private TMP_InputField[] _molecCounts;
    [SerializeField] private PlayableDirector[] _fluidsZero;
    [SerializeField] private PlayableDirector[] _fluidsWater;
    [SerializeField] private PlayableDirector[] _fluidsReact;
    [SerializeField] private TMP_InputField[] _texts;
    [SerializeField] private TextMeshProUGUI[] _componentTexts;
    [SerializeField] private TextMeshProUGUI[] _molecTexts;
    [SerializeField] private DropSpawner[] _dropSpawners;

    [SerializeField] private TextMeshProUGUI _n2Molec;
    [SerializeField] private Button _simulationButton;
    [SerializeField] private TextMeshProUGUI _simulationText;
    [SerializeField] private Animator _electroFilter;
    [SerializeField] private NewCollectors _collector;
    [SerializeField] private TemperatureCatalizator _catalizator;
    [SerializeField] private GameObject _componentsCameras;
    [SerializeField] private GameObject _electroObject;
    [SerializeField] private GameObject _electroObjectTable;
    [SerializeField] private GameObject _noElectroObject;
    [SerializeField] private NasosScript _waterZeroNasos;
    [SerializeField] private NasosScript _waterNasos;
    [SerializeField] private NasosScript _reactNasos;
    [SerializeField] private Button _electroButton;
    [SerializeField] private TMP_InputField _electroInput;
    [SerializeField] private TextMeshProUGUI _electroFilterText;
    [SerializeField] private TextMeshProUGUI _catalizatorText;
    [SerializeField] private TextMeshProUGUI _waterEmulText;
    [SerializeField] private TextMeshProUGUI _reactEmulText;
    [SerializeField] private TextMeshProUGUI _sheloshEmulText;
    [SerializeField] private TextMeshProUGUI _sborCO2Text;
    [SerializeField] private GameObject _tables;
    [SerializeField] private ElectrofilterTable _electroTable;
    [SerializeField] private KatalizatorTable _catalizatorTable;
    [SerializeField] private KatalizatorTableOutElectro _catalizatorOutElectroTable;
    [SerializeField] private CoolingDisplays _coolingDisplay;
    [SerializeField] private SheloshTable _sheloshTable;
    [SerializeField] private WaterTable _waterTable;
    [SerializeField] private ReactTable _reactTable;
    [SerializeField] private SborTable _sborTable;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _catalizatorButton1;
    [SerializeField] private Button _catalizatorButton2;

    public bool StartSimulationTemp { get; private set; }
    public bool StartSimulationContent { get; private set; }
    private float _simulationTime = 3400f;
    public float FluidDelayZero { get; set; } = 0f;
    public float FluidDelayWater { get; set; } = 15f;
    public float FluidDelayReact { get; set; } = 30f;
    private int _maxMolecCount = 130;

    private void Start()
    {
        InitializeTexts();
    }

    private void Update()
    {
        UpdateSimulationTime();
        UpdateComponentStates();
    }

    private void InitializeTexts()
    {
        string[] initialTexts = {
            "150 °C", "0.01 мА/см²", "80 %", "Циолит", "Платина", "5 °C", "Вода", "5 °C", "Едкий натрий", "Цеолиты",
            "0,5 м/с", "3 кВ/м", "500 °C", "3 кВ/м", "1 моль/м", "0,05 м/с", "1,5 моль/м", "0,5 кПа", "0,2 кПа", "Включить", "2", "2"
        };
        for (int i = 0; i < initialTexts.Length; i++)
        {
            _texts[i].text = initialTexts[i];
        }
    }

    private void UpdateSimulationTime()
    {
        if (_simulationTime > 0)
        {
            if (StartSimulationTemp && StartSimulationContent)
            {
                _simulationTime -= Time.deltaTime;
                FluidDelayWater -= Time.deltaTime;
                FluidDelayReact -= Time.deltaTime;
                FluidDelayZero -= Time.deltaTime;
                _tables.SetActive(true);

                if (FluidDelayZero < 0) PlayFluids(_fluidsZero);
                if (FluidDelayWater < 0) PlayFluids(_fluidsWater);
                if (FluidDelayReact < 0) PlayFluids(_fluidsReact);
            }
            else
            {
                ResetSimulation();
            }
        }
    }

    private void PlayFluids(PlayableDirector[] fluids)
    {
        foreach (var fluid in fluids)
        {
            fluid.Play();
        }
    }

    public void ResetSimulation()
    {
        _electroButton.interactable = true;
        _electroInput.interactable = true;
        _catalizatorButton1.interactable = true;
        _catalizatorButton2.interactable = true;
        _simulationButton.interactable = true;
        _simulationText.text = "Симулировать";
        _simulationTime = 3400f;
        //_componentsCameras.SetActive(false);

        foreach (var smoke in _smokes)
        {
            smoke.Stop();
        }
        _electroFilter.Play("NewColecAnimStop");
        _collector.StopColumnProcess();
        _coolingDisplay.StopDelay();
        _catalizator.StopSimulation();
        _tables.SetActive(false);
        StopFluids(_fluidsWater);
        StopFluids(_fluidsReact);
        StopDropSpawners();
    }

    private void StopFluids(PlayableDirector[] fluids)
    {
        foreach (var fluid in fluids)
        {
            fluid.Stop();
        }
    }

    private void StopDropSpawners()
    {
        foreach (var dropSpawner in _dropSpawners)
        {
            dropSpawner.stopCor();
        }
    }

    private void UpdateComponentStates()
    {
        UpdateTextFields();
        UpdateSimulationStatus();
        CheckMolecularCounts();
    }

    private void UpdateTextFields()
    {
        //if (_texts[13].text != " " && _texts[12].text != " ")
        //{
        //    float result = 1 - Mathf.Exp(-float.Parse(_texts[13].text) * 2 / float.Parse(_texts[12].text));
        //    _electroFilterText.text = $"Эффект. электрофильтра: " + (result * 100).ToString("0.")+ " %";
        //}

        //if (_texts[20].text != " ")
        //{
        //    float result = CalculateCatalizatorEffect(_texts[20].text);
        //    _catalizatorText.text = $"Эффект. катализатора: " + (result * 100).ToString("0.") + " %";
        //}

        //if (_texts[14].text != " " && _texts[15].text != " ")
        //{
        //    float result = float.Parse(_texts[14].text) * 10 * float.Parse(_texts[15].text);
        //    _waterEmulText.text = $"Эффект. водяного эмуль.:" + (result * 100).ToString("0.") + " %";
        //}

        //if (_texts[16].text != " " && _texts[17].text != " ")
        //{
        //    float result = float.Parse(_texts[16].text) * 10 * float.Parse(_texts[17].text);
        //    _reactEmulText.text = $"Эффект. реагент. эмуль.: " + (result * 100).ToString("0.") + " %";
        //}

        //if (_texts[18].text != " " && _texts[19].text != " ")
        //{
        //    float result = (10 * float.Parse(_texts[18].text) * float.Parse(_texts[19].text)) /
        //                    (1 + float.Parse(_texts[18].text) * float.Parse(_texts[19].text));
        //    _sborCO2Text.text = $"Эффект. сбор CO2: " + (result * 100).ToString("0.") + " %";
        //}
    }

    private float CalculateCatalizatorEffect(string temperatureText)
    {
        float temperature = float.Parse(temperatureText);
        float result = 1;
        if (temperature > 5) result -= (temperature - 5) * 0.01f;
        return Mathf.Clamp(result, 0.1f, 1f);
    }

    private void UpdateSimulationStatus()
    {
        if (_simulationText.text == "Симулировать")
        {
            StartSimulationTemp = false;
            StartSimulationContent = false;
        }
        else
        {
            StartSimulationTemp = true;
            StartSimulationContent = true;
        }
    }

    private void CheckMolecularCounts()
    {
        if (_molecCounts[0].text != " " && int.Parse(_molecCounts[0].text) > _maxMolecCount)
        {
            _molecCounts[0].text = _maxMolecCount.ToString();
        }

        if (_molecCounts[1].text != " " && int.Parse(_molecCounts[1].text) > _maxMolecCount)
        {
            _molecCounts[1].text = _maxMolecCount.ToString();
        }

        if (_molecCounts[2].text != " " && int.Parse(_molecCounts[2].text) > _maxMolecCount)
        {
            _molecCounts[2].text = _maxMolecCount.ToString();
        }
    }

    public void Simulate()
    {
        StartSimulationTemp = true;
        StartSimulationContent = true;
        foreach (var smoke in _smokes) smoke.Play();
        _simulationButton.interactable = false;
        _simulationText.text = "Идет симуляция";

        if (_texts[13].text != "")
        {
            if (_texts[13].text == "3 кВ/м")
            {
                _electroFilter.speed = 0.7f;
                Debug.Log("0.7");
            }
            else if (_texts[13].text == "4 кВ/м")
            {
                _electroFilter.speed = 0.8f;
                Debug.Log("0.8");
            }
            else
            {
                _electroFilter.speed = 1f;
                Debug.Log("1");
            }
        }
    }
}
