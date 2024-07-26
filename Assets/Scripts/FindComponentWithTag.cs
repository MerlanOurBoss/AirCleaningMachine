using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindComponentWithTag : MonoBehaviour
{
    public string tagName;
    public GameObject coefficientObj;
    private GameObject foundObject;
    private void Start()
    {
        foundObject = GameObject.FindGameObjectWithTag(tagName);

        if (foundObject == null)
        {
            gameObject.SetActive(false);
            coefficientObj.SetActive(false);
        }
    }
}
