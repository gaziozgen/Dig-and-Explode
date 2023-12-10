using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using System;

public class Mine : FateMonoBehaviour, IPooledObject
{
    public int Type = 0;
    public int Category = 0;
    [SerializeField] GameObject[] levels;
    [SerializeField] int baseValue = 10;

    [HideInInspector] public float Distance = 0;

    public int Level { get; private set; } = 0;

    public int Value => (Level+1) * baseValue;

    public Mine SetLevel(int newLevel)
    {
        levels[Level].SetActive(false);
        levels[newLevel].SetActive(true);
        Level = newLevel;
        return this;
    }

    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        Distance = 0;
        SetLevel(0);
    }

    public void OnRelease()
    {

    }
}
