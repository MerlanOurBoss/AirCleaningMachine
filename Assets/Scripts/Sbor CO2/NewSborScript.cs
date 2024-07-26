using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NewSborScript : MonoBehaviour
{
    public ParticleSystem smokeInCapsul;
    public ParticleSystem smokeOutCapsul;
    public ParticleSystem smokeOutCapsulSecond;
    public ParticleSystem smokeParInCapsul;

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

    public GameObject[] gates;
    void Start()
    {
        gates[1].SetActive(true);
        gates[3].SetActive(true);
        gates[5].SetActive(true);

        gates[0].SetActive(false);
        gates[2].SetActive(false);
        gates[4].SetActive(false);

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
                    smokeOutCapsul.Play();
                    smokeParInCapsul.Stop();
                    smokeOutCapsulSecond.Stop();
                }
                if (fillingCount > 60 && fillingCount < 80)
                {
                    first_display = Mathf.Lerp(first_display, 100f, 2 * Time.deltaTime);
                }

                Invoke("AbsentOn", 5f);

                gates[0].SetActive(true);
                gates[1].SetActive(false);


                gates[4].SetActive(false);
                gates[5].SetActive(true);


                if (fillingCount >= 150)
                {
                    filling = false;
                    unfilling = true;
                    count++;
                    fillingCount = 0;
                    gates[2].SetActive(false);
                    gates[3].SetActive(true);
                }
            }
        }
        else if (_startingProcess && count == 1)
        {
            if (unfilling && !filling)
            {
                fillingCount += 5 * Time.deltaTime;

                gate.SetActive(true);
                smokeParInCapsul.Play();
                smokeOutCapsulSecond.Play();
                smokeInCapsul.Stop();
                smokeOutCapsul.Stop();

                if (fillingCount > 60 && fillingCount < 80)
                {
                    first_display = Mathf.Lerp(first_display, 0f, 2 * Time.deltaTime);
                }

                Invoke("AbsentOff", 5f);

                gates[4].SetActive(true);
                gates[5].SetActive(false);

                gates[0].SetActive(false);
                gates[1].SetActive(true);


                if (fillingCount >= 150)
                {
                    filling = true;
                    unfilling = false;
                    count= 0;
                    fillingCount = 0;
                    gates[2].SetActive(true);
                    gates[3].SetActive(false);
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
        gate.SetActive(true);

        smokeOutCapsul.Stop();
        smokeInCapsul.Stop();

        absent.color = Color.Lerp(absent.color, Color.white, .15f * Time.fixedDeltaTime);
        first_display = Mathf.Lerp(first_display, 0, 2 * Time.deltaTime);

    }
}
