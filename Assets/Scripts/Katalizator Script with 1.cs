using TMPro;
using UnityEngine;

public class KatalizatorScriptwith1 : MonoBehaviour
{
    public TMP_InputField text;

    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;

    public ParticleSystem smokes;

    public void SetObjectsState(bool state)
    {
        obj1.SetActive(state);
        obj2.SetActive(state);
        obj3.SetActive(!state);

        if (state)
        {
            smokes.Play();
        }
        else
        {
            smokes.Stop();
        }
    }

    public void ActivateObj2()
    {
        SetObjectsState(true);
    }

    public void ActivateObj1()
    {
        SetObjectsState(false);
    }
}