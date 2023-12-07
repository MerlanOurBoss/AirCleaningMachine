using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class SimulationScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField _myTextMeshProTemper;
    [SerializeField] private TMP_InputField _myTextMeshProContent;

    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private ParticleSystem _myBubble;

    [SerializeField] private PlayableDirector[] _myFluids;

    [SerializeField] private Button _simulationButton;
    [SerializeField] private TextMeshProUGUI _simulationText;

    public GameObject _errorTextTemp;
    public GameObject _errorTextContent;

    private bool _startSimulationTemp = false;
    private bool _startSimulationContent = false;
    private float _simulationTime = 20f;
    private float _fluidDelay = 20f;

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
            _simulationText.text = "Èäåò ñèìóëÿöèÿ";
            _simulationTime -= 1 * Time.deltaTime;
            _fluidDelay -= 1 * Time.deltaTime;
            Debug.Log(_simulationTime.ToString("0"));
        }
        else
        {
            _simulationButton.interactable = true;
            _simulationText.text = "Ñèìóëèðîâàòü";
            _simulationTime = 10f; // change here to 50f

            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Stop();
            }
            foreach (PlayableDirector fluid in _myFluids)
            {
                fluid.Stop();
            }
        }
    }

    public void StartSimulation()
    {
        if (_myTextMeshProTemper.text == "")
        {
            Debug.Log("null");
            _errorTextTemp.SetActive(true);
            _startSimulationTemp = false;
        }
        else if (_myTextMeshProTemper.text == "25 °Ñ")
        {
            Debug.Log("25 °Ñ");
            _errorTextTemp.SetActive(false);
            _startSimulationTemp = true;

            foreach(ParticleSystem smoke in _mySmokes)
            {
                smoke.Play();
                _myBubble.Play();
            }
            foreach(PlayableDirector fluid in _myFluids)
            {
                fluid.Play();
            }
        }
        else if (_myTextMeshProTemper.text == "60 °Ñ")
        {
            Debug.Log("60 °Ñ");
            _errorTextTemp.SetActive(false);
        }
        else if (_myTextMeshProTemper.text == "250 °Ñ")
        {
            Debug.Log("250 °Ñ");
            _errorTextTemp.SetActive(false);
        }
        else if (_myTextMeshProTemper.text == "300 °Ñ")
        {
            Debug.Log("300 °Ñ");
            _errorTextTemp.SetActive(false);
        }
        else if (_myTextMeshProTemper.text == "450 °Ñ")
        {
            Debug.Log("450 °Ñ");
            _errorTextTemp.SetActive(false);
        }
        else if (_myTextMeshProTemper.text == "500 °Ñ")
        {
            Debug.Log("500 °Ñ");
            _errorTextTemp.SetActive(false);
        }

    }

    public void ContentChoseSimulation()
    {
        if (_myTextMeshProContent.text == "")
        {
            Debug.Log("null");  
            _errorTextContent.SetActive(true);
            _startSimulationContent = false;
        }
        else if (_myTextMeshProContent.text == "CO")
        {
            _errorTextContent.SetActive(false);
            _startSimulationContent = true;           
        }
        else if(_myTextMeshProContent.text == "CH4O2")
        {
            _errorTextContent.SetActive(false);
            _startSimulationContent = true;
        }
        else if(_myTextMeshProContent.text == "S2O4")
        {
            _errorTextContent.SetActive(false);
            _startSimulationContent = true;
        }
        else if(_myTextMeshProContent.text == "N2O2")
        {
            _errorTextContent.SetActive(false);
            _startSimulationContent = true;
        }
    }
}
