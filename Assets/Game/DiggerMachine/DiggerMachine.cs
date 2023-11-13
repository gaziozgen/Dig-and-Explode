using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiggerMachine : MonoBehaviour
{
    [SerializeField] float minePassRate = 0.5f;
    [SerializeField] Transform useTransform;
    [SerializeField] UnityEvent UseMachine;
    [SerializeField] GameObject[] minePrefabs;
    [SerializeField] Transform pileCenter;
    [SerializeField] Vector3 itemSize;
    [SerializeField] Vector2 dimensions;

    static DiggerMachine instance = null;
    public static DiggerMachine Instance => instance;

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

    public void Use(Transform player)
    {
        UseMachine.Invoke();
        player.transform.position = useTransform.position;
        player.transform.forward = useTransform.forward;
    }

    public void AddOre(int type, int value)
    {
        mineCounts[type] += minePassRate * value;
        while (mineCounts[type] > 1)
        {
            mineCounts[type] -= 1;
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
}
