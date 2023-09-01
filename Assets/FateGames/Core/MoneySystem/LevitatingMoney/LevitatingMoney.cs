using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevitatingMoney : FateMonoBehaviour, IPooledObject
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] Canvas canvas;
    [SerializeField] Color baseTextColor;

    Transform camTransform = null;

    private void Awake()
    {
        camTransform = Camera.main.transform;
    }

    private void Update()
    {
        transform.LookAt(camTransform.position);
    }

    public void Levitate(int value, float pathHeight, Vector3 spawnPosition)
    {
        float duration = 0.6f;
        float fadeDuration = 0.1f;
        moneyText.text = "$" + value.ToString();
        moneyText.alpha = 1;

        moneyText.DOKill();
        DOVirtual.DelayedCall(duration - fadeDuration, () =>
        {
            moneyText.DOFade(0, fadeDuration);
        });
        transform.position = spawnPosition;

        transform.DOMove(spawnPosition + Vector3.up * pathHeight, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Release();
        });
    }

    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        canvas.enabled = true;
    }

    public void OnRelease()
    {
        canvas.enabled = false;
    }
}
