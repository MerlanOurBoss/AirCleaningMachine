using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ComponentDecr : MonoBehaviour
{
    public TextMeshProUGUI[] ElectroFilter1;
    public TextMeshProUGUI[] ElectroFilter2;
    public TextMeshProUGUI[] ElectroFilter3;
    public TextMeshProUGUI[] ElectroFilter4;

    private float electro1;
    private float electro2;
    private float electro3;
    private float electro4;

    private void Start()
    {
        electro1 = 560;
        electro2 = 100;
        electro3 = 215;
        electro4 = 42;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var e in ElectroFilter1)
        {
            e.text = electro1.ToString("0.");
        }
        if (electro1 > 120)
        {
            float n = electro1 / 2;
            electro1 -= n * Time.deltaTime;
        }

        foreach (var e in ElectroFilter2)
        {
            e.text = electro2.ToString("0.");
        }
        if (electro2 > 10)
        {
            float n = electro2 / 2;
            electro2 -= n * Time.deltaTime;
        }

        foreach (var e in ElectroFilter3)
        {
            e.text = electro3.ToString("0.");
        }
        if (electro3 > 20)
        {
            float n = electro3 / 2;
            electro3 -= n * Time.deltaTime;
        }

        foreach (var e in ElectroFilter4)
        {
            e.text = electro4.ToString("0.");
        }
        if(electro4 > 4)
        {
            float n = electro4 / 2;
            electro4 -=  n * Time.deltaTime;
        }
    }
}
