using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SxemaSelectSborCO2 : MonoBehaviour
{
    [SerializeField] private Animator _sborSelectAnim;

    [SerializeField] private AborberPercent[] aborberPercents;

    public GameObject child;

    private void Start()
    {
        child.SetActive(false);
    }
    public void OnMouseSelect()
    {
        child.SetActive(true);
        _sborSelectAnim.Play("LineZeroSelectSbor");
    }

    public void OnMouseDiselected()
    {
        child.SetActive(false);
        _sborSelectAnim.Play("LineStopSelectSbor");
    }
    public void StartAbsorber(int i)
    {
        aborberPercents[i].startProcess = true;
        aborberPercents[i].backProcess = false;
        aborberPercents[i].stopProcess = false;
    }

    public void StopAbsorber(int i)
    {
        aborberPercents[i].backProcess = true;
        aborberPercents[i].startProcess = false;

        aborberPercents[i].stopProcess = false;
    }

    public void ImmediatelyStopAbsorbers()
    {
        aborberPercents[0].stopProcess = true;
        aborberPercents[0].backProcess = false;
        aborberPercents[0].startProcess = false;

        aborberPercents[1].stopProcess = true;
        aborberPercents[1].backProcess = false;
        aborberPercents[1].startProcess = false;
    }
}
