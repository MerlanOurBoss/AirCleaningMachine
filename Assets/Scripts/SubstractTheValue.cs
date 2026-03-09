using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SubstractTheValue : MonoBehaviour
{
    public TMP_InputField[] _inputFields;

    public TMP_Text _maxValue;
    private int totalValue = 150;
    private int[] previousValues;

    [SerializeField] private TMP_InputField _gasFlowMain;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private GameObject mainParent;

    [SerializeField] private float threshold = 200000f;  // порог
    [SerializeField] private float autoHideSeconds = 7f; // время показа

    [Header("Other Components")]
    [SerializeField] private Translator _translator;
    
    private Coroutine hideCoroutine;
    private double lastParsedFlow = double.NaN; // предыдущее распознанное значение
    private bool notificationVisible = false;

    private void Start()
    {
        var translateObj = GameObject.FindGameObjectWithTag("Translator");
        _translator = translateObj.GetComponent<Translator>();

        if (_translator != null)
        {
            _translator.OnLanguageChanged += OnLanguageChanged;

            OnLanguageChanged(_translator.currentLanguage);
        }
        
        _gasFlowMain.text = "100000 м³/ч";
                
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

    private void OnLanguageChanged(Translator.Language lang)
    {
        switch (lang)
        {
            case Translator.Language.Russian:
                if (_gasFlowMain.text == "100000 м³/ч" || _gasFlowMain.text == "100000 м³/сағ" || _gasFlowMain.text == "100000 m³/h")
                {
                    _gasFlowMain.text = "100000 м³/ч";
                } 
                else if (_gasFlowMain.text == "150000 м³/ч - ТЭЦ Павлодар" || _gasFlowMain.text == "150000 м³/сағ - ЖЭС Павлодар" || _gasFlowMain.text == "150000 m³/h - CHP Pavlodar")
                {
                    _gasFlowMain.text = "150000 м³/ч - ТЭЦ Павлодар";
                }
                else if (_gasFlowMain.text == "200000 м³/ч" || _gasFlowMain.text == "200000 м³/сағ" || _gasFlowMain.text == "200000 m³/h")
                {
                    _gasFlowMain.text = "200000 м³/ч"; 
                }
                else if (_gasFlowMain.text == "250000 м³/ч - ТЭЦ Алматы" || _gasFlowMain.text == "250000 м³/сағ - ЖЭС Алматы" || _gasFlowMain.text == "250000 m³/h - CHP Almaty")
                {
                    _gasFlowMain.text = "250000 м³/ч - ТЭЦ Алматы"; 
                }
                else if (_gasFlowMain.text == "400000 м³/ч" || _gasFlowMain.text == "400000 м³/сағ" || _gasFlowMain.text == "400000 m³/h")
                {
                    _gasFlowMain.text = "400000 м³/ч"; 
                }
                else if (_gasFlowMain.text == "500000 м³/ч" || _gasFlowMain.text == "500000 м³/сағ" || _gasFlowMain.text == "500000 m³/h")
                {
                    _gasFlowMain.text = "500000 м³/ч";
                }
                break;

            case Translator.Language.Kazakh:
                if (_gasFlowMain.text == "100000 м³/ч" || _gasFlowMain.text == "100000 м³/сағ" || _gasFlowMain.text == "100000 m³/h")
                {
                    _gasFlowMain.text = "100000 м³/сағ";
                } 
                else if (_gasFlowMain.text == "150000 м³/ч - ТЭЦ Павлодар" || _gasFlowMain.text == "150000 м³/сағ - ЖЭС Павлодар" || _gasFlowMain.text == "150000 m³/h - CHP Pavlodar")
                {
                    _gasFlowMain.text = "150000 м³/сағ - ЖЭС Павлодар";
                }
                else if (_gasFlowMain.text == "200000 м³/ч" || _gasFlowMain.text == "200000 м³/сағ" || _gasFlowMain.text == "200000 m³/h")
                {
                    _gasFlowMain.text = "200000 м³/сағ"; 
                }
                else if (_gasFlowMain.text == "250000 м³/ч - ТЭЦ Алматы" || _gasFlowMain.text == "250000 м³/сағ - ЖЭС Алматы" || _gasFlowMain.text == "250000 m³/h - CHP Almaty")
                {
                    _gasFlowMain.text = "250000 м³/сағ - ЖЭС Алматы"; 
                }
                else if (_gasFlowMain.text == "400000 м³/ч" || _gasFlowMain.text == "400000 м³/сағ" || _gasFlowMain.text == "400000 m³/h")
                {
                    _gasFlowMain.text = "400000 м³/сағ"; 
                }
                else if (_gasFlowMain.text == "500000 м³/ч" || _gasFlowMain.text == "500000 м³/сағ" || _gasFlowMain.text == "500000 m³/h")
                {
                    _gasFlowMain.text = "500000 м³/сағ";
                }
                break;

            case Translator.Language.English:
                if (_gasFlowMain.text == "100000 м³/ч" || _gasFlowMain.text == "100000 м³/сағ" || _gasFlowMain.text == "100000 m³/h")
                {
                    _gasFlowMain.text = "100000 m³/h";
                } 
                else if (_gasFlowMain.text == "150000 м³/ч - ТЭЦ Павлодар" || _gasFlowMain.text == "150000 м³/сағ - ЖЭС Павлодар" || _gasFlowMain.text == "150000 m³/h - CHP Pavlodar")
                {
                    _gasFlowMain.text = "150000 m³/h - CHP Pavlodar";
                }
                else if (_gasFlowMain.text == "200000 м³/ч" || _gasFlowMain.text == "200000 м³/сағ" || _gasFlowMain.text == "200000 m³/h")
                {
                    _gasFlowMain.text = "200000 m³/h"; 
                }
                else if (_gasFlowMain.text == "250000 м³/ч - ТЭЦ Алматы" || _gasFlowMain.text == "250000 м³/сағ - ЖЭС Алматы" || _gasFlowMain.text == "250000 m³/h - CHP Almaty")
                {
                    _gasFlowMain.text = "250000 m³/h - CHP Almaty"; 
                }
                else if (_gasFlowMain.text == "400000 м³/ч" || _gasFlowMain.text == "400000 м³/сағ" || _gasFlowMain.text == "400000 m³/h")
                {
                    _gasFlowMain.text = "400000 m³/h"; 
                }
                else if (_gasFlowMain.text == "500000 м³/ч" || _gasFlowMain.text == "500000 м³/сағ" || _gasFlowMain.text == "500000 m³/h")
                {
                    _gasFlowMain.text = "500000 m³/h";
                }
                break;
        }
    }
    
    private void Update()
    {
        if (_gasFlowMain.text == "100000 м³/ч " || _gasFlowMain.text == "100000 м³/сағ" || _gasFlowMain.text == "100000 m³/h")
        {
            mainParent.transform.localScale = new Vector3(1, 1, 1); 
        }
        else if (_gasFlowMain.text == "150000 м³/ч - ТЭЦ Павлодар" || _gasFlowMain.text == "150000 м³/сағ - ЖЭС Павлодар" || _gasFlowMain.text == "150000 m³/h - CHP Pavlodar")
        {
            mainParent.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f); 
        }
        else if (_gasFlowMain.text == "200000 м³/ч" || _gasFlowMain.text == "200000 м³/сағ" || _gasFlowMain.text == "200000 m³/h")
        {
            mainParent.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f); 
        }
        else if (_gasFlowMain.text == "250000 м³/ч - ТЭЦ Алматы" || _gasFlowMain.text == "250000 м³/сағ - ЖЭС Алматы" || _gasFlowMain.text == "250000 m³/h - CHP Almaty")
        {
            mainParent.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); 
        }
        else if (_gasFlowMain.text == "400000 м³/ч" || _gasFlowMain.text == "400000 м³/сағ" || _gasFlowMain.text == "400000 m³/h")
        {
            mainParent.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f); 
        }
        else if (_gasFlowMain.text == "500000 м³/ч" || _gasFlowMain.text == "500000 м³/сағ" || _gasFlowMain.text == "500000 m³/h")
        {
            mainParent.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f); 
        }
        
        if (!TryParseGasFlow(_gasFlowMain?.text, out var flow))
            return;

        // реагируем только если значение реально изменилось
        if (!Approximately(flow, lastParsedFlow))
        {
            lastParsedFlow = flow;

            if (flow > threshold)
            {
                ShowNotification(flow);
            }
            else
            {
                HideNotification();
            }
        }

        for (int i = 0; i < _inputFields.Length; i++)
        {
            int index = i;
            _inputFields[i].onValueChanged.AddListener((newValue) => OnInputFieldValueChanged(index, newValue));
        }
    }
    private void ShowNotification(double flow)
    {
        notificationPanel.SetActive(true);
        notificationVisible = true;

        notificationText.text =
            $"Значение расхода газа превышает допустимый предел в {threshold:N0} м³/ч.\r\n" +
            $"Вы выбрали: {flow:N0} м³/ч. Для корректной работы систему необходимо разделить на несколько параллельных потоков.";

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideNotificationAfterDelay(autoHideSeconds));
    }

    private void HideNotification()
    {
        if (!notificationVisible) return;
        if (hideCoroutine != null) { StopCoroutine(hideCoroutine); hideCoroutine = null; }
        notificationPanel.SetActive(false);
        notificationText.text = "";
        notificationVisible = false;
    }

    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideNotification();
    }

    // Пример обработчика, если нужно что-то делать при изменении конкретного поля
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
            Debug.LogWarning("Введите корректное число");

            _inputFields[index].text = previousValues[index].ToString();
        }
    }

    // Парсим числа из строк вида "500000 м³/ч", "400 000", "250,5"
    private static bool TryParseGasFlow(string raw, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        var cleaned = new string(raw.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',' || ch == ' ').ToArray());
        cleaned = cleaned.Replace(" ", "").Replace(',', '.');
        return double.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    // Сравнение с допуском (чтобы не дергаться от мелких изменений)
    private static bool Approximately(double a, double b, double eps = 1e-3)
    {
        if (double.IsNaN(a) || double.IsNaN(b)) return false;
        return Mathf.Abs((float)(a - b)) < eps;
    }

    private void UpdateTotalText()
    {
        _maxValue.text = totalValue.ToString() + " %";
    }
}
