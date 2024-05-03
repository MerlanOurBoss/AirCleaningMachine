using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoolingDisplays : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] displays;

    private float firstDisplay = 24f;
    private float secondDisplay = 24f;

    public float delay;
    private float copyDelay;
    public bool isEnbale = false;

    private void Start()
    {
        copyDelay = delay;
        displays[0].text = firstDisplay.ToString("0.");
        displays[1].text = secondDisplay.ToString("0.");
    }

    private void Update()
    {        
        displays[0].text = firstDisplay.ToString("0." + "°Ñ");
        displays[1].text = secondDisplay.ToString("0." + "°Ñ");

        if (isEnbale)
        {
            delay -= Time.deltaTime;
            if (delay < 3)
            {
                firstDisplay = Mathf.Lerp(firstDisplay, 245f, Time.deltaTime);
            }

            if (delay < 0)
            {
                secondDisplay = Mathf.Lerp(secondDisplay, 90f, Time.deltaTime);
            }
            else if (delay < -10)
            {
                isEnbale = false;
            }
        }
    }

    public void StopDelay()
    {
        delay = copyDelay;
        firstDisplay = 24f;
        secondDisplay = 24f;
    }
}
