using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;

public class StorePage : Singleton<StorePage>
{
    public static bool Initialized { get; private set; } = true;
    public bool IsRestored { get => SaveManager.Instance.GetBool("isPurchasesRestored", false); set => SaveManager.Instance.SetBool("isPurchasesRestored", value); }
    public RevenueCatManager RevenueCatManager { get => revenueCatManager; }

    [SerializeField] string adjustTokenAND;
    [SerializeField] string adjustTokenIOS;
    [SerializeField] CustomerInfoVariable customerInfo;
    [SerializeField] RevenueCatManager revenueCatManager;
    [SerializeField] private List<PurchaseSuccessfulEvent> purchaseSuccessfulEvents = new();
    [SerializeField] private UnityEvent onPurchaseSuccessful = new();


    public string[] GetProductIdentifiers()
    {
        string[] productIdentifiers = new string[purchaseSuccessfulEvents.Count];
        for (int i = 0; i < purchaseSuccessfulEvents.Count; i++)
        {
            productIdentifiers[i] = purchaseSuccessfulEvents[i].id;
        }
        return productIdentifiers;
    }


    public void RestorePurchases()
    {
        revenueCatManager.RestorePurchases();

    }
    public Purchases.StoreProduct GetProduct(string productID)
    {
        try
        {
            return revenueCatManager.StoreProducts[productID];

        }
        catch (Exception)
        {
            return null;
        }
    }


    public void HandlePurchase(string productID)
    {
        Debug.Log("HandlePurchase: " + productID);
        revenueCatManager.PurchaseProduct(productID);
    }


    public void GiveProduct(string id)
    {
        for (int i = 0; i < purchaseSuccessfulEvents.Count; i++)
        {
            PurchaseSuccessfulEvent purchaseSuccessfulEvent = purchaseSuccessfulEvents[i];
            if (purchaseSuccessfulEvent.id == id && purchaseSuccessfulEvent.onSuccessful != null)
            {
                purchaseSuccessfulEvent.onSuccessful.Invoke();
                return;
            }
        }
    }

    [System.Serializable]
    public class PurchaseSuccessfulEvent
    {
        public string id;
        public UnityEvent onSuccessful = new();

    }
}