using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
public class DiggingHead : FateMonoBehaviour
{
    [SerializeField] Transform centerTransform;

    public Transform CenterTransform { get => centerTransform; }
    public ISwerveMoveable Moveable { get; private set; }

    private void Awake()
    {
        Moveable = GetComponent<ISwerveMoveable>();
    }
}
