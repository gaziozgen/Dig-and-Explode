using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using static StorePagePanel;

public class NoAds : MonoBehaviour
{
    [SerializeField] StorePagePanel.StorePageProduct storePageProduct;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => StorePage.Initialized);
        Initialize();
    }

    private void Initialize()
    {
        Purchases.StoreProduct product = StorePage.Instance.GetProduct(storePageProduct.id);
        if (product == null) return;
        storePageProduct.button.onClick.AddListener(() => StorePage.Instance.HandlePurchase(storePageProduct.id));
        storePageProduct.priceText.text = $"{product.CurrencyCode} {product.Price}";

    }
}
