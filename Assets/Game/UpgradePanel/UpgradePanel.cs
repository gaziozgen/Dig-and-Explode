using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class UpgradePanel : UIElement
{
    UpgradeCard[] cards;
    [SerializeField] RectTransform container;
    [SerializeField] UnityEvent onShow, onHide;
    bool opened = false;
    private void Awake()
    {
        cards = GetComponentsInChildren<UpgradeCard>();
    }

    public override void Show()
    {
        if (opened) return;
        opened = true;
        base.Show();
        DOTween.Kill(container);
        container.anchoredPosition = new Vector2(0, -1150);
        container.DOAnchorPos(Vector2.zero, 0.3f);
        if (onShow != null)
            onShow.Invoke();
    }

    public override void Hide()
    {
        if (!opened) return;
        opened = false;
        base.Hide();
        if (onHide != null)
            onHide.Invoke();
    }

    public void UpdateCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            UpgradeCard card = cards[i];
            if (card.gameObject.activeSelf)
            {
                card.UpdateCard();
            }
        }
    }
}
