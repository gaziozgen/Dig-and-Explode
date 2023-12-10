using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeableVIPRoomOptionContainer : MonoBehaviour
{
    [SerializeField] UpgradeableVIPRoomOption[] options;

    public UpgradeableVIPRoomOption[] Options { get => options; }

    private void OnValidate()
    {
        options = GetComponentsInChildren<UpgradeableVIPRoomOption>(true);
    }
}
