using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup voyagerCanvasGroup, fateCanvasGroup;
    [SerializeField] float voyagerDelay;
    [SerializeField] float fateDelay;
    [SerializeField] float stopDuration = 1;
    [SerializeField] float animationDuration = 1;
    public bool Finished = false;

    private void Start()
    {
        voyagerCanvasGroup.alpha = 0;
        DOVirtual.DelayedCall(voyagerDelay, () =>
        {
            voyagerCanvasGroup.DOFade(1, animationDuration).OnComplete(() =>
            {
                DOVirtual.DelayedCall(stopDuration, () =>
                {
                    voyagerCanvasGroup.DOFade(0, animationDuration).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        Finished = true;
                    });
                });
            });
        });

        fateCanvasGroup.alpha = 0;
        DOVirtual.DelayedCall(fateDelay, () =>
        {
            fateCanvasGroup.DOFade(1, animationDuration).OnComplete(() =>
            {
                DOVirtual.DelayedCall(stopDuration - (fateDelay - voyagerDelay), () =>
                {
                    fateCanvasGroup.DOFade(0, animationDuration).OnComplete(() =>
                    {

                    });
                });
            });
        });
    }
}
