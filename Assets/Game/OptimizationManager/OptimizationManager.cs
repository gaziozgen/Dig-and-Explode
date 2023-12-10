using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OptimizationManager : MonoBehaviour
{
    static OptimizationManager instance = null;
    public static OptimizationManager Instance => instance;

    [Header("Lock Settings")]
    [SerializeField] float focusTime = 1;
    [SerializeField] float focusSpeed = 0.1f;
    [SerializeField] float maxDistanceToLock = 0.1f;
    [SerializeField] int checkedRigidbodiedPerFrame = 10;

    [Header("Unlock Area")]
    [SerializeField] Transform toolPos;
    [SerializeField] float toolRadius = 1.5f;
    [SerializeField] int unlockAreaArraySize = 500;
    [SerializeField] float upperCheckCenterHeight = 5;

    [Header("Other")]
    [SerializeField] int maxDugCount = 100;
    //[SerializeField] int minMaxDugCount = 100;
    [SerializeField] LayerMask dugLayerMask;
    [SerializeField] DugRuntimeSet runtimeSet;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI protectedParts;
    [SerializeField] TextMeshProUGUI activeParts;
    [SerializeField] TextMeshProUGUI focusedParts;
    [SerializeField] TextMeshProUGUI lockedParts;

    List<Ore> items => runtimeSet.List;
    List<(Ore, float, Vector3)> focusedOres = new();
    float squaredFocusSpeed;
    float squaredlockDistance;
    Collider[] dugsOnArea;
    int dugCountOnArea = 0;
    int currentCheckIndex = 0;

    private void Awake()
    {
        instance = this;
        dugsOnArea = new Collider[unlockAreaArraySize];
        squaredFocusSpeed = Mathf.Pow(focusSpeed, 2);
        squaredlockDistance = Mathf.Pow(maxDistanceToLock, 2);
        runtimeSet.Clear();
        runtimeSet.OnAdd += OnDugAdd;
    }

    int lockedOres = 0;
    private void FixedUpdate()
    {
        UnlockArea(toolPos.position, toolRadius);

        if (items.Count > 0)
        {
            // yakýnda olmayan ve hýzý yavaþ duglarý focusa al
            int controlCount = Mathf.Min(items.Count, checkedRigidbodiedPerFrame);
            for (int i = 0; i < controlCount; i++)
            {
                Ore ore = items[(currentCheckIndex + i) % items.Count];
                ore.SavePos();
                if (!IsInUnlockArea(ore) && NotInFocusedOres(ore) && ore.Rigidbody.velocity.sqrMagnitude <= squaredFocusSpeed)
                    FocusOre(ore);
            }
            currentCheckIndex += controlCount;

            // zamaný gelen focus objelerini kontrol et ve karar var
            while (focusedOres.Count > 0 && focusedOres[0].Item2 <= Time.time)
            {
                if (focusedOres[0].Item1 != null && (focusedOres[0].Item3 - focusedOres[0].Item1.transform.position).sqrMagnitude <= squaredlockDistance && !IsInUnlockArea(focusedOres[0].Item1))
                {
                    lockedOres++;
                    focusedOres[0].Item1.SetRigidbodyKinematic(true);
                }
                focusedOres.RemoveAt(0);
            }
        }

        /*int valueTotal = 0;
        for (int i = 0; i < items.Count; i++) valueTotal += items[i].OreValue;*/
        protectedParts.text = dugCountOnArea + " protected particles";
        activeParts.text = items.Count + "/" + maxDugCount + " active particles";
        focusedParts.text = focusedOres.Count + " focused particles";
        lockedParts.text = lockedOres + " locked particles";
        //Debug.Log("in runtime set: " + items.Count + " - on focus: " + focusedOres.Count + " - locked: " + lockedOres + " - in active area: " + dugCountOnArea);
    }

    /*private void Update()
    {
        if (maxDugCount > minMaxDugCount)
        {
            if (Time.deltaTime > 1 / 60) maxDugCount -= 2;
            else if (Time.deltaTime > 1 / 100) maxDugCount -= 1;
        }
    }*/

    private void OnDugAdd()
    {
        int oversupply = items.Count - maxDugCount;
        if (oversupply <= 0) return;

        for (int i = 0; i < oversupply; i++)
        {
            DiggerMachine.Instance.AddMine(items[0].Type, items[0].OreValue);
            items[0].DestroyOre(true);
            /*bool transferred = false;
            for (int j = items.Count - 1; j >= 0; j--)
                if (items[0].Type == items[j].Type)
                {
                    transferred = true;
                    items[j].AddOreValue(items[0].OreValue);
                    items[0].DestroyOre();
                    items.RemoveAt(0);
                }

            if (!transferred)
            {
                DiggerMachine.Instance.AddOre(items[0].Type, items[0].OreValue);
                items[0].DestroyOre();
                items.RemoveAt(0);
            }*/
        }
    }

    public void UnlockArea(Vector3 objCenter, float radius, bool upperCheck = true)
    {
        Vector3 center = objCenter + (upperCheck? Vector3.up * upperCheckCenterHeight: Vector3.zero);
        Vector3 range = new Vector3(radius, radius, 1) + (upperCheck ? Vector3.up * upperCheckCenterHeight : Vector3.zero);
        dugCountOnArea = Physics.OverlapBoxNonAlloc(center, range, dugsOnArea, Quaternion.identity, dugLayerMask, QueryTriggerInteraction.Ignore);

        /*// yakýndaki duglardan biri focusListesinde ise çýkart
        for (int i = focusedOres.Count - 1; i > 0; i--)
            if (InNearDugs(focusedOres[i].Item1)) focusedOres.RemoveAt(i);*/

        // alanda kilitli dug varsa kilidini aç
        for (int i = 0; i < dugCountOnArea; i++)
        {
            Ore ore = dugsOnArea[i].GetComponent<Ore>();
            if (ore.IsRigidbodyKinematic)
            {
                ore.SetRigidbodyKinematic(false);
                lockedOres--;
            }
        }
    }

    private void FocusOre(Ore ore)
    {
        focusedOres.Add((ore, Time.time + focusTime, ore.transform.position));
    }

    private bool NotInFocusedOres(Ore ore)
    {
        for (int i = 0; i < focusedOres.Count; i++)
            if (focusedOres[i].Item1 == ore) return false;
        return true;
    }

    private bool IsInUnlockArea(Ore ore)
    {
        for (int i = 0; i < dugCountOnArea; i++)
            if (dugsOnArea[i] == ore.DynamicCollider) return true;
        return false;
    }
}
