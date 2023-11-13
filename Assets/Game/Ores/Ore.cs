using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using System;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class Ore : FateMonoBehaviour, IDiggable, IVacuumable
{
    [SerializeField] int type = 0;
    [SerializeField] Gem gem = null;
    [SerializeField] float power = 1f;
    [SerializeField] float vacuumDestroyDistance = 0.1f;
    [SerializeField] float vacuumAcceleration = 5;
    [SerializeField] Material staticMeshMaterial, dynamicMeshMaterial;
    [SerializeField] DugRuntimeSet dugRuntimeSet = null;

    public int OreValue { get; private set; } = 1;

    Rigidbody rb;
    Collider staticCollider;
    Collider dynamicCollider;
    Collider trigger;
    MeshRenderer meshRenderer;
    bool isDug = false;

    Action onVacuumEnd;
    Action onGetDug = null;
    float vacuumStartTime = -1;

    private void Awake()
    {
        Collider[] colliders = GetComponents<Collider>();

        trigger = colliders[0];
        staticCollider = colliders[1];
        dynamicCollider = colliders[2];

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = staticMeshMaterial;
    }

    /*private void Start()
    {
        DOVirtual.DelayedCall(-transform.position.y/10 - 4, GetDug);
    }*/

    public int Type => type;

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

        CheckDugMember();
    }

    public void GetVacuumed(Vacuum vacuum, Action onVacuumEnd)
    {
        if (!isDug) return;
        rb.isKinematic = true;
        dynamicCollider.enabled = false;
        this.onVacuumEnd = onVacuumEnd;
        vacuumStartTime = Time.time;

        CheckDugMember();
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

    public bool IsRigidbodyKinematic => rb.isKinematic;

    public void SetRigidbodyKinematic(bool kinematic)
    {
        rb.isKinematic = kinematic;
        CheckDugMember();
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public bool IsDestroyed { get; private set; } = false;
    public void DestroyOre(bool withAnimation = false)
    {
        IsDestroyed = true;
        dynamicCollider.enabled = false;
        SetRigidbodyKinematic(true);

        if (withAnimation) transform.DOScale(0, 0.5f).SetEase(Ease.InCirc).OnComplete(() => Destroy(gameObject));
        else Destroy(gameObject);
    }

    public void AddOreValue(int value)
    {
        OreValue += value;
    }
    public Collider DynamicCollider => dynamicCollider;


    bool inRuntimeSet = false;
    private void CheckDugMember()
    {
        if (inRuntimeSet)
        {
            if (IsDestroyed || rb.isKinematic || !gameObject.activeInHierarchy || vacuumStartTime != -1)
            {
                dugRuntimeSet.Remove(this);
                inRuntimeSet = false;
            }
        }
        else
        {
            if (!IsDestroyed && !rb.isKinematic && gameObject.activeInHierarchy && isDug && vacuumStartTime == -1)
            {
                dugRuntimeSet.Add(this);
                inRuntimeSet = true;
            }
        }

        if (dugRuntimeSet.List.Contains(this) != inRuntimeSet)
            Debug.LogError(dugRuntimeSet.List.Contains(this) + " " + inRuntimeSet, this);
    }

    private void OnEnable()
    {
        CheckDugMember();
    }

    private void OnDisable()
    {
        CheckDugMember();
    }
}
