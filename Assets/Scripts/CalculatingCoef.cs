using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculatingCoef : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] _myTexts;
    [SerializeField] private TextMeshProUGUI SheloshCoef;

    private void Start()
    {
        _myTexts[0].text = "5 ∞C";
        _myTexts[1].text = "¬ода";
        _myTexts[2].text = "0,085 м/с";
        _myTexts[3].text = "0,75 моль/м";

    }
    private void Update()
    {
        if (_myTexts[2].text != " " && _myTexts[3].text != " ")
        {
            float resReact = float.Parse(_myTexts[2].text[.._myTexts[2].text.IndexOf(" ")]) * 10 * float.Parse(_myTexts[3].text[.._myTexts[3].text.IndexOf(" ")]);
            SheloshCoef.text = "Ёффект. щелоч. эмуль.: " + (resReact * 100).ToString("0.") + " %";
        }
    }
}
