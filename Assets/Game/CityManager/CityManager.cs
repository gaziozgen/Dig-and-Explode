using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    [SerializeField] GameObject[] customerPrefabs;
    [SerializeField] Transform spawnPositionsParent;

    static CityManager instance = null;
    public static CityManager Instance => instance;

    FateObjectPool<Customer>[] customerPools;
    List<Vector3> spawnPositions = new();

    private void Awake()
    {
        instance = this;
        customerPools = new FateObjectPool<Customer>[customerPrefabs.Length];

        for (int i = 0; i < customerPrefabs.Length; i++)
            customerPools[i] = new(customerPrefabs[i], true, 10, 30);

        for (int i = 0; i < spawnPositionsParent.childCount; i++)
            spawnPositions.Add(spawnPositionsParent.GetChild(i).position);

    }

    public Customer GetCustomer()
    {
        Customer customer = customerPools[UnityEngine.Random.Range(0, customerPools.Length)].Get();
        customer.Agent.Warp(RandomSpawnPos());
        return customer;
    }

    public void ReciveCustomer(Customer customer, Action onRecive)
    {
        if (customer.Coroutine != null) Debug.LogError("not empty coroutine", customer);
        customer.Coroutine = StartCoroutine(SendCustomer(customer, onRecive));
    }

    private IEnumerator SendCustomer(Customer customer, Action onRecive)
    {
        customer.Agent.GoToClosestPoint(RandomSpawnPos());
        yield return customer.Agent.WaitUntilReached;
        onRecive();
        customer.Coroutine = null;
        //Debug.Log("release by city manager", customer);
        customer.Release();
    }

    private Vector3 RandomSpawnPos()
    {
        int i = UnityEngine.Random.Range(0, spawnPositions.Count - 1);
        return spawnPositions[i];
    }
}
