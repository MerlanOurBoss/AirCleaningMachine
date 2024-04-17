using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KatalizatorScriptwith1 : MonoBehaviour
{
    public TMP_InputField text;

    public GameObject obj1;
    public GameObject obj2;

    public GameObject obj3;

    public ParticleSystem smokes;

    public void obj2Void()
    {
        obj1.SetActive(true);
        obj2.SetActive(true);
        obj3.SetActive(false);
        smokes.Play();
    }

    public void obj1Void()
    {
        obj1.SetActive(false);
        obj2.SetActive(false);
        obj3.SetActive(true);
        smokes.Stop();
    }
}
