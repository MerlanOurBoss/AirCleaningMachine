using UnityEngine;

public class MoveAndRotateAlongPoints : MonoBehaviour
{
    public Transform[] points; // ������ ����� ��������
    public float speed = 2f;   // �������� ��������
    public float rotationSpeed = 5f; // �������� ��������
    public TemperatureCatalizator temp;
    public ParticleSystem pc;

    private int currentPointIndex = 0; // ������ ������� �����

    void Start()
    {
        pc.Stop();
    }
    void Update()
    {
        if (temp.isStarting)
        {
            pc.Play();
            if (points.Length == 0) return; // ���������, ���� �� �����

            Transform targetPoint = points[currentPointIndex];

            // 1. ��������� ����������� � ��������� �����
            Vector3 direction = (targetPoint.position - transform.position).normalized;

            // 2. ��������� ���� ������ �� ��� Z
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 3. ������ ������� ������ �� Z-���
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 4. ������� ������ �����
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            // 5. ���������, �������� �� �� �����
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex++; // ������� � ��������� �����

                if (currentPointIndex >= points.Length)
                {
                    transform.position = points[0].position; // �������� � ������ �����
                    currentPointIndex = 1; // �������� �������� �� ������ �����
                }
            }
        }
    }
}
