using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour
{
    public Material originalMaterial;
    public Material changeMaterial;
    public void ChangeColor()
    {
        
        GameObject smoke = GameObject.FindGameObjectWithTag("StartSmoke");

        SmokeFindTrigger sm = FindAnyObjectByType<SmokeFindTrigger>();
        SmokeTriggerForOven so = FindAnyObjectByType<SmokeTriggerForOven>();
        SmokeTriggerForElectro el = FindAnyObjectByType<SmokeTriggerForElectro>();
        SmokeTriggerForKataz kt = FindAnyObjectByType<SmokeTriggerForKataz>();
        SmokeTriggerForCooling coo = FindAnyObjectByType<SmokeTriggerForCooling>();
        SmokeTriggerToCollector sc = FindAnyObjectByType<SmokeTriggerToCollector>();
        SmokeTriggerToSteam ss = FindAnyObjectByType<SmokeTriggerToSteam>();
        SmokeTriggerToDryAir sda = FindAnyObjectByType<SmokeTriggerToDryAir>();

        if (coo != null)
        {
            coo.StartSmoke();
        }

        if (sc != null)
        {
            sc.StartSmoke();
        }

        smoke.GetComponent<ParticleSystem>().Play();
        MoveObjectWithMouse[] moveScript = FindObjectsOfType<MoveObjectWithMouse>();
        foreach (MoveObjectWithMouse move in moveScript)
        {
            move.gameObject.GetComponent<Renderer>().material = changeMaterial;
        }
        GameObject[] vfx = GameObject.FindGameObjectsWithTag("VFX");
        foreach (GameObject obj in vfx)
        {
            obj.GetComponent<ParticleSystem>().Play();
        }
    }

    public void BackColor()
    {
        MoveObjectWithMouse[] moveScript = FindObjectsOfType<MoveObjectWithMouse>();
        GameObject smoke = GameObject.FindGameObjectWithTag("StartSmoke");
        GameObject[] vfx = GameObject.FindGameObjectsWithTag("VFX");
        smoke.GetComponent<ParticleSystem>().Stop();
        foreach (MoveObjectWithMouse move in moveScript)
        {
            move.gameObject.GetComponent<Renderer>().material = originalMaterial;
        }
        foreach (GameObject obj in vfx)
        {
            obj.GetComponent<ParticleSystem>().Stop();
        }
    }
}
