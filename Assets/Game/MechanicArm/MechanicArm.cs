using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames.Core;

public class MechanicArm : MonoBehaviour
{
    //[SerializeField] float duration;

    [SerializeField] Vector2 rootAngleRange;
    [SerializeField] Vector2 arm1AngleRange;
    [SerializeField] Vector2 arm2AngleRange;

    [SerializeField] float[] moveSpeeds;
    [SerializeField] int[] carryCounts;

    [SerializeField] IntVariable carryCountLevel;
    [SerializeField] IntVariable speedLevel;

    [SerializeField] DiggerMachine diggerMachine;
    [SerializeField] Transform root;
    [SerializeField] Transform arm1;
    [SerializeField] Transform arm2;
    [SerializeField] Transform tip;

    [SerializeField] Transform holdPositionsParent;

    List<Transform> holdPositions = new();
    List<Mine> holdedMines = new();
    Sequence sequence;

    private void Awake()
    {
        for (int i = 0; i < holdPositionsParent.childCount; i++)
            holdPositions.Add(holdPositionsParent.GetChild(i));

        SetupSequence();
        speedLevel.OnValueChanged.AddListener((int before, int after) => SetupSequence());
    }

    private void Start()
    {
        CheckMineLeft();
    }

    public void CheckMineLeft()
    {
        if (diggerMachine.MineCount() > 0)
        {
            if (!sequence.IsPlaying())
                sequence.Play();
        }
        else if (sequence.IsPlaying())
            sequence.Pause();
    }

    float[] timeRatios = { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
    private float JobTime(int jobNo)
    {
        return timeRatios[jobNo] * moveSpeeds[speedLevel.Value];
    }

    private void SetupSequence()
    {
        sequence = DOTween.Sequence();
        sequence.SetLoops(-1);
        sequence.Pause();

        // alma
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.y, JobTime(0)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.y, JobTime(0)).OnComplete(() => StartCoroutine(CollectMines())));

        // kaldýrma
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.x, JobTime(1)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.x, JobTime(1)));

        // döndürme
        sequence.Append(root.DOLocalRotate(Vector3.up * rootAngleRange.y, JobTime(2)));

        // býrakma
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.y, JobTime(3)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.y, JobTime(3)).OnComplete(() => StartCoroutine(Drop())));

        // ilk haline dönme
        sequence.Append(arm1.DOLocalRotate(Vector3.right * arm1AngleRange.x, JobTime(4)));
        sequence.Join(arm2.DOLocalRotate(Vector3.right * arm2AngleRange.x, JobTime(4)));
        sequence.Join(root.DOLocalRotate(Vector3.up * rootAngleRange.x, JobTime(4)).OnComplete(CheckMineLeft));
    }

    private IEnumerator CollectMines()
    {
        sequence.Pause();
        int collectCount = Mathf.Min(diggerMachine.MineCount(), carryCounts[carryCountLevel.Value]);
        for (int i = 0; i < collectCount; i++)
        {
            Mine mine = diggerMachine.GetMine();
            mine.transform.parent = tip;
            mine.transform.position = holdPositions[holdedMines.Count].position;
            mine.transform.rotation = holdPositions[holdedMines.Count].rotation;
            holdedMines.Add(mine);
            yield return new WaitForSeconds(0.1f);
        }
        sequence.Play();
    }

    private IEnumerator Drop()
    {
        sequence.Pause();
        while (holdedMines.Count > 0)
        {
            Mine mine = holdedMines[holdedMines.Count - 1];
            holdedMines.RemoveAt(holdedMines.Count - 1);
            Destroy(mine.gameObject);
            yield return new WaitForSeconds(0.1f);
        }

        sequence.Play();
    }
}
