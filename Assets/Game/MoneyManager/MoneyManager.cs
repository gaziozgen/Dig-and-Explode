using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance = null;
    [SerializeField] GameObject money3dPrefab;
    [HideInInspector] public FateObjectPool<Money3D> MoneyPool = null;

    private void Awake()
    {
        Instance = this;
        MoneyPool = new FateObjectPool<Money3D>(money3dPrefab, true, 50, 100);
    }

}
