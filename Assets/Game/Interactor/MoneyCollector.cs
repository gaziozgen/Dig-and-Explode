using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.Events;
using DG.Tweening;

[DisallowMultipleComponent]
[RequireComponent(typeof(Interactor))]
public class MoneyCollector : FateMonoBehaviour
{
    [SerializeField] SoundEntity moneyPushSound;
    [SerializeField] UnityEvent onMoneyCollectingZoneInteracted, onPoolPartyZoneInteracted;



    Interactor interactor;

    private void Awake()
    {
        interactor = GetComponent<Interactor>();
        for (int i = 0; i < 5; i++)
        {
            Interactor.Interaction moneyPileInteraction = new("Money Pile", InteractWithMoneyPile, () => true, 0.1f, false, true);
            interactor.AddInteraction(moneyPileInteraction);
        }
    }

    public IEnumerator InteractWithMoneyPile(GameObject obj)
    {
        MoneyPile moneyPile = obj.GetComponentInParent<MoneyPile>();
        if (moneyPile == null || moneyPile.Count == 0) yield break;
        int firstMoney = moneyPile.Count;
        int count = 0;
        while (moneyPile.Count > 0)
        {
            int remaining = Mathf.Clamp(Random.Range(1, firstMoney / 8), 1, moneyPile.Count);
            for (int i = 0; i < remaining; i++)
            {
                if (!moneyPile.TryRemove(out Money3D money)) break;
                MoneyUI.Instance.AddMoney(money.value);
                money.transform.DOJump(transform.position + Vector3.up * 1.5f, 2, 1, 0.2f).OnComplete(() => money.Release());
            }
            GameManager.Instance.PlayHaptic();
            SoundWorker worker = GameManager.Instance.PlaySound(moneyPushSound);
            //worker.SetPitch(1 + 0.05f * count++); ######################################################################################################
            yield return new WaitForSeconds(0.05f);
        }
        //AdManager.Instance.ShowInterstitial();

    }
}
