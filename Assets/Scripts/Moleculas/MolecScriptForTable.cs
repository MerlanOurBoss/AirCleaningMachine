using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MolecScriptForTable : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] orig;
    [SerializeField] private TMP_InputField[] second;

    private bool isBool = false;
    private void Update()
    {
        if (isBool)
        {
            for (int i = 0; i < orig.Length; i++)
            {
                orig[i].text = second[i].text;
            }
        }
        
    }
    public void getDataMolec()
    {
        isBool = true;
        for (int i = 0; i < second.Length; i++)
        {
            second[i].text = orig[i].text;
        }
    }

    public void sendDataMolec()
    {
        isBool = false;
    }
}
