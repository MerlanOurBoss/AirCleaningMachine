using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseExited : MonoBehaviour
{

    private void OnMouseExit()
    {
        if (gameObject.GetComponent<PipeConnection>().isActivated == true)
        {
            gameObject.GetComponent<PipeConnection>().isConnected = false;
            gameObject.GetComponent<PipeConnection>().isActivated = false;
        }
        else if (gameObject.GetComponent<PipeConnection>().isActivated == false)
        {
            gameObject.GetComponent<PipeConnection>().isConnected = false;
        }
    }
}
