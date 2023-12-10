using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetProductPriceToText : MonoBehaviour
{
    [SerializeField] string productId;
    [SerializeField] TMPro.TextMeshProUGUI priceText;
    [SerializeField] float multiplier = 1;
    IEnumerator Start()
    {
        yield return new WaitUntil(()=> StorePage.Initialized);
        Purchases.StoreProduct product = StorePage.Instance.GetProduct(productId);
        if (product == null) yield break;
        priceText.text = $"{Mathf.FloorToInt((float)product.Price * multiplier)}.99";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
