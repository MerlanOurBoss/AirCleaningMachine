using geniikw.DataRenderer2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SxemaGeneral : MonoBehaviour
{
    [SerializeField] private Animator sxemAnim;
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
            sxemAnim.Play("LinesStop");
            cubesAnim.Play("CubesStop");
            isOff = false;
        }
    }
    public void SxemaStart() 
    {
        sxemAnim.Play("LinesAnim");
        cubesAnim.Play("CubesAnim");
    }
    
    public void SxemasStop()
    {
        sxemAnim.Play("LinesStop");
        cubesAnim.Play("CubesStop");
    }
}
