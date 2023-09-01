using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Linq;
using System;
using UnityEngine.Events;
using com.adjust.sdk;
using UnityEngine.Purchasing.Security;

public class StorePage : Singleton<StorePage>, IDetailedStoreListener
{
    public static bool Initialized { get; private set; } = false;
    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
    [SerializeField] string adjustTokenAND;
    [SerializeField] string adjustTokenIOS;
    [SerializeField] private List<PurchaseSuccessfulEvent> purchaseSuccessfulEvents = new();
    [SerializeField] private UnityEvent onPurchaseSuccessful = new();

    public Product GetProduct(string productID)
    {
        Product result = null;
        foreach (Product product in storeController.products.all)
            if (product.definition.id == productID)
            {
                result = product;
                break;
            }
        return result;
    }
    protected override async void Awake()
    {
        base.Awake();
        if (duplicated) return;
        InitializationOptions options = new();
        options.
#if UNITY_EDITOR || DEBUG
        SetEnvironmentName("test");
#else
        SetEnvironmentName("production");
#endif
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }
    void HandleIAPCatalogLoaded(AsyncOperation operation)
    {
        ResourceRequest request = operation as ResourceRequest;
        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");
#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay));
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore));
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                    StandardPurchasingModule.Instance(AppStore.NotSpecified));
#endif
        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void HandlePurchase(string productID)
    {
        storeController.InitiatePurchase(productID);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        Initialized = true;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Error initializing IAP because of {error}." +
            $"\r\nShow a message to the player depending on the error.");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"Initialize failed.\nerror: {error}\nmessage: {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"Purchase failed.\nproduct: {product.definition.id}\nmessage: {failureDescription.message}\nreason: {failureDescription.reason}\nproductId: {failureDescription.productId}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed.\nproduct: {product.definition.id}\nreason: {failureReason}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {

        bool validPurchase = true; // Presume valid for platforms with no R.V.

        // Unity IAP's validation logic is only included on these platforms.
#if !UNITY_EDITOR && !DEBUG && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX)
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try
        {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif

        if (validPurchase)
        {
            // Unlock the appropriate content here.
            Product product = purchaseEvent.purchasedProduct;
            for (int i = 0; i < purchaseSuccessfulEvents.Count; i++)
            {
                PurchaseSuccessfulEvent purchaseSuccessfulEvent = purchaseSuccessfulEvents[i];
                if (purchaseSuccessfulEvent.id == purchaseEvent.purchasedProduct.definition.id && purchaseSuccessfulEvent.onSuccessful != null)
                    purchaseSuccessfulEvent.onSuccessful.Invoke();
            }
#if !UNITY_EDITOR && !DEBUG
#if UNITY_IOS
        string adjustToken = adjustTokenIOS;
#else
        string adjustToken = adjustTokenAND;
#endif
            AdjustEvent adjustEvent = new(adjustToken);
            adjustEvent.setRevenue((double)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);
            adjustEvent.addCallbackParameter("product_id", product.definition.id);
            adjustEvent.setTransactionId(product.transactionID);
            Adjust.trackEvent(adjustEvent);
#endif
            onPurchaseSuccessful.Invoke();
        }

        return PurchaseProcessingResult.Complete;

    }

    [System.Serializable]
    public class PurchaseSuccessfulEvent
    {
        public string id;
        public UnityEvent onSuccessful = new();

    }
}
