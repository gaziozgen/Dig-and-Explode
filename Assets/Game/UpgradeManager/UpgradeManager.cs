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
    [SerializeField] IntVariable bombCapacityLevelVariable;
    [SerializeField] int bombCapacityLevel = 0;
    [SerializeField] IntVariable ropeLengthLevelVariable;
    [SerializeField] int ropeLengthLevel = 0;
    [SerializeField] IntVariable armSpeedLevelVariable;
    [SerializeField] int armSpeedLevel = 0;
    [SerializeField] IntVariable armCarryLevelVariable;
    [SerializeField] int armCarryLevel = 0;
    [SerializeField] IntVariable beltSpeedLevelVariable;
    [SerializeField] int beltSpeedLevel = 0;
    [SerializeField] IntVariable belt1LevelVariable;
    [SerializeField] int belt1Level = 0;
    [SerializeField] IntVariable belt2LevelVariable;
    [SerializeField] int belt2Level = 0;
    [SerializeField] IntVariable belt3LevelVariable;
    [SerializeField] int belt3Level = 0;

    private void Start()
    {
        diggerLevelVariable.Value = diggerLevel;
        bombLevelVariable.Value = bombLevel;
        bombCapacityLevelVariable.Value = bombCapacityLevel;
        ropeLengthLevelVariable.Value = ropeLengthLevel;
        armSpeedLevelVariable.Value = armSpeedLevel;
        armCarryLevelVariable.Value = armCarryLevel;
        beltSpeedLevelVariable.Value = beltSpeedLevel;
        belt1LevelVariable.Value = belt1Level;
        belt2LevelVariable.Value = belt2Level;
        belt3LevelVariable.Value = belt3Level;
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

    /*public void UpgradeBombCapacityLevel()
    {
        bombCapacityLevel++;
        bombCapacityLevelVariable.Value = bombCapacityLevel;
    }*/
    
    public void UpgradeRopeLengthLevel()
    {
        ropeLengthLevel++;
        ropeLengthLevelVariable.Value = ropeLengthLevel;
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

    public void UpgradeBeltSpeedLevel()
    {
        beltSpeedLevel++;
        beltSpeedLevelVariable.Value = beltSpeedLevel;
    }

    public void UpgradeBelt1Level()
    {
        belt1Level++;
        belt1LevelVariable.Value = belt1Level;
    }

    public void UpgradeBelt2Level()
    {
        belt2Level++;
        belt2LevelVariable.Value = belt2Level;
    }

    public void UpgradeBelt3Level()
    {
        belt3Level++;
        belt3LevelVariable.Value = belt3Level;
    }
}
