using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SborManager : MonoBehaviour
{
    private int countOfCapsul;
    public TextMeshProUGUI countText;
    [SerializeField] private GameObject FirstCapsul;
    [SerializeField] private GameObject SecondCapsul;

    private bool isFull = false;

    private void Start()
    {
        countOfCapsul = 0;
    }

    private void Update()
    {
        countText.text = countOfCapsul.ToString();

        if (countOfCapsul == 0)
        {
            isFull = false;
            FirstCapsul.SetActive(false);
            SecondCapsul.SetActive(false);
        }
        if (countOfCapsul == 1)
        {
            
            FirstCapsul.SetActive(true); 
            SecondCapsul.SetActive(false);
        }
        if (countOfCapsul == 2)
        {
            isFull = true;
            FirstCapsul.SetActive(true);
            SecondCapsul.SetActive(true);
        }
        
    }

    public void PlusCountOfCapsul()
    {
        if (!isFull)
        {
            countOfCapsul++;
        }
    }

    public void MinusCountOfCapsul()
    {
        if (isFull)
        {
            countOfCapsul--;
        }
    }
}
