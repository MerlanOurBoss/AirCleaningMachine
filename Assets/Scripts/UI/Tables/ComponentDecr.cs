using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComponentDecr : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] Temperature;
    [SerializeField] private TextMeshProUGUI[] Dust;
    [SerializeField] private TextMeshProUGUI[] SolidParticles;
    [SerializeField] private TextMeshProUGUI[] Zola;
    public TextMeshProUGUI[] CO;
    [SerializeField] private TextMeshProUGUI[] CO2;
    [SerializeField] private TextMeshProUGUI[] NO;
    [SerializeField] private TextMeshProUGUI[] NO2;
    [SerializeField] private TextMeshProUGUI[] SO2;
    [SerializeField] private TextMeshProUGUI[] CH4;
    [SerializeField] private TextMeshProUGUI[] H2S;
    [SerializeField] private TextMeshProUGUI[] O2;
    public TextMeshProUGUI[] N2;

    [SerializeField] private TextMeshProUGUI Temperature_orig;
    [SerializeField] private TextMeshProUGUI Dust_orig;
    [SerializeField] private TextMeshProUGUI SolidParticles_orig;
    [SerializeField] private TextMeshProUGUI Zola_orig;
    [SerializeField] private TextMeshProUGUI CO_orig;
    [SerializeField] private TextMeshProUGUI CO2_orig;
    [SerializeField] private TextMeshProUGUI NO_orig;
    [SerializeField] private TextMeshProUGUI NO2_orig;
    [SerializeField] private TextMeshProUGUI SO2_orig;
    [SerializeField] private TextMeshProUGUI CH4_orig;
    [SerializeField] private TextMeshProUGUI H2S_orig;
    [SerializeField] private TextMeshProUGUI O2_orig;
    [SerializeField] private TextMeshProUGUI N2_orig;


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
