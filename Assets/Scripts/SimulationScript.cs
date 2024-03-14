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
    [SerializeField] private DropSpawner[] _dropSpawns;

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

    [SerializeField] private ElectrofilterTable elec;
    [SerializeField] private KatalizatorTable kataz;
    [SerializeField] private WaterTable water;
    [SerializeField] private ReactTable react;
    [SerializeField] private SborTable sbor;
    [SerializeField] private Canvas canvas;
        

    private bool _startSimulationTemp = false;
    private bool _startSimulationContent = false;
    private float _simulationTime = 1400f;
    private float _fluidDelayWater = 15f;
    private float _fluidDelayReact = 32f;

    private int max = 130;
    private void Start()
    {
        _myTexts[0].text = "150 °C";
        _myTexts[1].text = "0.01 мА/см²";
        _myTexts[2].text = "80 %";
        _myTexts[4].text = "Циолит";
        _myTexts[5].text = "Платина";
        _myTexts[6].text = "5 °C";
        _myTexts[7].text = "Вода";
        _myTexts[8].text = "5 °C";
        _myTexts[9].text = "Едкий натрий ";
        _myTexts[10].text = "5 °C";
        _myTexts[11].text = "Цеолиты";
        _myTexts[12].text = "0,5 м/с";
        _myTexts[13].text = "3 кВ/м";
        _myTexts[20].text = "500 °C";
        _myTexts[14].text = "0,085 м/с";
        _myTexts[15].text = "1 моль/м";
        _myTexts[16].text = "0,05 м/с";
        _myTexts[17].text = "1,5 моль/м";
        _myTexts[18].text = "0,5 кПа";
        _myTexts[19].text = "0,2 кПа";
    }

    private void Update()
    {
        if (_myTexts[13].text != " " && _myTexts[12].text != " ")
        {   
            float resEle = 1 - Mathf.Exp(-(float.Parse(_myTexts[13].text[.._myTexts[13].text.IndexOf(" ")]) * 2) / float.Parse(_myTexts[12].text[.._myTexts[12].text.IndexOf(" ")]));
            //Debug.Log(resEle);
            ElectroFilter.text = "Эффект. электрофильтра: " + (resEle * 100).ToString("0.") + " %";
        }

        if (_myTexts[20].text != " ")
        {
            float resKataz = 0;
            if (_myTexts[20].text == "300 °C")
            {
                resKataz = 1 + Mathf.Exp(-0.05f * (float.Parse(_myTexts[20].text[.._myTexts[20].text.IndexOf(" ")]) - (float.Parse(_myTexts[20].text[.._myTexts[20].text.IndexOf(" ")]) - 50f)) * 1f);
                resKataz = 1 / resKataz;
            }
            else if (_myTexts[20].text == "400 °C")
            {
                resKataz = 1 + Mathf.Exp(-0.05f * (float.Parse(_myTexts[20].text[.._myTexts[20].text.IndexOf(" ")]) - (float.Parse(_myTexts[20].text[.._myTexts[20].text.IndexOf(" ")]) - 150f)) * 1f);
                resKataz = 1 / resKataz;
            }
            else if (_myTexts[20].text == "500 °C")
            {
                resKataz = 1 + Mathf.Exp(-0.05f * (float.Parse(_myTexts[20].text[.._myTexts[20].text.IndexOf(" ")]) - (float.Parse(_myTexts[20].text[.._myTexts[20].text.IndexOf(" ")]) - 100f)) * 1f);
                resKataz = 1 / resKataz;
            }
            Katalizator.text = "Эффект. катализатора: " + (resKataz * 100).ToString("0.") + " %";
        }

        if (_myTexts[14].text != " " && _myTexts[15].text != " ")
        {
            float resWater = float.Parse(_myTexts[14].text[.._myTexts[14].text.IndexOf(" ")]) * 10 * float.Parse(_myTexts[15].text[.._myTexts[15].text.IndexOf(" ")]);
            WaterEmul.text = "Эффект. водяного эмуль.: " + (resWater * 100).ToString("0.") + " %";
        }

        if (_myTexts[16].text != " " && _myTexts[17].text != " ")
        {
            float resReact = float.Parse(_myTexts[16].text[.._myTexts[16].text.IndexOf(" ")]) * 10 * float.Parse(_myTexts[17].text[.._myTexts[17].text.IndexOf(" ")]);
            ReactEmul.text = "Эффект. реагент. эмуль.: " + (resReact * 100).ToString("0.") + " %";
        }

        if (_myTexts[18].text != " " && _myTexts[19].text != " ")
        {
            float resSborCO2 = (10 * float.Parse(_myTexts[18].text[.._myTexts[18].text.IndexOf(" ")]) * float.Parse(_myTexts[19].text[.._myTexts[19].text.IndexOf(" ")])) / (1 + (float.Parse(_myTexts[18].text[.._myTexts[18].text.IndexOf(" ")]) * float.Parse(_myTexts[19].text[.._myTexts[19].text.IndexOf(" ")])));
            SborCO2.text = "Эффект. сбор CO2: " + (resSborCO2 * 100).ToString("0.") + " %";
        }
        


        if (_simulationTime <= 0)
        {
            _startSimulationTemp = false;
            _startSimulationContent = false;
        }
        if (_startSimulationTemp && _startSimulationContent)
        {
            canvas.enabled = false;
            _simulationButton.interactable = false;
            _simulationText.text = "Идет симуляция";
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
            canvas.enabled = true;
            _simulationButton.interactable = true;
            _simulationText.text = "Симулировать";
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

    [System.Obsolete]
    public void StartSimulation()
    {
        elec.isEnable = true;
        kataz.isEnable = true;
        water.isEnable = true;
        react.isEnable = true;
        sbor.isEnable = true;   
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

        if (_myTexts[0].text == "150 °C"  && _myTexts[3].text == "300 °C" 
            && _myTexts[6].text == "5 °C" && _myTexts[8].text == "5 °C" && _myTexts[10].text == "5 °C")
        {

        }
        else if (_myTexts[0].text == "300 °C" && _myTexts[3].text == "400 °C"
            && _myTexts[6].text == "15 °C" && _myTexts[8].text == "15 °C" && _myTexts[10].text == "15 °C")
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
                // Здесь можно задать что будет влиять на дымы
                //smoke.startColor = new Color(smoke.startColor.r, smoke.startColor.g, smoke.startColor.b, 0.3f);
                smoke.emissionRate = smoke.emissionRate - smoke.emissionRate / 2;
            }
            foreach (DropSpawner spawner in _dropSpawns)
            {
                spawner.spawnInterval = spawner.spawnInterval * 2;
            }
        }
        else if (_myTexts[0].text == "400 °C" && _myTexts[3].text == "500 °C"
            && _myTexts[6].text == "25 °C" && _myTexts[8].text == "25 °C" && _myTexts[10].text == "25 °C")
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
                //smoke.startLifetime = smoke.startLifetime - smoke.startLifetime / 2;
                //smoke.startSpeed = smoke.startSpeed + 2f;
                //smoke.startColor = new Color(smoke.startColor.r, smoke.startColor.g, smoke.startColor.b, 0.7f);
                smoke.emissionRate = smoke.emissionRate - smoke.emissionRate / 1.5f;
            }
            foreach (DropSpawner spawner in _dropSpawns)
            {
                spawner.spawnInterval = spawner.spawnInterval * 1.5f;
            }
        }

        //float resEle = 1 - Mathf.Exp(-(float.Parse(_myTexts[13].text) * 2) / float.Parse(_myTexts[12].text));
        //Debug.Log(resEle);
        //ElectroFilter.text = "Эффект. электрофильтра: " + (resEle * 100).ToString("0.") + " %";
        ComponentsCameras.SetActive(true);
    }
}
