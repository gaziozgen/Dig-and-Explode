using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames.Core;

public class MechanicArm : MonoBehaviour
{
    [SerializeField] DiggerMachine diggerMachine;
    [SerializeField] BeltManager mechanicBelt;
    //[SerializeField] float duration;

    [SerializeField] Vector2 rootAngleRange;
    [SerializeField] Vector2 arm1AngleRange;
    [SerializeField] Vector2 arm2AngleRange;

    [SerializeField] float[] moveSpeeds;
    [SerializeField] int[] carryCounts;

    [SerializeField] IntVariable carryCountLevel;
    [SerializeField] IntVariable speedLevel;

    [SerializeField] Transform root;
    [SerializeField] Transform arm1;
    [SerializeField] Transform arm2;
    [SerializeField] Transform tip;

    [SerializeField] Transform holdPositionsParent;

    [HideInInspector] public List<Mine> HoldedMines = new();

    List<Transform> holdPositions = new();
    Sequence sequence;
    bool upgradeOnNextLoop = false;
    WaitUntil waitUntilStackFinish;

    private void Awake()
    {
        waitUntilStackFinish = new(() => HoldedMines.Count == 0);
        for (int i = 0; i < holdPositionsParent.childCount; i++)
            holdPositions.Add(holdPositionsParent.GetChild(i));

        SetupSequence();
        speedLevel.OnValueChanged.AddListener((int before, int after) =>
        {
            upgradeOnNextLoop = true;
        });
    }

    private void Start()
    {
        CheckMineLeft();
    }

    public void CheckMineLeft()
    {
        if (upgradeOnNextLoop)
        {
            upgradeOnNextLoop = false;
            sequence.Kill();
            SetupSequence();
        }

        if (diggerMachine.MineCount() > 0)
        {
            if (!sequence.IsPlaying() && HoldedMines.Count == 0)
                sequence.Play();
        }
        else if (sequence.IsPlaying())
            sequence.Pause();
    }

    float[] timeRatios = { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f }; // total must be 1
    private float JobTime(int jobNo)
    {
        return timeRatios[jobNo] * moveSpeeds[speedLevel.Value];
    }

    private void SetupSequence()
    {
        sequence = DOTween.Sequence();
        sequence.SetLoops(-1);
        sequence.Pause();

        // almak icin yaklasma
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.y, JobTime(0)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.y, JobTime(0)).OnComplete(() => StartCoroutine(CollectMines())));

        // kaldýrma
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.x, JobTime(1)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.x, JobTime(1)));

        // döndürme
        sequence.Append(root.DOLocalRotate(Vector3.up * rootAngleRange.y, JobTime(2)));

        // býrakmak ici yaklasma
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.y, JobTime(3)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.y, JobTime(3)).OnComplete(() => StartCoroutine(Drop())));

        // ilk haline dönme
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.x, JobTime(4)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.x, JobTime(4)));
        sequence.Join(root.DOLocalRotate(Vector3.up * rootAngleRange.x, JobTime(4)).OnComplete(CheckMineLeft));
    }

    private IEnumerator CollectMines()
    {
        float collectDuration = 0.2f;
        float collectInterval = 0.1f;
        sequence.Pause();
        int collectCount = Mathf.Min(diggerMachine.MineCount(), carryCounts[carryCountLevel.Value]);
        for (int i = 0; i < collectCount; i++)
        {
            Mine mine = diggerMachine.GetMine();
            mine.transform.parent = tip;
            mine.transform.DOMove(holdPositions[HoldedMines.Count].position, collectDuration);
            Tween tween = mine.transform.DORotateQuaternion(holdPositions[HoldedMines.Count].rotation, collectDuration);
            HoldedMines.Add(mine);

            if (i == collectCount - 1) tween.OnComplete(() => sequence.Play());
            yield return new WaitForSeconds(collectInterval);
        }
    }

    private IEnumerator Drop()
    {
        sequence.Pause();
        StartCoroutine(mechanicBelt.TakeMines());
        yield return waitUntilStackFinish;
        sequence.Play();
    }
}
