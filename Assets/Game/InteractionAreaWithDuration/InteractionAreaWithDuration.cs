using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.UI;
using Firebase.Analytics;
using UnityEngine.Events;

public class InteractionAreaWithDuration : MonoBehaviour
{
    [SerializeField] DiggerMachine machine;
    [SerializeField] Image fillImage;
    [SerializeField] Collider m_Collider;
    [SerializeField] UIElement uiElement;
    [SerializeField] Transform spawnPoint;
    [SerializeField] UnityEvent onInteractionComplete;

    public bool HasMachine => machine != null;

    public void Use()
    {
        onInteractionComplete.Invoke();
    }

    public void UseMachine(Transform player)
    {
        machine.Use(player);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            LoadLevel();
    }*/

    public void UpdateFillImage(float rate)
    {
        if (fillImage) fillImage.fillAmount = rate;
    }

    public void Hide()
    {
        m_Collider.enabled = false;
        uiElement.Hide();
    }
    public void Show()
    {
        m_Collider.enabled = true;
        uiElement.Show();
    }

}
