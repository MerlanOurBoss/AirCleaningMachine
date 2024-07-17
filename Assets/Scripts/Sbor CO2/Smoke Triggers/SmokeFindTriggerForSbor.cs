using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeFindTriggerForSbor : MonoBehaviour
{
    [SerializeField] string _targetTag;

    private ParticleSystem ps;
    public GameObject colliders;
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(_targetTag))
        {
            ps.trigger.AddCollider(other.GetComponent<Collider>());
            Debug.Log("ddasdasd");
            CapsulScript parent = other.transform.GetComponent<CapsulScript>();
            if (parent != null)
            {
                parent.isTriggerFromOven = true;
            }
        }
    }
}
