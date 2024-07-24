using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorScript : MonoBehaviour
{
    [SerializeField] Animator fluid;

    public bool isTrigger = false;
    private bool isAnimationStarted = false;
    private float toStartFluid = 5f;


    private void Update()
    {
        if (isTrigger)
        {
            toStartFluid -= Time.deltaTime;

        }
        if (toStartFluid < 0 && !isAnimationStarted)
        {
            fluid.Play("WaterAnimation");
            isAnimationStarted = true;
        }
    }
}
