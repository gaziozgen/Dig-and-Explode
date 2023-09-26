using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingHeadSwitcher : MonoBehaviour
{
    [SerializeField] DiggingHead[] heads;
    DiggingHead currentHead;

    public DiggingHead CurrentHead { get => currentHead; }

    private void Awake()
    {
        SwitchHead(0);
    }

    public void SwitchHead(int index)
    {
        if (index < 0 || index >= heads.Length)
        {
            Debug.LogError("Wrong index!", this);
            return;
        }
        for (int i = 0; i < heads.Length; i++)
        {
            heads[i].transform.gameObject.SetActive(i == index);
        }
        DiggingHead previousHead = currentHead;
        currentHead = heads[index];
        if (previousHead)
            currentHead.CenterTransform.position = previousHead.CenterTransform.position;
    }

}
