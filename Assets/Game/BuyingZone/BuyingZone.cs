using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
using Firebase.Analytics;

public class BuyingZone : FateMonoBehaviour
{
    [Header("Properties")]
    [SerializeField] int originalPrice = 10, price = 10;
    [SerializeField] NumberRemoteConfig originalPriceRemoteConfig;
    //[SerializeField] GameObject obj;
    [SerializeField] bool faceDown;
    [SerializeField] TMPro.TextMeshProUGUI priceText, nameText;
    [SerializeField] Image fillImage;
    [SerializeField] string zoneName = "Buy";
    [SerializeField] GameEvent gameEvent;
    [SerializeField] GameEvent onBuyingZoneBoughtEvent;
    SphereCollider sphereCollider;
    bool bought = false;
    [SerializeField] float scaleAnimationDuration = 0.5f;
    [SerializeField] UIElement uiElement;
    [SerializeField] Collider m_Collider;
    [SerializeField] TransformRuntimeSet all, boughtSet;
    [SerializeField] UnityEvent onBought;
    [SerializeField] UnityEvent onBoughtFirstTime;
    [SerializeField] Transform canvasTransform;

    int oldOriginalPrice = -1;

    public bool CanGiveMoney { get => price > 0; }
    public bool Bought { get => bought; }
    public int Price { get => price; }

    void Awake()
    {
        sphereCollider = GetComponentInChildren<SphereCollider>();
        if (faceDown)
        {
            canvasTransform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        }
        if (originalPriceRemoteConfig)
        {
            SetOriginalPrice((int)GameManager.Instance.GetNumberConfig(originalPriceRemoteConfig));
        }
        //obj.SetActive(false);
    }

    private void OnEnable()
    {
        if (all)
            all.Add(transform);
    }

    private void OnDisable()
    {
        if (all)
            all.Remove(transform);
        if (boughtSet)
            boughtSet.Remove(transform);
    }

    public void ScaleAnimation(Transform trans)
    {
        Vector3 originalScale = trans.localScale;
        trans.localScale = Vector3.zero;
        trans.DOScale(originalScale, scaleAnimationDuration).SetEase(Ease.OutBack);
    }

    void OnValidate()
    {
        UpdateNameText();
        UpdatePriceText();
        UpdateFillImage();
    }

    public void UpdatePriceText()
    {
        if (priceText)
        {
            priceText.text = price == 0 ? "Free" : ("$" + price.ToString());
        }
    }
    public void UpdateNameText()
    {
        if (nameText) nameText.text = zoneName;
    }

    public void UpdateFillImage()
    {
        if (fillImage) fillImage.fillAmount = 1 - (float)price / originalPrice;
    }

    private void Start()
    {
        if (price == oldOriginalPrice)
        {
            price = originalPrice;
            UpdatePriceText();
            UpdateFillImage();
        }
        if (price > originalPrice) price = originalPrice;
        if (!CanGiveMoney) Buy(false);
        else
        {
            UpdateNameText();
            UpdatePriceText();
            UpdateFillImage();
        }
    }

    public void SetParentToNull(Transform transform)
    {
        transform.SetParent(null);
    }

    public void Hide()
    {
        Debug.Log("Hide", this);
        uiElement.Hide();
        m_Collider.enabled = false;
    }

    public void Show()
    {
        Debug.Log("Show", this);
        uiElement.Show();
        m_Collider.enabled = true;
    }


    public void Buy(bool invokeEvent = true)
    {
        if (bought) return;
        bought = true;
        gameObject.SetActive(false);
        if (boughtSet)
            boughtSet.Add(transform);
        if (onBought != null)
            onBought.Invoke();
        if (invokeEvent)
        {
            if (gameEvent)
                gameEvent.Raise();
            if (onBuyingZoneBoughtEvent)
                onBuyingZoneBoughtEvent.Raise();
            onBoughtFirstTime?.Invoke();
        }
    }

    public void SetOriginalPrice(int newOriginalPrice)
    {
        if (newOriginalPrice < 0) return;
        oldOriginalPrice = originalPrice;
        originalPrice = newOriginalPrice;
        UpdateFillImage();
    }

    public void GiveMoney(Money3D money)
    {
        if (!CanGiveMoney) return;
        price = Mathf.Clamp(price - money.value, 0, int.MaxValue);
        //price -= money.value;

        money.transform.DOJump(transform.position, money.transform.position.y * 0.65f, 1, 0.2f).OnComplete(() =>
        {
            UpdatePriceText();
            UpdateFillImage();
            if (price <= 0)
            {
                if (!bought)
                {
                    Buy();
                }
            }
            money.Release();
        });
    }

}
/*
#if UNITY_EDITOR
[CustomEditor(typeof(BuyingZone))]
public class BuyingZoneEditor : Editor
{
    BuyingZone buyingZone;

    private void OnEnable()
    {
        buyingZone = target as BuyingZone;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Show"))
        {
            buyingZone.Show();
            EditorUtility.SetDirty(buyingZone);
        }
        if (GUILayout.Button("Hide"))
        {
            buyingZone.Hide();
            EditorUtility.SetDirty(buyingZone);
        }
        DrawDefaultInspector();
    }
}
#endif
*/