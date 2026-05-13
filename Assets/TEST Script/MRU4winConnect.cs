using System.IO;
using UnityEngine;

public class MRUReader : MonoBehaviour
{
    public string filePath = @"C:\MRU\data.csv";

    private string lastLine = "";

    void Update()
    {
        if (!File.Exists(filePath))
            return;

        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length == 0)
            return;

        string currentLine = lines[lines.Length - 1];

        if (currentLine != lastLine)
        {
            lastLine = currentLine;

            Debug.Log("MRU DATA: " + currentLine);

            string[] values = currentLine.Split(';');

            float co2 = float.Parse(values[1]);

            Debug.Log("CO2 = " + co2);
        }
    }
}