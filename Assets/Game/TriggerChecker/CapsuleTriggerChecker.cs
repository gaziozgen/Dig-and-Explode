using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class CapsuleTriggerChecker : FateMonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float height = 2, radius = 0.5f;
    [SerializeField] int maxColliders = 1;
    Collider[] hitColliders;

    private void Awake()
    {
        hitColliders = new Collider[maxColliders];
    }

    public int CheckTrigger(out Collider[] hitColliders)
    {
        hitColliders = this.hitColliders;
        for (int i = 0; i < hitColliders.Length; i++)
        {
            hitColliders[i] = null;
        }
        Vector3 point0 = transform.position + radius * Vector3.up;
        Vector3 point1 = transform.position + (height - radius) * Vector3.up;
        int numColliders = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, hitColliders, layerMask, QueryTriggerInteraction.Collide);
        return numColliders;
    }
}
