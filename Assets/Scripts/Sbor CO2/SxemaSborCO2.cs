using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class SxemaSborCO2 : MonoBehaviour
{
    [SerializeField] private float delay = 50f;
    public bool isStarting = false;

    [SerializeField] private GameObject[] baseGates;
    [SerializeField] private GameObject[] optionalGates;

    [Header("Mathematical Values")]
    [SerializeField] private TMP_InputField mathVolume;
    [SerializeField] private TMP_InputField mathDiametr;

    [Header("Mathematical UI")]
    [SerializeField] private TextMeshProUGUI mathSpeed;
    [SerializeField] public float mathGasSpeed { get; private set; } = 0f;


    private bool isStarted1 = false;
    private bool isStarted2 = false;
    private bool isStarted2_1 = false;
    private float fillingCount = 0;

    private float volumeValue = 0;
    private float diametValue = 0;
    // Update is called once per frame
    public void Starting()
    {
        isStarting = true;
    }
    void Update()
    {

        if (mathVolume.text.Length > 0 && mathDiametr.text.Length > 0)
        {
            volumeValue = float.Parse(mathVolume.text[..mathVolume.text.IndexOf(" ")]);
            diametValue = float.Parse(mathDiametr.text[..mathDiametr.text.IndexOf(" ")]);
        }

        mathGasSpeed = (4 * volumeValue) / (3600 * 3.14f * (diametValue * diametValue));
        mathSpeed.text = "U = " + mathGasSpeed.ToString("0.") + " м/с";
        
        if (isStarting)
        {
            
            delay -= Time.deltaTime;
            if (delay < 0)
            {
                delay = 0;
                isStarted1 = true;
                isStarting = false;
            }
        }

        if (isStarted1)
        {
            fillingCount += 5 * Time.deltaTime;
            baseGates[0].SetActive(false);
            baseGates[1].SetActive(true);
            optionalGates[0].SetActive(false); // вход в Адсорбер (вкл)
            optionalGates[1].SetActive(true);

            if (fillingCount > 50)
            {
                
                optionalGates[2].SetActive(false); // выход (вкл)
                optionalGates[3].SetActive(true);
                baseGates[2].SetActive(false);
                baseGates[3].SetActive(true);
            }

            if (fillingCount > 52)
            {
                optionalGates[22].SetActive(false);
                optionalGates[23].SetActive(true);
            }
            if (fillingCount >= 150)
            {
                optionalGates[4].SetActive(false); // Горячий газ (вкл)
                optionalGates[5].SetActive(true);

                optionalGates[22].SetActive(true);
                optionalGates[23].SetActive(false);

                isStarted1 = false;
                isStarted2 = true;

                fillingCount = 0;
            }
        }

        else if (isStarted2)
        {
            fillingCount += 5 * Time.deltaTime;

            optionalGates[0].SetActive(true); // вход в Адсорбер (выкл)
            optionalGates[1].SetActive(false);

            optionalGates[2].SetActive(true); // выход (выкл)
            optionalGates[3].SetActive(false);


            optionalGates[6].SetActive(false); // в Компрессор (вкл)
            optionalGates[7].SetActive(true);

            optionalGates[8].SetActive(false); // вход 2 в Адсорбер (вкл)
            optionalGates[9].SetActive(true);

            optionalGates[14].SetActive(true); // в Компрессор 2 (выкл)
            optionalGates[15].SetActive(false);

            if (fillingCount > 20 && fillingCount < 24)
            {
                optionalGates[16].SetActive(false);
                optionalGates[17].SetActive(true);
            }

            if (fillingCount > 25 && fillingCount < 29)
            {
                optionalGates[18].SetActive(false);
                optionalGates[19].SetActive(true);
            }

            if (fillingCount > 30 && fillingCount < 34)
            {
                optionalGates[20].SetActive(false);
                optionalGates[21].SetActive(true);
            }


            if (fillingCount > 50 && fillingCount < 55)
            {
                optionalGates[10].SetActive(false); // выход 2  (вкл)
                optionalGates[11].SetActive(true);
            }

            if (fillingCount > 52 && fillingCount < 56)
            {
                optionalGates[22].SetActive(false);
                optionalGates[23].SetActive(true);
            }

            if (fillingCount >= 150)
            {
                optionalGates[4].SetActive(true); // Горячий газ (выкл)
                optionalGates[5].SetActive(false);

                optionalGates[12].SetActive(false); // Горячий газ 2 (вкл)
                optionalGates[13].SetActive(true);

                optionalGates[22].SetActive(true);
                optionalGates[23].SetActive(false);

                isStarted2 = false;
                isStarted2_1 = true;

                fillingCount = 0;
            }
        }

        else if (isStarted2_1)
        {
            fillingCount += 5 * Time.deltaTime;

            optionalGates[8].SetActive(true); // вход 2 в Адсорбер (выкл)
            optionalGates[9].SetActive(false);

            optionalGates[10].SetActive(true); // выход 2 (выкл)
            optionalGates[11].SetActive(false);

            optionalGates[14].SetActive(false); // в Компрессор 2 (вкл)
            optionalGates[15].SetActive(true);

            optionalGates[6].SetActive(true); // в Компрессор (выкл)
            optionalGates[7].SetActive(false);

            optionalGates[0].SetActive(false); // вход в Адсорбер (вкл)
            optionalGates[1].SetActive(true);


            if (fillingCount > 50 && fillingCount < 55)
            {
                optionalGates[2].SetActive(false); // выход (вкл)
                optionalGates[3].SetActive(true);
                optionalGates[22].SetActive(false);
                optionalGates[23].SetActive(true);
            }

            if (fillingCount >= 150)
            {
                optionalGates[12].SetActive(true); // Горячий газ 2 (выкл)
                optionalGates[13].SetActive(false);

                optionalGates[4].SetActive(false); // Горячий газ (вкл)
                optionalGates[5].SetActive(true);

                optionalGates[22].SetActive(true);
                optionalGates[23].SetActive(false);

                isStarted2_1 = false;
                isStarted2 = true;

                fillingCount = 0;
            }
        }



    }

    public void Reset()
    {
        optionalGates[0].SetActive(true); // вход 2 в Адсорбер (выкл)
        optionalGates[4].SetActive(true);

        optionalGates[8].SetActive(true); // выход 2 (выкл)
        optionalGates[12].SetActive(true);

        optionalGates[6].SetActive(true); // в Компрессор 2 (вкл)
        optionalGates[2].SetActive(true);

        optionalGates[14].SetActive(true); // в Компрессор (выкл)
        optionalGates[10].SetActive(true);


        optionalGates[1].SetActive(false);
        optionalGates[3].SetActive(false);
        optionalGates[5].SetActive(false);
        optionalGates[7].SetActive(false);
        optionalGates[9].SetActive(false);
        optionalGates[11].SetActive(false);
        optionalGates[13].SetActive(false);
        optionalGates[15].SetActive(false);

        optionalGates[16].SetActive(true);
        optionalGates[17].SetActive(false);
        optionalGates[18].SetActive(true);
        optionalGates[19].SetActive(false);
        optionalGates[20].SetActive(true);
        optionalGates[21].SetActive(false);
        optionalGates[22].SetActive(true);
        optionalGates[23].SetActive(false);

        baseGates[0].SetActive(true); // вход в Адсорбер (вкл)
        baseGates[2].SetActive(true);
        baseGates[1].SetActive(false);
        baseGates[3].SetActive(false);


        isStarted1 = false;
        isStarted2 = false;
        isStarted2_1 = false;
        isStarting = false;
        fillingCount = 0;
    }
}
