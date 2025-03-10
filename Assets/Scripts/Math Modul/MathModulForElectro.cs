﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathModulForElectro : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private TMP_InputField _Plotnost;
    [SerializeField] private TMP_InputField _speed;
    [SerializeField] private TMP_InputField _radius;
    [SerializeField] private TMP_InputField _charge;

    [SerializeField] private Animator _electroFilter;

    [SerializeField] private TextMeshProUGUI mass;
    [SerializeField] private TextMeshProUGUI potential;
    [SerializeField] private TextMeshProUGUI field;
    [SerializeField] private TextMeshProUGUI acceleration;
    [SerializeField] private Translator translator;

    private float massPotok = 0;
    private float electricPotential  = 0;
    private float electricField = 0;

    private float forceSoprtif = 0;
    private float forceGrafity = 0;
    private float forceElectric = 0;

    private float particleAcceleration = 0;

    private void Start()
    {
        _Plotnost.text = "0,1 мА/см²";
        _speed.text = "2 м/с";
        _radius.text = "0,5 мм ";
        _charge.text = "3 Кл";
    }

    [System.Obsolete]
    private void Update()
    {
        if (translator.currentLanguage == Translator.Language.Russian)
        {
            mass.text = "Массовый поток газа: " + massPotok.ToString("0.000") + " кг/с";
            potential.text = "Потенциал: " + electricPotential.ToString("0.000") + " Дж/Кл";
            field.text = "Электрическое поле: " + electricField.ToString("0.000") + " Н/Кл";
            acceleration.text = "Ускорение частиц: " + particleAcceleration.ToString("0.000") + " м/с²";
        }
        else if (translator.currentLanguage == Translator.Language.Kazakh)
        {
            mass.text = "Газдың массалық ағыны: " + massPotok.ToString("0.000") + " кг/с";
            potential.text = "Потенциал: " + electricPotential.ToString("0.000") + " Дж/Кл";
            field.text = "Электр өрісі: " + electricField.ToString("0.000") + " Н/Кл";
            acceleration.text = "Бөлшектердің үдеуі: " + particleAcceleration.ToString("0.000") + " м/с²";
        }
        else
        {
            mass.text = "Mass flow of gas: " + massPotok.ToString("0.000") + " kg/s";
            potential.text = "Potential: " + electricPotential.ToString("0.000") + " J/Kl";
            field.text = "Electric field: " + electricField.ToString("0.000") + " N/Kl";
            acceleration.text = "Particle acceleration: " + particleAcceleration.ToString("0.000") + " m/s²";
        }



        if (_speed.text == "0,5 м/с")
        {
            _electroFilter.speed = 0.7f;
        }
        else if (_speed.text == "1,5 м/с")
        {
            _electroFilter.speed = 0.8f;
        }
        else
        {
            _electroFilter.speed = 1f;
        }

        if (_Plotnost.text == "0,01 мА/см²")
        {
            ParticleSystem.ColorOverLifetimeModule colorModul = _mySmokes[0].colorOverLifetime;
            ParticleSystem.ColorOverLifetimeModule colorModul2 = _mySmokes[1].colorOverLifetime;

            Gradient currentGradient = colorModul.color.gradient;
            Gradient currentGradient2 = colorModul2.color.gradient;

            Gradient newGradient = new Gradient();
            newGradient.SetKeys(
                currentGradient.colorKeys,
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.4f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(0.4f, 1.0f) // Конечная альфа
                }
            );
            colorModul.color = new ParticleSystem.MinMaxGradient(newGradient);

            Gradient newGradient2 = new Gradient();
            newGradient2.SetKeys(
                currentGradient2.colorKeys,
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.4f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(0.4f, 1.0f) // Конечная альфа
                }
            );

            colorModul2.color = new ParticleSystem.MinMaxGradient(newGradient2);
        }
        else if (_Plotnost.text == "0,05 мА/см²")
        {
            ParticleSystem.ColorOverLifetimeModule colorModul = _mySmokes[0].colorOverLifetime;
            ParticleSystem.ColorOverLifetimeModule colorModul2 = _mySmokes[1].colorOverLifetime;

            Gradient currentGradient = colorModul.color.gradient;
            Gradient currentGradient2 = colorModul2.color.gradient;

            Gradient newGradient = new Gradient();
            newGradient.SetKeys(
                currentGradient.colorKeys,
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.7f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(0.7f, 1.0f) // Конечная альфа
                }
            );
            colorModul.color = new ParticleSystem.MinMaxGradient(newGradient);

            Gradient newGradient2 = new Gradient();
            newGradient2.SetKeys(
                currentGradient2.colorKeys,
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.7f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(0.7f, 1.0f) // Конечная альфа
                }
            );

            colorModul2.color = new ParticleSystem.MinMaxGradient(newGradient2);
        }
        else
        {
            ParticleSystem.ColorOverLifetimeModule colorModul = _mySmokes[0].colorOverLifetime;
            ParticleSystem.ColorOverLifetimeModule colorModul2 = _mySmokes[1].colorOverLifetime;

            Gradient currentGradient = colorModul.color.gradient;
            Gradient currentGradient2 = colorModul2.color.gradient;

            Gradient newGradient = new Gradient();
            newGradient.SetKeys(
                currentGradient.colorKeys,
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(1.0f, 1.0f) // Конечная альфа
                }
            );
            colorModul.color = new ParticleSystem.MinMaxGradient(newGradient);

            Gradient newGradient2 = new Gradient();
            newGradient2.SetKeys(
                currentGradient2.colorKeys,
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f), // Начальная альфа
                    new GradientAlphaKey(1.0f, 1.0f) // Конечная альфа
                }
            );

            colorModul2.color = new ParticleSystem.MinMaxGradient(newGradient2);
        }

        massPotok = float.Parse(_Plotnost.text[.._Plotnost.text.IndexOf(" ")].ToString())
            * float.Parse(_speed.text[.._speed.text.IndexOf(" ")].ToString()) * 0.05f;

        electricPotential = -float.Parse(_Plotnost.text[.._Plotnost.text.IndexOf(" ")].ToString()) / -8.85f;

        electricField = -1 * electricPotential;

        forceSoprtif = -6 * 3.14f * 1.8f
            * float.Parse(_radius.text[.._radius.text.IndexOf(" ")].ToString())
            * float.Parse(_speed.text[.._speed.text.IndexOf(" ")].ToString());

        forceGrafity = massPotok * 9.8f;

        forceElectric = float.Parse(_charge.text[.._charge.text.IndexOf(" ")].ToString()) * electricField;

        particleAcceleration = (1 / massPotok) * (forceSoprtif * forceGrafity * forceElectric);
    }
}
