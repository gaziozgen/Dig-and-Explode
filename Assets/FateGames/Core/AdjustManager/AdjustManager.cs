using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Fate/Adjust Manager")]
public class AdjustManager : ScriptableObject
{
    [SerializeField] string appTokenAND = "";
    [SerializeField] string appTokenIOS = "";
    [SerializeField] string appOpenTokenAND = "";
    [SerializeField] string appOpenTokenIOS = "";
    public void Initialize()
    {
#if UNITY_IOS
        string appToken = appTokenIOS;
#else
        string appToken = appTokenAND;
#endif
#if DEBUG
        AdjustEnvironment environment = AdjustEnvironment.Sandbox;
#else
        AdjustEnvironment environment = AdjustEnvironment.Production;
#endif
        AdjustConfig adjustConfig = new AdjustConfig(appToken, environment);
        Adjust.start(adjustConfig);

        SendAppOpenEvent();
    }

    public void SendAppOpenEvent()
    {
#if UNITY_IOS
        string appOpenToken = appOpenTokenIOS;
#else
        string appOpenToken = appOpenTokenAND;
#endif
        AdjustEvent adjustEvent = new AdjustEvent(appOpenToken);
        Adjust.trackEvent(adjustEvent);
    }
}
