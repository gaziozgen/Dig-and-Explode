using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] UIElement uiElement;


    public void Show()
    {
        GameManager.Instance.PauseGame();
        uiElement.Show();
    }

    public void Hide()
    {
        GameManager.Instance.ResumeGame();
        uiElement.Hide();
    }
}
