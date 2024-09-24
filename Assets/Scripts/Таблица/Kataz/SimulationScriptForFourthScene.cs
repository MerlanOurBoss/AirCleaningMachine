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

    [SerializeField] private TMP_InputField[] _myTexts;
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
    [SerializeField] private TMP_InputField electroInput;

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

    [SerializeField] private KatalizatorScriptwith1 katazScriptWith1_1;
    [SerializeField] private Button kataz1;
    [SerializeField] private KatalizatorScriptwith1 katazScriptWith1_2;
    [SerializeField] private Button kataz2;



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
        //_myTexts[21].text = "Включить";
        //_myTexts[22].text = "2";
        //_myTexts[23].text = "2";
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


        //if (_myTexts[21].text == "Включить")
        //{
        //    ElectoroObject.SetActive(true);
        //    NoElectroObject.SetActive(false);
        //    ElectoroObjectTable.SetActive(true);
        //    kataz.enabled = true;
        //    katazOutElectro.enabled = false;
        //}
        //else if (_myTexts[21].text == "Отключить")
        //{
        //    ElectoroObject.SetActive(false);
        //    NoElectroObject.SetActive(true);
        //    ElectoroObjectTable.SetActive(false);
        //    ElectroFilter.text = "Эффект. электрофильтра: 0%";
        //    kataz.enabled = false;
        //    katazOutElectro.enabled = true;
        //}

        

        if (_simulationTime <= 0)
        {
            _startSimulationTemp = false;
            _startSimulationContent = false;

        }
        if (_startSimulationTemp && _startSimulationContent)
        {
            //electro.interactable = false;
            electroInput.interactable = false;
            katazScriptWith1_1.text.interactable = false;
            kataz1.interactable = false;
            katazScriptWith1_2.text.interactable = false;
            kataz2.interactable = false;
            _simulationButton.interactable = false;
            _simulationText.text = "Идет симуляция";
            waterZeroNasos.enabled = true;
            waterNasos.enabled = true;
            reactNasos.enabled = true;
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
            //electro.interactable = true;
            electroInput.interactable = true;
            katazScriptWith1_1.text.interactable = true;
            kataz1.interactable = true;
            katazScriptWith1_2.text.interactable = true;
            kataz2.interactable = true;
            //canvas.enabled = true;
            waterZeroNasos.enabled = false;
            waterNasos.enabled = false;
            reactNasos.enabled = false;
            _simulationButton.interactable = true;
            _simulationText.text = "Симулировать";
            _simulationTime = 3400f;
            //ComponentsCameras.SetActive(false);
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
            _myCatalizator.StartSimulation();
            foreach (DropSpawner drop in _dropSpawns)
            {
                drop.startCor();
            }
        }
    }

    public void StartSimulation()
    {
        if (_myTexts[18].text == "Включить")
        {
            kataz.isEnable = true;
            katazOutElectro.isEnable = false;
        }
        else if (_myTexts[18].text == "Отключить")
        {
            kataz.isEnable = false;
            katazOutElectro.isEnable = true;
        }

        if (_myTexts[19].text == "2")
        {
            katazScriptWith1_1.SetObjectsState(false);
        }
        else if (_myTexts[19].text == "1")
        {
            katazScriptWith1_1.SetObjectsState(true);
        }

        if (_myTexts[20].text == "2")
        {
            katazScriptWith1_2.SetObjectsState(false);
        }
        else if (_myTexts[20].text == "1")
        {
            katazScriptWith1_2.SetObjectsState(true);
        }

        water.isEnable = true;
        water.RecalculateData();
        elec.isEnable = true;
        shelosh.isEnable = true;
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
        //ComponentsCameras.SetActive(true);
    }

    public void StopSimulation()
    {
        _startSimulationTemp = false;
        _startSimulationContent = false;
        _fluidDelayZero = _fluidDelayZeroPrivate;
        _fluidDelayWater = _fluidDelayWaterPrivate; //15f
        _fluidDelayReact = _fluidDelayReactPrivate ; //32f
        _electroAnimDelay = _electroAnimDelayPrivate;
}
}
