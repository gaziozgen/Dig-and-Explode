using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Purchases;

public class RevenueCatManager : Purchases.UpdatedCustomerInfoListener
{
    public bool ProductsGet { get; private set; } = false;
    [SerializeField] Purchases purchases;
    [SerializeField] StorePage storePage;
    [SerializeField] private UnityEvent OnCustomerInfoUpdated = new();
    [SerializeField] private UnityEvent onPurchaseSuccessful = new();

    public CustomerInfoVariable CustomerInfo;
    Dictionary<string, StoreProduct> storeProducts = new();
    List<string> givenEntitlements { get => SaveManager.Instance.GetStringList("givenEntitlements"); set => SaveManager.Instance.SetStringList("givenEntitlements", value); }
    public Dictionary<string, StoreProduct> StoreProducts { get => storeProducts; }

    public void Initialize()
    {
        Configure();
        CollectData();
        GetProducts();
    }

    public void PrintEntitlements()
    {
        Debug.Log("PrintEntitlements");
        for (int i = 0; i < givenEntitlements.Count; i++)
        {
            Debug.Log(givenEntitlements[i]);
        }
    }

    public void Configure()
    {
        purchases.SetLogLevel(LogLevel.Debug);
#if UNITY_IOS
        PurchasesConfiguration.Builder builder = Purchases.PurchasesConfiguration.Builder.Init(purchases.revenueCatAPIKeyApple);
#else
        PurchasesConfiguration.Builder builder = Purchases.PurchasesConfiguration.Builder.Init(purchases.revenueCatAPIKeyGoogle);
#endif
        PurchasesConfiguration purchasesConfiguration =
            builder
                .Build();
        purchases.Configure(purchasesConfiguration);
    }

    public void CollectData()
    {
        purchases.CollectDeviceIdentifiers();
        string adid = Adjust.getAdid();
        Debug.Log("collectData adid: " + adid);
        purchases.SetAdjustID(adid);
        if (adid == null || adid == "")
        {
            Debug.LogWarning("Adjust.getAdid() is null");
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(5);
                CollectData();
            }
            StartCoroutine(Routine());
        }
    }

    public void GetProducts()
    {
        purchases.GetProducts(storePage.GetProductIdentifiers(), (products, error) =>
        {
            if (error != null) Debug.LogError(error.Message);
            else
            {
                for (int i = 0; i < products.Count; i++)
                {
                    StoreProduct storeProduct = products[i];
                    storeProducts.Add(storeProduct.Identifier, storeProduct);
                }
                ProductsGet = true;
            }

        }, "inapp");
    }

    public void GetOfferings()
    {
        Debug.Log("Get offerings");
        purchases.GetOfferings((Offerings offerings, Error error) =>
        {
            for (int i = 0; i < offerings.Current.AvailablePackages.Count; i++)
            {
                Debug.Log("Package: " + offerings.Current.AvailablePackages[i].Identifier);
            }
        });
    }

    public void PurchaseProduct(string id)
    {
        Debug.Log("Purchase product: " + id);
        purchases.PurchaseProduct(id, (string productIdentifier, CustomerInfo customerInfo, bool userCancelled, Error error) =>
        {
            if (!userCancelled)
            {
                if (error != null)
                {
                    Debug.LogError(error.Message);
                }
                else
                {
                    foreach (string entitlement in customerInfo.Entitlements.Active.Keys)
                    {
                        if (!givenEntitlements.Contains(entitlement))
                        {
                            givenEntitlements.Add(entitlement);
                            Debug.Log("Add entitlement: " + entitlement);
                        }
                    }
                    Debug.Log("Purchase successful: " + id);
                    storePage.GiveProduct(id);
                    SetCustomerInfo(customerInfo);
                    onPurchaseSuccessful?.Invoke();
                }
            }
            else
            {
                Debug.Log("User cancelled the purchase");
            }
        }, "inapp");
    }

    public void GiveEntitlementReward(string entitlement)
    {
        //TODO will change
        Debug.Log("GiveEntitlementReward: " + entitlement);
        storePage.GiveProduct(entitlement);
    }

    public void RestorePurchases()
    {
        purchases.RestorePurchases((info, error) =>
        {
            SetCustomerInfo(info);
        });
    }
    private void SetCustomerInfo(CustomerInfo customerInfo)
    {
        CustomerInfo.Value = customerInfo;
        foreach (string entitlement in customerInfo.Entitlements.Active.Keys)
        {
            if (!givenEntitlements.Contains(entitlement))
            {
                givenEntitlements.Add(entitlement);
                Debug.Log("Add entitlement: " + entitlement);
                GiveEntitlementReward(entitlement);
            }
        }

        OnCustomerInfoUpdated.Invoke();
    }
    public override void CustomerInfoReceived(CustomerInfo customerInfo)
    {
        SetCustomerInfo(customerInfo);
    }
}
