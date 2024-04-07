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
    public Material _cylinder;

    [SerializeField] private ParticleSystem _mySmoke;
    [SerializeField] private ParticleSystem _mySmoke2;

    [SerializeField] private PlayableDirector flow;
    [SerializeField] private AlembicStreamPlayer flows;

    [SerializeField] private GameObject obj1;
    [SerializeField] private GameObject obj1_1;
    [SerializeField] private GameObject obj2;
    [SerializeField] private GameObject obj2_1;
    [SerializeField] private GameObject obj3;
    [SerializeField] private GameObject obj3_1;
    [SerializeField] private GameObject obj4;
    [SerializeField] private GameObject obj4_1;
    [SerializeField] private GameObject obj5;
    [SerializeField] private GameObject obj5_1;
    [SerializeField] private GameObject obj6;
    [SerializeField] private GameObject obj6_1;

    private bool filling = true;
    private bool unfilling = false;

    private bool _startingProcess = false; //false
    private bool _startingDelay = false;

    private float delay = 5f; //update
    private float fillingCount = 0;
    private int count = 0;

    private float first_display = 0;
    private float second_display = 0;
    private float third_display = 0;
    private float fourth_display = 0;
    private float fifth_display = 0;
    private float sixth_display = 0;

    public float cylinder_filling = -29f;
    private float a;

    void Start()
    {
        a = cylinder_filling;
        absent[0].color = Color.white;
        absent[1].color = Color.white;

        obj1.SetActive(true);
        obj1_1.SetActive(false);
        obj2.SetActive(true);
        obj2_1.SetActive(false);
        obj3.SetActive(true);
        obj3_1.SetActive(false);
        obj4.SetActive(true);
        obj4_1.SetActive(false);
        obj5.SetActive(true);
        obj5_1.SetActive(false);
        obj6.SetActive(true);
        obj6_1.SetActive(false);
    }

    void Update()
    {
        _cylinder.SetFloat("_Filling", cylinder_filling);
        displays[0].text = first_display.ToString("0.");
        displays[1].text = second_display.ToString("0.");
        displays[2].text = third_display.ToString("0.");
        displays[3].text = fourth_display.ToString("0.");
        displays[4].text = fifth_display.ToString("0.");
        displays[5].text = sixth_display.ToString("0.");
        if (cylinder_filling >= 29)
        {
            cylinder_filling = -29;
        }
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
                    obj1.SetActive(false);
                    obj1_1.SetActive(true);
                    obj5.SetActive(false);
                    obj5_1.SetActive(true);
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

                    obj4.SetActive(false);
                    obj4_1.SetActive(true);

                    co2_SecondTubeOut.Play();
                    co2_ThirdTubeOut.Play();
                    co2_FourthTubeOut.Play();
                    co2_FifthTubeOut.Play();
                    co2_SixthTubeOut.Play();
                    cleanAir_FirstTubeOut.Play();
                    _mySmoke.Play();
                    dustAir_FirstTubeIn.Play();

                    par_Second.Play();

                    obj1_1.SetActive(true);
                    obj1.SetActive(false);
                    obj5_1.SetActive(true);
                    obj5.SetActive(false);

                    obj2.SetActive(true);
                    obj2_1.SetActive(false);
                    obj6.SetActive(true);
                    obj6_1.SetActive(false);
                }
                else if (fillingCount > 35 && fillingCount < 65)
                {
                    sixth_display = Mathf.Lerp(sixth_display, 100f, 2 * Time.deltaTime);
                    third_display = Mathf.Lerp(third_display, 0, 5f * Time.deltaTime);
                }
                else if (fillingCount > 85 && fillingCount < 90)
                {
                    flow.Play();
                    //first_display = Mathf.Lerp(first_display, 1950f, 2 * Time.deltaTime);
                }
                else if (fillingCount > 105 && fillingCount < 125)
                {
                    flow.Stop();
                    flows.CurrentTime = 0;
                    cylinder_filling = Mathf.Lerp(cylinder_filling, cylinder_filling + 3, 1 * Time.deltaTime);
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
                else if (fillingCount > 153 && fillingCount < 160)
                {

                    obj4.SetActive(true);
                    obj4_1.SetActive(false);

                }
                else if (fillingCount > 160 && fillingCount < 200)
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
                gates[4].SetActive(true);
                gates[3].SetActive(false);
                gates[5].SetActive(false);
                gates[6].SetActive(true);
                 
                if (fillingCount >= 250)
                {
                    hotAir_Second.Stop();
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

                    obj3.SetActive(false);
                    obj3_1.SetActive(true);


                    co2_FirstTubeOut.Play();
                    co2_ThirdTubeOut.Play();
                    co2_FourthTubeOut.Play();
                    co2_FifthTubeOut.Play();
                    co2_SixthTubeOut.Play();
                    dustAir_SecondTubeIn.Play();
                    _mySmoke2.Play();
                    cleanAir_SecondTubeOut.Play();

                    par_First.Play();

                    obj1_1.SetActive(false);
                    obj1.SetActive(true);
                    obj5_1.SetActive(false);
                    obj5.SetActive(true);

                    obj2.SetActive(false);
                    obj2_1.SetActive(true);
                    obj6.SetActive(false);
                    obj6_1.SetActive(true);
                }
                else if (fillingCount > 35 && fillingCount < 65)
                {
                    fifth_display = Mathf.Lerp(fifth_display, 100f, 2  * Time.deltaTime);
                    first_display = Mathf.Lerp(first_display, 0, 5f * Time.deltaTime);
                }
                else if (fillingCount > 85 && fillingCount < 90)
                {
                    flow.Play();
                }
                else if (fillingCount > 105 && fillingCount < 125)
                {
                    flow.Stop();
                    flows.CurrentTime = 0;
                    cylinder_filling = Mathf.Lerp(cylinder_filling, cylinder_filling + 3, 1 * Time.deltaTime);
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
                    
                    flows.CurrentTime = 0;

                }
                else if (fillingCount > 153 && fillingCount < 160)
                {

                    obj3.SetActive(true);
                    obj3_1.SetActive(false);
                }

                else if (fillingCount > 160 && fillingCount < 200)
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
                gates[4].SetActive(false);
                gates[5].SetActive(true);
                gates[6].SetActive(false);


                if (fillingCount >= 250)
                {
                    hotAir_First.Stop();
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
        delay = 60; // update to 60f
    }

    public void StopColumnProcess()
    {
        _startingProcess = false;
        _startingProcess = false;
        _startingDelay = false;
        fillingCount = 0;
        cylinder_filling = -29;

        obj1.SetActive(true);
        obj1_1.SetActive(false);
        obj2.SetActive(true);
        obj2_1.SetActive(false);
        obj3.SetActive(true);
        obj3_1.SetActive(false);
        obj4.SetActive(true);
        obj4_1.SetActive(false);
        obj5.SetActive(true);
        obj5_1.SetActive(false);
        obj6.SetActive(true);
        obj6_1.SetActive(false);

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
