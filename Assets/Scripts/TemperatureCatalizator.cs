using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemperatureCatalizator : MonoBehaviour
{
    public Material firstMaterial;
    public Material secondMaterial;
    public Material thirdMaterial;
    public ParticleSystem fire;
    public Animator anim;
    public Animator anim1;
    public TextMeshProUGUI texTemp1;
    public TextMeshProUGUI texTemp2;

    public Color originColor;
    public Color firstColor;
    public Color secondColor;
    public Color thirdColor;

    private float delay = 16f;
    private float firsTemp1 = 26;
    private float firsTemp2 = 26;
    private bool canTemp1 = false;
    private bool canTemp2 = false;

    void Start()
    {
        firstMaterial.color = originColor;
        secondMaterial.color = originColor;
        thirdMaterial.color = originColor;
        Invoke("StartSimulation", 15f);
    }

    // Update is called once per frame
    void Update()
    {
        string str = firsTemp1.ToString("0");
        texTemp1.text = str + "�C";

        string str2 = firsTemp2.ToString("0");
        texTemp2.text = str2 + "�C";
        delay -= 1 * Time.deltaTime;
        
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

        if (delay < 10 && !canTemp1)
        {
            firsTemp2 = Mathf.Lerp(firsTemp2, 160, 1.5f * Time.deltaTime);
        }
        if (delay < 8 && !canTemp2)
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
        fire.Play();
        anim.Play("Catalizaotr_Anim");
        anim1.Play("Catalizaotr_Anim");
    }
}