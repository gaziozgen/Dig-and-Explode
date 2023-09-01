using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
public class UIElement : FateMonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TransformRuntimeSet UIRuntimeSet;
    [SerializeField] bool addToUIRuntimeSet = false;
    public virtual void Show()
    {
        canvas.enabled = true;
        if (addToUIRuntimeSet) UIRuntimeSet.Add(transform);
    }
    public virtual void Hide()
    {
        canvas.enabled = false;
        if (addToUIRuntimeSet) UIRuntimeSet.Remove(transform);
    }
    protected virtual void Reset()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    private void OnDisable()
    {
        if (addToUIRuntimeSet) UIRuntimeSet.Remove(transform);
    }
}
