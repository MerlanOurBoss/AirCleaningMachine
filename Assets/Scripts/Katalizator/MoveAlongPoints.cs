using UnityEngine;

public class MoveAndRotateAlongPoints : MonoBehaviour
{
    public Transform[] points; // Массив точек маршрута
    public float speed = 2f;   // Скорость движения
    public float rotationSpeed = 5f; // Скорость поворота
    public TemperatureCatalizator temp;
    public ParticleSystem pc;

    private int currentPointIndex = 0; // Индекс текущей точки

    void Start()
    {
        pc.Stop();
    }
    void Update()
    {
        if (temp.isStarting)
        {
            pc.Play();
            if (points.Length == 0) return; // Проверяем, есть ли точки

            Transform targetPoint = points[currentPointIndex];

            // 1. Вычисляем направление к следующей точке
            Vector3 direction = (targetPoint.position - transform.position).normalized;

            // 2. Вычисляем угол только по оси Z
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 3. Создаём поворот только по Z-оси
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 4. Двигаем объект вперёд
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            // 5. Проверяем, достигли ли мы точки
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex++; // Переход к следующей точке

                if (currentPointIndex >= points.Length)
                {
                    transform.position = points[0].position; // Телепорт к первой точке
                    currentPointIndex = 1; // Начинаем движение ко второй точке
                }
            }
        }
    }
}
