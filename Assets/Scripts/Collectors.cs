using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectors : MonoBehaviour
{
     private float _fillingColums = -50f;
     private float _fillingColums1 = -50f;
     private float _fillingColums2 = -50f;
     private float _fillingColums3 = -50f;
     private float _fillingColums2_1 = 50f;
     private float _fillingColums3_1 = 50f;

    [SerializeField] private ParticleSystem _mySmoke;
    [SerializeField] private ParticleSystem _mySmoke1;

    [SerializeField] private MeshRenderer fillingMaterial;
    [SerializeField] private MeshRenderer fillingMaterial1;
    [SerializeField] private MeshRenderer fillingMaterial2;
    [SerializeField] private MeshRenderer fillingMaterial3;
    [SerializeField] private MeshRenderer fillingMaterial2_1;
    [SerializeField] private MeshRenderer fillingMaterial3_1;

    public Material absent;
    public Material absent1;
    public Material absent2;
    public Material absent3;
    public Material absent2_1;
    public Material absent3_1;

    public Material colorSphere;
    public Material colorSphere1;
    public Color startColor;
    public Color targetColor;

    public Material _greenLight;
    public Material _redLight;

    public MeshRenderer _firstColumnLight;
    public MeshRenderer _firstColumnLight_1;
    public MeshRenderer _secondColumnLight;
    public MeshRenderer _secondColumnLight_2;

    private bool filling = true;
    private bool unfilling = false;

    private bool _startingProcess = false;
    private bool _startingDelay = false;
    private float delay = 60f; //update

    private int count = 0;

    private void Start()
    {
        fillingMaterial.GetComponent<MeshRenderer>().sharedMaterial = absent;
        fillingMaterial1.GetComponent<MeshRenderer>().sharedMaterial = absent1;
        colorSphere.color = startColor;
        colorSphere1.color = startColor;
    }
    private void Update()
    {
        fillingMaterial.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Filling", _fillingColums);
        fillingMaterial1.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Filling", _fillingColums1);
        fillingMaterial2.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Filling", _fillingColums2);
        fillingMaterial3.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Filling", _fillingColums3);
        fillingMaterial2_1.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Filling", _fillingColums2_1);
        fillingMaterial3_1.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Filling", _fillingColums3_1);

        if (_startingDelay)
        {
            delay -= 1 * Time.deltaTime;
            Debug.Log(delay.ToString("0"));

            if (delay <= 0)
            {
                _startingProcess = true;
            }
        }
        
        if (_startingProcess && count == 0)
        {
            if (filling && !unfilling)
            {
                _fillingColums += 5 * Time.deltaTime;
                _firstColumnLight.material = _greenLight;
                _secondColumnLight.material = _redLight;
                colorSphere.color = Color.Lerp(colorSphere.color, targetColor, .2f * Time.deltaTime);
                if (_fillingColums >= 50)
                {
                    filling = false;
                    unfilling = true;
                    count++;
                }
            }
        }
        else if (_startingProcess && count == 1)
        {
            if (filling && !unfilling)
            {
                _fillingColums += 5 * Time.deltaTime;
                _fillingColums1 -= 5 * Time.deltaTime;
                _mySmoke.Stop();
                _mySmoke1.Play();
                colorSphere.color = Color.Lerp(colorSphere.color, targetColor, .2f * Time.deltaTime);
                colorSphere1.color = Color.Lerp(colorSphere1.color, startColor, .2f * Time.deltaTime);
                _fillingColums3 += 5 * Time.deltaTime;
                _fillingColums2_1 += 5 * Time.deltaTime;
                _firstColumnLight.material = _greenLight;
                _secondColumnLight.material = _redLight;
                _firstColumnLight_1.material = _redLight;
                _secondColumnLight_2.material = _greenLight;
                if (_fillingColums >= 50)
                {
                    filling = false;
                    unfilling = true;
                    _fillingColums3 = -50f;
                    _fillingColums3_1 = -50f;
                }
            }
            else if (unfilling && !filling)
            {
                _fillingColums -= 5 * Time.deltaTime;
                _fillingColums1 += 5 * Time.deltaTime;
                _mySmoke.Play();
                _mySmoke1.Stop();
                colorSphere.color = Color.Lerp(colorSphere.color, startColor, .2f * Time.deltaTime);
                colorSphere1.color = Color.Lerp(colorSphere1.color, targetColor, .2f * Time.deltaTime);
                _fillingColums2 += 5 * Time.deltaTime;
                _fillingColums3_1 += 5 * Time.deltaTime;
                _firstColumnLight.material = _redLight;
                _secondColumnLight.material = _greenLight;
                _firstColumnLight_1.material = _greenLight;
                _secondColumnLight_2.material = _redLight;
                if (_fillingColums <= -50)
                {
                    filling = true;
                    unfilling = false;
                    _fillingColums2 = -50f;
                    _fillingColums2_1 = -50f;
                }
            }
        }
        else if (!_startingProcess)
        {
            _mySmoke.Stop();
            _mySmoke1.Stop();
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
        _firstColumnLight_1.material = _redLight;
        _secondColumnLight.material = _redLight;
        _secondColumnLight_2.material = _redLight;
    }
}
