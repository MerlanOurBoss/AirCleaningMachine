using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ComponentDecr : MonoBehaviour
{
    public TextMeshProUGUI[] Temperature;
    public TextMeshProUGUI[] Dust;
    public TextMeshProUGUI[] SolidParticles;
    public TextMeshProUGUI[] Zola;
    public TextMeshProUGUI[] CO;
    public TextMeshProUGUI[] CO2;
    public TextMeshProUGUI[] NO;
    public TextMeshProUGUI[] NO2;
    public TextMeshProUGUI[] SO2;
    public TextMeshProUGUI[] CH4;
    public TextMeshProUGUI[] H2S;
    public TextMeshProUGUI[] O2;
    public TextMeshProUGUI[] N2;

    public TextMeshProUGUI Temperature_orig;
    public TextMeshProUGUI Dust_orig;
    public TextMeshProUGUI SolidParticles_orig;
    public TextMeshProUGUI Zola_orig;
    public TextMeshProUGUI CO_orig;
    public TextMeshProUGUI CO2_orig;
    public TextMeshProUGUI NO_orig;
    public TextMeshProUGUI NO2_orig;
    public TextMeshProUGUI SO2_orig;
    public TextMeshProUGUI CH4_orig;
    public TextMeshProUGUI H2S_orig;
    public TextMeshProUGUI O2_orig;
    public TextMeshProUGUI N2_orig;


    private void Start()
    {
        foreach (TextMeshProUGUI m in Temperature)
        {
            m.text = Temperature_orig.text;
        }
        foreach (TextMeshProUGUI m in Dust)
        {
            m.text = Dust_orig.text;
        }
        foreach (TextMeshProUGUI m in SolidParticles)
        {
            m.text = SolidParticles_orig.text;
        }
        foreach (TextMeshProUGUI m in Zola)
        {
            m.text = Zola_orig.text;
        }
        foreach (TextMeshProUGUI m in CO)
        {
            m.text = CO_orig.text;
        }
        foreach (TextMeshProUGUI m in CO2)
        {
            m.text = CO2_orig.text;
        }
        foreach (TextMeshProUGUI m in NO)
        {
            m.text = NO_orig.text;
        }
        foreach (TextMeshProUGUI m in NO2)
        {
            m.text = NO2_orig.text;
        }
        foreach (TextMeshProUGUI m in SO2)
        {
            m.text = SO2_orig.text;
        }
        foreach (TextMeshProUGUI m in CH4)
        {
            m.text = CH4_orig.text;
        }
        foreach (TextMeshProUGUI m in H2S)
        {
            m.text = H2S_orig.text;
        }
        foreach (TextMeshProUGUI m in O2)
        {
            m.text = O2_orig.text;
        }
        foreach (TextMeshProUGUI m in N2)
        {
            m.text = N2_orig.text;
        }
    }

}
