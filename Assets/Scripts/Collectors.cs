using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectors : MonoBehaviour
{
    [SerializeField] private float _fillingColums = -50f;

    [SerializeField] private ParticleSystem _mySmokes;
    private Material fillingMaterial;

    public Material _greenLight;
    public Material _redLight;

    public MeshRenderer _firstColumnLight;
    public MeshRenderer _secondColumnLight;

    private bool filling = true;
    private bool unfilling = false;

    private bool _startingProcess = false;
    private bool _startingDelay = false;
    private float delay = 60f; //update

    private void Start()
    {
        fillingMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }
    private void Update()
    {
        fillingMaterial.SetFloat("_Filling", _fillingColums);
        if (_startingDelay )
        {
            delay -= 1 * Time.deltaTime;
            Debug.Log(delay.ToString("0"));

            if (delay <= 0)
            {
                _startingProcess = true;
            }
        }        

        if (_startingProcess)
        {
            if (filling && !unfilling)
            {
                _fillingColums += 5 * Time.deltaTime;
                _mySmokes.Play();
                _firstColumnLight.material = _greenLight;
                _secondColumnLight.material = _redLight;
                if (_fillingColums >= 50)
                {
                    filling = false;
                    unfilling = true;
                }
            }
            else if (unfilling && !filling)
            {
                _fillingColums -= 5 * Time.deltaTime;
                _mySmokes.Stop();
                _firstColumnLight.material = _redLight;
                _secondColumnLight.material = _greenLight;
                if (_fillingColums <= -50)
                {
                    filling = true;
                    unfilling = false;
                }
            }
        }
        else if (!_startingProcess)
        {
            _mySmokes.Stop();
        }
    }

    public void StartColumnProcess()
    {
        _startingDelay = true;
        delay = 60;
    }

    public void StopColumnProcess()
    {
        _startingDelay = false;
        _startingProcess = false;
        _firstColumnLight.material = _redLight;
        _secondColumnLight.material = _redLight;
    }
}
