using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GeneralManagerForElectroFilter : MonoBehaviour
{
    [Header("ElectroFilter")]
    [Space]

    [SerializeField] private ParticleSystem[] _electroFilterSmokes;
    [SerializeField] private Animator _electroFilterAnimation;
    public void StartElectroFilterSystem()
    {
        foreach (ParticleSystem smoke in _electroFilterSmokes)
        {
            smoke.Play();
        }
        _electroFilterAnimation.Play("NewColecAnim");
    }
}
