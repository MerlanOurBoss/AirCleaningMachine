using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManagment : MonoBehaviour
{
    private GameObject[] pipes;

    void Update()
    {
        pipes = GameObject.FindGameObjectsWithTag("Pipe");
        
    }
}
