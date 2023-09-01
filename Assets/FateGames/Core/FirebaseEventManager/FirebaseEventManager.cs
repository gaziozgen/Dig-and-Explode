using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fate/Firebase/Firebase Event Manager")]
public class FirebaseEventManager : ScriptableObject
{
    private readonly static string levelProgressEventName = "LevelProgress";
    public static void SendEvent(string eventName)
    {
        if (eventName == null || eventName == "") return;
        FirebaseAnalytics.LogEvent(eventName);
    }
    public static void SendEvent(string eventName, params Parameter[] parameters)
    {
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }

    public static void SendLevelStartEvent(int level)
    {
        Debug.Log($"SendLevelStartEvent: {level}");
        FirebaseAnalytics.LogEvent(levelProgressEventName, new Parameter("levelStarted", $"LEVEL_{level}"));
    }

    public static void SendLevelCompleteEvent(int level)
    {
        Debug.Log($"SendLevelCompleteEvent: {level}");
        FirebaseAnalytics.LogEvent(levelProgressEventName, new Parameter("levelCompleted", $"LEVEL_{level}"));
    }

    public static void SendRVWatchedEvent(string rvName)
    {
        Debug.Log($"SendRVWatchedEvent: {rvName}");
        if (rvName == null || rvName == "") return;
        FirebaseAnalytics.LogEvent($"RV_Watched_{rvName}");

    }

}
