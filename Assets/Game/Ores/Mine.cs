using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using System;

public class Mine : FateMonoBehaviour, IPooledObject
{
    public Action Release { get; set; }

    public void OnObjectSpawn()
    {

    }

    public void OnRelease()
    {

    }
}
