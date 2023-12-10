using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaController : MonoBehaviour
{
    [SerializeField] GameObject[] areas;
    [SerializeField] int lastAreaIdex = 1;
    [SerializeField] GameObject upperCamera;
    [SerializeField] BoolVariable isOnUpperArea;
    [SerializeField] UnityEvent onUpperAreaOpen;
    [SerializeField] UnityEvent onUpperAreaClose;

    private void Awake()
    {
        isOnUpperArea.Value = true;
    }

    public void SetArea(int newAreaIndex)
    {
        if (lastAreaIdex == 0) onUpperAreaClose.Invoke();
        else
        {
            areas[lastAreaIdex].SetActive(false);
            onUpperAreaOpen.Invoke();
        }
        lastAreaIdex = newAreaIndex;
        areas[lastAreaIdex].SetActive(true);
        upperCamera.SetActive(lastAreaIdex == 0);
        isOnUpperArea.Value = lastAreaIdex == 0;
        AdManager.Instance.ShowInterstitial();
    }
}
