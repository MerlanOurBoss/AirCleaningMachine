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

    public Material absent;

    public TextMeshProUGUI display;
    public Color targetColor;

    private float first_display = 0;


    private bool hasTriggered = false;

    public bool a = false;

    void Start()
    {
        absent.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        display.text = first_display.ToString("0.");

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

            if (ColorsApproximatelyEqual(absent.color, targetColor) && !a)
            {
                first_display = Mathf.Lerp(first_display, 100f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(first_display, 100f))
                {
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

            if (ColorsApproximatelyEqual(absent.color, Color.white) && a)
            {
                first_display = Mathf.Lerp(first_display, 0f, 2 * Time.deltaTime);
                if (ApproximatelyEqual(first_display, 0f))
                {
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