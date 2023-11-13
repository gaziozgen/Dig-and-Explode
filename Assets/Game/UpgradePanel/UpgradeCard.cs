using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames.Core;
using UnityEngine.Events;
using GameAnalyticsSDK;
using Firebase.Analytics;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI priceText, nameText, rewardedButtonText;
    [SerializeField] Image iconImage;
    [SerializeField] Button buyButton, rewardedButton, buyWithGemButton;
    [SerializeField] Slider levelSlider;
    [SerializeField] string whose = "";
    [SerializeField] UnityEvent onBuy;
    [Header("Visual Attributes")]
    [SerializeField] string name;
    [SerializeField] Sprite icon;
    [Header("Attributes")]
    [SerializeField] int[] prices;
    [SerializeField] IntReference level;
    [SerializeField] int maxLevel = 5;
    MoneyButtonState moneyButtonState = MoneyButtonState.NONE;
    GemButtonState gemButtonState = GemButtonState.NONE;
    private enum MoneyButtonState { NONE, MAX, AFFORDABLE, NOT_AFFORDABLE }
    private enum GemButtonState { NONE, MAX, AFFORDABLE, NOT_AFFORDABLE }

    //int price => basePrice + (level.Value) * priceIncreasePerLevel;
    int price => prices[level.Value];

    private void Awake()
    {
        Debug.Log("okudu");
        level.Variable.OnValueChanged.AddListener((int before, int after) => UpdateCard());
    }

    private MoneyButtonState DecideMoneyButtonState()
    {
        if (level.Value >= maxLevel)
            return MoneyButtonState.MAX;
        else if (MoneyUI.Money >= price)
            return MoneyButtonState.AFFORDABLE;
        else
            return MoneyButtonState.NOT_AFFORDABLE;
    }
    private GemButtonState DecideGemButtonState()
    {
        if (level.Value >= maxLevel)
            return GemButtonState.MAX;
        else if (GemUI.Gem >= 1)
            return GemButtonState.AFFORDABLE;
        else
            return GemButtonState.NOT_AFFORDABLE;
    }

    private void OnEnable()
    {
        buyButton.onClick.AddListener(Buy);
        buyWithGemButton.onClick.AddListener(BuyWithGem);
        rewardedButton.onClick.AddListener(Rewarded);
    }
    private void OnDisable()
    {
        buyButton.onClick.RemoveListener(Buy);
        buyWithGemButton.onClick.RemoveListener(BuyWithGem);
        rewardedButton.onClick.RemoveListener(Rewarded);
    }
    private void OnValidate()
    {
        if (iconImage && icon)
            iconImage.sprite = icon;
        if (nameText)
            nameText.text = name;
    }

    private void Start()
    {
        UpdateCard();
    }

    public void UpdateGemButton()
    {
        GemButtonState newGemButtonState = DecideGemButtonState();
        if (gemButtonState == newGemButtonState) return;
        gemButtonState = newGemButtonState;
        switch (gemButtonState)
        {
            case GemButtonState.MAX:
                rewardedButton.interactable = false;
                rewardedButtonText.text = "MAX";
                rewardedButton.gameObject.SetActive(true);
                buyWithGemButton.gameObject.SetActive(false);
                break;
            case GemButtonState.AFFORDABLE:
                rewardedButton.gameObject.SetActive(false);
                buyWithGemButton.gameObject.SetActive(true);
                break;
            case GemButtonState.NOT_AFFORDABLE:
                rewardedButton.interactable = true;
                rewardedButtonText.text = "Free";
                rewardedButton.gameObject.SetActive(true);
                buyWithGemButton.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void UpdateMoneyButton()
    {
        MoneyButtonState newMoneyButtonState = DecideMoneyButtonState();
        if (moneyButtonState == newMoneyButtonState) return;
        moneyButtonState = newMoneyButtonState;
        switch (moneyButtonState)
        {
            case MoneyButtonState.MAX:
                buyButton.interactable = false;
                break;
            case MoneyButtonState.AFFORDABLE:
                buyButton.interactable = true;
                break;
            case MoneyButtonState.NOT_AFFORDABLE:
                buyButton.interactable = false;
                break;
            default:
                break;
        }
    }
    public void UpdateMoneyButtonPrice()
    {
        if (moneyButtonState == MoneyButtonState.MAX) priceText.text = "MAX";
        else
        {
            priceText.text = price == 0 ? "Free" : price.ToString();
        }
    }
    public void UpdateSlider()
    {
        levelSlider.value = (float)level.Value / maxLevel;
    }
    public void UpdateCard()
    {
        UpdateGemButton();
        UpdateMoneyButton();
        UpdateMoneyButtonPrice();
        UpdateSlider();
    }

    public void Buy()
    {
        if (MoneyUI.Money < this.price || level.Value >= maxLevel) return;
        int price = this.price;
        MoneyUI.Instance.SpendMoney(price);
        Upgrade();
    }

    public void BuyWithGem()
    {
        if (GemUI.Gem < 1 || level.Value >= maxLevel) return;
        int price = 1;
        GemUI.Instance.SpendGem(price);
        Upgrade();
    }

    public void Rewarded()
    {
        if (level.Value >= maxLevel) return;
        AdManager.Instance.ShowRewardedAd($"employeeUpgrade_{whose.ToLower()}_{name.ToLower()}",() => { }, () =>
        {
            Upgrade();
        });
    }

    public void Upgrade()
    {
        if (level.Value >= maxLevel) return;
        onBuy.Invoke();
        UpdateCard();
    }

}
