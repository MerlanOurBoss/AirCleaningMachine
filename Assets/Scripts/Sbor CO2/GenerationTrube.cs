using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationTrube : MonoBehaviour
{
    public GameObject pipePrefab; // Префаб цилиндра
    public Transform startPoint;  // Начальная точка
    public Transform endPoint;    // Конечная точка

    void Start()
    {
        GeneratePipe(startPoint.position, endPoint.position);
    }

    void GeneratePipe(Vector3 start, Vector3 end)
    {
        // Создаем новый цилиндр
        GameObject pipe = Instantiate(pipePrefab);

        // Вычисляем положение цилиндра между начальной и конечной точками
        pipe.transform.position = (start + end) / 2;

        // Вычисляем расстояние между точками
        float distance = Vector3.Distance(start, end);

        // Задаем высоту цилиндра (по оси Y) в соответствии с расстоянием
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z);

        // Направляем цилиндр от начала к концу
        pipe.transform.up = end - start;
    }
}
