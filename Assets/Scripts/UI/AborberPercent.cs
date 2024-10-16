using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AborberPercent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _percentValue;
    [SerializeField] private GameObject[] _objects;
    public bool startProcess = false;
    public bool backProcess = false;
    public bool stopProcess = false;
    private float percentValue;
    public bool resumeAndPause = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in _objects)
        {
            obj.SetActive(false);
        }    
    }

    // Update is called once per frame
    void Update()
    {
        _percentValue.text = percentValue.ToString("0.") + "%";

        if (stopProcess)
        {
            foreach (GameObject gameObject in _objects)
            {
                gameObject.SetActive(false);
            }
            percentValue = Mathf.Lerp(percentValue, 0f, 2000f * Time.deltaTime);
        }

        if (startProcess)
        {
            percentValue = Mathf.Lerp(percentValue, 100f, 0.23f * Time.deltaTime);
            if (percentValue > 1)
            {
                _objects[0].SetActive(true);
            }
            if (percentValue > 15)
            {
                _objects[1].SetActive(true);
            }
            if (percentValue > 30)
            {
                _objects[2].SetActive(true);
            }
            if (percentValue > 45)
            {
                _objects[3].SetActive(true);
            }
            if (percentValue > 60)
            {
                _objects[4].SetActive(true);
            }
            if (percentValue > 75)
            {
                _objects[5].SetActive(true);
            }
            if (percentValue > 90)
            {
                _objects[6].SetActive(true);
            }
        }
        if (backProcess)
        {
            percentValue = Mathf.Lerp(percentValue, 0f, 0.3f * Time.deltaTime);
            
            if (percentValue < 90)
            {
                _objects[6].SetActive(false);
            }
            if (percentValue < 75)
            {
                _objects[5].SetActive(false);
            }
            if (percentValue < 60)
            {
                _objects[4].SetActive(false);
            }
            if (percentValue < 45)
            {
                _objects[3].SetActive(false);
            }
            if (percentValue < 30)
            {
                _objects[2].SetActive(false);
            }
            if (percentValue < 15)
            {
                _objects[1].SetActive(false);
            }
            if (percentValue < 1)
            {
                _objects[0].SetActive(false);
            }
        }
    }
}
