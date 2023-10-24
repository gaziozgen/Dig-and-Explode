using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Ore : FateMonoBehaviour, IDiggable, IVacuumable
{
    [SerializeField] Gem gem = null;
    [SerializeField] float power = 1f;
    [SerializeField] float vacuumDestroyDistance = 0.1f;
    [SerializeField] float vacuumAcceleration = 5;
    [SerializeField] Material staticMeshMaterial, dynamicMeshMaterial;

    Rigidbody rb;
    Collider staticCollider;
    Collider dynamicCollider;
    Collider trigger;
    MeshRenderer meshRenderer;
    bool isDug = false;

    Action onVacuumEnd;
    Action onGetDug = null;
    float vacuumStartTime;


    private void Awake()
    {
        Collider[] colliders = GetComponents<Collider>();

        trigger = colliders[0];
        staticCollider = colliders[1];
        dynamicCollider = colliders[2];

        /*for (int i = 0; i < 2; i++)
        {
            if (colliders[i].isTrigger) trigger = colliders[i];
            else staticCollider = colliders[i];
        }

        dynamicCollider = GetComponent<BoxCollider>();*/

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = staticMeshMaterial;
    }

    /*private void Update()
    {
        rb.ResetInertiaTensor();
        rb.angularVelocity = Vector3.forward * rb.angularVelocity.z;
    }*/

    public float Power() { return power; }

    public bool IsDug()
    {
        return isDug;
    }

    public void GetDug()
    {
        if (isDug) return;
        isDug = true;

        if (onGetDug != null) onGetDug();
        rb.isKinematic = false;
        meshRenderer.material = dynamicMeshMaterial;
        gameObject.layer = 9;
        trigger.enabled = false;
        staticCollider.enabled = false;
        dynamicCollider.enabled = true;
    }

    public void GetVacuumed(Vacuum vacuum, Action onVacuumEnd)
    {
        if (!isDug) return;
        rb.isKinematic = true;
        dynamicCollider.enabled = false;
        this.onVacuumEnd = onVacuumEnd;
        vacuumStartTime = Time.time;
    }

    public void OnVacuum(Vacuum vacuum)
    {
        Vector3 difference = vacuum.VacuumCenterPoint.position - transform.position;
        float differenceMagnitude = difference.magnitude;

        Vector3 targetPos = Vector3.MoveTowards(transform.position, vacuum.VacuumCenterPoint.position,
            vacuum.BaseVacuumSpeed * Time.deltaTime * Mathf.Pow(1 + Time.time - vacuumStartTime, vacuumAcceleration));
        transform.position = targetPos;
        if (differenceMagnitude <= vacuumDestroyDistance) FinishVacuum();
    }

    public void FinishVacuum()
    {
        onVacuumEnd();
        Destroy(gameObject);
    }

    public void SetOnGetDug(Action onGetDug)
    {
        this.onGetDug = onGetDug;
    }

    public Rigidbody GetRigidbody() { return rb; }
}
