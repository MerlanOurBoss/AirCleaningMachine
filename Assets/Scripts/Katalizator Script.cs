using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatalizatorScript : MonoBehaviour
{
    [SerializeField] private GameObject[] modules;

    [SerializeField] private GameObject traingleOrig;
    [SerializeField] private GameObject traingleSecond;



    private void Start()
    {
        modules[0].transform.position = new Vector3(0, 0, 0);
        modules[1].transform.position = new Vector3(0, 0, 0);
        modules[2].transform.position = new Vector3(0, 0, 0);
        modules[3].transform.position = new Vector3(0, 0, 0);
        traingleOrig.SetActive(true);
        traingleSecond.SetActive(false);
    }
    public void changePosition0()
    {
        modules[0].transform.position = new Vector3(0, 0, 0);
        modules[1].transform.position = new Vector3(0, 0, 0);
        modules[2].transform.position = new Vector3(0, 0, 0);
        modules[3].transform.position = new Vector3(0, 0, 0);
        traingleOrig.SetActive(true);
        traingleSecond.SetActive(false);
    }

    public void changePosition1()
    {
        modules[0].transform.position = new Vector3(0, 0, 0);
        modules[1].transform.position = new Vector3(0, 0, 0);

        modules[2].transform.position = new Vector3(147f, -0.315589905f, 0);
        modules[3].transform.position = new Vector3(-147f, 0, 0);
        traingleOrig.SetActive(false);
        traingleSecond.SetActive(true);
    }

    public void changePosition2()
    {
        modules[0].transform.position = new Vector3(0, 0, 0);
        modules[1].transform.position = new Vector3(99.7f, 0, 0);

        modules[2].transform.position = new Vector3(147f, -0.315589905f, 0);
        modules[3].transform.position = new Vector3(-194.05f, -0.1f, 0);
        traingleOrig.SetActive(false);
        traingleSecond.SetActive(true);
    }
}
