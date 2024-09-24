using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDRchange : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    
    private bool isActiveGameObject = true;

    private void Update()
    {
        if (!isActiveGameObject)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
        }
    }

    public void ChangedBool()
    {
        isActiveGameObject = !isActiveGameObject;
    }

}
