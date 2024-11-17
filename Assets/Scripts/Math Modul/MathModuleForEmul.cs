using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathModuleForEmul : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private TMP_InputField _gasFlow;
    [SerializeField] private TMP_InputField _waterFlow;

    private float _gasSpeed = 0;
    private float _waterSpeed = 0;
    private float _gasMassFlow = 0;
    private float _waterMassFlow = 0;
    private float _reynoldsNumber = 0;
    private float _massTransfer = 0;

    private readonly float deametr = 2f;

    private readonly float empiricalConstantsA = 0.5f;
    private readonly float empiricalConstantsB = 0.8f;

    private readonly float deametrDroplet = 1;
    private readonly float gasDensity = 1.98f;
    private readonly float gasDynamicViscosity = 1.5f;
    void Start()
    {
        _gasFlow.text = "";
        _waterFlow.text = "";
    }

    void Update()
    {
        _gasSpeed = (4 * float.Parse(_gasFlow.text[.._gasFlow.text.IndexOf(" ")].ToString())) / (3.14159f * Mathf.Pow(deametr, 2));
            
        _waterSpeed = (4 * float.Parse(_waterFlow.text[.._waterFlow.text.IndexOf(" ")].ToString())) / (3.14159f * Mathf.Pow(deametr, 2));

        _gasMassFlow = (_gasSpeed * 3.14159f * Mathf.Pow(deametr, 2)) / 4;

        _waterMassFlow = (_waterSpeed * 3.14159f * Mathf.Pow(deametr, 2)) / 4;

        _reynoldsNumber = (gasDensity * _gasSpeed * deametrDroplet) / gasDynamicViscosity;

        _massTransfer = empiricalConstantsA * Mathf.Pow((_reynoldsNumber / deametrDroplet), empiricalConstantsB);
    }
}
