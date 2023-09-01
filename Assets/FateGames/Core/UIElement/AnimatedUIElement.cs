using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimatedUIElement : UIElement
{
    [SerializeField] RectTransform container;
    public override void Show()
    {
        base.Show();
        DOTween.Kill(transform);
        container.localScale = Vector3.zero;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(container.DOScale(Vector3.one * 1.1f, 0.15f));
        mySequence.Append(container.DOScale(Vector3.one, 0.05f));
    }

}
