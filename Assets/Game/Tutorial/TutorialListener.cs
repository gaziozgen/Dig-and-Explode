using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialListener : MonoBehaviour
{
    [SerializeField] TutorialData tutorialData;
    [SerializeField] UnityEvent onStarted = new();
    [SerializeField] UnityEvent onCompleted = new();
    bool startInvoked = false;
    bool completeInvoked = false;

    private void OnEnable()
    {
        startInvoked = false;
        completeInvoked = false;
        //Debug.LogWarning("#### OnEnable " + tutorialData);
        if (!tutorialData) return;
        //onStarted.AddListener(() => Debug.LogWarning("#### OnStarted " + tutorialData));
        //onCompleted.AddListener(() => Debug.LogWarning("#### OnCompleted " + tutorialData));
        tutorialData.OnStarted.AddListener(() => { if (startInvoked) return; onStarted.Invoke(); startInvoked = true; });
        tutorialData.OnCompleted.AddListener(() => { if (completeInvoked) return; onCompleted.Invoke(); completeInvoked = true; });
    }
    private void OnDisable()
    {
        if (!tutorialData) return;
        tutorialData.OnStarted.RemoveListener(() => { if (startInvoked) return; onStarted.Invoke(); startInvoked = true; });
        tutorialData.OnCompleted.RemoveListener(() => { if (completeInvoked) return; onCompleted.Invoke(); completeInvoked = true; });
    }

    private void Start()
    {
        //Debug.LogWarning("#### Start " + tutorialData);
        if (!tutorialData) return;
        if (tutorialData.Started && !startInvoked)
        {
            //Debug.LogWarning("#### START OnStarted " + tutorialData);
            onStarted.Invoke();
            startInvoked = true;
        }
        if (tutorialData.Completed && !completeInvoked)
        {
            //Debug.LogWarning("#### START OnCompleted " + tutorialData);
            onCompleted.Invoke();
            completeInvoked = true;
        }
    }
}
