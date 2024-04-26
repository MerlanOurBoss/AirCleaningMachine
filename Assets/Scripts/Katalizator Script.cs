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

    public float x;
    public float y;
    public float z;

    public float delaySmoke; //13

    private void Start()
    {
        changePosition0();
    }

    [System.Obsolete]
    public void changePosition0()
    {
        modules[0].transform.position = new Vector3(0 - x, 0 - y, 0 - z);
        modules[1].transform.position = new Vector3(0 - x, 0 - y, 0 - z);
        modules[2].transform.position = new Vector3(0 - x, 0 - y, 0 - z);
        modules[3].transform.position = new Vector3(0 - x, 0 - y, 0 - z);
        traingleOrig.SetActive(true);
        traingleSecond.SetActive(false);
        fourthObj.material = realMat;

        smokes2[0].startDelay = delaySmoke;
        smokes2[1].startDelay = delaySmoke + 1;
        smokes2[2].startDelay = delaySmoke + 2;

        isSecond = false;
    }

    [System.Obsolete]
    public void changePosition1()
    {
        modules[0].transform.position = new Vector3(0 - x, 0 - y, 0 - z);
        modules[1].transform.position = new Vector3(0 - x, 0 - y, 0 - z);

        modules[2].transform.position = new Vector3(147f - x, -0.315589905f - y, 0 - z);
        modules[3].transform.position = new Vector3(-147f - x, 0 - y, 0 - z);
        traingleOrig.SetActive(false);
        traingleSecond.SetActive(true);
        fourthObj.material = secondMat;

        smokes2[0].startDelay = delaySmoke;
        smokes2[1].startDelay = delaySmoke + 1;
        smokes2[2].startDelay = delaySmoke + 2;

        isSecond = true;
    }

    [System.Obsolete]
    public void changePosition2()
    {
        modules[0].transform.position = new Vector3(0 - x, 0 - y, 0 - z);
        modules[1].transform.position = new Vector3(99.7f - x, 0 - y, 0 - z);

        modules[2].transform.position = new Vector3(147f - x, -0.315589905f - y, 0 - z);
        modules[3].transform.position = new Vector3(-194.05f - x, -0.1f - y, 0 - z);
        traingleOrig.SetActive(false);
        traingleSecond.SetActive(true);

        fourthObj.material = secondMat;
        smokes2[0].startDelay = delaySmoke - 2;
        smokes2[1].startDelay = delaySmoke - 1;
        smokes2[2].startDelay = delaySmoke;
        isSecond = true;
    }
}
