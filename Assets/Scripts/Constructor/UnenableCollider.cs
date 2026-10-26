using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnenableCollider : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.ConnectionPoint))
        {
            gameObject.GetComponent<PipeConnection>().isConnected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.ConnectionPoint))
        {
            gameObject.GetComponent<PipeConnection>().isConnected = false;
        }
    }
}
