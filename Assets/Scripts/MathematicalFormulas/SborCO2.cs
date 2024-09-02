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
        sorbentSbor.text = "÷еолиты";
        volumeSbor.text = "100 м3/ч";
        tempAdsorbSbor.text = "40 ∞C";
        tempDisorbSbor.text = "200 ∞C";
        diametrSbor.text = "0.1 м";
    }

    public void ChangeSmoke()
    {
        if (volumeSbor.text == "100 м3/ч")
        {
            newSbor.timingDelay = 150f;
        }
        else if (volumeSbor.text == "150 м3/ч")
        {
            newSbor.timingDelay = 170f;
        }
        else if (volumeSbor.text == "200 м3/ч")
        {
            newSbor.timingDelay = 200f;
        }
    }
}
