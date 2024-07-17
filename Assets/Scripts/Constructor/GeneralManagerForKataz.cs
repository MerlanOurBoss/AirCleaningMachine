using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManagerForKataz : MonoBehaviour
{
    [Header("Kataz")]
    [Space]

    [SerializeField] private ParticleSystem[] _katazSmokes;
    public void StartKatazSystem()
    {
        foreach (ParticleSystem smoke in _katazSmokes)
        {
            smoke.Play();
        }
    }
}
