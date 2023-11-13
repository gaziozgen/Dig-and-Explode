using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    [SerializeField] GameObject[] areas;
    [SerializeField] int lastAreaIdex = 1;
    [SerializeField] GameObject upperCamera;
    [SerializeField] BoolVariable isOnUpperArea;

    private void Awake()
    {
        isOnUpperArea.Value = true;
    }

    public void SetArea(int newAreaIndex)
    {
        if (lastAreaIdex != 0) areas[lastAreaIdex].SetActive(false);
        lastAreaIdex = newAreaIndex;
        areas[lastAreaIdex].SetActive(true);
        upperCamera.SetActive(lastAreaIdex == 0);
        isOnUpperArea.Value = lastAreaIdex == 0;
    }
}
