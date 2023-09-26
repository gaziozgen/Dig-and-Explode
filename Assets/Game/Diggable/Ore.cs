using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Ore : FateMonoBehaviour, IDiggable, IVacuumable
{
    Rigidbody rigidbody;
    MeshRenderer meshRenderer;
    [SerializeField] Material staticMeshMaterial, dynamicMeshMaterial;
    [SerializeField] Collider staticCollider;
    bool isDug = false;

    public bool IsDug { get => isDug; }
    public Rigidbody Rigidbody { get => rigidbody; }

    private void Awake()
    {
        InitializeRigidbody();
        InitializeCollider();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = staticMeshMaterial;
    }

    private void InitializeRigidbody()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    private void InitializeCollider()
    {
        staticCollider.isTrigger = true;
        staticCollider.enabled = true;
    }

    public void GetDug()
    {
        if (isDug) return;
        rigidbody.isKinematic = false;
        //rigidbody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
        staticCollider.enabled = false;
        meshRenderer.material = dynamicMeshMaterial;
        isDug = true;
    }

    public void GetVacuumed(Vacuum vacuum)
    {
        if (!isDug) return;
        Vector3 difference = (vacuum.Head.position - transform.position);
        Vector3 pos = Vector3.MoveTowards(rigidbody.position, rigidbody.position + 0.05f * Time.deltaTime / difference.sqrMagnitude * difference.normalized, difference.magnitude);
        rigidbody.MovePosition(pos);
        if ((rigidbody.position - vacuum.Head.position).magnitude <= 0.05f)
            Deactivate();
    }
}
