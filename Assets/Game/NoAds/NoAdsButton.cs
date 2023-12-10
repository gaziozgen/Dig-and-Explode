using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class NoAdsButton : MonoBehaviour
{
    [SerializeField] UIElement uIElement;

    private void Start()
    {
        if (AdManager.Disabled) uIElement.Hide();
    }
}
