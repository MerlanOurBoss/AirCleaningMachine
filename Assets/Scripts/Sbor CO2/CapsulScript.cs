using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CapsulScript : MonoBehaviour
{
    public bool isTriggerFromOven = false;
    public bool isTriggerFromDryAir = false;
    public bool isTriggerFromSteam = false;

    public ParticleSystem smokeToCollector;
    public ParticleSystem smokeInCapsul;
    public ParticleSystem smokeOutCapsul;

    public Material absent;

    public TextMeshProUGUI[] displays;
    public Color targetColor;

    private GameObject smokeFromDryAir;
    public GameObject smokeFromStream;


    private float first_display = 0;
    private float second_display = 0;
    private float third_display = 0;

    private bool hasTriggered = false;
    private bool hasTriggered1 = false;
    private bool hasTriggered2 = false;
    public bool a = false;
    private void Start()
    {
        absent.color = Color.white;
    }
    private void Update()
    {
        smokeFromDryAir = GameObject.FindGameObjectWithTag("SmokeDryAir");
        smokeFromStream = GameObject.FindGameObjectWithTag("SmokeStream");
        displays[0].text = first_display.ToString("0.");
        displays[1].text = second_display.ToString("0.");
        displays[2].text = third_display.ToString("0.");
        if (isTriggerFromOven)
        {
            if (!hasTriggered)
            {
                smokeInCapsul.Play();
                smokeOutCapsul.Play();
                hasTriggered  = true;
            }

            Invoke("AbsentOn", 1f);

            if (ColorsApproximatelyEqual(absent.color, targetColor) && !a)
            {
                first_display = Mathf.Lerp(first_display, 100f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(first_display, 100f))
                {
                    smokeFromStream.GetComponent<ParticleSystem>().Play();
                    a = true;
                }
            }
            
        }
        if (isTriggerFromSteam) // пар идет с парогениратора и задевает триггер в Capsul, и isTriggerFromSteam = true
        {
            isTriggerFromOven = false;

            if (!hasTriggered1)
            {
                smokeToCollector.Play();
                smokeInCapsul.Stop();
                smokeOutCapsul.Stop();
                hasTriggered1 = true;
            }
                
            bool a1 = false;
            if (a1 == false)
            {
                first_display = Mathf.Lerp(first_display, 0f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(first_display,0))
                {
                    a1 = true;
                }
            }
            bool b = false;
            if (!b)
            {
                second_display = Mathf.Lerp(second_display, 100f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(second_display, 100f))
                {
                    b = true;
                }
            }
            bool c = false;
            if (!c)
            {
                third_display = Mathf.Lerp(third_display, 1950f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(third_display, 1950f))
                {
                    c = true;
                }
            }
            
        }
        if (isTriggerFromDryAir) // в коллекторе срабатывает триггер и isTriggerFromDryAir = true
        {
            isTriggerFromSteam = false;

            if (!hasTriggered2)
            {
                smokeFromStream.GetComponent<ParticleSystem>().Stop();
                smokeToCollector.Stop();
                hasTriggered2 = true;
            }
                
            bool c1 = false;
            if (!c1)
            {
                third_display = Mathf.Lerp(third_display, 0f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(third_display, 0))
                {
                    c1 = true;
                }
            }

            bool b1 = false;
            if (!b1)
            {
                second_display = Mathf.Lerp(second_display, 0f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(second_display, 0))
                {
                    b1 = true;
                }
            }

            Invoke("AbsentOff", 5f);
            if (ColorsApproximatelyEqual(absent.color, Color.white))
            {
                smokeFromDryAir.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    public void AbsentOn()
    {
        absent.color = Color.Lerp(absent.color, targetColor, .1f * Time.fixedDeltaTime);
    }

    public void AbsentOff()
    {
        absent.color = Color.Lerp(absent.color, Color.white, .15f * Time.fixedDeltaTime);
    }

    bool ApproximatelyEqual(float a, float b, float tolerance = 0.1f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }
    bool ColorsApproximatelyEqual(Color c1, Color c2, float tolerance = 0.01f)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance &&
               Mathf.Abs(c1.a - c2.a) < tolerance;
    }
}
