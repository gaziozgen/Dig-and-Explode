using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Tool: FateMonoBehaviour
{
    public abstract float SlowdownMultiplier();

    public abstract void OnWork();

    public abstract void OnNotWork();

    public abstract void OnSelect();

    public abstract void OnDeselect();
}
