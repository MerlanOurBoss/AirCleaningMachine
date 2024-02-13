using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class SimulationScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private TMP_InputField[] _molecCount;
    [SerializeField] private PlayableDirector[] _myFluidsWater;
    [SerializeField] private PlayableDirector[] _myFluidsReact;
    [SerializeField] private GameObject[] _dropCreating;
    //[SerializeField] private GameObject[] _errors;
    [SerializeField] private TMP_InputField[] _myTexts;
    [SerializeField] private TextMeshProUGUI[] _componentsText;
    [SerializeField] private TextMeshProUGUI[] _molText;

    [SerializeField] private TextMeshProUGUI _N2molec;
    [SerializeField] private Button _simulationButton;
    [SerializeField] private TextMeshProUGUI _simulationText;

    [SerializeField] private Animator _electroFilter;
    [SerializeField] private Animator _lightBulb;
    [SerializeField] private NewCollectors _myCollector;
    [SerializeField] private TemperatureCatalizator _myCatalizator;

    [SerializeField] private GameObject ComponentsCameras;

    [SerializeField] private TextMeshProUGUI ElectroFilter;
    [SerializeField] private TextMeshProUGUI Katalizator;
    [SerializeField] private TextMeshProUGUI WaterEmul;
    [SerializeField] private TextMeshProUGUI ReactEmul;
    [SerializeField] private TextMeshProUGUI SborCO2;


    private bool _startSimulationTemp = false;
    private bool _startSimulationContent = false;
    private float _simulationTime = 1400f;
    private float _fluidDelayWater = 15f;
    private float _fluidDelayReact = 32f;

    private int max = 110;
    private void Start()
    {
        //_startSimulationTemp = true;
        //_startSimulationContent = true;
        //StartSmokesAndFluids();
    }

    private void Update()
    {
        if (_myTexts[13].text != " " && _myTexts[12].text != " ")
        {
            float resEle = 1 - Mathf.Exp(-(float.Parse(_myTexts[13].text) * 2) / float.Parse(_myTexts[12].text));
            Debug.Log(resEle);
            ElectroFilter.text = "Ёффект. электрофильтра: " + (resEle * 100).ToString("0.") + " %";
        }

        if (_myTexts[20].text != " ")
        {
            float resKataz = 0;
            if (_myTexts[20].text == "300")
            {
                resKataz = 1 + Mathf.Exp(-0.05f * (float.Parse(_myTexts[20].text) - (float.Parse(_myTexts[20].text) - 50f)) * 1f);
                resKataz = 1 / resKataz;
            }
            else if (_myTexts[20].text == "400")
            {
                resKataz = 1 + Mathf.Exp(-0.05f * (float.Parse(_myTexts[20].text) - (float.Parse(_myTexts[20].text) - 150f)) * 1f);
                resKataz = 1 / resKataz;
            }
            else if (_myTexts[20].text == "500")
            {
                resKataz = 1 + Mathf.Exp(-0.05f * (float.Parse(_myTexts[20].text) - (float.Parse(_myTexts[20].text) - 100f)) * 1f);
                resKataz = 1 / resKataz;
            }
            Katalizator.text = "Ёффект. катализатора: " + (resKataz * 100).ToString("0.") + " %";
        }

        if (_myTexts[14].text != " " && _myTexts[15].text != " ")
        {
            float resWater = float.Parse(_myTexts[14].text) * 10 * float.Parse(_myTexts[15].text);
            WaterEmul.text = "Ёффект. вод€ного эмуль.: " + (resWater * 100).ToString("0.") + " %";
        }

        if (_myTexts[16].text != " " && _myTexts[17].text != " ")
        {
            float resReact = float.Parse(_myTexts[16].text) * 10 * float.Parse(_myTexts[17].text);
            ReactEmul.text = "Ёффект. реагент. эмуль.: " + (resReact * 100).ToString("0.") + " %";
        }

        if (_myTexts[18].text != " " && _myTexts[19].text != " ")
        {
            float resSborCO2 = (10 * float.Parse(_myTexts[18].text) * float.Parse(_myTexts[19].text)) / (1 + (float.Parse(_myTexts[18].text) * float.Parse(_myTexts[19].text)));
            SborCO2.text = "Ёффект. сбор CO2: " + (resSborCO2 * 100).ToString("0.") + " %";
        }
        


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
            ComponentsCameras.SetActive(false);
            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Stop();
            }
            _electroFilter.Play("NewColecAnimStop");
            _myCollector.StopColumnProcess();
            _myCatalizator.StopSimulation();
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
            _myCollector.StartColumnProcess3();
            _myCatalizator.StartSimulation();
            _lightBulb.Play("LightsAnimation");
            foreach (GameObject drop in _dropCreating)
            {
                drop.SetActive(true);
            }
        }
    }

    public void StartSimulation()
    {
        for(int i = 0; i <= 8; i++)
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

        if (_myTexts[0].text == "" || _myTexts[1].text == "" || _myTexts[2].text == "")
        {
            _componentsText[0].color = Color.red;
            _startSimulationTemp = false;
        }
        else
        {
            _componentsText[0].color = Color.black;
            _startSimulationTemp = true;
        }

        if (_myTexts[3].text == "" || _myTexts[4].text == "" || _myTexts[5].text == "")
        {
            _componentsText[1].color = Color.red;
            _startSimulationTemp = false;
        }
        else
        {
            _componentsText[1].color = Color.black;
            _startSimulationTemp = true;
        }

        if (_myTexts[6].text == "" || _myTexts[7].text == "" )
        {
            _componentsText[2].color = Color.red;
            _startSimulationTemp = false;
        }
        else
        {
            _componentsText[2].color = Color.black;
            _startSimulationTemp = true;
        }

        if (_myTexts[8].text == "" || _myTexts[9].text == "")
        {
            _componentsText[3].color = Color.red;
            _startSimulationTemp = false;
        }
        else
        {
            _componentsText[3].color = Color.black;
            _startSimulationTemp = true;
        }

        if (_myTexts[10].text == "" || _myTexts[11].text == "")
        {
            _componentsText[4].color = Color.red;
            _startSimulationTemp = false;
        }
        else
        {
            _componentsText[4].color = Color.black;
            _startSimulationTemp = true;
            StartSmokesAndFluids();
        }

        if (_myTexts[0].text == "150 ∞C"  && _myTexts[3].text == "300 ∞C" 
            && _myTexts[6].text == "5 ∞C" && _myTexts[8].text == "5 ∞C" && _myTexts[10].text == "5 ∞C")
        {

        }
        else if (_myTexts[0].text == "300 ∞C" && _myTexts[3].text == "400 ∞C"
            && _myTexts[6].text == "15 ∞C" && _myTexts[8].text == "15 ∞C" && _myTexts[10].text == "15 ∞C")
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.startSpeed = smoke.startSpeed + 1f;
            }
        }
        else if (_myTexts[0].text == "400 ∞C" && _myTexts[3].text == "500 ∞C"
            && _myTexts[6].text == "25 ∞C" && _myTexts[8].text == "25 ∞C" && _myTexts[10].text == "25 ∞C")
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.startLifetime = smoke.startLifetime - smoke.startLifetime / 2;
                smoke.startSpeed = smoke.startSpeed + 2f;
            }
        }

        //float resEle = 1 - Mathf.Exp(-(float.Parse(_myTexts[13].text) * 2) / float.Parse(_myTexts[12].text));
        //Debug.Log(resEle);
        //ElectroFilter.text = "Ёффект. электрофильтра: " + (resEle * 100).ToString("0.") + " %";
        ComponentsCameras.SetActive(true);
    }
}
