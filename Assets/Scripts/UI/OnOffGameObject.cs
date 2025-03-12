using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffGameObject : MonoBehaviour
{
    private bool isOn = true;

    public void OnOff(GameObject obj)
    {
        if (isOn)
        {
            obj.SetActive(false);
            isOn = false;
        }
        else
        {
            obj.SetActive(true);
            isOn = true;
        }

    }
}
