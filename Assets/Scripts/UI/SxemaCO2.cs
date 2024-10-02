using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SxemaCO2 : MonoBehaviour
{
    [SerializeField] private Animator sxemSborCO2Anim;
    [SerializeField] private Animator cubesAnim;
    [SerializeField] private GameObject sxema;
    [SerializeField] private GameObject sxemaOriginal;

    private bool isOff = false;

    private bool isActivated = false;

    public void OnOffSxema()
    {
        if (!isActivated)
        {
            sxema.SetActive(true);
            sxemaOriginal.SetActive(false);
            sxemSborCO2Anim.Play("LinesCO2Stop");
            isActivated = true;
        }
        else
        {
            isOff = true;
            sxema.SetActive(false);
            sxemaOriginal.SetActive(true);
            sxemSborCO2Anim.Play("LinesCO2Stop");
            isActivated = false;
        }
    }

    private void Update()
    {
        if (isOff)
        {
            sxemSborCO2Anim.Play("LinesCO2Stop");
            isOff = false;
        }
    }
    public void SxemaStart()
    {
        sxemSborCO2Anim.Play("LinesCO2Anim");
    }

    public void SxemaStart2()
    {
        sxemSborCO2Anim.Play("LinesCO2AnimSecond");
    }
    public void SxemasStop()
    {
        sxemSborCO2Anim.Play("LinesCO2Stop");
    }
}
