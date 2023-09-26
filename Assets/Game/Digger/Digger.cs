using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class Digger : FateMonoBehaviour, ISwerveMoveable
{
    [SerializeField] LayerMask diggableLayerMask;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float rotationAcceleration = 10;
    [SerializeField] float maxRotationSpeed = 10;
    [SerializeField] int maxCollider = 40;
    [SerializeField] Transform centerTransform;
    [Header("Borders")]
    [SerializeField] float right;
    [SerializeField] float left;
    [SerializeField] float top;
    [SerializeField] float bottom;
    float rotationVelocity = 0;

    

    private void Update()
    {
        Rotate();
    }

    int count = 0;
    private void FixedUpdate()
    {
        rigidbody.velocity = Vector3.zero;
        count = 0;
        Collider[] colliders = Physics.OverlapSphere(centerTransform.position, 2, diggableLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Ore ore = colliders[i].GetComponent<Ore>();
            if (ore && ore.IsDug)
            {
                Vector3 difference = centerTransform.position - ore.transform.position;
                float distance = difference.magnitude;
                //ore.Rigidbody.AddForce(difference.normalized * rotationVelocity / difference.sqrMagnitude / 10f, ForceMode.Force);
                if (distance <= 0.2f)
                    count++;
            }
        }
        Debug.Log(count);

    }

    void AccelerateRotation()
    {
        rotationVelocity = Mathf.Clamp(rotationVelocity + Time.fixedDeltaTime * 10, 0, maxRotationSpeed);
    }

    void Rotate()
    {
        float rotationVelocity = this.rotationVelocity * (1 - Mathf.Clamp(count / (float)maxCollider, 0, 1) * 0.8f);
        centerTransform.rotation = Quaternion.Euler(centerTransform.rotation.eulerAngles + rotationAcceleration * Time.deltaTime * rotationVelocity * Vector3.back);

    }

    void BrakeRotation()
    {
        rotationVelocity = Mathf.Clamp(rotationVelocity - Time.deltaTime * 10, 0, maxRotationSpeed);

    }

    public void Move(Swerve swerve)
    {
        AccelerateRotation();
        float speed = Time.deltaTime * 0.5f * swerve.Rate;
        speed *= 1 - Mathf.Clamp(count / (float)maxCollider, 0, 1) * 0.8f;
        Vector3 pos = Vector3.MoveTowards(centerTransform.position, centerTransform.position + (Vector3)swerve.Direction, speed);
        pos.x = Mathf.Clamp(pos.x, -left, right);
        pos.y = Mathf.Clamp(pos.y, -bottom, top);
        centerTransform.position = pos;
    }

    public void OnStable()
    {
        BrakeRotation();
    }
}
