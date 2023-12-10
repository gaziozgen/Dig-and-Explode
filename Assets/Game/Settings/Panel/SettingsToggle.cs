using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsToggle : MonoBehaviour
{
    [SerializeField] private BoolVariable variable;
    [SerializeField] private Sprite offSprite, onSprite;
    [SerializeField] private Image image;
    [SerializeField] private UnityEvent onToggled, onTurnedOn, onTurnedOff;

    //bool value = true;

    private void Start()
    {
        //if (variable != null) variable.Value = value;
        SetImage();
    }

    public void SetImage()
    {
        image.sprite = variable.Value ? onSprite : offSprite;
    }

    public void Toggle()
    {
        variable.Value = !variable.Value;
        //value = variable.Value;
        SetImage();
        onToggled.Invoke();
        if (variable.Value) onTurnedOn.Invoke();
        else onTurnedOff.Invoke();
    }

}
