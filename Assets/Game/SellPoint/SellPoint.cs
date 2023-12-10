using DG.Tweening;
using FateGames.Core;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SellPoint : MonoBehaviour
{
    [SerializeField] int callTruckCount = 6;
    [SerializeField] int truckCapacity = 30;
    [SerializeField] float stopDistance = 30;
    [SerializeField] float jumpDistance = 30;
    [SerializeField] float truckSpeed = 5;
    [SerializeField] float wheelTurnSpeed = 10;
    [SerializeField] MoneyPile moneyPile;
    [SerializeField] Transform stackPos;
    [SerializeField] Vector3 stackItemSize;
    [SerializeField] Vector2 stackDimensionSize;
    [SerializeField] SoundEntity popSound;
    [SerializeField] PathCreator path;
    [SerializeField] IntVariable beltLevel;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] float mineMassMaxHeight;
    [SerializeField] Transform massMesh;
    [SerializeField] Transform truckMesh;
    [SerializeField] Transform truck;
    [SerializeField] Transform truckLoadPoint;
    [SerializeField] Transform[] wheels;

    ItemPile<Mine> minePile;
    List<int> mineTypes = new(); // sadece pile daki mine lar
    int mineValueOnTruck = 0; // save edilecek, sadece satýþta sýfýrlancak
    int mineCountInTruck = 0; // yolun sonunda sýfýrlanacak 
    float distance = 0;
    Tween lastScaleTween = null;

    public void Awake()
    {
        minePile = new(stackPos, stackItemSize, stackDimensionSize);
    }

    private void Start()
    {
        /*if (mineTypes.Count >= callTruckCount) // sadece kamyonla ifade edilecek ise
        {
            truck.gameObject.SetActive(true);
            distance = stopDistance;
            SetTructTransform(distance);
            countText.text = mineTypes.Count + "/" + truckCapacity;
            //CheckTruckFull();
        }*/
        for (int i = 0; i < mineTypes.Count; i++) AddMine(DiggerMachine.Instance.GetMineForSellPoint(mineTypes[i]).SetLevel(beltLevel.Value), false);
    }

    private void Update()
    {
        if (truck.gameObject.activeSelf && (minePile.Count > 0 || mineCountInTruck == truckCapacity)) // sahnede ve (geliyor veya gidiyor) ise
        {
            distance += Time.deltaTime * truckSpeed;
            if (distance >= path.path.length) // sýnýrý gectiyse sýfýrla
            {
                distance = 0;
                mineCountInTruck = 0;
                truck.gameObject.SetActive(false);
                CheckTruckCall();
            }
            else if (distance >= stopDistance && mineCountInTruck != truckCapacity) // durma noktasýnda ise
            {
                distance = stopDistance;
                SetTructTransform(distance);

                int min = Mathf.Min(minePile.Count, truckCapacity);
                for (int i = 0; i < min; i++)
                {
                    Mine mine = minePile.GetItem();
                    mineTypes.RemoveAt(mineTypes.Count - 1);
                    mineValueOnTruck += mine.Value;
                    mineCountInTruck++;
                    mine.transform.DOMove(truckLoadPoint.position, 0.5f).OnComplete(() => mine.Release());
                }
                UpdateTruck();
            }

            SetTructTransform(distance);
            for (int i = 0; i < wheels.Length; i++)
                wheels[i].localEulerAngles += wheelTurnSpeed * Time.deltaTime * Vector3.up * ((distance >= stopDistance) ? -1 : 1);
        }
    }

    public void AddMine(Mine mine, bool updateTypesList = true)
    {
        if (distance == stopDistance && mineCountInTruck < truckCapacity) // bekleyen dolmamýþ kamyon varsa
        {
            mineValueOnTruck += mine.Value;
            mineCountInTruck++;
            mine.transform.DOMove(truckLoadPoint.position, 0.5f).OnComplete(() => mine.Release());
            UpdateTruck();
        }
        else
        {
            if (updateTypesList) mineTypes.Add(mine.Type);
            minePile.AddItem(mine, updateTypesList);
            CheckTruckCall();
        }
    }

    private void UpdateTruck()
    {
        GameManager.Instance.PlaySoundOneShot(popSound);
        countText.text = mineCountInTruck + "/" + truckCapacity;
        Bounce();
        UpdateMineMass(mineCountInTruck);
        if (mineCountInTruck == truckCapacity)
        {
            moneyPile.AddAmountOfMoney(mineValueOnTruck, Mathf.CeilToInt(Mathf.Sqrt(mineValueOnTruck)), transform.position);
            distance = jumpDistance;
            SetTructTransform(distance);
            mineValueOnTruck = 0;
            truckMesh.localEulerAngles = Vector3.zero;
        }
    }

    private void CheckTruckCall()
    {
        if (minePile.Count >= callTruckCount && !truck.gameObject.activeSelf) // kamyon aktif deðilse ve mine pile sýnýrý gecildiyse
        {
            truckMesh.localEulerAngles = Vector3.up * 180;
            countText.text = "0/" + truckCapacity;
            truck.gameObject.SetActive(true);
            SetTructTransform(0);
            UpdateMineMass(0);
            UpdateTruck();
        }
    }

    private void UpdateMineMass(float count)
    {
        if (count > truckCapacity) count = truckCapacity;
        massMesh.localPosition = (count / truckCapacity) * mineMassMaxHeight * Vector3.forward;
    }

    private void Bounce()
    {
        if (lastScaleTween != null) lastScaleTween.Kill();
        lastScaleTween = truck.transform.DOScale(1.05f, 0.1f).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo).OnComplete(() => lastScaleTween = null);
    }

    private void SetTructTransform(float distance)
    {
        //Debug.Log(path.path.GetRotationAtDistance(distance).eulerAngles);
        truck.transform.SetPositionAndRotation(path.path.GetPointAtDistance(distance), path.path.GetRotationAtDistance(distance));
    }
}
