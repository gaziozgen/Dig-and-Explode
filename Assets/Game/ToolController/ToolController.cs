using FateGames.Core;
using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolController : FateMonoBehaviour
{
    [SerializeField] float baseSpeed;

    [SerializeField] LayerMask dugLayerMask;
    [SerializeField] float minimumReducedSpeedRatio = 0.2f;
    [SerializeField] float overlapRadius = 1;
    [SerializeField] int maxCollider = 40;
    [SerializeField] Rigidbody rb;
    [SerializeField] ToolSwitcher toolSwitcher;
    [SerializeField] GameObject rope;
    /*[Header("Borders")]
    [SerializeField] float right;
    [SerializeField] float left;
    [SerializeField] float top;
    [SerializeField] float bottom;*/

    public float SpeedMultiplier => 1 - Mathf.Clamp(numOverlapColliders / (float)maxCollider, 0, 1) * toolSwitcher.CurrentTool.SlowdownMultiplier() * (1 - minimumReducedSpeedRatio);

    bool isWorking = false;
    Collider[] overlapColliders;
    int numOverlapColliders = 0;

    private void Awake()
    {
        overlapColliders = new Collider[maxCollider];
        rope.SetActive(true);
    }

    private void FixedUpdate()
    {
        //Debug.Log(SpeedMultiplier);
        numOverlapColliders = Physics.OverlapSphereNonAlloc(transform.position, overlapRadius, overlapColliders, dugLayerMask, QueryTriggerInteraction.Ignore);
        if (isWorking) toolSwitcher.CurrentTool.OnWork();
        else toolSwitcher.CurrentTool.OnNotWork();
        rb.velocity = Vector3.zero;
    }

    public void Move(Swerve swerve)
    {
        float speed = Time.deltaTime * baseSpeed * swerve.Rate * SpeedMultiplier;
        /*Vector3 pos = Vector3.MoveTowards(transform.position, transform.position + (Vector3)swerve.Direction * speed, speed);
        *//*pos.x = Mathf.Clamp(pos.x, -left, right);
        pos.y = Mathf.Clamp(pos.y, -bottom, top);*//*
        transform.position = pos;*/
        rb.AddForce((Vector3)swerve.Direction * speed * rb.mass * 2500, ForceMode.Force);

    }

    public void SetWorking(bool working)
    {
        isWorking = working;
    }

    public Rigidbody GetRigidbody() { return rb; }
}