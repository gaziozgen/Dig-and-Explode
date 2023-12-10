using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fate/Adjust Manager")]
public class AdjustManager : ScriptableObject
{
    [SerializeField] string appTokenAND = "";
    [SerializeField] string appTokenIOS = "";
    [SerializeField] string appOpenTokenAND = "";
    [SerializeField] string appOpenTokenIOS = "";
    public IEnumerator Initialize()
    {
        bool initialized = false;
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
        adjustConfig.setSessionSuccessDelegate((sessionSuccessDelegate) =>
        {
            Debug.Log("adid: " + sessionSuccessDelegate.Adid);
            initialized = true;
        });
        adjustConfig.setSessionFailureDelegate((sessionFailureData) =>
        {
            Debug.Log("adid: " + sessionFailureData.Adid);
            initialized = true;
        });
        float startTime = Time.time;
        Adjust.start(adjustConfig);
        SendAppOpenEvent();
        yield return new WaitUntil(() => initialized || Time.time >= startTime + 2);
        if (!initialized)
            Debug.LogWarning("Adjust first install callback does not work");
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
