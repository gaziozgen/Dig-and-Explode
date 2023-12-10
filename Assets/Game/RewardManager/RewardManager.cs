using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/Reward Manager")]
public class RewardManager : ScriptableObject
{
    [SerializeField] UnityEvent onAdsRemoved = new();
    public void RemoveAds()
    {
        AdManager.Instance.DisableAds();
        onAdsRemoved.Invoke();
    }

    public void AddGem(int amount)
    {
        GemUI.Instance.BurstFlyingGem(amount, Mathf.Clamp(amount, 1, 20), Screen.height / 20f, new Vector2(Screen.width / 2f, Screen.height / 2f));
    }
    public void AddMoney(int amount)
    {
        MoneyUI.Instance.BurstFlyingMoney(amount, 25, Screen.height / 20f, new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    public void WatchRewardedVideo100Money()
    {
        AdManager.Instance.ShowRewardedAd("100money", () => { }, () => AddMoney(100));
    }
}
