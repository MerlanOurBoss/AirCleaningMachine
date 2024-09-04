using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewCapsulScript : MonoBehaviour
{
    public bool isTriggerFromOven = false;

    public ParticleSystem smokeInCapsul;
    public ParticleSystem smokeOutCapsul;
    public ParticleSystem smokeOutCapsulSecond;
    public ParticleSystem smokeParInCapsul;
    public Color targetColor;

    public Material absent;
    public TextMeshProUGUI display;
    private float first_display = 0;

    public Material absent2;
    public TextMeshProUGUI display2;
    private float second_display = 0;

    private bool hasTriggered = false;

    public bool a = false;

    public GameObject[] gates;
    void Start()
    {
        gates[1].SetActive(true);
        gates[3].SetActive(true);
        gates[5].SetActive(true);
        gates[7].SetActive(true); //close

        gates[0].SetActive(false);
        gates[2].SetActive(false);
        gates[4].SetActive(false);
        gates[6].SetActive(false); //open
        absent.color = Color.white;
        absent2.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        display.text = first_display.ToString("0.");
        display2.text = second_display.ToString("0.");

        if (isTriggerFromOven)
        {
            if (!hasTriggered)
            {
                smokeInCapsul.Play();
                smokeOutCapsul.Play();
                smokeParInCapsul.Stop();
                smokeOutCapsulSecond.Stop();
                hasTriggered = true;
            }

            Invoke("AbsentOn", 1f);
            Invoke("AbsentOff2", 1f);

            gates[0].SetActive(true);
            gates[1].SetActive(false);

            gates[4].SetActive(false);
            gates[5].SetActive(true);

            gates[6].SetActive(false);
            gates[7].SetActive(true);

            if (ColorsApproximatelyEqual(absent.color, targetColor) && !a && ColorsApproximatelyEqual(absent2.color, Color.white))
            {
                first_display = Mathf.Lerp(first_display, 100f, 2 * Time.deltaTime);
                second_display = Mathf.Lerp(second_display, 0f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(first_display, 100f))
                {
                    gates[2].SetActive(false);
                    gates[3].SetActive(true);
                    a = true;
                }
            }

        }
        if (a)
        {
            isTriggerFromOven = false;
            smokeParInCapsul.Play();
            smokeOutCapsulSecond.Play();
            smokeInCapsul.Stop();
            smokeOutCapsul.Stop();

            Invoke("AbsentOff", 3f);
            Invoke("AbsentOn2", 3f);

            gates[4].SetActive(true);
            gates[5].SetActive(false);

            gates[0].SetActive(false);
            gates[1].SetActive(true);

            if (ColorsApproximatelyEqual(absent.color, Color.white) && a && ColorsApproximatelyEqual(absent2.color, targetColor))
            {
                first_display = Mathf.Lerp(first_display, 0f, 2 * Time.deltaTime);
                second_display = Mathf.Lerp(second_display, 100f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(first_display, 0f))
                {
                    gates[2].SetActive(true);
                    gates[3].SetActive(false);

                    gates[6].SetActive(false);
                    gates[7].SetActive(true);
                    a = false;
                    isTriggerFromOven = true;
                    hasTriggered = false;
                    
                }
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

    public void AbsentOn2()
    {
        absent2.color = Color.Lerp(absent2.color, targetColor, .1f * Time.fixedDeltaTime);
    }

    public void AbsentOff2()
    {
        absent2.color = Color.Lerp(absent2.color, Color.white, .15f * Time.fixedDeltaTime);
    }

    bool ApproximatelyEqual(float a, float b, float tolerance = 0.1f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }
    bool ColorsApproximatelyEqual(Color c1, Color c2, float tolerance = 0.1f)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance &&
               Mathf.Abs(c1.a - c2.a) < tolerance;
    }
}
