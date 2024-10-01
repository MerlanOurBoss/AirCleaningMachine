using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SxemaCO2 : MonoBehaviour
{
    [SerializeField] private Animator sxemSborCO2Anim;
    [SerializeField] private Animator cubesAnim;
    [SerializeField] private GameObject sxema;
    [SerializeField] private GameObject mycamera;
    [SerializeField] private GameObject cameraUI;

    private bool isOff = false;

    private bool isActivated = false;

    public void OnOffSxema()
    {
        if (!isActivated)
        {
            sxema.SetActive(true);
            mycamera.SetActive(false);
            cameraUI.SetActive(false);
            isActivated = true;
        }
        else
        {
            isOff = true;
            sxema.SetActive(false);
            mycamera.SetActive(true);
            cameraUI.SetActive(true);
            isActivated = false;
        }
    }

    private void Update()
    {
        if (isOff)
        {
            sxemSborCO2Anim.Play("LinesCO2Stop");
            //cubesAnim.Play("CubesStop");
            isOff = false;
        }
    }
    public void SxemaStart()
    {
        sxemSborCO2Anim.Play("LinesCO2Anim");
        Invoke(nameof(SxemaStart2), 7f);
        //cubesAnim.Play("CubesAnim");
    }

    public void SxemaStart2()
    {
        sxemSborCO2Anim.Play("LinesCO2AnimSecond");
    }
    public void SxemasStop()
    {
        sxemSborCO2Anim.Play("LinesCO2Stop");
        //cubesAnim.Play("CubesStop");
    }
}
