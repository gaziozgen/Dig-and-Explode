using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Player : FateMonoBehaviour
{
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float rotateSpeed = 10;
    [SerializeField] float animationSpeedMultiplier = 1;
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform upperCamTransform;
    [SerializeField] BoolVariable isOnUpperArea;
    [SerializeField] Animator animator;

    bool locked = false;
    public void SetLocked(bool isLocked)
    {
        locked = isLocked;
        if (isLocked) Stop();
    }

    public void MoveBySwerve(Swerve swerve)
    {
        if (!isOnUpperArea.Value || locked) return;
        //float swerveXMultiplier = 0.1f;

        Vector3 direction = Quaternion.Euler(0, upperCamTransform.eulerAngles.y, 0) * new Vector3(swerve.Direction.x/* * swerveXMultiplier*/, 0, swerve.Direction.y).normalized;
        if (direction != Vector3.zero)
        {
            characterController.SimpleMove(direction * moveSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotateSpeed);
            animator.SetFloat("Speed", moveSpeed * animationSpeedMultiplier);
        }
    }

    public void Stop()
    {
        animator.SetFloat("Speed", 0);
    }
}
