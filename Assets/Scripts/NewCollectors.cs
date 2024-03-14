using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCollectors : MonoBehaviour
{
    public ParticleSystem zeroSmoke;
    public ParticleSystem firstSmoke;
    public ParticleSystem secondSmoke;
    public ParticleSystem thirdSmoke;

    public Material[] absent;
    public Transform[] panels;

    public Color targetColor;
    public Material _greenLight;
    public Material _redLight;

    public MeshRenderer _firstColumnLight;
    public MeshRenderer _firstColumnLight_1;
    public MeshRenderer _secondColumnLight;
    public MeshRenderer _secondColumnLight_2;


    [SerializeField] private ParticleSystem _mySmoke;
    [SerializeField] private ParticleSystem _mySmoke1;
    [SerializeField] private ParticleSystem _mySmoke2;

    private bool filling = true;
    private bool unfilling = false;

    private bool _startingProcess = false; //false
    private bool _startingDelay = true;

    private float delay = 60f; //update
    private float fillingCount = 0;
    private int count = 0;

    void Start()
    {
        absent[0].color = Color.white;
        absent[1].color = Color.white;

        panels[0].rotation = Quaternion.Euler(0f, 22f, 0f);
        panels[1].rotation = Quaternion.Euler(0f, -198f, 0f);
    }

    void FixedUpdate()
    { 
        if (_startingDelay)
        {
            if (delay <= 0)
            {
                _startingProcess = true;
                _startingDelay = false;
            }
            delay -= 1 * Time.deltaTime;
            //Debug.Log(delay.ToString("0"));

            
        }

        if (_startingProcess && count == 0)
        {
            if (filling && !unfilling)
            {
                zeroSmoke.Play();
                panels[0].rotation = Quaternion.Euler(0f, 22f, 0f);

                fillingCount += 5 * Time.fixedDeltaTime;
                _firstColumnLight.material = _greenLight;
                _secondColumnLight.material = _redLight;
                Invoke("StartColumnProcess0", 5f);
                if (fillingCount >= 50)
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
                zeroSmoke.Play();
                firstSmoke.Stop();
                secondSmoke.Stop();
                thirdSmoke.Play();

                _mySmoke.Stop();
                _mySmoke2.Stop();
                _mySmoke1.Play();

                panels[1].rotation = Quaternion.Euler(0f, -157f, 0f);
                panels[0].rotation = Quaternion.Euler(0f, 22f, 0f);
                fillingCount += 5 * Time.fixedDeltaTime;

                Invoke("StartColumnProcess", 5f);

                _firstColumnLight.material = _greenLight;
                _secondColumnLight.material = _redLight;
                _firstColumnLight_1.material = _redLight;
                _secondColumnLight_2.material = _greenLight;
                if (fillingCount >= 50)
                {
                    filling = false;
                    unfilling = true;
                    fillingCount = 0;
                }
            }
            else if (unfilling && !filling)
            {
                zeroSmoke.Stop();
                firstSmoke.Play();
                secondSmoke.Play();
                thirdSmoke.Stop();

                _mySmoke.Play();
                _mySmoke2.Play();
                _mySmoke1.Stop();
                
                panels[0].rotation = Quaternion.Euler(0f, -22f, 0f);
                panels[1].rotation = Quaternion.Euler(0f, -198f, 0f);
                fillingCount += 5 * Time.fixedDeltaTime;

                Invoke("StartColumnProcess2", 5f);

                _firstColumnLight.material = _redLight;
                _secondColumnLight.material = _greenLight;
                _firstColumnLight_1.material = _greenLight;
                _secondColumnLight_2.material = _redLight;
                if (fillingCount >= 50)
                {
                    filling = true;
                    unfilling = false;
                    fillingCount = 0;
                }
            }
        }
        else if (!_startingProcess)
        {

        }
    }

    public void StartColumnProcess0()
    {
        absent[0].color = Color.Lerp(absent[0].color, targetColor, .1f * Time.fixedDeltaTime);
    }

    public void StartColumnProcess()
    {
        absent[1].color = Color.Lerp(absent[1].color, Color.white, .1f * Time.fixedDeltaTime);
        absent[0].color = Color.Lerp(absent[0].color, targetColor, .1f * Time.fixedDeltaTime);
    }

    public void StartColumnProcess2()
    {
        absent[1].color = Color.Lerp(absent[1].color, targetColor, .1f * Time.fixedDeltaTime);
        absent[0].color = Color.Lerp(absent[0].color, Color.white, .1f * Time.fixedDeltaTime);
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
        _firstColumnLight.material = _redLight;
        _firstColumnLight_1.material = _redLight;
        _secondColumnLight.material = _redLight;
        _secondColumnLight_2.material = _redLight;
    }
}
