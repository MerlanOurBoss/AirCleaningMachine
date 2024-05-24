using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PipeConnection : MonoBehaviour
{
    public Transform entrance;
    public float pos_x_90 = 0;
    public float pos_y_90 = 0;
    public float pos_z_90 = 0;

    public float rot_x_90 = 0;
    public float rot_y_90 = 0;
    public float rot_z_90 = 0;

    private bool triggerActive = true;

    private void OnTriggerStay(Collider other)
    {
        if (triggerActive && other.CompareTag("Pipe"))
        {
            Debug.Log("ff");
            StartCoroutine(ChangeColorForDuration(other.gameObject, Color.green, 2f));
            other.transform.SetParent(transform);
            other.transform.localPosition = new Vector3(pos_x_90, pos_y_90, pos_z_90);
            other.transform.Rotate(rot_x_90, rot_y_90, rot_z_90);
            other.GetComponent<MoveObjectWithMouse>().isDragging = false;
            other.GetComponent<MoveObjectWithMouse>().ToggleCollider(false);
            triggerActive = false;
            GetComponent<Collider>().enabled = false;
        }
    }

    private IEnumerator ChangeColorForDuration(GameObject obj, Color color, float duration)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Color originalColor = rend.material.color;
            rend.material.color = color;
            yield return new WaitForSeconds(duration);
            rend.material.color = originalColor;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Rotate(90, 0f, 0f, Space.Self);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Rotate(-90, 0f, 0f, Space.Self);
        }
    }
}
