using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
public class Follower : FateMonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 threshold;
    [SerializeField] bool fixedUpdate;
    [SerializeField] bool freezeX;
    [SerializeField] bool freezeY;
    [SerializeField] bool freezeZ;

    private void LateUpdate()
    {
        if (!fixedUpdate) Follow();
    }

    private void FixedUpdate()
    {
        if (fixedUpdate) Follow();
    }
    public void Follow()
    {
        if (!target) return;
        Vector3 position = target.position + offset;
        if (freezeX || Mathf.Abs(position.x - transform.position.x) <= threshold.x)
            position.x = transform.position.x;
        if (freezeY || Mathf.Abs(position.y - transform.position.y) <= threshold.y)
            position.y = transform.position.y;
        if (freezeZ || Mathf.Abs(position.z - transform.position.z) <= threshold.z)
            position.z = transform.position.z;
        transform.position = position;
    }
}
