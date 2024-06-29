using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAT : MonoBehaviour
{
    public Transform target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(target);
    }
}
