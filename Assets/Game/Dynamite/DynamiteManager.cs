using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamiteManager : MonoBehaviour
{
    [SerializeField] GameObject dynamitePrefab;
    FateObjectPool<Dynamite> dynamitePool;

    [SerializeField] int bombCount = 10;
    [SerializeField] float baseRange = 1;
    [SerializeField] float basePower = 1;
    [SerializeField] float rangeIncreasePerLevel = 0.5f;
    [SerializeField] float powerIncreasePerLevel = 1;
    [SerializeField] int level = 0;
    [SerializeField] GameObject outline;
    [SerializeField] TextMeshProUGUI bombText;
    [SerializeField] GameEvent onBombControllOn;
    [SerializeField] GameEvent onBombControllOff;

    public bool BombControl { get; private set; } = false;

    private void Awake()
    {
        dynamitePool = new FateObjectPool<Dynamite>(dynamitePrefab, true, 10, 10);
        bombText.text = bombCount.ToString();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && BombControl && bombCount > 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Ore ore = hit.transform.GetComponent<Ore>();

                if (ore != null)
                {
                    bombCount--;
                    bombText.text = bombCount.ToString();
                    Dynamite dynamite = dynamitePool.Get(hit.point);
                    dynamite.Explode(baseRange + level * rangeIncreasePerLevel, basePower + level * powerIncreasePerLevel);
                }
            }
        }
    }

    public void LevelUp()
    {
        level++;
    }

    public void ToggleBombControl()
    {
        BombControl = !BombControl;
        outline.SetActive(BombControl);
        if (BombControl) onBombControllOn.Raise();
        else onBombControllOff.Raise();
    }
}
