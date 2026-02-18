using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CertificateGeneral : MonoBehaviour
{
    [SerializeField] private GameObject certificate;
    [SerializeField] private GameObject mycamera;
    [SerializeField] private GameObject cameraUI;
    private bool isActivated = false;
    private bool isPaused = false;

    public void OnOffCertificate()
    {
        if (!isActivated)
        {
            mycamera.SetActive(false);
            cameraUI.SetActive(false);
            certificate.SetActive(true);
            isActivated = true;
        }
        else
        {
            mycamera.SetActive(true);
            cameraUI.SetActive(true);
            certificate.SetActive(false);
            isActivated = false;
        }
    }

}
