using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GemUI : FateMonoBehaviour
{
    private static GemUI instance = null;
    public static GemUI Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GemUI>();
            return instance;
        }
    }
    Camera mainCamera;
    [SerializeField] private RectTransform imageTransform;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private GameEvent onGemChanged = null;
    [SerializeField] private GameObject flyingGemPrefab = null;
    [SerializeField] private SoundEntity burstSound = null;
    public static int Gem { get => SaveManager.Gem; set => SaveManager.Gem = value; }
    private bool animating = false;

    public Vector2 GemTarget { get => imageTransform.position; }

    private FateObjectPool<FlyingGem> flyingGemPool;

    protected void Awake()
    {
        instance = this;
        flyingGemPool = new(flyingGemPrefab, true, 20, 20);
        mainCamera = Camera.main;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void DirectFlyingGem(int amount, float startSize, Vector2 spawnPosition)
    {
        FlyingGem money = MakeGem();
        money.DirectGoToUI(amount, startSize, spawnPosition);
    }
    public void DirectFlyingGem(int amount, float startSize, Vector3 spawnPosition)
    {
        Vector2 screenPos = mainCamera.WorldToScreenPoint(spawnPosition);
        DirectFlyingGem(amount, startSize, screenPos);
    }

    public void BurstFlyingGem(int amount, int count, float radius, Vector2 spawnPosition)
    {
        GameManager.Instance.PlaySound(burstSound);
        int valueOfSingleMoneyImage = amount / count;
        int remainder = amount - (valueOfSingleMoneyImage * count);

        for (int i = 0; i < count; i++)
        {
            FlyingGem gem = MakeGem();

            int gain = valueOfSingleMoneyImage;
            if (i == count - 1) gain += remainder;

            float randomAngle = UnityEngine.Random.Range(0, 360);
            float distance = UnityEngine.Random.Range(0, 1f) * radius;

            Vector2 midPosition = spawnPosition + new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * distance;

            gem.GoUIWithBurstMove(gain, spawnPosition, midPosition);
        }
    }

    private FlyingGem MakeGem()
    {
        FlyingGem gem = flyingGemPool.Get();
        if (gem.transform.parent != canvas) gem.transform.SetParent(canvas);
        gem.transform.localScale = Vector3.one;
        return gem;
    }

    public void AddGem(int amount)
    {
        Gem += amount;
        UpdateUI();
    }

    public void SpendGem(int amount)
    {
        Gem -= amount;
        UpdateUI();
    }

    public void SetGemFromDevMode(string money)
    {
        try
        {
            int result = int.Parse(money.Substring(0, money.Length - 1));
            Gem = result;
            UpdateUI();
            print("money updated");
        }
        catch (FormatException)
        {
            print("Unable to parse");
        }
    }

    private void UpdateUI()
    {
        moneyText.text = Gem < 1000? numberFormat(Gem).Replace(",", ".") : "∞";
        onGemChanged.Raise();
        BounceGem();
    }

    private void BounceGem()
    {
        if (animating) return;
        animating = true;
        imageTransform.DOScale(1.2f, 0.05f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { animating = false; });
    }

    public enum suffixes
    {
        p, // p is a placeholder if the value is under 1 thousand
        K, // Thousand
        M, // Million
        B, // Billion
        T, // Trillion
        Q, // Quadrillion
    }

    public static string numberFormat(long money)
    {
        int decimals = 2; //How many decimals to round to
        string r = money.ToString(); //Get a default return value

        foreach (suffixes suffix in Enum.GetValues(typeof(suffixes))) //For each value in the suffixes enum
        {
            var currentVal = 1 * Math.Pow(10, (int)suffix * 3); //Assign the amount of digits to the base 10
            var suff = Enum.GetName(typeof(suffixes), (int)suffix); //Get the suffix value
            if ((int)suffix == 0) //If the suffix is the p placeholder
                suff = String.Empty; //set it to an empty string

            if (money >= currentVal)
                r = Math.Round((money / currentVal), decimals, MidpointRounding.ToEven).ToString() + suff; //Set the return value to a rounded value with suffix
            else
                return r; //If the value wont go anymore then return
        }
        return r; // Default Return
    }
}
