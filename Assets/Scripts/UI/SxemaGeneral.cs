using geniikw.DataRenderer2D;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SxemaGeneral : MonoBehaviour
{
    [SerializeField] private Animator sxemAnim;
    [SerializeField] private Animator cubesAnim;
    [SerializeField] private GameObject sxema;
    [SerializeField] private GameObject mycamera;
    [SerializeField] private GameObject cameraUI;

    [SerializeField] private GameObject[] backgrounds;

    private bool isOff = false;

    private bool isActivated = false;
    private bool isPaused = false;

    public void OnOffSxema()
    {
        if (!isActivated)
        {
            sxema.SetActive(true);
            mycamera.SetActive(false);
            cameraUI.SetActive(false);
            isActivated = true;
            backgrounds[1].SetActive(true);
            backgrounds[0].SetActive(false);
        }
        else
        {
            isOff = true;
            sxema.SetActive(false);
            mycamera.SetActive(true);
            cameraUI.SetActive(true);
            isActivated = false;
            backgrounds[0].SetActive(true);
            backgrounds[1].SetActive(false);
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

    public void PauseSxem()
    {
        if (!isPaused)
        {
            sxemAnim.speed = 0;
            cubesAnim.speed = 0;
            isPaused = true;
        }
        else
        {
            sxemAnim.speed = 1;
            cubesAnim.speed = 1;
            isPaused = false;
        }
    }
}
