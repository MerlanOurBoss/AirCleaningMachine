using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemperatureCatalizator : MonoBehaviour
{
    public Material firstMaterial;
    public Material secondMaterial;
    public Material thirdMaterial;
    [SerializeField] private ParticleSystem[] fire;
    public Animator anim;
    public TextMeshProUGUI texTemp1;
    public TextMeshProUGUI texTemp2;

    public Color originColor;
    public Color firstColor;
    public Color secondColor;
    public Color thirdColor;
    public KatalizatorScript kataz;

    private float delay = 6f;
    private float firsTemp1 = 26;
    private float firsTemp2 = 26;
    private bool canTemp1 = false;
    private bool canTemp2 = false;

    private bool canStart = false;

    void Start()
    {
        firstMaterial.color = originColor;
        secondMaterial.color = originColor;
        thirdMaterial.color = originColor;
        //Invoke("StartSimulation", 15f);
    }

    // Update is called once per frame
    void Update()
    {
        if (canStart)
        {
            delay -= 1 * Time.deltaTime;
        }
        
        string str = firsTemp1.ToString("0");
        texTemp1.text = str + "°C";

        string str2 = firsTemp2.ToString("0");
        texTemp2.text = str2 + "°C";
        
        if (!kataz.isSecond)
        {
            if (canTemp1)
            {
                thirdMaterial.color = Color.Lerp(thirdMaterial.color, Color.white, Time.deltaTime);
                firsTemp1 = Mathf.Lerp(firsTemp1, 500, 1.5f * Time.deltaTime);
            }

            if (canTemp2)
            {
                firstMaterial.color = Color.Lerp(firstMaterial.color, Color.white, Time.deltaTime);
                secondMaterial.color = Color.Lerp(secondMaterial.color, Color.white, Time.deltaTime);
                firsTemp2 = Mathf.Lerp(firsTemp2, 320, 1.5f * Time.deltaTime);
            }
        }
        if (kataz.isSecond) 
        {
            if (canTemp1)
            {
                thirdMaterial.color = Color.Lerp(thirdMaterial.color, Color.white, Time.deltaTime);
            }

            if (canTemp2)
            {
                firstMaterial.color = Color.Lerp(firstMaterial.color, Color.white, Time.deltaTime);
                secondMaterial.color = Color.Lerp(secondMaterial.color, Color.white, Time.deltaTime);
                firsTemp2 = Mathf.Lerp(firsTemp2, 320, 1.5f * Time.deltaTime);
            }
        }
        

        if (delay < 5 && !canTemp1)
        {
            firsTemp2 = Mathf.Lerp(firsTemp2, 160, 1.5f * Time.deltaTime);
        }
        if (delay < 3 && !canTemp2)
        {
            firsTemp1 = Mathf.Lerp(firsTemp1, 160, 1.5f * Time.deltaTime);
        }
        if (delay < 0) 
        {
            canTemp1 = true;
        }
        if (delay < -2)
        {
            canTemp2 = true;
        }
    }

    public void StartSimulation()
    {
        anim.Play("Catalizaotr_Anim");
        canStart = true;
        foreach (ParticleSystem item in fire)
        {
            item.Play();
        }

    }

    public void StopSimulation()
    {
        foreach (ParticleSystem item in fire)
        {
            item.Stop();
        }
        delay = 6f;
        canStart = false;
        anim.StopPlayback();
        anim.StopPlayback();
        canTemp1 = false;
        canTemp2 = false;

        firstMaterial.color = originColor;
        secondMaterial.color = originColor;
        thirdMaterial.color = originColor;
    }
}
