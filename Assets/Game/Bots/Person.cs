using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : FateMonoBehaviour
{
    public Agent Agent;
    [HideInInspector] public Coroutine Coroutine;
}
