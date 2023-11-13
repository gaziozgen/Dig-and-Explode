using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeZone : MonoBehaviour
{
    [SerializeField] UnityEvent onInteracted, onLeaved;

    public UnityEvent OnLeaved { get => onLeaved; }

    public void Interact()
    {
        onInteracted.Invoke();
    }

    public void Leave()
    {
        onLeaved.Invoke();
    }
}
