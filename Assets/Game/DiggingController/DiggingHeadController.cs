using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingHeadController : MonoBehaviour
{
    [SerializeField] DiggingHeadSwitcher switcher;
    [SerializeField] Swerve swerve;
    bool isOnSwerve = false;

    private void Awake()
    {
        swerve.OnStart.AddListener((swerve) => EnableSwerve());
        swerve.OnRelease.AddListener((swerve) => DisableSwerve());
    }
    public void EnableSwerve()
    {
        Debug.Log("EnableSwerve", this);
        isOnSwerve = true;
    }

    public void DisableSwerve()
    {
        Debug.Log("DisableSwerve", this);
        isOnSwerve = false;
    }


    private void FixedUpdate()
    {
        if (isOnSwerve)
            MoveHead();
        else
            KeepStable();
    }

    public void MoveHead()
    {
        switcher.CurrentHead.Moveable.Move(swerve);
    }
    public void KeepStable()
    {
        switcher.CurrentHead.Moveable.OnStable();
    }
}
