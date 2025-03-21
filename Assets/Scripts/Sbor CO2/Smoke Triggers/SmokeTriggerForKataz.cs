using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTriggerForKataz : MonoBehaviour
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
            GeneralManagerForCooling parent = other.transform.GetComponent<GeneralManagerForCooling>();

            if (parent != null)
            {
                parent.StartCoolingSystem();
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
