using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fate/Firebase Manager")]
public class FirebaseManager : ScriptableObject
{
    [SerializeField] private RemoteConfigManager remoteConfigManager;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public IEnumerator Initialize()
    {
        bool done = false;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {

                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("Enabling firebase data collection.");
            }
            else
            {
                Debug.LogError(
                    "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
            done = true;
        });
        yield return new WaitUntil(() => done);
        yield return remoteConfigManager.Initialize();
    }
}
