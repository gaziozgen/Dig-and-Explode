using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ObjectivePanel : UIElement
{
    [SerializeField] RectTransform container;
    Vector2 initialAnchoredPosition;
    [SerializeField] Image iconImage;
    [SerializeField] RectTransform checkImage;
    [SerializeField] TMPro.TextMeshProUGUI quantityText;
    [SerializeField] Slider quantitySlider;
    int currentQuantity = 0;
    int maxQuantity = 1;
    //Tween scale1, delay1, delay2, scale2, pos1;
    Sequence hideSequence;

    private void Awake()
    {
        initialAnchoredPosition = container.anchoredPosition;
    }

    public override void Show()
    {
        base.Show();
        checkImage.gameObject.SetActive(false);

        /*if (scale1 != null)
        {
            scale1.Kill();
            scale1 = null;
        }
        if (delay1 != null)
        {
            delay1.Kill();
            delay1 = null;
        }
        if (delay2 != null)
        {
            delay2.Kill();
            delay2 = null;
        }
        if (scale2 != null)
        {
            scale2.Kill();
            scale2 = null;
        }
        if (pos1 != null)
        {
            pos1.Kill();
            pos1 = null;
        }*/
        /*DOTween.Kill(container);
        DOTween.Kill(this);*/
        //Debug.LogWarning("###################### Show " + TutorialManager.Instance.TutorialLevel);
        container.anchoredPosition = initialAnchoredPosition + Vector2.down * 150;
        container.localScale = Vector3.zero;

        if (hideSequence != null) hideSequence.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOVirtual.DelayedCall(2.05f, () =>
        {
            //container.DOScale(Vector3.one * 1f, 0.3f);
        }, false));
        sequence.Join(container.DOScale(Vector3.one * 1.2f, 1f).SetEase(Ease.OutElastic));
        sequence.Append(container.DOAnchorPosY(initialAnchoredPosition.y, 0.4f).SetEase(Ease.OutBack));
        sequence.Join(container.DOScale(Vector3.one * 1f, 0.3f));

        /*container.DOScale(Vector3.one * 1.2f, 1f).SetEase(Ease.OutElastic);
        DOVirtual.DelayedCall(2.05f, () =>
        {
            container.DOScale(Vector3.one * 1f, 0.3f);
        });
        DOVirtual.DelayedCall(2, () =>
        {
            container.DOAnchorPosY(initialAnchoredPosition.y, 0.4f).SetEase(Ease.OutBack);
        });*/
        /*scale1 = container.DOScale(Vector3.one * 1.2f, 1f).SetEase(Ease.OutElastic).OnComplete(() =>
        {
            scale1 = null;
        });
        delay1 = DOVirtual.DelayedCall(2.05f, () =>
        {
            scale2 = container.DOScale(Vector3.one * 1f, 0.3f).OnComplete(() =>
            {
                scale2 = null;
            });
        }).OnComplete(() =>
        {
            delay1 = null;
        });
        delay2 = DOVirtual.DelayedCall(2, () =>
        {
            pos1 = container.DOAnchorPosY(initialAnchoredPosition.y, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                pos1 = null;
            });
        }).OnComplete(() =>
        {
            delay2 = null;
        });*/

    }

    public override void Hide()
    {
        //Debug.LogWarning("HIDE");
        checkImage.localScale = Vector3.zero;
        checkImage.gameObject.SetActive(true);

        hideSequence = DOTween.Sequence();
        hideSequence.Append(checkImage.DOScale(Vector3.one, 1).SetEase(Ease.OutBack));
        hideSequence.Append(container.DOScale(Vector3.zero, 1).SetEase(Ease.InBack));
        hideSequence.OnComplete(() =>
        {
            hideSequence = null;
            base.Hide();
        });

    }

    public void SetIcon(Sprite iconSprite)
    {
        iconImage.sprite = iconSprite;
    }

    public void SetMaxQuantity(int quantity)
    {
        if (quantity == 0) return;
        maxQuantity = quantity;
        quantityText.text = $"{currentQuantity}/{maxQuantity}";
        AdjustSlider();
    }
    public void SetCurrentQuantity(int quantity)
    {
        currentQuantity = quantity;
        quantityText.text = $"{currentQuantity}/{maxQuantity}";
        AdjustSlider();
    }

    public void AdjustSlider()
    {
        quantitySlider.value = (float)currentQuantity / maxQuantity;
    }

}
