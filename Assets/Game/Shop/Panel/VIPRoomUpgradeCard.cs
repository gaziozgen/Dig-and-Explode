using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VIPRoomUpgradeCard : MonoBehaviour
{
    [SerializeField] Image roomImage;
    [SerializeField] TMPro.TextMeshProUGUI moneyText, xpText, ticketText;
    [SerializeField] Button getButton;
    [SerializeField] Button previewButton;
    [SerializeField] Button ticketButton;
    UpgradeableVIPRoomOption option = null;

    public void Prepare(UpgradeableVIPRoomOption option)
    {
        this.option = option;
        roomImage.sprite = option.roomImage;
        moneyText.text = $"x{option.moneyMultiplier}";
        xpText.text = $"+{option.xp}";
        getButton.onClick.RemoveAllListeners();
        getButton.onClick.AddListener(() => option.previewAction?.Invoke());
        if (option.rewarded)
            getButton.onClick.AddListener(() => AdManager.Instance.ShowRewardedAd("VIPRoomUpgrade", () => { }, () => { option.getAction?.Invoke(); VIPRoomUpgradePanel.Instance.Hide(); }));
        else
            getButton.onClick.AddListener(() =>
            {
                option.getAction?.Invoke();
                AdManager.Instance.ShowInterstitial();
            });
        if (ticketButton)
        {
            ticketButton.onClick.RemoveAllListeners();
            if (GemUI.Gem >= 1)
            {
                ticketText.color = Color.white;
                ticketButton.interactable = true;
                ticketButton.onClick.AddListener(() =>
                {
                    GemUI.Instance.SpendGem(1);
                    option.getAction?.Invoke();
                });
            }
            else
            {
                ticketButton.interactable = false;
                ticketText.color = Color.red;
            }
            ticketButton.onClick.AddListener(VIPRoomUpgradePanel.Instance.Hide);
        }
        if (!option.rewarded)
            getButton.onClick.AddListener(() => { VIPRoomUpgradePanel.Instance.Hide(); });
        previewButton.onClick.RemoveAllListeners();
        previewButton.onClick.AddListener(() => { option.previewAction?.Invoke(); });

    }
}
