using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : FateMonoBehaviour, ISwerveMoveable
{
    [SerializeField] Transform vacuumCenterPoint;
    [SerializeField] float vacuumRadius = 0.2f;
    [SerializeField] LayerMask vacuumLayerMask = 0;
    [SerializeField] Transform head;
    [SerializeField] Rigidbody rigidbody;

    public Transform Head { get => head; }

    private void FixedUpdate()
    {
        VacuumAll();
        rigidbody.velocity = Vector3.zero;
    }

    public void VacuumAll()
    {
        Collider[] colliders = Physics.OverlapSphere(vacuumCenterPoint.position, vacuumRadius, vacuumLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            IVacuumable vacuumable = colliders[i].GetComponentInParent<IVacuumable>();
            if (vacuumable != null)
                vacuumable.GetVacuumed(this);

            else
            {
                Debug.Log(colliders[i].transform, colliders[i]);
            }
        }
    }

    public void Move(Swerve swerve)
    {
        float speed = Time.deltaTime * 0.5f * swerve.Rate;
        Vector3 pos = Vector3.MoveTowards(transform.position, transform.position + (Vector3)swerve.Direction, speed);
        /*pos.x = Mathf.Clamp(pos.x, -left, right);
        pos.y = Mathf.Clamp(pos.y, -bottom, top);*/
        transform.position = pos;
        transform.forward = (Vector3)swerve.Direction;
    }

    public void OnStable()
    {
        
    }
}
