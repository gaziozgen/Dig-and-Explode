using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Fate/Applovin Manager")]
public class ApplovinManager : ScriptableObject
{
    [SerializeField] private string userID = "USER_ID";
    [SerializeField] private string sdkKey = "09mBPe6fn7Tg_xo6p4-shNiAaXlBrtK4zAFXmPKNwdK3df-td8R7o5CgUWUpH3LQb2Mxxmp8AKngmcXgROmQJV";

    public IEnumerator Initialize()
    {
        bool done = false;
#if !DEBUG
        MaxSdk.SetCreativeDebuggerEnabled(false);
#endif
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            done = true;
        };

        MaxSdk.SetSdkKey(sdkKey);
        MaxSdk.SetUserId(userID);
        MaxSdk.InitializeSdk();
        yield return new WaitUntil(() => done);
    }
}
