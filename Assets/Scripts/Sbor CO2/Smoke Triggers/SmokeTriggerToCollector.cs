using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTriggerToCollector : MonoBehaviour
{
    public string needTag = "";
    private ParticleSystem ps;
    public GameObject colliders;

    private bool hasTriggered = false;
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();

    }
    private void Update()
    {
        colliders = GameObject.FindGameObjectWithTag(needTag);
    }
    public void StartSmoke()
    {
        if (colliders != null)
        {
            ps.trigger.AddCollider(colliders.GetComponent<Collider>());
        }
    }

    [System.Obsolete]
    private void OnParticleTrigger()
    {
        Debug.Log("Smoke entered to trigger");
        colliders = GameObject.FindGameObjectWithTag(needTag);
        GameObject smokeFromDryAir = GameObject.FindGameObjectWithTag("SmokeDryAir");

        if (!hasTriggered)
        {
            smokeFromDryAir.GetComponent<ParticleSystem>().Play();
            hasTriggered = true;
        }
    }
}
