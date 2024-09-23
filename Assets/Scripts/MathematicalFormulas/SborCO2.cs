using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SborCO2 : MonoBehaviour
{
    [Header("Texts")]
    public TMP_InputField sorbentSbor;
    public TMP_InputField volumeSbor;
    public TMP_InputField tempAdsorbSbor;
    public TMP_InputField tempDisorbSbor;
    public TMP_InputField diametrSbor;
    public TMP_InputField fanSpeedSbor;

    [Header("Sbor Script")]
    public NewSborScript newSbor;

    [Header("Smoke")]
    public ParticleSystem[] smokes;

    [Header("Fan Animation")]
    public Animator[] fansAnim;

    private void Start()
    {
        sorbentSbor.text = "�������";
        volumeSbor.text = "100 �3/�";
        tempAdsorbSbor.text = "40 �C";
        tempDisorbSbor.text = "200 �C";
        diametrSbor.text = "0,1 �";
        fanSpeedSbor.text = "500 ��/���";
    }

    public void ChangeSmoke()
    {
        if (volumeSbor.text == "100 �3/�")
        {
            newSbor.timingDelay = 150f;
        }
        else if (volumeSbor.text == "150 �3/�")
        {
            newSbor.timingDelay = 170f;
        }
        else if (volumeSbor.text == "200 �3/�")
        {
            newSbor.timingDelay = 200f;
        }


        if (fanSpeedSbor.text == "1000 ��/���")
        {
            fansAnim[0].speed = 3;
            fansAnim[1].speed = 3;
        }
        else if (fanSpeedSbor.text == "1500 ��/���")
        {
            fansAnim[0].speed = 5;
            fansAnim[1].speed = 5;
        }
        else if (fanSpeedSbor.text == "500 ��/���")
        {
            fansAnim[0].speed = 1;
            fansAnim[1].speed = 1;
        }
    }
}
