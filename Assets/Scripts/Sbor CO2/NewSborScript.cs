using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NewSborScript : MonoBehaviour
{
    public ParticleSystem smokeToExit;
    public ParticleSystem smokeInCapsul;

    public TextMeshProUGUI display;

    public Material absent;
    public Color targetColor;

    private float first_display = 0;
    public GameObject gate;

    private bool filling = true;
    private bool unfilling = false;

    private bool _startingProcess = false; //false
    public bool _startingDelay = false;

    public float delay; //update
    public float delayUpdate; //update

    private float fillingCount = 0;
    private int count = 0;

    void Start()
    {
        absent.color = Color.white;
    }

    void Update()
    {
        display.text = first_display.ToString("0.");

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
                    smokeInCapsul.Play();
                }
                if (fillingCount > 60 && fillingCount < 80)
                {
                    first_display = Mathf.Lerp(first_display, 100f, 2 * Time.deltaTime);
                }

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
            if (unfilling && !filling)
            {
                fillingCount += 5 * Time.deltaTime;

                gate.SetActive(true);
                smokeToExit.Play();
                smokeInCapsul.Stop();

                if (fillingCount > 60 && fillingCount < 80)
                {
                    first_display = Mathf.Lerp(first_display, 0f, 2 * Time.deltaTime);
                }

                Invoke("AbsentOff", 5f);


                if (fillingCount >= 150)
                {
                    filling = true;
                    unfilling = false;
                    count= 0;
                    fillingCount = 0;
                }
            }
        }
    }
    public void AbsentOn()
    {
        absent.color = Color.Lerp(absent.color, targetColor, 1f * Time.fixedDeltaTime);
    }

    public void AbsentOff()
    {
        absent.color = Color.Lerp(absent.color, Color.white, .15f * Time.fixedDeltaTime);
    }

    public void StartColumnProcess3()
    {
        _startingDelay = true;
        delayUpdate = delay;
    }

    public void StopColumnProcess()
    {
        _startingProcess = false;
        _startingProcess = false;
        _startingDelay = false;
        fillingCount = 0;
        delay = delayUpdate;
        gate.SetActive(true);

        smokeToExit.Stop();
        smokeInCapsul.Stop();

        absent.color = Color.Lerp(absent.color, Color.white, .15f * Time.fixedDeltaTime);
        first_display = Mathf.Lerp(first_display, 0, 2 * Time.deltaTime);

    }
}
