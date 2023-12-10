using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : Person, IPooledObject
{
    [SerializeField] Animator animator;

    public void SetSit(bool sit)
    {
        animator.SetBool("Sit", sit);
    }

    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        animator.SetBool("Sit", false);
    }

    public void OnRelease()
    {

    }
}
