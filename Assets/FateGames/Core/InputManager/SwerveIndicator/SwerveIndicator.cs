using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIElement))]
public class SwerveIndicator : FateMonoBehaviour
{
    [SerializeField] RectTransform outCircle, inCircle;
    [SerializeField] Swerve swerve;
    [SerializeField] UIElement uiElement;

    private void Start()
    {
        Debug.Log(Screen.height);
        Debug.Log(swerve.Size);
        outCircle.sizeDelta = new Vector2(swerve.Size * 2, swerve.Size * 2);
        inCircle.sizeDelta = new Vector2(swerve.Size, swerve.Size);
        uiElement.Hide();
    }

    public void SetOutCirclePosition(Swerve swerve)
    {
        outCircle.position = swerve.AnchorPosition;
    }

    public void SetInCirclePosition(Swerve swerve)
    {
        inCircle.position = swerve.MousePosition;
    }
}
