using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTriggerForElectro : MonoBehaviour
{
    [SerializeField] string _targetTag;
    [SerializeField] string _colliderTargetTag;

    private ParticleSystem ps;
    private bool hasTriggered = false;
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    [System.Obsolete]
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(_targetTag))
        {
            Debug.Log(other.name);
            GeneralManagerForKataz parent = other.transform.GetComponent<GeneralManagerForKataz>();
            TemperatureCatalizator parent1 = other.transform.GetComponent<TemperatureCatalizator>();

            if (parent != null)
            {
                parent1.StartSimulation();
            }
            if (!hasTriggered)
            {
                Collider[] allChildColliders = other.GetComponentsInChildren<Collider>();

                foreach (Collider childCollider in allChildColliders)
                {
                    if (childCollider.CompareTag(_colliderTargetTag))
                    {
                        childCollider.isTrigger = true;
                        parent.StartKatazSystem();
                        ps.trigger.AddCollider(childCollider);
                    }
                }
                hasTriggered = true;
            }
        }
    }
}
