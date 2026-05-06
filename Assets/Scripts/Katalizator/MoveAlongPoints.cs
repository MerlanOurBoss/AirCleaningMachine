using UnityEngine;

public class MoveAndRotateAlongPoints : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public TemperatureCatalizator temp;
    public ParticleSystem pc;

    private int currentPointIndex = 0;

    void Start()
    {
        pc.Stop();
    }
    void Update()
    {
        if (temp.isStarting)
        {
            pc.Play();
            if (points.Length == 0) return;

            Transform targetPoint = points[currentPointIndex];

            Vector3 direction = (targetPoint.position - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex++;

                if (currentPointIndex >= points.Length)
                {
                    transform.position = points[0].position;
                    currentPointIndex = 1;
                }
            }
        }
    }
}
