using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiggerMachine : MonoBehaviour
{
    [SerializeField] float orePerMine = 5;
    [SerializeField] Transform useTransform;
    [SerializeField] UnityEvent UseMachine;
    [SerializeField] GameObject[] minePrefabs;
    [SerializeField] Transform pileCenter;
    [SerializeField] Vector3 itemSize;
    [SerializeField] Vector2 dimensions;

    static DiggerMachine instance = null;
    public static DiggerMachine Instance => instance;

    List<int> mineTypes = new();
    ItemPile<Mine> pile;
    List<FateObjectPool<Mine>> minePools = new();
    List<float> mineCounts = new();

    [SerializeField] GameEvent onMineCollected;

    private void Awake()
    {
        instance = this;
        pile = new ItemPile<Mine>(pileCenter, itemSize, dimensions);
        for (int i = 0; i < minePrefabs.Length; i++)
        {
            minePools.Add(new(minePrefabs[i], true, 30, 50));
            mineCounts.Add(0);
        }
    }

    private void Start()
    {
        for (int i = 0; i < mineTypes.Count; i++) AddMine(mineTypes[i], (int)orePerMine, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) AddMine(0, 5);
        if (Input.GetKeyDown(KeyCode.M)) MoneyUI.Instance.AddMoney(100);
    }

    public void Use(Transform player)
    {
        UseMachine.Invoke();
        player.transform.position = useTransform.position;
        player.transform.forward = useTransform.forward;
    }

    public void AddMine(int type, int value, bool updateTypesList = true)
    {
        if (type == -1) return;
        mineCounts[type] += value / orePerMine;
        while (mineCounts[type] >= 1)
        {
            mineCounts[type] -= 1;
            if (updateTypesList) mineTypes.Add(type);
            Place(minePools[type].Get());
        }
    }

    private void Place(Mine mine)
    {
        pile.AddItem(mine);
        onMineCollected.Raise();
    }

    public Mine GetMine()
    {
        return pile.GetItem();
    }

    public int MineCount()
    {
        return pile.Count;
    }

    public void DequeueTypeList()
    {
        mineTypes.RemoveAt(0);
    }

    public Mine GetMineForSellPoint(int type)
    {
        return minePools[type].Get();
    }
}
