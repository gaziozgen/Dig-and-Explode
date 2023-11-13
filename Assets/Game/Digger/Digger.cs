using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digger : Tool
{
    [SerializeField] List<DiggerLevel> levels;
    public EffectPool DigEffectPool;
    [SerializeField] EffectPool recoilEffectPool;
    [SerializeField] IntVariable level;
    [SerializeField] float pushPowerMultiplier = 1;
    [SerializeField] float basePower = 1;
    [SerializeField] float powerIncreasePerLevel = 1;
    [SerializeField] float slowdownDereasePerLevel = 0.1f;
    [SerializeField] float rotationAccelerationDuration = 2;
    [SerializeField] float baseMaxRotationSpeed = 10;
    [SerializeField] float maxRotationSpeedIncreasePerLevel = 50;
    [SerializeField] ToolController ToolController;

    public bool Working { get; private set; }

    int lastLevel = 0;
    float rotationVelocity = 0;
    float finalRotationVelocity;

    private float MaxSpeed => baseMaxRotationSpeed + level.Value * maxRotationSpeedIncreasePerLevel;

    private void OnEnable()
    {
        UpdateLevel();
    }

    public override float SlowdownMultiplier()
    {
        return Mathf.Pow(1 - slowdownDereasePerLevel, level.Value);
    }

    public float PushPower()
    {
        return finalRotationVelocity * pushPowerMultiplier;
    }

    public void UpdateLevel()
    {
        if (lastLevel < level.Value)
        {
            int dif = level.Value - lastLevel;
            for (int i = 0; i < dif; i++)
                levels[i].Apply();
            lastLevel = level.Value;
        }
    }

    public void Recoil(Vector3 direction, Vector3 pos)
    {
        recoilEffectPool.Get().transform.position = pos;
        ToolController.Recoil(direction);
    }

    public float Power() { return basePower + level.Value * powerIncreasePerLevel; }

    void AccelerateRotation()
    {
        rotationVelocity = Mathf.Clamp(rotationVelocity + Time.deltaTime * MaxSpeed / rotationAccelerationDuration, 0, MaxSpeed);
    }

    void BrakeRotation()
    {
        rotationVelocity = Mathf.Clamp(rotationVelocity - Time.deltaTime * MaxSpeed / rotationAccelerationDuration, 0, MaxSpeed);
    }

    void Rotate()
    {
        finalRotationVelocity = rotationVelocity * ToolController.SpeedMultiplier;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Time.deltaTime * finalRotationVelocity * Vector3.back);
    }

    public override void OnWork()
    {
        if (!Working) Working = true;
        AccelerateRotation();
        Rotate();
    }

    public override void OnNotWork()
    {
        if (Working) Working = false;
        BrakeRotation();
        Rotate();
    }

    public override void OnSelect()
    {
        Activate();
        rotationVelocity = 0;
    }

    public override void OnDeselect()
    {
        Deactivate();
    }
}

[Serializable]
public class DiggerLevel
{
    public List<GameObject> deactivateList;
    public List<GameObject> activateList;

    public void Apply()
    {
        for (int i = 0; i < deactivateList.Count; i++)
            deactivateList[i].SetActive(false);
        for (int i = 0; i < activateList.Count; i++)
            activateList[i].SetActive(true);
    }
}
