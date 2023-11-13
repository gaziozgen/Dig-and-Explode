using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVacuumable
{
    public int Type { get; }

    public int OreValue { get; }

    public void GetVacuumed(Vacuum vacuum, Action onVacuumEnd);

    public void OnVacuum(Vacuum vacuum);

    public void FinishVacuum();
}
