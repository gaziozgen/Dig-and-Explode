using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIPRoomUpgradePanel : MonoBehaviour
{
    static VIPRoomUpgradePanel instance = null;
    public static VIPRoomUpgradePanel Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<VIPRoomUpgradePanel>();
            return instance;
        }
    }
    [SerializeField] Transform cardContainer;
    [SerializeField] GameObject cardPrefab, rewardedCardPrefab;
    [SerializeField] UIElement uiElement;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    private void Awake()
    {
        instance = this;
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
    public void Show(UpgradeableVIPRoomOptionContainer optionContainer, int level)
    {
        levelText.text = $"LEVEL {level}";
        while (cardContainer.childCount > 0)
        {
            Transform child = cardContainer.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < optionContainer.Options.Length; i++)
        {
            UpgradeableVIPRoomOption option = optionContainer.Options[i];
            VIPRoomUpgradeCard card;
            if (!option.rewarded)
                card = Instantiate(cardPrefab, cardContainer).GetComponent<VIPRoomUpgradeCard>();
            else
                card = Instantiate(rewardedCardPrefab, cardContainer).GetComponent<VIPRoomUpgradeCard>();
            card.Prepare(option);
        }
        uiElement.Show();
    }
    public void Hide()
    {

        uiElement.Hide();
    }
}
