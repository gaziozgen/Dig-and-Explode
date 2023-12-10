using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fate/Firebase/Firebase Event Manager")]
public class FirebaseEventManager : ScriptableObject
{
    private readonly static string levelProgressEventName = "LevelProgress";
    static int level = 1;

    public static void SendTaskStartEvent(string taskName)
    {
        Debug.Log($"SendTaskStartEvent: {taskName}");
        if (taskName == null || taskName == "") return;
        FirebaseAnalytics.LogEvent(taskProgressEventName, new Parameter("taskStarted", taskName));
    }
    public static void SendTaskCompleteEvent(string taskName)
    {
        Debug.Log($"SendTaskCompleteEvent: {taskName}");
        if (taskName == null || taskName == "") return;
        FirebaseAnalytics.LogEvent(taskProgressEventName, new Parameter("taskCompleted", taskName));
    }
    private readonly static string taskProgressEventName = "TaskProgress";

    public static void SendUpgradeEvent(string upgradeName)
    {
        Debug.Log($"SendTaskStartEvent: {upgradeName}");
        if (upgradeName == null || upgradeName == "") return;
        FirebaseAnalytics.LogEvent("UpgradeProgress", new Parameter("upgraded", upgradeName));
    }

    public static void SendEvent(string eventName)
    {
        if (eventName == null || eventName == "") return;
        FirebaseAnalytics.LogEvent(eventName);
    }
    public static void SendLevelCompleteEvent()
    {
        FirebaseAnalytics.LogEvent("LevelProgress", "levelCompleted", $"LEVEL_{level}");
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
