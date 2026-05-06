using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIscript : MonoBehaviour
{
    public GameObject gameObjects;
    public Image images;

    public Sprite[] mysprites;
    private bool isActiveGameObject = true;
    private void Update()
    {
        if (!isActiveGameObject)
        {
            images.sprite = mysprites[0];
            gameObjects.SetActive(true);
        }
        else
        {
            images.sprite = mysprites[1];
            gameObjects.SetActive(false);
        }
    }

    public void ChangedBool()
    {
        isActiveGameObject = !isActiveGameObject;
        Debug.Log("active" + isActiveGameObject);
    }
    public void TrueBool()
    {
        isActiveGameObject = true;
    }
}
