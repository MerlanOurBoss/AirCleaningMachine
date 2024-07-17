using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GeneralManagerForConstructor: MonoBehaviour
{
    [Header("ElectroFilter")]
    [Space]

    public bool isTriggerFromElectro = false;
    [SerializeField] private ParticleSystem[] _electroFilterSmokes;
    [SerializeField] private Animator _electroFilterAnimation;    

    private void Update()
    {
        if (isTriggerFromElectro)
        {
            foreach (ParticleSystem smoke in _electroFilterSmokes)
            {
                smoke.Play();
            }
            _electroFilterAnimation.Play("NewColecAnim");
        }
    }
}
