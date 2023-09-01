using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class FlyingMoney : FateMonoBehaviour, IPooledObject
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform imageTransform = null;
    [SerializeField] private SoundEntity coinSound = null;

    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        canvas.enabled = true;
    }

    public void OnRelease()
    {
        canvas.enabled = false;
    }

    public void Levitate(int value, float size, float pathHeight, Vector2 startPosition)
    {
        GameManager.Instance.PlaySound(coinSound);
        MoneyUI.Instance.AddMoney(value);

        float duration = 0.4f;
        float endSize = 1f;

        Vector3 target = startPosition + pathHeight * Vector2.up;
        imageTransform.position = startPosition;
        imageTransform.localScale = Vector3.one * size;
        imageTransform.DOScale(Vector3.one * endSize, duration).SetEase(Ease.Linear);
        DOTween.To(() => imageTransform.position, (x) => imageTransform.position = x, target, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Release();
        });
    }

    public void DirectGoToUI(int amount, float startSize, Vector3 startPositon)
    {
        float duration = 0.6f;
        float endSize = 1f;

        Vector3 target = MoneyUI.Instance.MoneyTarget;
        imageTransform.position = startPositon;
        imageTransform.localScale = Vector3.one * startSize;
        imageTransform.DOScale(Vector3.one * endSize, duration).SetEase(Ease.InCubic);
        DOTween.To(() => imageTransform.position, (x) => imageTransform.position = x, target, duration).SetEase(Ease.InCubic).OnComplete(() =>
        {
            MoneyUI.Instance.AddMoney(amount);
            GameManager.Instance.PlaySound(coinSound);
            Release();
        });
    }

    public void GoUIWithBurstMove(int amount, Vector2 startPositon, Vector2 midPosition)
    {
        imageTransform.position = startPositon;
        imageTransform.localScale = Vector3.one;
        imageTransform.DOMove(midPosition, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Vector3 target = MoneyUI.Instance.MoneyTarget;
            float randomTimeRange = 1f + UnityEngine.Random.Range(-0.3f, 0.3f);
            DOTween.To(() => imageTransform.position, (x) => imageTransform.position = x, target, randomTimeRange).SetEase(Ease.InCubic).OnComplete(() =>
            {
                MoneyUI.Instance.AddMoney(amount);
                GameManager.Instance.PlaySound(coinSound);
                Release();
            });
        });
    }
}
