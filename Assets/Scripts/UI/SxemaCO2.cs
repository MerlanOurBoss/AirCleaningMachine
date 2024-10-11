using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SxemaCO2 : MonoBehaviour
{
    [SerializeField] private Animator sxemSborCO2Anim;
    [SerializeField] private Animator cubesAnim;
    [SerializeField] private GameObject sxema;
    [SerializeField] private GameObject sxemaOriginal;

    [SerializeField] private TextMeshProUGUI[] gazValue;
    [SerializeField] private TextMeshProUGUI[] manValue;
    [SerializeField] private TextMeshProUGUI pauseText;
    private float gaz1 = 0;
    private float gaz2 = 0;
    private float gaz3 = 0;

    private float man1 = 0;
    private float man2 = 0;
    private float man3 = 0;

    private bool ifFirstGazeOpen = false;
    private bool ifSecondGazeOpen = false;
    private bool ifThirdGazeOpen = false;
    private bool ifSecondGazeOpenCO2 = false;
    private bool ifThirdGazeOpenCO2 = false;

    private bool ifFirstManOpen = false;
    private bool ifSecondManOpen = false;
    private bool ifThirdManOpen = false;

    private bool isOff = false;

    private bool isActivated = false;

    private bool isPaused = false;

    public void Start()
    {
        pauseText.text = "Pause";
        gazValue[0].text = "0.0";
        gazValue[1].text = "0.0";
        gazValue[2].text = "0.0";

        manValue[0].text = "0.0";
        manValue[1].text = "0.0";
        manValue[2].text = "0.0";
    }
    public void OnOffSxema()
    {
        if (!isActivated)
        {
            sxema.SetActive(true);
            sxemaOriginal.SetActive(false);
            sxemSborCO2Anim.Play("LinesStopSbor");
            isActivated = true;
        }
        else
        {
            isOff = true;
            sxema.SetActive(false);
            sxemaOriginal.SetActive(true);
            sxemSborCO2Anim.Play("LinesStopSbor");
            isActivated = false;
        }
    }

    private void Update()
    {
        Debug.Log("ifFirstGazeOpen = " + ifFirstGazeOpen);
        gazValue[0].text = gaz1.ToString("0.0");
        gazValue[1].text = gaz2.ToString("0.0");
        gazValue[2].text = gaz3.ToString("0.0");

        manValue[0].text = man1.ToString("0.0");
        manValue[1].text = man2.ToString("0.0");
        manValue[2].text = man3.ToString("0.0");

        if (isOff)
        {
            sxemSborCO2Anim.Play("LinesStopSbor");
            isOff = false;
        }

        if (ifFirstManOpen)
        {
            man1 = Mathf.Lerp(man1, 1800f, 3 * Time.deltaTime);
            if (man1 >= 1799.99)
            {
                ifFirstManOpen = false;
            }
        }

        if (ifSecondManOpen)
        {
            man2 = Mathf.Lerp(man2, 1800f, 3 * Time.deltaTime);
            if (man2 >= 1799.99)
            {
                ifSecondManOpen = false;
            }
        }

        if (ifThirdManOpen)
        {
            man3 = Mathf.Lerp(man3, 1800f, 3 * Time.deltaTime);
            if (man3 >= 1799.99)
            {
                ifThirdManOpen = false;
            }
        }

        if (ifFirstGazeOpen)
        {
            gaz1 = Mathf.Lerp(gaz1, 1000f, 3 * Time.deltaTime);
            if (gaz1 >= 999.99)
            {
                ifFirstGazeOpen = false;
            }
        }

        if (ifSecondGazeOpen)
        {
            gaz2 = Mathf.Lerp(gaz2, 100f, 2 * Time.deltaTime);
            if (gaz2 >= 99.99)
            {
                ifSecondGazeOpen = false;
            }
        }

        if (ifThirdGazeOpen)
        {
            gaz3 = Mathf.Lerp(gaz3, 100f, 2 * Time.deltaTime);
            if (gaz3 >= 99.99)
            {
                ifThirdGazeOpen = false;
            }
        }

        if (ifSecondGazeOpenCO2)
        {
            gaz2 = Mathf.Lerp(gaz2, 1000f, 3 * Time.deltaTime);
            if (gaz2 >= 999.99)
            {
                ifSecondGazeOpenCO2 = false;
            }
        }

        if (ifThirdGazeOpenCO2)
        {
            gaz3 = Mathf.Lerp(gaz3, 1000f, 3 * Time.deltaTime);
            if (gaz3 >= 999.99)
            {
                ifThirdGazeOpenCO2 = false;
            }
        }
    }
    public void SxemaStart()
    {
        sxemSborCO2Anim.Play("LinesZeroAction");
    }

    public void StartFirstGaz()
    {
        ifFirstGazeOpen = true;
    }

    public void StartSecondGaz()
    {
        ifSecondGazeOpen = true;
    }    
    
    public void StartThirdGaz()
    {
        ifThirdGazeOpen = true;
    }

    public void StartSecondGazCO2()
    {
        ifSecondGazeOpenCO2 = true;
    }

    public void StartThirdGazCO2()
    {
        ifThirdGazeOpenCO2 = true;
    }

    public void StartFisrtMan()
    {
        ifFirstManOpen = true;
    }

    public void StartSecondMan()
    {
        ifSecondManOpen = true;
    }

    public void StartThirdMan()
    {
        ifThirdManOpen = true;
    }

    public void ResetGaz()
    {
        gaz1 = 0;
        gaz2 = 0;
        gaz3 = 0;
        man1 = 0;
        man2 = 0;
        man3 = 0;
    }

    public void PauseSxema()
    {
        if (!isPaused)
        {
            pauseText.text = "Resume";
            sxemSborCO2Anim.speed = 0;
            isPaused = true;
        }
        else
        {
            pauseText.text = "Pause";
            sxemSborCO2Anim.speed = 1;
            isPaused = false;
        }
    }
    public void SxemaStart2()
    {
        sxemSborCO2Anim.Play("LinesMainAction");
    }
    public void SxemasStop()
    {
        sxemSborCO2Anim.Play("LinesStopSbor");
        gaz1 = 0;
        gaz2 = 0;
        gaz3 = 0;
        man1 = 0;
        man2 = 0;
        man3 = 0;

        ifFirstGazeOpen = false;
        ifSecondGazeOpen = false;
        ifThirdGazeOpen = false;
        ifSecondGazeOpenCO2 = false;
        ifThirdGazeOpenCO2 = false;

        ifFirstManOpen = false;
        ifSecondManOpen = false;
        ifThirdManOpen = false;
}
}
