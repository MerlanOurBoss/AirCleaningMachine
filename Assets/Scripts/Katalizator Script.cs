using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatalizatorScript : MonoBehaviour
{
    [SerializeField] private GameObject[] modules;

    [SerializeField] private GameObject traingleOrig;
    [SerializeField] private GameObject traingleSecond;

    [SerializeField] private ParticleSystem[] smokes1;
    [SerializeField] private ParticleSystem[] smokes2;

    [SerializeField] private MeshRenderer fourthObj;

    public Material realMat;
    public Material secondMat;

    public bool isSecond = false;

    private void Start()
    {
        changePosition0();
    }

    [System.Obsolete]
    public void changePosition0()
    {
        modules[0].transform.position = new Vector3(0, 0, 0);
        modules[1].transform.position = new Vector3(0, 0, 0);
        modules[2].transform.position = new Vector3(0, 0, 0);
        modules[3].transform.position = new Vector3(0, 0, 0);
        traingleOrig.SetActive(true);
        traingleSecond.SetActive(false);
        fourthObj.material = realMat;

        smokes2[0].startDelay = 13f;
        smokes2[1].startDelay = 14f;
        smokes2[2].startDelay = 15f;

        isSecond = false;
    }

    [System.Obsolete]
    public void changePosition1()
    {
        modules[0].transform.position = new Vector3(0, 0, 0);
        modules[1].transform.position = new Vector3(0, 0, 0);

        modules[2].transform.position = new Vector3(147f, -0.315589905f, 0);
        modules[3].transform.position = new Vector3(-147f, 0, 0);
        traingleOrig.SetActive(false);
        traingleSecond.SetActive(true);
        fourthObj.material = secondMat;

        smokes2[0].startDelay = 13f;
        smokes2[1].startDelay = 14f;
        smokes2[2].startDelay = 15f;

        isSecond = true;
    }

    [System.Obsolete]
    public void changePosition2()
    {
        modules[0].transform.position = new Vector3(0, 0, 0);
        modules[1].transform.position = new Vector3(99.7f, 0, 0);

        modules[2].transform.position = new Vector3(147f, -0.315589905f, 0);
        modules[3].transform.position = new Vector3(-194.05f, -0.1f, 0);
        traingleOrig.SetActive(false);
        traingleSecond.SetActive(true);

        fourthObj.material = secondMat;
        smokes2[0].startDelay = 11f;
        smokes2[1].startDelay = 12f;
        smokes2[2].startDelay = 13f;
        isSecond = true;
    }
}
