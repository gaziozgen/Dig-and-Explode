using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using System;

public class Money3D : FateMonoBehaviour, IPooledObject
{
    [SerializeField] Rigidbody rb;
    public int value = 1;

    public Action Release { get; set; }
    public Rigidbody Rigidbody { get => rb; }

    public void OnObjectSpawn()
    {
        transform.rotation = Quaternion.identity;

        value = 1;
    }

    public void OnRelease()
    {
        DG.Tweening.DOTween.Kill(transform);
        transform.SetParent(null);
        if (gameObject.layer == 9) gameObject.layer = 10;
        if (rb)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
