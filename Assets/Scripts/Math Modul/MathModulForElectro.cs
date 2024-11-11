using System.Collections;
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

    public float massPotok = 0;
    public float electricPotential  = 0;
    public float electricField = 0;

    public float forceSoprtif = 0;
    public float forceGrafity = 0;
    public float forceElectric = 0;

    public float particleAcceleration = 0;

    private void Update()
    {
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

        Debug.Log("Mass Potok = " + massPotok);
        Debug.Log("Electric Potential = " + electricPotential);
        Debug.Log("Electric field = " + electricField);
        Debug.Log("Particle Acceleration = " + particleAcceleration);
    }
}
