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
        GameObject streamSmoke = GameObject.FindGameObjectWithTag("SmokeStream");
        SmokeTriggerForCooling coo = FindAnyObjectByType<SmokeTriggerForCooling>();
        SmokeTriggerToCollector sc = FindAnyObjectByType<SmokeTriggerToCollector>();

        if (coo != null)
        {
            coo.StartSmoke();
        }

        if (sc != null)
        {
            sc.StartSmoke();
        }

        if (streamSmoke != null)
        {
            streamSmoke.GetComponent<ParticleSystem>().Play();
        }

        if (smoke != null)
        {
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
    }

    public void BackColor()
    {
        GameObject smoke = GameObject.FindGameObjectWithTag("StartSmoke");
        if (smoke != null)
        {
            MoveObjectWithMouse[] moveScript = FindObjectsOfType<MoveObjectWithMouse>();
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
}
