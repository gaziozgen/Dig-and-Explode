using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class DiggerTeeth : FateMonoBehaviour
{
    [SerializeField] Transform tip;
    [SerializeField] Digger digger;
    private void OnTriggerStay(Collider other)
    {
        if (!digger.Working) return;
        IDiggable diggable = other.GetComponent<IDiggable>();
        if (diggable != null)
        {
            if (diggable.IsDug())
            {
                diggable.AddForce(digger.PushPower() * tip.forward);
            }
            else
            {
                if (diggable.Power() <= digger.Power())
                {
                    diggable.GetDug();
                    digger.onDigged.Raise();
                    GameManager.Instance.PlayHaptic();
                    GameManager.Instance.PlaySoundOneShot(digger.DigSound);
                    digger.DigEffectPool.Get().transform.position = other.transform.position;
                }
                else
                {
                    Vector3 dir = transform.position - other.transform.position;
                    dir.z = 0;
                    digger.Recoil(dir.normalized, tip.position);
                }
            }

        }
    }
}
