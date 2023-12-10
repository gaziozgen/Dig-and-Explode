using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using FateGames.Core;

public class TutorialManager : FateMonoBehaviour
{
    private static TutorialManager instance = null;
    public static TutorialManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<TutorialManager>();
            return instance;
        }
    }
    public int TutorialLevel = -1;
    [SerializeField] Tutorial headTutorial;
    [SerializeField] int startIndex = 0;
    [SerializeField] BoolVariable allTutorialsCompleted;
    [SerializeField] private TextMeshProUGUI infoText = null;
    [SerializeField] private ObjectivePanel objectivePanel = null;
    [SerializeField] private Transform targetPointer = null;
    //[SerializeField] private NumberRemoteConfig disableCameraMovement;
    PlayerTutorialArrow playerTutorialArrow;


    [SerializeField] private UnityEvent onTutorialStarted;
    [SerializeField] private UnityEvent onTutorialFinished;

    public Vector3 TargetPos { get => targetPointer.position; }
    public ObjectivePanel ObjectivePanel { get => objectivePanel; }

    private Follower mainCameraController = null;

    void Awake()
    {
        instance = this;

        if (TutorialLevel < 0)
        {
            int count = 0;
            Tutorial currentTutorial = headTutorial;
            while (currentTutorial != null)
            {
                currentTutorial = currentTutorial.GetNextTutorial();
                count++;
            }
        }
        playerTutorialArrow = FindObjectOfType<PlayerTutorialArrow>();
        mainCameraController = Camera.main.GetComponent<Follower>();
    }

    private void Start()
    {
        StartCurrentTutorial();
    }

    private void StartCurrentTutorial()
    {
        Debug.Log("TutorialLevel: " + TutorialLevel);
        Tutorial currentTutorial = headTutorial;
        int count = 0;
        while (count++ < TutorialLevel && currentTutorial != null)
        {
            currentTutorial = currentTutorial.GetNextTutorial();
        }
        if (currentTutorial != null)
        {
            if (currentTutorial.root)
            {
                Tutorial root = currentTutorial.root;
                currentTutorial = headTutorial;
                int i = 0;
                while (currentTutorial != root && currentTutorial != null)
                {
                    i++;
                    currentTutorial = currentTutorial.GetNextTutorial();
                }
                Debug.Log("root index " + i);
                Tutorial tutorialToUncomplete = currentTutorial;
                while (tutorialToUncomplete != null)
                {
                    //Debug.Log("Started 4: " + tutorialToUncomplete.Started + " " + tutorialToUncomplete);
                    tutorialToUncomplete.Started = false;
                    //Debug.Log("Started 5: " + tutorialToUncomplete.Started + " " + tutorialToUncomplete);
                    //Debug.Log("Completed 2: " + tutorialToUncomplete.Completed + " " + tutorialToUncomplete);
                    tutorialToUncomplete.Completed = false;
                    //Debug.Log("Completed 3: " + tutorialToUncomplete.Completed + " " + tutorialToUncomplete);
                    tutorialToUncomplete = tutorialToUncomplete.GetNextTutorial();
                }
                TutorialLevel = i;
            }
            Tutorial a = headTutorial;
            while (a != null && a != currentTutorial)
            {
                //Debug.Log("Started 6: " + a.Started + " " + a);

                a.Started = true;
                //Debug.Log("Started 7: " + a.Started + " " + a);
                //Debug.Log("Completed 4: " + a.Completed + " " + a);
                a.Completed = true;
                //Debug.Log("Completed 5: " + a.Completed + " " + a);
                a = a.GetNextTutorial();
            }
            if (currentTutorial != null)
            {
                currentTutorial.StartCheckingRequirements();
            }
        }
        else
        {
            Tutorial a = headTutorial;
            while (a != null && a != currentTutorial)
            {
                //Debug.Log("Started 8: " + a.Started + " " + a);
                a.Started = true;
                //Debug.Log("Started 9: " + a.Started + " " + a);
                //Debug.Log("Completed 6: " + a.Completed + " " + a);
                a.Completed = true;
                //Debug.Log("Completed 7: " + a.Completed + " " + a);
                a = a.GetNextTutorial();
            }
            allTutorialsCompleted.Value = true;
        }


    }
    public void SkipAllAndReload()
    {
        TutorialLevel = int.MaxValue;
        GameManager.Instance.LoadCurrentLevel();
    }
    public void SetTutorialLevel(int tutorialLevel)
    {
        TutorialLevel = tutorialLevel;
    }
    Tween moveTween0, moveTween1, delayedTween;
    public void Focus(Transform target, float transitionDuration, float waitDuration, bool moveCamera, float offsetY = 0)
    {
        Debug.Log("Focus", target);
        onTutorialStarted.Invoke();
        targetPointer.gameObject.SetActive(true);
        targetPointer.position = target.position + offsetY * Vector3.up;
        moveTween0.Kill(true);
        delayedTween.Kill(true);
        moveTween1.Kill(true);
        if (moveCamera/* && GameManager.Instance.GetNumberConfig(disableCameraMovement) <= 0*/)
        {
            mainCameraController.enabled = false;
            Vector3 lastPos = mainCameraController.transform.position;
            Vector3 offset = lastPos - PlayerTutorialArrow.Instance.transform.position;


            moveTween0 = mainCameraController.transform.DOMove(target.position + offset, transitionDuration).OnComplete(() =>
             {
                 moveTween0 = null;
                 delayedTween = DOVirtual.DelayedCall(waitDuration, () =>
                 {
                     delayedTween = null;
                     moveTween1 = mainCameraController.transform.DOMove(lastPos, transitionDuration).OnComplete(() =>
                     {
                         moveTween1 = null;
                         onTutorialFinished.Invoke();
                         mainCameraController.enabled = true;
                     }).SetEase(Ease.OutQuad);
                 }, false);
             }).SetEase(Ease.OutQuad);
        }
        else
        {
            onTutorialFinished.Invoke();
        }

    }

    public void TutorialComplete()
    {
        Debug.Log("TutorialComplete");
        targetPointer.gameObject.SetActive(false);
    }

    public void ShowInfoText(string text, Sprite icon)
    {
        objectivePanel.SetIcon(icon);
        objectivePanel.Show();
        infoText.text = text;
    }

    public void CleanText()
    {
        objectivePanel.Hide();
    }


}
