using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using DG.Tweening;

public class UpgradeableVIPRoomOption : FateMonoBehaviour
{
    [SerializeField] Transform[] itemsToAnimate;
    public int xp = 1;
    public float moneyMultiplier = 1;
    public Sprite roomImage;
    public bool rewarded = false;
    public System.Action getAction;
    public System.Action previewAction;

    public void AnimatePreview()
    {
        for (int i = 0; i < itemsToAnimate.Length; i++)
        {
            Transform item = itemsToAnimate[i];
            Vector3 defaultScale = item.localScale;
            Vector3 startScale = item.localScale;
            startScale.y /= 2f;
            item.localScale = startScale;
            item.DOScaleY(defaultScale.y, 0.2f).SetEase(Ease.OutBack);
        }
    }
}
