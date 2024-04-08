using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SubstractTheValue : MonoBehaviour
{
    public TMP_InputField[] _inputFields;

    public TMP_Text _maxValue;
    private int totalValue = 150;
    private int[] previousValues;
    

    private void Start()
    {
        previousValues = new int[_inputFields.Length];

        for (int i = 0; i < _inputFields.Length; i++)
        {
            int defaultValue = 10;
            if (i == 0)
            {
                defaultValue = 20;
            }
            else if (i == 1)
            {
                defaultValue = 30;
            }
            else if (i == 4)
            {
                defaultValue = 20;
            }
            else if (i == 5)
            {
                defaultValue = 20;
            }
            else if (i == 6)
            {
                defaultValue = 20;
            }

            _inputFields[i].text = defaultValue.ToString();
            previousValues[i] = defaultValue;
            totalValue -= defaultValue;
        }

        UpdateTotalText();
    }

    private void Update()
    {
        for (int i = 0; i < _inputFields.Length; i++)
        {
            int index = i;
            _inputFields[i].onValueChanged.AddListener((newValue) => OnInputFieldValueChanged(index, newValue));
        }
    }

    private void OnInputFieldValueChanged(int index, string newValue)
    {
        if (int.TryParse(newValue, out int currentNumber))
        {
            totalValue -= (currentNumber - previousValues[index]);

            previousValues[index] = currentNumber;

            UpdateTotalText();
        }
        else
        {
            Debug.LogWarning("¬ведите корректное число");

            _inputFields[index].text = previousValues[index].ToString();
        }

    }

    private void UpdateTotalText()
    {
        _maxValue.text = totalValue.ToString() + " %";
    }
}
