using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class NewCollectors : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustAir_FirstTubeIn;
    [SerializeField] private ParticleSystem dustAir_SecondTubeIn;

    [SerializeField] private ParticleSystem co2_FirstTubeOut;
    [SerializeField] private ParticleSystem co2_SecondTubeOut;
    [SerializeField] private ParticleSystem co2_ThirdTubeOut;
    [SerializeField] private ParticleSystem co2_FourthTubeOut;
    [SerializeField] private ParticleSystem co2_FifthTubeOut;
    [SerializeField] private ParticleSystem co2_SixthTubeOut;

    [SerializeField] private ParticleSystem cleanAir_FirstTubeOut;
    [SerializeField] private ParticleSystem cleanAir_SecondTubeOut;

    [SerializeField] private ParticleSystem hotAir_Main;
    [SerializeField] private ParticleSystem hotAir_First;
    [SerializeField] private ParticleSystem hotAir_Second;

    [SerializeField] private ParticleSystem par_First;
    [SerializeField] private ParticleSystem par_Second;

    [SerializeField] private TextMeshProUGUI[] displays;

    public GameObject[] gates;

    public Material[] absent;

    public Color targetColor;
    public Animator waterFilling;

    [SerializeField] private ParticleSystem _mySmoke;
    [SerializeField] private ParticleSystem _mySmoke2;

    private bool filling = true;
    private bool unfilling = false;

    private bool _startingProcess = false; //false
    private bool _startingDelay = false;

    public float delay; //update
    public float delayUpdate; //update
    private float fillingCount = 0;
    private int count = 0;

    private float first_display = 0;
    private float second_display = 0;
    private float third_display = 0;
    private float fourth_display = 0;
    private float fifth_display = 0;
    private float sixth_display = 0;


    void Start()
    {
        absent[0].color = Color.white;
        absent[1].color = Color.white;
    }

    void Update()
    {
        displays[0].text = first_display.ToString("0.");
        displays[1].text = second_display.ToString("0.");
        displays[2].text = third_display.ToString("0.");
        displays[3].text = fourth_display.ToString("0.");
        displays[4].text = fifth_display.ToString("0.");
        displays[5].text = sixth_display.ToString("0.");

        if (_startingDelay)
        {
            delay -= 1 * Time.deltaTime;
            if (delay <= 0)
            {
                _startingProcess = true;
                _startingDelay = false;
            }
        }

        if (_startingProcess && count == 0)
        {
            if (filling && !unfilling)
            {
                fillingCount += 5 * Time.deltaTime;

                if (fillingCount < 1)
                {
                    cleanAir_FirstTubeOut.Play();
                    _mySmoke.Play();
                    dustAir_FirstTubeIn.Play();
                }
                if (fillingCount > 60 && fillingCount < 80)
                {
                    //first_display = Mathf.Lerp(first_display, 1950f, 2 * Time.deltaTime);
                }


                gates[0].SetActive(true);
                gates[3].SetActive(true);
                gates[5].SetActive(true);

                Invoke("AbsentOn", 5f);

                if (fillingCount >= 150)
                {
                    filling = false;
                    unfilling = true;
                    count++;
                    fillingCount = 0;
                }
            }
        }
        else if (_startingProcess && count == 1)
        {
            if (filling && !unfilling)
            {
                fillingCount += 5 * Time.deltaTime;
                if (fillingCount > 0 && fillingCount < 20)
                {
                    third_display = Mathf.Lerp(third_display, 100f, 2 * Time.deltaTime);
                }
                if (fillingCount > 30 && fillingCount < 51)
                {
                    fourth_display = Mathf.Lerp(fourth_display, 1950f, 2 * Time.deltaTime);
                }
                if (fillingCount < 1)
                {
                    dustAir_SecondTubeIn.Stop();
                    _mySmoke2.Stop();
                    cleanAir_SecondTubeOut.Stop();

                    co2_SecondTubeOut.Play();
                    co2_ThirdTubeOut.Play();
                    co2_FourthTubeOut.Play();
                    co2_FifthTubeOut.Play();
                    co2_SixthTubeOut.Play();
                    cleanAir_FirstTubeOut.Play();
                    _mySmoke.Play();
                    dustAir_FirstTubeIn.Play();

                    par_Second.Play();
                }
                else if (fillingCount > 35 && fillingCount < 65)
                {
                    hotAir_First.Stop();
                    gates[4].SetActive(true);
                    gates[5].SetActive(false);


                    sixth_display = Mathf.Lerp(sixth_display, 100f, 2 * Time.deltaTime);
                    third_display = Mathf.Lerp(third_display, 0, 5f * Time.deltaTime);
                }
                else if (fillingCount > 85 && fillingCount < 90)
                {
                    waterFilling.Play("WaterAnimation");
                    //first_display = Mathf.Lerp(first_display, 1950f, 2 * Time.deltaTime);
                }
                else if (fillingCount > 150 && fillingCount < 152)
                {
                    hotAir_Main.Play();
                    hotAir_Second.Play();
                    
                    par_Second.Stop();
                    co2_SecondTubeOut.Stop();
                    co2_ThirdTubeOut.Stop();
                    co2_FourthTubeOut.Stop();
                    co2_FifthTubeOut.Stop();
                    co2_SixthTubeOut.Stop();

                }

                else if (fillingCount > 170 && fillingCount < 200)
                {
                    fourth_display = Mathf.Lerp(fourth_display, 0f, 2 * Time.deltaTime);

                }
                else if (fillingCount > 200 && fillingCount < 220)
                {
                    sixth_display = Mathf.Lerp(sixth_display, 0f, 2 * Time.deltaTime);
                    
                }

                Invoke("AbsentOn", 5f);
                Invoke("Absent1Off", 38f);

                gates[0].SetActive(false);
                gates[1].SetActive(true);
                gates[2].SetActive(true);
                
                gates[3].SetActive(false);
                
                gates[6].SetActive(true);
                 
                if (fillingCount >= 250)
                {
                    
                    hotAir_Main.Stop();
                    
                    filling = false;
                    unfilling = true;
                    fillingCount = 0;
                }

            }
            else if (unfilling && !filling)
            {
                fillingCount += 5 * Time.deltaTime;
                if (fillingCount > 0 && fillingCount < 20)
                {
                    first_display = Mathf.Lerp(first_display, 100f, 2 * Time.deltaTime);
                }
                if (fillingCount > 30 && fillingCount < 51)
                {
                    second_display = Mathf.Lerp(second_display, 1950f, 2 * Time.deltaTime);
                }
                if (fillingCount < 1)
                {
                    cleanAir_FirstTubeOut.Stop();
                    _mySmoke.Stop();
                    dustAir_FirstTubeIn.Stop();

                    co2_FirstTubeOut.Play();
                    co2_ThirdTubeOut.Play();
                    co2_FourthTubeOut.Play();
                    co2_FifthTubeOut.Play();
                    co2_SixthTubeOut.Play();
                    dustAir_SecondTubeIn.Play();
                    _mySmoke2.Play();
                    cleanAir_SecondTubeOut.Play();

                    par_First.Play();
                }

                else if (fillingCount > 35 && fillingCount < 65)
                {
                    gates[4].SetActive(false);
                    gates[5].SetActive(true);

                    hotAir_Second.Stop();

                    fifth_display = Mathf.Lerp(fifth_display, 100f, 2  * Time.deltaTime);
                    first_display = Mathf.Lerp(first_display, 0, 5f * Time.deltaTime);
                }
                else if (fillingCount > 85 && fillingCount < 90)
                {
                    waterFilling.Play("WaterAnimation");
                }

                else if (fillingCount > 150 && fillingCount < 152)
                {
                    hotAir_First.Play();
                    hotAir_Main.Play();

                    par_First.Stop();
                    co2_FirstTubeOut.Stop();
                    co2_ThirdTubeOut.Stop();
                    co2_FourthTubeOut.Stop();
                    co2_FifthTubeOut.Stop();
                    co2_SixthTubeOut.Stop();

                }

                else if (fillingCount > 170 && fillingCount < 200)
                {
                    second_display = Mathf.Lerp(second_display, 0f, 2 * Time.deltaTime);

                }
                else if (fillingCount > 200 && fillingCount < 220)
                {
                    fifth_display = Mathf.Lerp(fifth_display, 0f, 2 * Time.deltaTime);
                    
                }

                Invoke("Absent1On", 5f);
                Invoke("AbsentOff", 38f);

                gates[0].SetActive(true);
                gates[1].SetActive(false);
                gates[2].SetActive(false);
                gates[3].SetActive(true);
                
                gates[6].SetActive(false);


                if (fillingCount >= 250)
                {
                    
                    hotAir_Main.Stop();
                    

                    filling = true;
                    unfilling = false;
                    fillingCount = 0;                    
                }

            }
        }
    }
    public void AbsentOn()
    {
        absent[0].color = Color.Lerp(absent[0].color, targetColor, .1f * Time.fixedDeltaTime);
    }

    public void AbsentOff()
    {
        absent[0].color = Color.Lerp(absent[0].color, Color.white, .15f * Time.fixedDeltaTime);
    }

    public void Absent1On()
    {
        absent[1].color = Color.Lerp(absent[1].color, targetColor, .1f * Time.fixedDeltaTime);
    }

    public void Absent1Off()
    {
        absent[1].color = Color.Lerp(absent[1].color, Color.white, .15f * Time.fixedDeltaTime);
    }

    public void StartColumnProcess3()
    {
        _startingDelay = true;
        delay = delayUpdate;
    }

    public void StopColumnProcess()
    {
        _startingProcess = false;
        _startingProcess = false;
        _startingDelay = false;
        fillingCount = 0;


        absent[0].color = Color.Lerp(absent[0].color, Color.white, .15f * Time.fixedDeltaTime);
        absent[1].color = Color.Lerp(absent[1].color, Color.white, .15f * Time.fixedDeltaTime);
        _mySmoke.Stop();
        _mySmoke2.Stop();
        dustAir_FirstTubeIn.Stop();
        dustAir_SecondTubeIn.Stop();

        co2_FirstTubeOut.Stop();
        co2_SecondTubeOut.Stop();
        co2_ThirdTubeOut.Stop();
        co2_FourthTubeOut.Stop();
        co2_FifthTubeOut.Stop();
        co2_SixthTubeOut.Stop();

        cleanAir_FirstTubeOut.Stop();
        cleanAir_SecondTubeOut.Stop();

        hotAir_Main.Stop();
        hotAir_First.Stop();
        hotAir_Second.Stop();

        par_First.Stop();
        par_Second.Stop();

        first_display = Mathf.Lerp(first_display, 0, 2 * Time.deltaTime);
        second_display = Mathf.Lerp(second_display, 0, 2 * Time.deltaTime);
        third_display = Mathf.Lerp(third_display, 0, 2 * Time.deltaTime);
        fourth_display = Mathf.Lerp(fourth_display, 0, 2 * Time.deltaTime);
        fifth_display = Mathf.Lerp(fifth_display, 0, 2 * Time.deltaTime);
        sixth_display = Mathf.Lerp(sixth_display, 0, 2 * Time.deltaTime);
    }
}
