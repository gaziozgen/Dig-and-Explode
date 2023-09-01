using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RemoteConfig<T> : ScriptableObject
{
    [SerializeField] public string id = "";
    [SerializeField] public T defaultValue;
}
