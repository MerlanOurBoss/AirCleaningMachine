using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NewCollectors : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustAir_FirstTubeIn;
    [SerializeField] private ParticleSystem dustAir_SecondTubeIn;

    [SerializeField] private ParticleSystem co2_FirstTubeOut;
    [SerializeField] private ParticleSystem co2_SecondTubeOut;

    [SerializeField] private ParticleSystem cleanAir_FirstTubeOut;
    [SerializeField] private ParticleSystem cleanAir_SecondTubeOut;

    public GameObject[] gates;

    public Material[] absent;

    public Color targetColor;
    public Material _greenLight;
    public Material _redLight;

    //public MeshRenderer _firstColumnLight;
    //public MeshRenderer _firstColumnLight_1;
    //public MeshRenderer _secondColumnLight;
    //public MeshRenderer _secondColumnLight_2;


    [SerializeField] private ParticleSystem _mySmoke;
    [SerializeField] private ParticleSystem _mySmoke2;

    private bool filling = true;
    private bool unfilling = false;

    private bool _startingProcess = false; //false
    private bool _startingDelay = false;

    private float delay = 5f; //update
    private float fillingCount = 0;
    private int count = 0;

    void Start()
    {

        absent[0].color = Color.white;
        absent[1].color = Color.white;
    }

    void Update()
    {
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
                if (fillingCount < 1)
                {
                    cleanAir_FirstTubeOut.Play();
                    _mySmoke.Play();
                    dustAir_FirstTubeIn.Play();
                }

                gates[0].SetActive(true);
                gates[3].SetActive(true);

                Invoke("StartColumnProcess0", 5f);

                fillingCount += 5 * Time.deltaTime;
                if (fillingCount >= 250)
                {
                    filling = false;
                    unfilling = true;
                    count++;
                    fillingCount = 0;
                }

                //_firstColumnLight.material = _greenLight;
                //_secondColumnLight.material = _redLight;


            }
        }
        else if (_startingProcess && count == 1)
        {
            if (filling && !unfilling)
            {
                if (fillingCount < 1)
                {
                    co2_FirstTubeOut.Stop();
                    co2_SecondTubeOut.Play();

                    dustAir_SecondTubeIn.Stop();
                    _mySmoke2.Stop();
                    cleanAir_SecondTubeOut.Stop();

                    cleanAir_FirstTubeOut.Play();
                    _mySmoke.Play();
                    dustAir_FirstTubeIn.Play();
                }

                Invoke("StartColumnProcess", 5f);

                gates[0].SetActive(false);
                gates[1].SetActive(true);
                gates[2].SetActive(true);
                gates[3].SetActive(false);

                fillingCount += 5 * Time.deltaTime;
                if (fillingCount >= 250)
                {
                    filling = false;
                    unfilling = true;
                    fillingCount = 0;
                }


                //_firstColumnLight.material = _greenLight;
                //_secondColumnLight.material = _redLight;
                //_firstColumnLight_1.material = _redLight;
                //_secondColumnLight_2.material = _greenLight;

            }
            else if (unfilling && !filling)
            {
                if (fillingCount < 1)
                {
                    co2_FirstTubeOut.Play();
                    co2_SecondTubeOut.Stop();

                    dustAir_SecondTubeIn.Play();
                    _mySmoke2.Play();
                    cleanAir_SecondTubeOut.Play();

                    cleanAir_FirstTubeOut.Stop();
                    _mySmoke.Stop();
                    dustAir_FirstTubeIn.Stop();
                }

                gates[0].SetActive(true);
                gates[1].SetActive(false);
                gates[2].SetActive(false);
                gates[3].SetActive(true);

                Invoke("StartColumnProcess2", 5f);

                fillingCount += 5 * Time.deltaTime;
                if (fillingCount >= 250)
                {
                    filling = true;
                    unfilling = false;
                    fillingCount = 0;
                }


                //_firstColumnLight.material = _redLight;
                //_secondColumnLight.material = _greenLight;
                //_firstColumnLight_1.material = _greenLight;
                //_secondColumnLight_2.material = _redLight;

            }
        }
        else if (!_startingProcess)
        {

        }
    }

    public void StartColumnProcess0()
    {
        absent[0].color = Color.Lerp(absent[0].color, targetColor, .05f * Time.fixedDeltaTime);
    }

    public void StartColumnProcess()
    {
        absent[1].color = Color.Lerp(absent[1].color, Color.white, .05f * Time.fixedDeltaTime);
        absent[0].color = Color.Lerp(absent[0].color, targetColor, .05f * Time.fixedDeltaTime);
    }

    public void StartColumnProcess2()
    {
        absent[1].color = Color.Lerp(absent[1].color, targetColor, .05f * Time.fixedDeltaTime);
        absent[0].color = Color.Lerp(absent[0].color, Color.white, .05f * Time.fixedDeltaTime);
    }

    public void StartColumnProcess3()
    {
        _startingDelay = true;
        delay = 60;
    }

    public void StopColumnProcess()
    {
        _startingDelay = false;
        _startingProcess = false;
        //_firstColumnLight.material = _redLight;
        //_firstColumnLight_1.material = _redLight;
        //_secondColumnLight.material = _redLight;
        //_secondColumnLight_2.material = _redLight;
    }
}
