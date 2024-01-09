using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDots : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dots")) 
        {
            Destroy(other.gameObject);
        }
    }
}
