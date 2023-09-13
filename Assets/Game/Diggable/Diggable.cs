using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Diggable : FateMonoBehaviour
{
    Rigidbody rigidbody;
    [SerializeField] GameObject staticMeshObject, dynamicMeshObject;
    [SerializeField] Collider staticCollider;
    [SerializeField] Collider dynamicCollider;
    bool isDug = false;

    private void Awake()
    {
        InitializeRigidbody();
        InitializeCollider();
        dynamicMeshObject.SetActive(false);
        staticMeshObject.SetActive(true);
    }

    private void InitializeRigidbody()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    private void InitializeCollider()
    {
        dynamicCollider.isTrigger = false;
        staticCollider.isTrigger = true;
        dynamicCollider.enabled = false;
        staticCollider.enabled = true;
    }

    public void GetDug()
    {
        if (isDug) return;
        rigidbody.isKinematic = false;
        //rigidbody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
        dynamicCollider.enabled = true;
        staticCollider.enabled = false;
        dynamicMeshObject.SetActive(true);
        staticMeshObject.SetActive(false);
        isDug = true;
    }
}
