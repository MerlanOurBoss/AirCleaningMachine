using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SxemaElectrofilter : MonoBehaviour
{
    [SerializeField] private Animator sxemElectroAnim;
    [SerializeField] private GameObject sxema;
    [SerializeField] private GameObject sxemaOriginal;


    [SerializeField] private TextMeshProUGUI pauseText;

    private bool isOff = false;

    private bool isActivated = false;

    private bool isPaused = false;

    public void Start()
    {
        pauseText.text = "Pause";
    }
    public void OnOffSxema()
    {
        if (!isActivated)
        {
            sxema.SetActive(true);
            sxemaOriginal.SetActive(false);
            sxemElectroAnim.Play("Stop");
            isActivated = true;
        }
        else
        {
            isOff = true;
            sxema.SetActive(false);
            sxemaOriginal.SetActive(true);
            sxemElectroAnim.Play("Stop");
            isActivated = false;
        }
    }

    private void Update()
    {
        

        if (isOff)
        {
            sxemElectroAnim.Play("Stop");
            isOff = false;
        }
    }
    public void SxemaStart()
    {
        sxemElectroAnim.Play("Main");
    }


    public void PauseSxema()
    {
        if (!isPaused)
        {
            pauseText.text = "Resume";
            sxemElectroAnim.speed = 0;
            isPaused = true;
        }
        else
        {
            pauseText.text = "Pause";
            sxemElectroAnim.speed = 1;
            isPaused = false;
        }
    }
    public void SxemasStop()
    {
        sxemElectroAnim.Play("Stop");
    }
}
