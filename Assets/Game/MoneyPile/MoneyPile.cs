using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FateGames.Core;
using DG.Tweening;
using UnityEngine.Events;

public class MoneyPile : FateMonoBehaviour
{
    [SerializeField] int width = 5, length = 5;
    [SerializeField] Vector3 offset = Vector3.one;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] int amount = 0;
    [SerializeField] UnityEvent onDepleted;
    [SerializeField] int capacity = 90;
    //[SerializeField] SoundEntity sound;
    //[SerializeField] MoneyPileRuntimeSet moneyPileRuntimeSet;
    List<Money3D> moneys = new();
    public int Count => moneys.Count;
    int floorCapacity => width * length;
    Vector3 pivot;


     void Awake()
    {
        pivot = new Vector3((width - 1) * offset.x / 2f, -offset.y / 2f, (length - 1) * offset.z / 2f);
        sphereCollider.radius = length * 0.75f;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        int value = Mathf.CeilToInt(amount / 30f);
        int totalAmount = 0;
        while (totalAmount < amount)
        {
            int floor = Count / floorCapacity;
            int index = Count % floorCapacity;
            int layer = Mathf.FloorToInt(Mathf.Sqrt(index));
            int layerIndex = index - Mathf.RoundToInt(Mathf.Pow(layer, 2));
            int groupIndex = layerIndex / 2;
            bool even = layerIndex % 2 == 0;
            Vector2Int groupPosition = new(layer, groupIndex);
            Vector2 floorPosition = even ? groupPosition : new Vector2Int(groupPosition.y, groupPosition.x);
            Vector3 position = transform.position - pivot + new Vector3(floorPosition.y * offset.x, floor * offset.y, floorPosition.x * offset.z);
            Money3D money = MoneyManager.Instance.MoneyPool.Get();
            money.value = Mathf.Clamp(value, 1, amount - totalAmount);
            totalAmount += money.value;
            moneys.Add(money);
            money.transform.SetPositionAndRotation(position, Quaternion.Euler(money.transform.eulerAngles + Vector3.up * Random.Range(-10f, 10f)));
        }

        if (amount == 0)
            onDepleted.Invoke();
    }

    public void Add(Money3D money)
    {
        if (Count >= capacity)
        {
            money.Release();
            return;
        }
        int floor = Count / floorCapacity;
        int index = Count % floorCapacity;
        int layer = Mathf.FloorToInt(Mathf.Sqrt(index));
        int layerIndex = index - Mathf.RoundToInt(Mathf.Pow(layer, 2));
        int groupIndex = layerIndex / 2;
        bool even = layerIndex % 2 == 0;
        Vector2Int groupPosition = new(layer, groupIndex);
        Vector2 floorPosition = even ? groupPosition : new Vector2Int(groupPosition.y, groupPosition.x);
        Vector3 position = transform.position - pivot + new Vector3(floorPosition.y * offset.x, floor * offset.y, floorPosition.x * offset.z);
        moneys.Add(money);
        amount += money.value;
        money.transform.DOJump(position, 2, 1, 0.2f);
        money.transform.DORotate(money.transform.eulerAngles + Vector3.up * Random.Range(-10f, 10f), 0.2f);

        //UpdateMoneyCount();
    }

    public void AddAmountOfMoney(int amount, int count, Vector3 from)
    {
        int value = Mathf.CeilToInt(amount / (float)count);
        int totalAmount = 0;
        while (totalAmount < amount)
        {
            Money3D money = MoneyManager.Instance.MoneyPool.Get(from);
            money.value = Mathf.Clamp(value, 1, amount - totalAmount);
            totalAmount += money.value;
            Add(money);
        }
    }

    public bool TryRemove(out Money3D money)
    {
        //GameManager.Instance.PlaySoundOneShot(sound);
        money = null;
        if (Count == 0) return false;
        money = moneys[^1];
        int moneyValue = money.value;
        moneys.RemoveAt(Count - 1);
        DOTween.Kill(money);
        amount -= moneyValue;

        if (amount == 0) onDepleted.Invoke();
        return true;
    }

    public int TotalValue()
    {
        /*int total = 0;
        for (int i = 0; i < moneys.Count; i++) total += moneys[i].value;
        return total;*/
        return amount;
    }

    public void ClearAllMoneys()
    {
        Money3D money;
        while (0 < Count)
        {
            TryRemove(out money);
            money.Release();
        }
    }

    /*private void UpdateMoneyCount()
    {
        if (CollectAllMoneys.Instance && CollectAllMoneys.Instance.isActiveAndEnabled) CollectAllMoneys.Instance.UpdateMoneyCount();
    }*/

    /*private void OnEnable()
    {
        moneyPileRuntimeSet.Add(this);
    }

    private void OnDisable()
    {
        moneyPileRuntimeSet.Remove(this);
    }*/
}

/*#if UNITY_EDITOR
[CustomEditor(typeof(MoneyPile))]
public class MoneyPileEditor : Editor
{
    MoneyPile moneyPile;
    private void OnEnable()
    {
        moneyPile = target as MoneyPile;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (Application.isPlaying && GUILayout.Button("Add Money"))
        {
            moneyPile.Add(Club.Instance.Money3DPool.Get());
        }
    }
}
#endif*/