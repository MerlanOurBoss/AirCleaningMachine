using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderAnimationScript : MonoBehaviour
{
    public Animator animator;
    public Animator animator2;
    public string str;
    public string str1;

    public Image image;

    public Sprite[] mysprites;
    private bool isActiveGameObject = false;

    private void Start()
    {
        image.sprite = mysprites[1];
        //animator.Play("Slider Animation");
        //animator2.Play(str1);
        //isActiveGameObject = false;
    }

    public void buttonPressed()
    {
        if (isActiveGameObject)
        {
            image.sprite = mysprites[1];
            animator.Play("Slider Animation");
            animator2.Play(str1);
            isActiveGameObject = false;
        }
        else
        {
            image.sprite = mysprites[0];
            animator.Play("Slider Animationi Exit");
            animator2.Play(str);
            isActiveGameObject = true;

        }
    }
}
