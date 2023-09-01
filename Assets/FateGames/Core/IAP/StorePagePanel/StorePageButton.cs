using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePageButton : MonoBehaviour
{
    [SerializeField] UIElement uiElement;

    private IEnumerator Start()
    {
        /*uiElement.Hide();
        if (!StorePagePanel.isShowedForTheFirstTime && FateSaveManager.TotalPlaytime < 600) yield break;*/
        yield return new WaitUntil(() => StorePage.Initialized);
        uiElement.Show();
    }
}
