using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTriggerForOven : MonoBehaviour
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
            GeneralManagerForElectroFilter parent = other.transform.GetComponent<GeneralManagerForElectroFilter>();
            if (parent != null)
            {
                parent.StartElectroFilterSystem();
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
