using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Digger : Tool
{
    [SerializeField] List<DiggerLevel> levels;
    public EffectPool DigEffectPool;
    [SerializeField] EffectPool recoilEffectPool;
    [SerializeField] IntVariable level;
    public SoundEntity DigSound;
    public GameEvent onDigged;
    //public SoundEntity DugSound;
    [SerializeField] float pushPowerMultiplier = 1;
    [SerializeField] float basePower = 1;
    [SerializeField] float powerIncreasePerLevel = 1;
    [SerializeField] float slowdownDecreasePerLevel = 0.1f;
    [SerializeField] float rotationAccelerationDuration = 2;
    [SerializeField] float baseMaxRotationSpeed = 10;
    [SerializeField] float maxRotationSpeedIncreasePerLevel = 50;
    [SerializeField] float giantDiggerDuration = 30;
    [SerializeField] Transform rotateParent;
    [SerializeField] Slider giantSlider;
    [SerializeField] GameObject normalLevels;
    [SerializeField] GameObject giantLevel;
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

    private void Update()
    {
        if (InGiantMode) UpdateGiantSlider();
    }

    #region Giant Digger

    bool InGiantMode => remainingGiantTime > 0;
    Action onGiantTimeUP = null;
    float remainingGiantTime = 0;

    public void SwichGiantDigger(Action callBack)
    {
        onGiantTimeUP = callBack;
        remainingGiantTime = giantDiggerDuration;

        giantSlider.gameObject.SetActive(true);
        giantLevel.SetActive(true);
        normalLevels.SetActive(false);
    }

    private void UpdateGiantSlider()
    {
        remainingGiantTime -= Time.deltaTime;

        if (remainingGiantTime > 0) giantSlider.value = remainingGiantTime / giantDiggerDuration;
        else
        {
            onGiantTimeUP.Invoke();
            giantSlider.gameObject.SetActive(false);
            giantLevel.SetActive(false);
            normalLevels.SetActive(true);
        }
    }
    #endregion

    public override float SlowdownMultiplier()
    {
        return InGiantMode ? 0 : Mathf.Pow(1 - slowdownDecreasePerLevel, level.Value);
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

    public float Power() { return InGiantMode ? 100 : (basePower + level.Value * powerIncreasePerLevel); }

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
        rotateParent.rotation = Quaternion.Euler(rotateParent.rotation.eulerAngles + Time.deltaTime * finalRotationVelocity * Vector3.back);
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
