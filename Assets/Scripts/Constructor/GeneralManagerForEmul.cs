using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GeneralManagerForEmul : MonoBehaviour
{
    [Header("Emul")]
    [Space]

    [SerializeField] private ParticleSystem[] _emulSmokes;
    [SerializeField] private PlayableDirector[] _emulFluid;

    private bool _isPlaying = false;
    public void StartEmulSystem()
    {
        if (!_isPlaying)
        {
            foreach (ParticleSystem smoke in _emulSmokes)
            {
                smoke.Play();
            }
            foreach (PlayableDirector fluid in _emulFluid)
            {
                fluid.Play();
            }
            _isPlaying = true;
        }
        
    }
}
