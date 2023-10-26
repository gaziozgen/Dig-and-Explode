using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class DiggerTeeth : FateMonoBehaviour
{
    [SerializeField] Digger digger;
    private void OnTriggerStay(Collider other)
    {
        if (!digger.Working) return;
        IDiggable diggable = other.GetComponent<IDiggable>();
        if (diggable != null && !diggable.IsDug())
        {
            if (diggable.Power() <= digger.Power())
            {
                diggable.GetDug();
                digger.EffectPool.Get().transform.position = other.transform.position;
            }
            else
            {
                Vector3 dir = transform.position - other.transform.position;
                dir.z = 0;
                digger.ToolController.Recoil(dir.normalized);
            }
        }
    }
}
