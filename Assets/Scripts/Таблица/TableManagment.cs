using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TableManagment : MonoBehaviour
{
    [SerializeField] private GameObject[] tables;

    private bool iEnabled = false;
    public void OnOrOffTable()
    {
        if (!iEnabled)
        {
            foreach (var table in tables)
            {
                table.SetActive(false);
            }
            iEnabled = true;
        }
        else
        {
            foreach (var table in tables)
            {
                table.SetActive(true);
            }
            iEnabled = false;
        }
    }
}
