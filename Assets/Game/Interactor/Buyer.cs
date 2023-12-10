using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Interactor))]
public class Buyer : FateMonoBehaviour
{
    [SerializeField] float buyingZoneSpeedFloor = 1.02f;
    [SerializeField] SoundEntity moneyPushSound;
    [SerializeField] SoundEntity buyingSound;
    [SerializeField] UnityEvent onHiringZoneInteracted, onPlayerZoneInteracted, onPlayerZoneLeaved;
    Interactor.Interaction upgradeZoneInteraction, interactAreaInteraction, offerZoneInteraction, buyingZoneInteraction;
    Interactor interactor;

    private void Awake()
    {
        interactor = GetComponent<Interactor>();
        buyingZoneInteraction = new("Buying Zone", InteractWithBuyingZone, () => MoneyUI.Money > 0, 0.5f, true, false);
        interactor.AddInteraction(buyingZoneInteraction);
        interactAreaInteraction = new("Level Portal", InteractWithLevelPortal, () => true, 0.5f, true, false);
        interactor.AddInteraction(interactAreaInteraction);
        upgradeZoneInteraction = new("Upgrade Zone", InteractWithUpgradeZone, () => true, 0.5f, true, false);
        interactor.AddInteraction(upgradeZoneInteraction);
    }

    public IEnumerator InteractWithBuyingZone(GameObject obj)
    {
        BuyingZone buyingZone = obj.GetComponent<BuyingZone>();
        if (!buyingZone) yield break;
        if (buyingZone.locked)
        {
            buyingZoneInteraction.onUnlock.RemoveAllListeners();
            buyingZoneInteraction.onUnlock.AddListener(() => { buyingZone.locked = false; buyingZoneInteraction.onUnlock.RemoveAllListeners(); });
            yield break;
        }
        int count = 0;
        while (MoneyUI.Money > 0 && buyingZone.CanGiveMoney)
        {
            GameManager.Instance.PlayHaptic();
            Money3D money = MoneyManager.Instance.MoneyPool.Get();
            money.transform.position = transform.position + Vector3.up * 1.5f;
            money.value = Mathf.Clamp(Mathf.Clamp(Mathf.FloorToInt(Mathf.Pow(buyingZoneSpeedFloor, count)), 1, buyingZone.Price), 1, MoneyUI.Money);
            MoneyUI.Instance.SpendMoney(money.value);
            GameManager.Instance.PlaySound(moneyPushSound);
            buyingZone.GiveMoney(money);
            if (!buyingZone.CanGiveMoney)
            {
                GameManager.Instance.PlaySound(buyingSound);
                AdManager.Instance.ShowInterstitial();
            }
            count++;
            yield return new WaitForSeconds(Mathf.Clamp(0.05f / Mathf.Pow(buyingZoneSpeedFloor, count), 0.02f, 1f));
        }
    }

    public IEnumerator InteractWithLevelPortal(GameObject obj)
    {
        InteractionAreaWithDuration InteractArea = obj.GetComponent<InteractionAreaWithDuration>();
        if (!InteractArea) yield break;
        interactAreaInteraction.onUnlock.RemoveAllListeners();
        interactAreaInteraction.onUnlock.AddListener(() => InteractArea.UpdateFillImage(0));
        float duration = 1;
        float remainingTime = duration;
        float step = Time.deltaTime;
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(step);
            remainingTime -= step;
            InteractArea.UpdateFillImage(1f - (remainingTime / duration));
        }
        if (InteractArea.HasMachine) InteractArea.UseMachine(transform);
        else InteractArea.Use();
    }

    public IEnumerator InteractWithUpgradeZone(GameObject obj)
    {
        UpgradeZone upgradeZone = obj.GetComponent<UpgradeZone>();
        if (!upgradeZone) yield break;
        upgradeZoneInteraction.onUnlock = upgradeZone.OnLeaved;
        GameManager.Instance.PlayHaptic();
        upgradeZone.Interact();
        yield break;
    }

}
