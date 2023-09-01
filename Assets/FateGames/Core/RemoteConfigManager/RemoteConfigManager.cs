using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Fate/Remote Config/Remote Config Manager")]
public class RemoteConfigManager : ScriptableObject
{
    [SerializeField] private NumberRemoteConfig[] numberRemoteConfigs = new NumberRemoteConfig[0];
    private bool isFetchCompleted = false;

    public long GetNumberConfig(NumberRemoteConfig numberRemoteConfig)
    {
        return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(numberRemoteConfig.id).LongValue;
    }
    public IEnumerator Initialize()
    {
        isFetchCompleted = false;
        // [START set_defaults]
        Dictionary<string, object> defaults = new Dictionary<string, object>();

        // These are the values that are used if we haven't fetched data from the
        // server
        // yet, or if we ask for values that the server doesn't have:ü
        foreach (NumberRemoteConfig config in numberRemoteConfigs)
        {
            if (config != null)
                defaults.Add(config.id, config.defaultValue);
        }

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
          .ContinueWithOnMainThread(task =>
          {
              // [END set_defaults]
              Debug.Log("RemoteConfig configured and ready!");

              FetchDataAsync();
          });
        yield return new WaitUntil(() => isFetchCompleted);
    }

    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    //[END fetch_async]

    private void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task =>
                {
                    Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                               info.FetchTime));
                });

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
        isFetchCompleted = true;
    }


}
