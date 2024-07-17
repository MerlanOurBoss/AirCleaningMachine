using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManagerForCooling : MonoBehaviour
{
    [Header("Cooling")]
    [Space]

    [SerializeField] private ParticleSystem[] _coolingSmokes;
    public void StartCoolingSystem()
    {
        foreach (ParticleSystem smoke in _coolingSmokes)
        {
            smoke.Play();
        }
    }
}
