using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemperatureCatalizator : MonoBehaviour
{
    [SerializeField] private TMP_Text[] displays;
    public float delay = 5f;

    public bool isStarting = false;
    private float displayValue0 = 0;
    private float displayValue1 = 0;
    private float displayValue2 = 0;
    private void Update()
    {
        displays[0].text = displayValue0.ToString("0." + " °C");
        displays[1].text = displayValue1.ToString("0." + " °C");
        displays[2].text = displayValue2.ToString("0." + " °C");
        if (isStarting)
        {
            
            delay -= Time.deltaTime;
            if (delay < 114 - 111 && delay > 100 - 111)
            {
                displayValue0 = Mathf.Lerp(displayValue0, 180f, 2 * Time.deltaTime);
            }

            if (delay < 51 - 111 && delay > 41 - 111)
            {
                displayValue1 = Mathf.Lerp(displayValue1, 500f, 2 * Time.deltaTime);
            }

            if (delay < 45 - 111 && delay > 35 - 111)
            {
                displayValue2 = Mathf.Lerp(displayValue2, 300f, 2 * Time.deltaTime);
            }
        }
    }
    public void StartSimulation()
    {
        isStarting = true;
    }

    public void StopSimulation()
    {
        displayValue0 = 0;
        displayValue1 = 0;
        displayValue2 = 0;
        isStarting = false;
        delay = 116f;
    }
}