using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] UpgradeableVIPRoom upgradeableRoom;
    [SerializeField] float[] baseIncomesPerSecond;
    [SerializeField] float vipIncomeBonus = 1.5f;
    [SerializeField] float orderDuration = 1;
    [SerializeField] float callCustomerInterval = 1;
    [SerializeField] List<Transform> Seats;
    [SerializeField] Transform QueueHead;
    [SerializeField] int maxQueueLength = 5;
    [SerializeField] Vector2 sitDurationRange = Vector2.one;
    [SerializeField] float defaultPaymentCheckTime = 10;
    [SerializeField] Animator cashier;
    [SerializeField] MoneyPile moneyPile;

    float IncomePerSecond => baseIncomesPerSecond[upgradeableRoom.Level] * (upgradeableRoom.optionIndex == 2 ? vipIncomeBonus : 1);
    bool HaveAvaliableSeat => seatAvaliableInfoList.Contains(true);

    PersonQueue<Customer> customerQueue;
    List<Customer> customerList = new();
    List<bool> seatAvaliableInfoList = new();
    WaitForSeconds waitForOrderDuration;
    int level = 0;
    float lastPayTime = 0;
    bool isVisualActive = true;

    private void Awake()
    {
        waitForOrderDuration = new(orderDuration);

        for (int i = 0; i < Seats.Count; i++)
            seatAvaliableInfoList.Add(true);

        customerQueue = new(QueueHead, 2, maxQueueLength,
            (Customer c) => StartCoroutine(CustomerBehaviour(c)),
            (Coroutine r) => StopCoroutine(r));
    }

    private void Start()
    {
        InvokeRepeating(nameof(CallCustomer), 0, callCustomerInterval);
        InvokeRepeating(nameof(DefaultPaymentOnVisualNotActive), defaultPaymentCheckTime, defaultPaymentCheckTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) CallCustomer();
        if (Input.GetKeyDown(KeyCode.T)) SetVisualActive(!isVisualActive);
    }

    private IEnumerator CustomerBehaviour(Customer customer)
    {
        yield return customer.Agent.WaitUntilReached;
        customer.transform.forward = QueueHead.forward;
        cashier.SetTrigger("Play");
        yield return waitForOrderDuration;
        customerQueue.Dequeue();
        PaymentFrom(customer.transform.position);
        if (HaveAvaliableSeat)
        {
            Transform seat = GetSeat();
            //Debug.Log("go customer sit", customer);
            customer.Agent.GoToClosestPoint(seat.position);
            yield return customer.Agent.WaitUntilReached;
            customer.transform.forward = seat.forward;
            customer.SetSit(true);
            yield return new WaitForSeconds(Random.Range(sitDurationRange.x, sitDurationRange.y));
            ReturnSeat(seat);
            customer.SetSit(false);
        }
        customer.Coroutine = null;
        CityManager.Instance.ReciveCustomer(customer, () => customerList.Remove(customer));
    }

    /*public void SetLevel(int newLevel)
    {
        levels[level].gameObject.SetActive(false);
        level = newLevel;
        levels[level].gameObject.SetActive(true);
        UpdateShop();
    }*/

    public void SetVisualActive(bool isAactive)
    {
        //Debug.Log("SetVisualActive " + isAactive);
        if (isVisualActive == isAactive) Debug.LogError("AGA NAPTIN AGA");
        isVisualActive = isAactive;

        if (isVisualActive) CallCustomer();
        else
        {
            for (int i = 0; i < customerList.Count; i++)
            {
                Customer customer = customerList[i];
                //Debug.Log("customer kill", customer);
                if (customer.Coroutine != null)
                {
                    StopCoroutine(customer.Coroutine);
                    customer.Coroutine = null;
                }
                customer.Agent.Stop();
                customer.Release();
            }
            customerQueue.Clear();
            customerList.Clear();
            ClearSeats();
        }
        cashier.gameObject.SetActive(isVisualActive && upgradeableRoom.Level != 0);
    }

    public void ResetTimer()
    {
        lastPayTime = Time.time;
    }

    private void DefaultPaymentOnVisualNotActive()
    {
        if (isVisualActive || upgradeableRoom.Level == 0) return;
        PaymentFrom(moneyPile.transform.position);
    }

    private void PaymentFrom(Vector3 pos)
    {
        int cost = NextProductCost();
        int count = Mathf.CeilToInt(Mathf.Sqrt(cost / 4));
        moneyPile.AddAmountOfMoney(cost, count, pos);
    }

    private int NextProductCost()
    {
        int cost = Mathf.CeilToInt((Time.time - lastPayTime) * IncomePerSecond);
        lastPayTime = Time.time;
        return cost;
    }

    /*private void UpdateShop()
    {
        customerQueue.SetStartPosition(levels[level].QueueHead);
        cashier.transform.position = levels[level].CashierSpot.position;
        cashier.transform.forward = levels[level].CashierSpot.forward;
        // eski müþteriler eski masalarda kalkana kadar kalýcak
    }*/

    private void CallCustomer()
    {
        if (customerQueue.Length >= maxQueueLength || !isVisualActive || upgradeableRoom.Level == 0) return;
        Customer customer = CityManager.Instance.GetCustomer();
        //Debug.Log("customer spawn", customer);
        customerList.Add(customer);
        customerQueue.Enqueue(customer);
    }

    private Transform GetSeat()
    {
        for (int i = 0; i < seatAvaliableInfoList.Count; i++)
        {
            if (seatAvaliableInfoList[i])
            {
                seatAvaliableInfoList[i] = false;
                return Seats[i];
            }
        }
        Debug.LogError("AGA NOLUYO AGA");
        return null;
    }

    private void ReturnSeat(Transform seat)
    {
        int index = Seats.IndexOf(seat);
        seatAvaliableInfoList[index] = true;
    }

    private void ClearSeats()
    {
        for (int i = 0; i < seatAvaliableInfoList.Count; i++)
            seatAvaliableInfoList[i] = true;
    }
}
