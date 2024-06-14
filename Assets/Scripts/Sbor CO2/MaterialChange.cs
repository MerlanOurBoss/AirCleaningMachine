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
        SmokeTriggerToCollector sc = FindAnyObjectByType<SmokeTriggerToCollector>();
        SmokeTriggerToSteam ss = FindAnyObjectByType<SmokeTriggerToSteam>();
        SmokeTriggerToDryAir sda = FindAnyObjectByType<SmokeTriggerToDryAir>();

        sm.StartSmoke();
        so.StartSmoke();
        sc.StartSmoke();
        ss.StartSmoke();
        sda.StartSmoke();

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
