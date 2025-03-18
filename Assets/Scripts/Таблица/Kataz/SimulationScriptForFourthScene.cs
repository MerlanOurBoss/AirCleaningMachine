using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class SimulationScriptForFourthScene: MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private TMP_InputField[] _molecCount;
    [SerializeField] private PlayableDirector[] _myFluidsZero;
    [SerializeField] private PlayableDirector[] _myFluidsWater;
    [SerializeField] private PlayableDirector[] _myFluidsReact;

    [SerializeField] private TextMeshProUGUI[] _componentsText;
    [SerializeField] private TextMeshProUGUI[] _molText;
    [SerializeField] private DropSpawner[] _dropSpawns;

    [SerializeField] private TextMeshProUGUI _N2molec;
    [SerializeField] private Button _simulationButton;
    [SerializeField] private TextMeshProUGUI _simulationText;

    [SerializeField] private Animator _electroFilter;
    [SerializeField] private NewSborScript _myCollector;
    [SerializeField] private TemperatureCatalizator _myCatalizator;

    [SerializeField] private GameObject ComponentsCameras;
    [SerializeField] private GameObject ElectoroObject;
    [SerializeField] private GameObject ElectoroObjectTable;
    [SerializeField] private GameObject NoElectroObject;

    [SerializeField] private NasosScript waterZeroNasos;
    [SerializeField] private NasosScript waterNasos;
    [SerializeField] private NasosScript reactNasos;

    [SerializeField] private Button electro;

    [SerializeField] private TextMeshProUGUI ElectroFilter;
    [SerializeField] private TextMeshProUGUI Katalizator;
    [SerializeField] private TextMeshProUGUI WaterEmul;
    [SerializeField] private TextMeshProUGUI ReactEmul;
    [SerializeField] private TextMeshProUGUI SborCO2;

    [SerializeField] private GameObject tables;
    [SerializeField] private KatalizatorTable kataz;

    [SerializeField] private ElectroSecond elec;
    [SerializeField] private KatalizatorTableOutElectro katazOutElectro;

    [SerializeField] private SheloshTable shelosh;
    [SerializeField] private WaterEmulFirst water;
    [SerializeField] private ReactTable react;
    [SerializeField] private SborTable sbor;
    [SerializeField] private Canvas canvas;

    public bool _startSimulationTemp = false;
    public bool _startSimulationContent = false;
    private float _simulationTime = 3400f;

    public float _fluidDelayZero;
    private float _fluidDelayZeroPrivate;
    public float _fluidDelayWater;
    private float _fluidDelayWaterPrivate;
    public float _fluidDelayReact;
    private float _fluidDelayReactPrivate;

    public float _electroAnimDelay;
    private float _electroAnimDelayPrivate;

    private int max = 130;
    private void Start()
    {
        _fluidDelayZeroPrivate = _fluidDelayZero;
        _fluidDelayWaterPrivate = _fluidDelayWater;
        _fluidDelayReactPrivate = _fluidDelayReact;
        _electroAnimDelayPrivate  = _electroAnimDelay;
    }

    private void Update()
    {
        if (_simulationTime <= 0)
        {
            _startSimulationTemp = false;
            _startSimulationContent = false;

        }
        if (_startSimulationTemp && _startSimulationContent)
        {
            _simulationButton.interactable = false;
            _simulationText.text = "Идет симуляция";
            waterZeroNasos.enabled = true;
            waterNasos.enabled = true;
            if (reactNasos != null)
            {
                reactNasos.enabled = true;
            }
            
            _simulationTime -= 1 * Time.deltaTime;
            _fluidDelayWater -= 1 * Time.deltaTime;
            _fluidDelayReact -= 1 * Time.deltaTime;
            _fluidDelayZero -= 1 * Time.deltaTime;
            _electroAnimDelay -= 1 * Time.deltaTime;
            tables.SetActive(true);

            if (_electroAnimDelay < 0)
            {
                _electroFilter.Play("NewColecAnim");
            }
            if (_fluidDelayZero < 0)
            {
                foreach (PlayableDirector fluid in _myFluidsZero)
                {
                    fluid.Play();
                }
            }
            if (_fluidDelayWater < 0)
            {
                foreach (PlayableDirector fluid in _myFluidsWater)
                {
                    fluid.Play();
                }
            }
            if (_fluidDelayReact < 0)
            {
                foreach (PlayableDirector fluid in _myFluidsReact)
                {
                    fluid.Play();
                }
            }
        }
        else
        {
            waterZeroNasos.enabled = false;
            waterNasos.enabled = false;
            if (reactNasos != null)
            {
                reactNasos.enabled = false;
            }
            
            _simulationButton.interactable = true;
            _simulationText.text = "Симулировать";
            _simulationTime = 3400f;
            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Stop();
            }
            _electroFilter.Play("NewColecAnimStop");
            _myCollector.StopColumnProcess();
            
            _myCatalizator.StopSimulation();
            tables.SetActive(false);
            foreach (PlayableDirector fluid in _myFluidsWater)
            {
                fluid.Stop();
            }
            foreach (PlayableDirector fluid in _myFluidsReact)
            {
                fluid.Stop();
            }
            foreach (DropSpawner drop in _dropSpawns)
            {
                drop.stopCor();
            }
        }
    }

    public void SubstractValue()
    {
        foreach (var mol in _molecCount)
        {
            int n = int.Parse(mol.text.ToString());

            max -= n;

            Debug.Log(n + "  " + max);

            _N2molec.text = max.ToString() + " %";
        }
    }
    public void StartSmokesAndFluids()
    {
        if (_startSimulationTemp && _startSimulationContent)
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Play();
            }

            _myCollector.StartColumnProcess();
            
            foreach (DropSpawner drop in _dropSpawns)
            {
                drop.startCor();
            }
        }
    }

    public void StartSimulation()
    {
        sbor.isEnable = true;
        _myCatalizator.StartSimulation();
        _startSimulationTemp = true; //надо будет убрать


        for (int i = 0; i <= 8; i++)
        {
            if (_molecCount[i].text == "")
            {
                _molText[i].color = Color.red;
                _startSimulationContent = false;
            }
            else
            {
                _molText[i].color = Color.black;
                _startSimulationContent = true;
            }
            
        }
    }

    public void StopSimulation()
    {
        _startSimulationTemp = false;
        _startSimulationContent = false;
        _myCatalizator.StopSimulation();
        _myCollector.StopColumnProcess();
        _fluidDelayZero = _fluidDelayZeroPrivate;
        _fluidDelayWater = _fluidDelayWaterPrivate; //15f
        _fluidDelayReact = _fluidDelayReactPrivate ; //32f
        _electroAnimDelay = _electroAnimDelayPrivate;
}
}
