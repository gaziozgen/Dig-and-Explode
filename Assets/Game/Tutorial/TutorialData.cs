using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/Tutorial/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    bool started = false;
    public bool Started
    {
        get => started; set
        {
            bool previousValue = started;
            started = value;
            if (started && !previousValue)
                OnStarted.Invoke();
        }
    }
    bool completed = false;
    public bool Completed
    {
        get => completed; set
        {
            bool previousValue = completed;
            completed = value;
            if (completed && !previousValue)
                OnCompleted.Invoke();
        }
    }
    public UnityEvent OnStarted, OnCompleted;

}
