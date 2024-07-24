using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeFindTrigger : MonoBehaviour
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
            if (!hasTriggered)
            {
                Collider[] allChildColliders = other.GetComponentsInChildren<Collider>();
                other.GetComponent<CollectorScript>().isTrigger = true;
                foreach (Collider childCollider in allChildColliders)
                {
                    if (childCollider.CompareTag(_colliderTargetTag))
                    {
                        int i = childCollider.transform.GetChildCount();
                        for (int j = 0; j < i; j++)
                        {
                            childCollider.transform.GetChild(j).GetComponent<ParticleSystem>().Play();
                        }
                        hasTriggered = true;
                    }
                }
            }


        }
    }
}
