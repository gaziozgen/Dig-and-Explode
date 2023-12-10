using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamiteManager : MonoBehaviour
{
    [SerializeField] Camera diggingCamera;
    [SerializeField] Transform toolTransform;
    [SerializeField] GameObject dynamitePrefab;
    [SerializeField] int bombCount = 5;
    [SerializeField] IntVariable maxBombCountLevel;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] float baseRange = 1;
    [SerializeField] float basePower = 1;
    [SerializeField] float rangeIncreasePerLevel = 0.5f;
    [SerializeField] float powerIncreasePerLevel = 1;
    [SerializeField] IntVariable level;
    [SerializeField] Slider countSlider;
    [SerializeField] GameObject outline;
    [SerializeField] GameObject header;
    [SerializeField] TextMeshProUGUI bombText;
    [SerializeField] GameEvent onBombControllOn;
    [SerializeField] GameEvent onBombControllOff;

    FateObjectPool<Dynamite> dynamitePool;
    float lastBombGetTime = 0;

    public bool BombControl { get; private set; } = false;

    int CurrentBombCapacity => 3 + maxBombCountLevel.Value;


    private void Awake()
    {
        dynamitePool = new FateObjectPool<Dynamite>(dynamitePrefab, true, 10, 10);
        bombText.text = bombCount.ToString();
    }

    private void Start()
    {
        if (bombCount == CurrentBombCapacity) countSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        /*bool onUI = EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null;
        if (Input.GetMouseButtonDown(0) && BombControl && bombCount > 0 && !onUI)
        {
            RaycastHit hit;
            Ray ray = diggingCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Ore ore = hit.transform.GetComponent<Ore>();
                if (ore != null) UseBomb(hit.point);
            }
        }*/
        CooldownUpdate();
    }

    private void UseBomb(Vector3 pos)
    {
        GameManager.Instance.PlayHaptic();
        if (bombCount == CurrentBombCapacity)
        {
            lastBombGetTime = CurrentTime();
            countSlider.gameObject.SetActive(true);
        }
        bombCount--;
        bombText.text = bombCount.ToString();

        Dynamite dynamite = dynamitePool.Get(pos);
        dynamite.Fall(baseRange + level.Value * rangeIncreasePerLevel, basePower + level.Value * powerIncreasePerLevel);
    }

    private void CooldownUpdate()
    {
        //Debug.Log(CurrentTime());
        if (bombCount == CurrentBombCapacity) return;

        float passedTime = CurrentTime() - lastBombGetTime;
        int bombGain = (int)(passedTime / reloadTime);
        if (passedTime < 0) bombGain = CurrentBombCapacity;

        if (bombGain > 0)
        {
            int finalGain = Math.Min(bombGain, CurrentBombCapacity - bombCount);
            bombCount += finalGain;
            if (bombCount != CurrentBombCapacity) lastBombGetTime += (int)(finalGain * reloadTime);
            else countSlider.gameObject.SetActive(false);
        }
        countSlider.value = passedTime / reloadTime;
        bombText.text = bombCount.ToString();
    }

    float CurrentTime()
    {
        DateTime date = DateTime.UtcNow;
        return /*date.Year * 3110400 + date.Month * 259200 + date.Day * 86400 + */date.Hour * 3600 + date.Minute * 60 + date.Second + date.Millisecond / 1000f;
    }

    public void ToggleBombControl()
    {
        if (bombCount > 0) UseBomb(toolTransform.position);
        /*BombControl = !BombControl;
        outline.SetActive(BombControl);
        header.SetActive(BombControl);
        if (BombControl) onBombControllOn.Raise();
        else onBombControllOff.Raise();*/
    }
}
