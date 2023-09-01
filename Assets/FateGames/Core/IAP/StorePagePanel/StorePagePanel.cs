using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class StorePagePanel : MonoBehaviour
{
    [SerializeField] StorePageProduct[] storePageProducts;
    [SerializeField] UIElement uiElement;
    [SerializeField] UnityEvent onShowed = new();
    public static bool isShowedForTheFirstTime { get => SaveManager.Instance.GetBool("isShowedForTheFirstTime", false); set => SaveManager.Instance.SetBool("isShowedForTheFirstTime", value); }
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => StorePage.Initialized);
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < storePageProducts.Length; i++)
        {
            StorePageProduct storePageProduct = storePageProducts[i];
            Product product = StorePage.Instance.GetProduct(storePageProduct.id);
            if (product == null) continue;
            storePageProduct.button.onClick.AddListener(() => StorePage.Instance.HandlePurchase(storePageProduct.id));
            storePageProduct.priceText.text = $"{product.metadata.isoCurrencyCode} {product.metadata.localizedPrice}";
        }
    }

    public void CheckFirstShow()
    {
        if (isShowedForTheFirstTime) return;
        uiElement.Show();
        onShowed.Invoke();
        isShowedForTheFirstTime = true;
    }

    public void RemoveAds()
    {
        Debug.Log("Remove ads!");
    }

    [System.Serializable]
    public class StorePageProduct
    {
        public string id;
        public Button button;
        public TMPro.TextMeshProUGUI priceText;
    }
}
