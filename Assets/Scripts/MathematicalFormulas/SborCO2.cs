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

    [Header("Sbor Script")]
    public NewSborScript newSbor;

    [Header("Smoke")]
    public ParticleSystem[] smokes;

    private void Start()
    {
        sorbentSbor.text = "�������";
        volumeSbor.text = "100 �3/�";
        tempAdsorbSbor.text = "40 �C";
        tempDisorbSbor.text = "200 �C";
        diametrSbor.text = "0.1 �";
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
    }
}
