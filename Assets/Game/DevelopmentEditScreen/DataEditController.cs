using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DataEditController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText = null;
    [SerializeField] UnityEvent onUIShow = null;
    [SerializeField] UnityEvent onUIHide = null;

    bool isUIHided = false;

    public void SetMoney()
    {
        MoneyUI.Instance.SetMoneyFromDevMode(moneyText.text);
    }

    public void ToggleUI()
    {
        if (isUIHided) onUIShow.Invoke();
        else onUIHide.Invoke();
        isUIHided = !isUIHided;
    }

    public void ToggleAds()
    {
        AdManager.Instance.ToggleAds();
    }

    public void SkipTutorials()
    {
        TutorialManager.Instance.TutorialLevel = int.MaxValue;
        GameManager.Instance.LoadCurrentLevel();
    }
}
