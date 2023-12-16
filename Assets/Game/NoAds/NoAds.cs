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
        Debug.Log("##################### Initialize NoAds");
        Purchases.StoreProduct product = StorePage.Instance.GetProduct(storePageProduct.id);
        Debug.Log("##################### " + (product == null).ToString());
        if (product == null) return;
        Debug.Log("##################### " + (product == null).ToString());
        storePageProduct.button.onClick.AddListener(() =>
        {
            Debug.Log("no ads clicked");
            Debug.Log(StorePage.Instance);
            StorePage.Instance.HandlePurchase(storePageProduct.id);
        });
        storePageProduct.priceText.text = $"{product.CurrencyCode} {product.Price}";

    }
}
