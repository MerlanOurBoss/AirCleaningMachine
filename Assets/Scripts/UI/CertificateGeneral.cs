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
            if (mycamera != null)
            {
                mycamera.SetActive(false);
            }

            if (cameraUI != null)
            {
                cameraUI.SetActive(false);
            }
            certificate.SetActive(true);
            isActivated = true;
        }
        else
        {
            if (mycamera != null)
            {
                mycamera.SetActive(true);
            }

            if (cameraUI != null)
            {
                cameraUI.SetActive(true);
            }
            certificate.SetActive(false);
            isActivated = false;
        }
    }

}
