using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmokeFindTriggerForEmul : MonoBehaviour
{
    [SerializeField] string _targetTag;
    [SerializeField] string _colliderTargetTag;

    private ParticleSystem ps;
    private bool hasTriggered = false;
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(_targetTag))
        {
            GeneralManagerForEmul parent = other.transform.GetComponent<GeneralManagerForEmul>();
            if (parent != null)
            {
                parent.StartEmulSystem();
            }
            if (!hasTriggered)
            {
                Collider[] allChildColliders = other.GetComponentsInChildren<Collider>();

                foreach (Collider childCollider in allChildColliders)
                {
                    if (childCollider.CompareTag(_colliderTargetTag))
                    {
                        childCollider.isTrigger = true;
                        ps.trigger.AddCollider(childCollider);
                    }
                }
                hasTriggered = true;
            }


        }
    }
}
