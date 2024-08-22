using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SborCO2 : MonoBehaviour
{
    public TMP_InputField sorbentSbor;
    public TMP_InputField volumeSbor;
    public TMP_InputField tempAdsorbSbor;
    public TMP_InputField tempDisorbSbor;
    public TMP_InputField diametrSbor;


    private void Start()
    {
        sorbentSbor.text = "÷еолиты";
        volumeSbor.text = "100 м3/ч";
        tempAdsorbSbor.text = "40 ∞C";
        tempDisorbSbor.text = "200 ∞C";
        diametrSbor.text = "0.1 м";
    }
}
