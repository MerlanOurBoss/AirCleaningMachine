using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForKlapon : MonoBehaviour
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
            KlaponManager parent = other.transform.GetComponent<KlaponManager>();
            if (!hasTriggered )
            {
                Collider[] allChildColliders = other.transform.GetComponentsInChildren<Collider>();

                foreach (Collider childCollider in allChildColliders)
                {
                    if (childCollider.CompareTag(_colliderTargetTag))
                    {
                        var triggerModule = ps.trigger;
                        childCollider.isTrigger = true;
                        ps.trigger.AddCollider(childCollider);
                        childCollider.enabled = false;
                        parent.OpenKlapon();
                        triggerModule.enter = ParticleSystemOverlapAction.Kill;
                    }
                    hasTriggered = true;
                }


            }
        }
    }
}
