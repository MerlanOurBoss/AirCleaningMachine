using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseTable : MonoBehaviour
{
    [SerializeField] private Slider[] sliders;
    [SerializeField] private TextMeshProUGUI[] texts;


    private void Update()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = sliders[i].value.ToString();
        }
    }
}
