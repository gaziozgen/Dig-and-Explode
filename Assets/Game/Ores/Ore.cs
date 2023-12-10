using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using System;
using DG.Tweening;

public class Ore : FateMonoBehaviour, IDiggable, IVacuumable
{
    [SerializeField] int type = 0;
    [SerializeField] Gem gem = null;
    [SerializeField] float power = 1f;
    [SerializeField] float vacuumDestroyDistance = 0.1f;
    [SerializeField] float vacuumAcceleration = 5;
    [SerializeField] Material /*staticMeshMaterial, */dynamicMeshMaterial = null;
    [SerializeField] DugRuntimeSet dugRuntimeSet = null;

    [SerializeField] Rigidbody rb;
    [SerializeField] MeshCollider trigger, staticCollider;
    [SerializeField] BoxCollider dynamicCollider;
    [SerializeField] MeshRenderer meshRenderer;

    public int Type => type;
    public int OreValue { get; private set; } = 1;
    public bool IsDestroyed => isDestroyed;
    public Collider DynamicCollider => dynamicCollider;
    public Rigidbody Rigidbody => rb;
    public bool IsRigidbodyKinematic => rb.isKinematic;

    bool isDug = false;
    bool isDestroyed = false;
    float vacuumStartTime = -1;
    float posX = 0;
    float posY = 0;
    float angleZ = 0;
    Action onVacuumEnd;
    Action onGetDug = null;

    private void OnValidate()
    {
        if (!Application.isPlaying && !rb)
        {
            gameObject.layer = 8;

            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.angularDrag = 1;
            rb.automaticInertiaTensor = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            trigger = gameObject.AddComponent<MeshCollider>();
            trigger.convex = true;
            trigger.isTrigger = true;
            trigger.cookingOptions = MeshColliderCookingOptions.None;

            staticCollider = gameObject.AddComponent<MeshCollider>();
            staticCollider.convex = true;
            staticCollider.cookingOptions = MeshColliderCookingOptions.None;

            dynamicCollider = gameObject.AddComponent<BoxCollider>();
            dynamicCollider.enabled = false;
            if (!gem) dynamicCollider.size = new Vector3(0.05f, 0.05f, 0.1f);

            meshRenderer = GetComponentInChildren<MeshRenderer>();

            Saveable saveable = GetComponent<Saveable>();
            if (saveable != null) saveable.target = this;
        }
    }

    private void Start()
    {
        LoadState();
        CheckDugMember();
        //DOVirtual.DelayedCall(-transform.position.y / 10 - 4, GetDug);
    }

    public float Power() { return power; }

    public bool IsDug()
    {
        return isDug;
    }

    public void GetDug(bool checkDug = true)
    {
        if (checkDug && isDug) return;
        isDug = true;

        if (onGetDug != null) onGetDug();
        rb.isKinematic = false;
        if (dynamicMeshMaterial != null)  meshRenderer.material = dynamicMeshMaterial;
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
        isDestroyed = true;
        gameObject.SetActive(false);
    }

    public void SetOnGetDug(Action onGetDug)
    {
        this.onGetDug = onGetDug;
    }

    public void SetRigidbodyKinematic(bool kinematic)
    {
        rb.isKinematic = kinematic;
        CheckDugMember();
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void DestroyOre(bool withAnimation = false)
    {
        isDestroyed = true;
        dynamicCollider.enabled = false;
        SetRigidbodyKinematic(true);

        if (withAnimation) transform.DOScale(0, 0.5f).SetEase(Ease.InCirc).OnComplete(() => gameObject.SetActive(false));
        else gameObject.SetActive(false);
    }

    public void AddOreValue(int value)
    {
        OreValue += value;
    }


    bool inRuntimeSet = false;
    private void CheckDugMember()
    {
        if (inRuntimeSet)
        {
            if (isDestroyed || rb.isKinematic || !gameObject.activeInHierarchy || vacuumStartTime != -1)
            {
                dugRuntimeSet.Remove(this);
                inRuntimeSet = false;
            }
        }
        else
        {
            if (!isDestroyed && !rb.isKinematic && gameObject.activeInHierarchy && isDug && vacuumStartTime == -1)
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
        SavePos();
    }

    private void LoadState()
    {
        if (isDestroyed || vacuumStartTime != -1)
        {
            Destroy(gameObject);
            return;
        }
        else if (isDug)
        {
            transform.position = new Vector3(posX, posY, 0);
            transform.eulerAngles = Vector3.forward * angleZ;
            GetDug(false);
        }
    }

    public void SavePos()
    {
        if (isDestroyed || vacuumStartTime != -1 || !isDug) return;

        posX = transform.position.x;
        posY = transform.position.y;
        angleZ = transform.eulerAngles.z;
    }
}
