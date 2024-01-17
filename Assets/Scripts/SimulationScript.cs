using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class SimulationScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField _myTemperature;
    [SerializeField] private TMP_InputField _mySpeed;
    [SerializeField] private TMP_InputField _myWidth;
    [SerializeField] private TMP_InputField _myTextMeshProContent;

    [SerializeField] private ParticleSystem[] _mySmokes;

    [SerializeField] private TMP_InputField[] _molecCount;
    [SerializeField] private TextMeshProUGUI _N2molec;

    [SerializeField] private PlayableDirector[] _myFluidsWater;
    [SerializeField] private PlayableDirector[] _myFluidsReact;

    [SerializeField] private Button _simulationButton;
    [SerializeField] private TextMeshProUGUI _simulationText;

    public Animator _electroFilter;
    public Animator _lightBulb;
    public Collectors _myCollector;


    public GameObject _errorTextTemp;
    public GameObject _errorTextSpeed;
    public GameObject _errorTextWidth;

    public GameObject _errorTextContent;

    public GameObject[] _dropCreating;
    private bool _startSimulationTemp = false;
    private bool _startSimulationContent = false;
    private float _simulationTime = 1400f;
    private float _fluidDelayWater = 15f;
    private float _fluidDelayReact = 32f;

    private int max = 110;
    private void Start()
    {
        _startSimulationTemp = true;
        _startSimulationContent = true;
        StartSmokesAndFluids();
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
            _simulationText.text = "»дет симул€ци€";
            _simulationTime -= 1 * Time.deltaTime;
            _fluidDelayWater -= 1 * Time.deltaTime;
            _fluidDelayReact -= 1 * Time.deltaTime;

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
            _simulationButton.interactable = true;
            _simulationText.text = "—имулировать";
            _simulationTime = 140f;

            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Stop();
            }
            _electroFilter.Play("NewColecAnimStop");
            _myCollector.StopColumnProcess();
            _lightBulb.Play("LightsAnimationStops");
            foreach (PlayableDirector fluid in _myFluidsWater)
            {
                fluid.Stop();
            }
            foreach (PlayableDirector fluid in _myFluidsReact)
            {
                fluid.Stop();
            }
            foreach (GameObject drop in _dropCreating)
            {
                drop.SetActive(false);
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
            _electroFilter.Play("NewColecAnim");
            _myCollector.StartColumnProcess();
            _lightBulb.Play("LightsAnimation");
            foreach (GameObject drop in _dropCreating)
            {
                drop.SetActive(true);
            }
        }
    }

    public void StartSimulation()
    {
        foreach(var mol in _molecCount)
        {
            if (mol.text == "")
            {
                _startSimulationContent = false;
                _errorTextContent.SetActive(true);
                break;
            }
            else
                _startSimulationContent = true;
                _errorTextContent.SetActive(false);
        }

        if (_mySpeed.text == "")
        {
            _errorTextSpeed.SetActive(true);
            _startSimulationTemp = false;
        }
        else
        {
            _errorTextSpeed.SetActive(false);
            _startSimulationTemp = true;
        }

        if (_myWidth.text == "")
        {
            _errorTextWidth.SetActive(true);
            _startSimulationTemp = false;
        }
        else
        {
            _errorTextWidth.SetActive(false);
            _startSimulationTemp = true;
        }

        if (_myTemperature.text == "")
        {
            Debug.Log("null");
            _errorTextTemp.SetActive(true);
            _startSimulationTemp = false;
        }
        else
        {
            _errorTextTemp.SetActive(false);
            _startSimulationTemp = true;
            StartSmokesAndFluids();
        }
    }
}
