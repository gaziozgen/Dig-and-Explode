using DG.Tweening;
using FateGames.Core;
using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tutorial : FateMonoBehaviour
{
    [Header("Analytics")]
    [SerializeField] bool sendEvent = false;
    [SerializeField] string taskName = "";
    bool isStartEventSent = false;
    [Header("Properties")]
    [SerializeField] TutorialData tutorialData;
    [SerializeField] public Tutorial root;
    [SerializeField] public bool partialTutorial;
    [SerializeField] public bool resetQuantityOnStart = false;
    [SerializeField] public bool showArrow = true;
    [SerializeField] private bool bypass = false;
    [SerializeField] string infoText = "Info text";
    [SerializeField] Sprite iconSprite;
    [SerializeField] Transform target = null;
    [SerializeField] Transform[] targets = null;
    [SerializeField] float transitionDuration = 1f;
    [SerializeField] float waitDuration = 1f;
    [SerializeField] bool moveCamera = true;
    [SerializeField] float offsetY = 0;
    [SerializeField] BoolVariable[] requirements;
    [SerializeField] Tutorial nextTutorial;
    [SerializeField] BoolVariable allTutorialsCompleted;
    [SerializeField] GameEvent quantityIncreaseEvent;
    [SerializeField] int currentQuantity, maxQuantity = 1;
    [SerializeField] int prizeGem = 0;
    [SerializeField] GameEvent tutorialCompletedGameEvent;
    bool canStart = false;
    [SerializeField] float delay = 0;
    public bool Started { get {  return tutorialData.Started; } set { Debug.Log(this, this); tutorialData.Started = value; } }
    public bool Completed { get => tutorialData.Completed; set => tutorialData.Completed = value; }
    bool started = false;
    bool inCompletePhase = false;
    bool isLastTutorialOfThePartialSet => !nextTutorial || !nextTutorial.partialTutorial || nextTutorial.root != root && nextTutorial.root != this;

    public Sprite IconSprite { get => iconSprite; }

    private void Awake()
    {
        //Debug.Log("Started 0: " + Started + " " + this);
        Started = false;
        //Debug.Log("Started 1: " + Started + " " + this);
        //Debug.Log("Completed 0: " + Completed + " " + this);
        Completed = false;
        //Debug.Log("Completed 1: " + Completed + " " + this);
        for (int i = 0; i < requirements.Length; i++)
        {
            requirements[i].Value = false;
            requirements[i].OnValueChanged.AddListener((previous, current) => CheckRequirements());
        }
        if (quantityIncreaseEvent)
            quantityIncreaseEvent.OnRaise += IncreaseQuantity;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < requirements.Length; i++)
        {
            requirements[i].OnValueChanged.RemoveListener((previous, current) => CheckRequirements());
        }
        if (quantityIncreaseEvent)
            quantityIncreaseEvent.OnRaise -= IncreaseQuantity;
    }

    int count = 0;
    private void IncreaseQuantity()
    {
        if (!started || Completed) return;
        currentQuantity = Mathf.Clamp(currentQuantity + 1, 0, maxQuantity);
        if (!partialTutorial || isLastTutorialOfThePartialSet)
        {
            TutorialManager.Instance.ObjectivePanel.SetCurrentQuantity(currentQuantity);
            if (targets.Length > currentQuantity)
            {
                Transform currentTarget = targets[currentQuantity];
                TutorialManager.Instance.Focus(currentTarget, transitionDuration, waitDuration, false, offsetY);
            }
        }
        if (currentQuantity >= maxQuantity)
            CompleteTutorial();
    }

    public Tutorial GetNextTutorial()
    {
        return nextTutorial;
    }

    public void StartCheckingRequirements()
    {
        canStart = true;
        CheckRequirements();
    }

    public void CheckRequirements()
    {
        if (!canStart) return;
        bool result = true;
        for (int i = 0; i < requirements.Length; i++)
        {
            if (!requirements[i].Value)
            {
                result = false;
                break;
            }
        }
        if (result)
            StartTutorial();
    }

    private void StartTutorial()
    {
        canStart = false;
        if (!quantityIncreaseEvent || resetQuantityOnStart) currentQuantity = 0;
        if (sendEvent && !isStartEventSent)
        {
            //FirebaseEventManager.SendTaskStartEvent(taskName);
            isStartEventSent = true;
        }
        if (bypass || currentQuantity >= maxQuantity)
        {
            Bypass();
            return;
        }
        PlayerTutorialArrow.Instance.ShowArrow = showArrow;
        started = true;
        if (root != null && partialTutorial)
        {
            if (target)
                TutorialManager.Instance.Focus(target, transitionDuration, waitDuration, moveCamera, offsetY);
        }
        else
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                TutorialManager.Instance.ObjectivePanel.SetMaxQuantity(maxQuantity);
                TutorialManager.Instance.ObjectivePanel.SetCurrentQuantity(currentQuantity);
                TutorialManager.Instance.ShowInfoText(infoText, iconSprite);
                if (target)
                    TutorialManager.Instance.Focus(target, transitionDuration, waitDuration, moveCamera, offsetY);
            }, false);
        }

        //Debug.Log("Started 2: " + Started + " " + this);
        Started = true;
        //Debug.Log("Started 3: " + Started + " " + this);
    }

    private void Bypass()
    {
        if (Completed)
        {
            Debug.LogError("Already completed", this);
            return;
        }
        if (sendEvent)
        {
            FirebaseEventManager.SendTaskCompleteEvent(taskName);
        }
        DOVirtual.DelayedCall(bypass || partialTutorial && !isLastTutorialOfThePartialSet ? 0 : 2, () =>
        {
            if (nextTutorial == null) allTutorialsCompleted.Value = true;
            else
            {
                nextTutorial.StartCheckingRequirements();
            }
        });
        //Debug.Log("Completed 8: " + Completed + " " + this);
        Completed = true;
        //Debug.Log("Completed 9: " + Completed + " " + this);
        TutorialManager.Instance.TutorialLevel++;
        tutorialCompletedGameEvent.Raise();
    }

    public void CompleteTutorial()
    {
        if (Completed || inCompletePhase)
        {
            //Debug.LogError("Already completed", this);
            return;
        }
        inCompletePhase = true;
        currentQuantity = maxQuantity;

        Debug.Log("CompleteTutorial");
        if (!partialTutorial || isLastTutorialOfThePartialSet)
        {
            TutorialManager.Instance.CleanText();
            TutorialManager.Instance.ObjectivePanel.SetCurrentQuantity(maxQuantity);
        }
        //TutorialManager.Instance.CleanText();

        TutorialManager.Instance.TutorialComplete();
        if (prizeGem > 0)
            GemUI.Instance.BurstFlyingGem(prizeGem, Mathf.Clamp(prizeGem, 1, 20), Screen.height / 20f, new Vector2(Screen.width / 2f, Screen.height / 2f));
        Bypass();
    }
}
