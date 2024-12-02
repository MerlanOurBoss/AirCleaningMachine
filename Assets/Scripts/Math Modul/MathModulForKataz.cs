using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathModulForKataz : MonoBehaviour
{
    [SerializeField] private TMP_InputField _temperatureText;
    [SerializeField] private TMP_InputField _pressureText;
    [SerializeField] private TMP_InputField _flowRateText;

    [SerializeField] private TextMeshProUGUI gasVelocity;
    [SerializeField] private TextMeshProUGUI gasDensitie;
    [SerializeField] private TextMeshProUGUI gasmassFlow;
    [SerializeField] private TextMeshProUGUI coGaz;

    private float velocity;
    private float density;
    private float massFlow;
    private float CO_Gaz_Out;

    private float CO_Gaz_In = 0.1f;
    private float k_CO = 0.02f;
    const float R = 8.314f;
    const double Pi = Math.PI;
    const float molarMass = 0.029f;

    private void Start()
    {
        _temperatureText.text = "25 °C";
        _pressureText.text = "101325 Па";
        _flowRateText.text = "1 м³/с";
    }
    private void Update()
    {
        gasVelocity.text = "Скорость газа: " + velocity.ToString("0.000") + " м/с";
        gasDensitie.text = "Плотность газа: " + density.ToString("0.000") + " кг/м³";
        gasmassFlow.text = "Массовый паток: " + massFlow.ToString("0.000") + " кг/с";
        coGaz.text = "Вых. концентрация CO: " + CO_Gaz_Out.ToString("0.000") + " моль/м³";

        float crossSectionArea = (float)((float)Pi * Math.Pow(0.5 / 2, 2));

        CO_Gaz_Out = (float)(CO_Gaz_In * Math.Pow(Math.E, k_CO * 1 * (1 / velocity)));
        density = (float.Parse(_pressureText.text[.._pressureText.text.IndexOf(" ")].ToString()) * molarMass) / (R * float.Parse(_temperatureText.text[.._temperatureText.text.IndexOf(" ")].ToString()));
        velocity = float.Parse(_flowRateText.text[.._flowRateText.text.IndexOf(" ")].ToString()) / crossSectionArea;
        Debug.Log("crossSectionArea " + crossSectionArea);
        massFlow = density * velocity * crossSectionArea;
    }
}
