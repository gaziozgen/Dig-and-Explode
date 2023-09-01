using com.adjust.sdk;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdManager : Singleton<AdManager>
{
    bool disabled { get => SaveManager.Instance.GetBool("adsRemoved", false); set => SaveManager.Instance.SetBool("adsRemoved", value); }
    public static bool Disabled { get => Instance.disabled; }
    [SerializeField] private UnityEvent onInterstitialDismissed = new();
    [Header("AD KEYS AND")]
    [SerializeField] private string BannerAdUnitIdAnd = "";
    [SerializeField] private string InterstitialAdUnitIdAnd = "";
    [SerializeField] private string RewardedAdUnitIdAnd = "";
    [Header("AD KEYS IOS")]
    [SerializeField] private string BannerAdUnitIdIOS = "";
    [SerializeField] private string InterstitialAdUnitIdIOS = "";
    [SerializeField] private string RewardedAdUnitIdIOS = "";
    private string BannerAdUnitId
    {
        get
        {
#if UNITY_STANDALONE || UNITY_IOS
            return BannerAdUnitIdIOS;
#elif UNITY_ANDROID
            return BannerAdUnitIdAnd;
#endif
        }
    }
    private string InterstitialAdUnitId
    {
        get
        {
#if UNITY_STANDALONE || UNITY_IOS
            return InterstitialAdUnitIdIOS;
#elif UNITY_ANDROID
            return InterstitialAdUnitIdAnd;
#endif
        }
    }
    private string RewardedAdUnitId
    {
        get
        {
#if UNITY_STANDALONE || UNITY_IOS
            return RewardedAdUnitIdIOS;
#elif UNITY_ANDROID
            return RewardedAdUnitIdAnd;
#endif
        }
    }
    [Header("REMOTE CONFIG")]
    [SerializeField] private RemoteConfigManager remoteConfigManager;
    [SerializeField] private NumberRemoteConfig timeIntervalBetweenInterstitialConfig, firstInterstitialTimeConfig, graceTimeConfig;
    float timeIntervalBetweenInterstitial => remoteConfigManager.GetNumberConfig(timeIntervalBetweenInterstitialConfig);
    float firstInterstitialTime => remoteConfigManager.GetNumberConfig(firstInterstitialTimeConfig);
    float graceTime => remoteConfigManager.GetNumberConfig(graceTimeConfig);
    public bool IsGraceTimePassed => SaveManager.TotalPlaytime > graceTime;
    #region Interstitial Ad Methods
    private int interstitialRetryAttempt = 0;
    private float lastAdShowTime = 0;
    private float lastInterstitialShowTime = 0;
    public bool canShowInterstitial => Time.time >= lastAdShowTime + timeIntervalBetweenInterstitial && Time.time >= firstInterstitialTime && graceTime <= SaveManager.TotalPlaytime;

    private void OnDisable()
    {
        Release();
    }
    public void DisableAds()
    {
        Debug.Log("DisableAds");
        disabled = true;
        HideBannerAd();
    }

    public void EnableAds()
    {
        Debug.Log("EnableAds");
        disabled = false;
        ShowBannerAd();
    }

    public void Initialize()
    {
        InitializeBannerAds();
        InitializeInterstitialAds();
        InitializeRewardedAds();
    }
    public void Release()
    {
        ReleaseBannerEvents();
        ReleaseInterstitialEvents();
        ReleaseRewardedEvents();
    }
    private void OnAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        AdjustAdRevenue adjustAdRevenue = new(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        adjustAdRevenue.setRevenue(arg2.Revenue, "USD");
        adjustAdRevenue.setAdRevenueNetwork(arg2.NetworkName);
        adjustAdRevenue.setAdRevenueUnit(arg2.AdUnitIdentifier);
        adjustAdRevenue.setAdRevenuePlacement(arg2.Placement);
        Adjust.trackAdRevenue(adjustAdRevenue);
    }
    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent; // Adjust


        // Load the first interstitial
        LoadInterstitial();
        //InitializeRewardedAds();
    }

    private void ReleaseInterstitialEvents()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent; // Adjust
    }

    void LoadInterstitial()
    {
        Debug.Log("Loading interstitial...");
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
    }

    public void ShowInterstitial()
    {
        if (Disabled) return;
        Debug.Log("ShowInterstitial");
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId) && canShowInterstitial)
        {
            Debug.Log("Showing");
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("INTWatched");
        }
        else
        {
            Debug.Log("Ad not ready");

        }
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'

        Debug.Log("Interstitial loaded");

        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));


        Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

        Invoke(nameof(LoadInterstitial), (float)retryDelay);
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        lastAdShowTime = Time.time;
        lastInterstitialShowTime = Time.time;
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Interstitial dismissed");
        LoadInterstitial();
        onInterstitialDismissed.Invoke();
    }






    #endregion

    #region Rewarded Ad Methods
    private bool rewardedAdSucceed = false;
    private Action onRewardedAdSucceed;
    private Action onRewardedAdFailed;
    private int rewardedRetryAttempt;

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent; // Adjust


        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void ReleaseRewardedEvents()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent; // Adjust
    }

    private void LoadRewardedAd()
    {
        Debug.Log("Loading rewarded ad...");
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    public void ShowRewardedAd(string rewardName, Action onFailed, Action onSucceed)
    {
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            onRewardedAdSucceed = onSucceed;
            onRewardedAdSucceed += () => FirebaseEventManager.SendRVWatchedEvent(rewardName);
            onRewardedAdFailed = onFailed;
            Debug.Log("Showing...");
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);

        }
        else
        {
            Debug.Log("Ad not ready...");

        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'

        Debug.Log("Rewarded ad loaded");

        // Reset retry attempt
        rewardedRetryAttempt = 0;
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));


        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad displayed");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Debug.Log("Rewarded ad dismissed");
        lastAdShowTime = Time.time;
        if (!rewardedAdSucceed && onRewardedAdFailed != null)
            onRewardedAdFailed.Invoke();
        else if (rewardedAdSucceed && onRewardedAdSucceed != null)
            onRewardedAdSucceed.Invoke();
        onRewardedAdSucceed = null;
        onRewardedAdFailed = null;
        rewardedAdSucceed = false;
        LoadRewardedAd();
    }


    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        Debug.Log("Rewarded ad received reward");
        rewardedAdSucceed = true;
    }


    #endregion

    #region Banner Ad Methods

    private void InitializeBannerAds()
    {
        Debug.Log("InitializeBannerAds");
        // Attach Callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent; // Adjust


        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.black);
    }

    private void ReleaseBannerEvents()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent; // Adjust
    }

    public void ShowBannerAd()
    {
        if (Disabled) return;
        if (SaveManager.TotalPlaytime <= graceTime)
        {
            IEnumerator Routine()
            {
                yield return new WaitUntil(() => SaveManager.TotalPlaytime > graceTime);
                ShowBannerAd();
            }
            StartCoroutine(Routine());
            return;
        }

        Debug.Log("ShowBannerAd");
        MaxSdk.ShowBanner(BannerAdUnitId);
    }

    public void HideBannerAd()
    {
        Debug.Log("HideBannerAd");
        MaxSdk.HideBanner(BannerAdUnitId);
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debug.Log("Banner ad loaded");
        ShowBannerAd();
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
    }


    #endregion

}
