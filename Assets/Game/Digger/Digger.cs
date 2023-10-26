using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Digger : Tool
{
    [SerializeField] List<DiggerLevel> levels;
    public EffectPool EffectPool;
    [SerializeField] int level = 0;
    [SerializeField] float basePower = 1;
    [SerializeField] float powerIncreasePerLevel = 1;
    [SerializeField] float slowdownDereasePerLevel = 0.1f;
    [SerializeField] float rotationAccelerationDuration = 2;
    [SerializeField] float baseMaxRotationSpeed = 10;
    [SerializeField] float maxRotationSpeedIncreasePerLevel = 50;

    public ToolController ToolController;

    public bool Working { get; private set; }

    float rotationVelocity = 0;

    private float MaxSpeed => baseMaxRotationSpeed + level * maxRotationSpeedIncreasePerLevel;

    private void Start()
    {
        SetLevel(level);
    }

    public override float SlowdownMultiplier()
    {
        return Mathf.Pow(1 - slowdownDereasePerLevel, level);
    }

    public void levelUp()
    {
        if (level == levels.Count) return;
        print("LEVEL UP");
        levels[level].Apply();
        level++;
    }

    public void SetLevel(int level)
    {
        this.level = level;
        for (int i = 0; i < level; i++)
            levels[i].Apply();
    }

    public float Power() { return basePower + level * powerIncreasePerLevel; }

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
        float finalRotationVelocity = rotationVelocity * ToolController.SpeedMultiplier;
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
