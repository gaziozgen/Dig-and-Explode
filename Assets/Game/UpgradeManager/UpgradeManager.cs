using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] IntVariable diggerLevelVariable;
    [SerializeField] int diggerLevel = 0;
    [SerializeField] IntVariable bombLevelVariable;
    [SerializeField] int bombLevel = 0;
    [SerializeField] IntVariable armSpeedLevelVariable;
    [SerializeField] int armSpeedLevel = 0;
    [SerializeField] IntVariable armCarryLevelVariable;
    [SerializeField] int armCarryLevel = 0;

    private void Start()
    {
        diggerLevelVariable.Value = diggerLevel;
        bombLevelVariable.Value = bombLevel;
    }

    public void UpgradeDiggerLevel()
    {
        diggerLevel++;
        diggerLevelVariable.Value = diggerLevel;
    }

    public void UpgradeBombLevel()
    {
        bombLevel++;
        bombLevelVariable.Value = bombLevel;
    }

    public void UpgradeArmSpeedLevel()
    {
        armSpeedLevel++;
        armSpeedLevelVariable.Value = armSpeedLevel;
    }

    public void UpgradeArmCarryLevel()
    {
        armCarryLevel++;
        armCarryLevelVariable.Value = armCarryLevel;
    }
}
