using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class Digger : FateMonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float rotationAcceleration = 10;
    [SerializeField] float maxRotationSpeed = 10;
    [SerializeField] Transform centerTransform;
    [SerializeField] Swerve swerve;
    [Header("Borders")]
    [SerializeField] float right;
    [SerializeField] float left;
    [SerializeField] float top;
    [SerializeField] float bottom;
    public bool isOnSwerve = false;

    public void EnableSwerve()
    {
        isOnSwerve = true;
    }

    public void DisableSwerve()
    {
        isOnSwerve = false;
    }

    private void FixedUpdate()
    {

        if (isOnSwerve)
        {
            Rotate();
            Move();
        }
        else
        {
            BrakeRotation();
        }
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Mathf.Clamp(rigidbody.angularVelocity.magnitude, 0, maxRotationSpeed) * new Vector3(Mathf.Abs(rigidbody.angularVelocity.normalized.x), Mathf.Abs(rigidbody.angularVelocity.normalized.y), Mathf.Abs(rigidbody.angularVelocity.normalized.z));
    }

    void Rotate()
    {
        //centerTransform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
        rigidbody.AddTorque(Vector3.forward * rotationAcceleration, ForceMode.Acceleration);
    }

    void BrakeRotation()
    {
        Vector3 angularVelocity = rigidbody.angularVelocity;
        angularVelocity = new Vector3(
            Mathf.Clamp(angularVelocity.x - Time.deltaTime * 10, 0, float.MaxValue),
            Mathf.Clamp(angularVelocity.y - Time.deltaTime * 10, 0, float.MaxValue),
            Mathf.Clamp(angularVelocity.z - Time.deltaTime * 10, 0, float.MaxValue));
        rigidbody.angularVelocity = angularVelocity;

    }

    void Move()
    {
        rigidbody.MovePosition(rigidbody.position + Time.deltaTime * 0.05f * swerve.Rate * rigidbody.angularVelocity.magnitude * (Vector3)swerve.Direction);
        Vector3 pos = rigidbody.position;
        pos.x = Mathf.Clamp(pos.x, -left, right);
        pos.y = Mathf.Clamp(pos.y, -bottom, top);
        rigidbody.position = pos;
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)swerve.Direction, Time.deltaTime * 0.2f);
    }
}
