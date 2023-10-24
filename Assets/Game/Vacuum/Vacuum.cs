using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vacuum : Tool
{
    [SerializeField] float rotateSpeed = 5f;
    [SerializeField] ToolController toolController;
    [SerializeField] Rope rope;
    [SerializeField] ParticleSystem vacuumEffect;
    [SerializeField] Animator animator;
    [SerializeField] Swerve swerve;
    [SerializeField] Transform vacuumAreaCenterPoint;
    [SerializeField] Transform vacuumCenterPoint;
    [SerializeField] float vacuumRadius = 1f;
    [SerializeField] float baseVacuumSpeed = 1;
    [SerializeField] int maxColliders = 50;
    [SerializeField] LayerMask vacuumLayerMask = 0;
    Collider[] overlapColliders;

    public Transform VacuumCenterPoint { get => vacuumCenterPoint; }
    public float BaseVacuumSpeed { get => baseVacuumSpeed; }

    bool working = false;
    Vector2 currentRotation = Vector2.down;
    List<IVacuumable> vacuumables = new List<IVacuumable>();

    private void Awake()
    {
        overlapColliders = new Collider[maxColliders];
    }

    private void FixedUpdate()
    {
        if (working) VacuumAll();
        for (int i = 0; i < vacuumables.Count; i++)
            vacuumables[i].OnVacuum(this);
    }

    private void OnDisable()
    {
        for (int i = vacuumables.Count - 1; i >= 0; i--)
            vacuumables[i].FinishVacuum();
    }

    public void VacuumAll()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(vacuumAreaCenterPoint.position, vacuumRadius, overlapColliders, vacuumLayerMask);
        for (int i = 0; i < numColliders; i++)
        {
            IVacuumable vacuumable = overlapColliders[i].GetComponent<IVacuumable>();
            if (vacuumable != null)
            {
                vacuumables.Add(vacuumable);
                vacuumable.GetVacuumed(this, () =>
                {
                    vacuumables.Remove(vacuumable);
                    rope.AddWave();
                });
            }

        }
    }

    public void ChangeRotation()
    {
        currentRotation = Vector2.MoveTowards(currentRotation, swerve.Direction, rotateSpeed * Time.deltaTime);
        if (swerve.Direction != Vector2.zero) transform.localEulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.down, currentRotation);
    }

    public override void OnWork()
    {
        ChangeRotation();
        if (!working)
        {
            working = true;
            vacuumEffect.gameObject.SetActive(true);
            animator.SetBool("work", true);
        }
    }

    public override void OnNotWork()
    {
        if (working)
        {
            working = false;
            vacuumEffect.gameObject.SetActive(false);
            animator.SetBool("work", false);
        }
    }

    public override void OnSelect()
    {
        Activate();
    }

    public override void OnDeselect()
    {
        Deactivate();
    }

    public override float SlowdownMultiplier()
    {
        return 1;
    }
}
