using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolSwitcher : MonoBehaviour
{
    [SerializeField] Tool[] tools;
    [SerializeField] Toggle[] toggles;
    Tool currentTool = null;

    public Tool CurrentTool { get => currentTool; }

    private void Awake()
    {
        currentTool = tools[0];
        toggles[0].isOn = true;
        toggles[0].interactable = false;
    }

    public void SwitchHead(int index)
    {
        if (tools[index] == currentTool) return;

        toggles[index].interactable = false;
        toggles[1 - index].interactable = true;
        toggles[1 - index].isOn = false;

        currentTool.OnDeselect();
        tools[index].OnSelect();
        currentTool = tools[index];
    }

}
