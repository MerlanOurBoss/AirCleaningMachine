using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class Collectors : MonoBehaviour
{
    [SerializeField] private float _fillingColums = -50f;

    private bool filling = false;
    private float delay = 20f; //update

    private void Update()
    {
        if (_fillingColums <= 50)
        {
            filling = false;
        }
        if (filling)
        {
            _fillingColums += 1 * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.B)) 
        {
            ActivateFilling();
        }
    }


    public void ActivateFilling()
    {
        filling = true;
    }
}
