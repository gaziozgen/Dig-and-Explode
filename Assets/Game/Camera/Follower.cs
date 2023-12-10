using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
public class Follower : FateMonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 threshold;
    [SerializeField] bool fixedUpdate;
    [SerializeField] bool freezeX;
    [SerializeField] bool freezeY;
    [SerializeField] bool freezeZ;

    bool locked = false;
    Transform mainTarget;

    private void Awake()
    {
        mainTarget = target;
    }

    private void Start()
    {
        //transform.position = TargetPos();
    }

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
        if (!target || locked) return;
        Vector3 position = TargetPos();

        transform.position = Vector3.MoveTowards(transform.position, position, speed * (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime));
    }

    Vector3 TargetPos()
    {
        Vector3 position = target.position + offset;
        Vector3 difference = position - transform.position;
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXX
        if (freezeX || Mathf.Abs(difference.x) <= threshold.x)
            position.x = transform.position.x;
        else if (difference.x > threshold.x)
            position.x -= threshold.x;
        else
            position.x += threshold.x;
        // YYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
        if (freezeY || Mathf.Abs(difference.y) <= threshold.y)
            position.y = transform.position.y;
        else if (difference.y > threshold.y)
            position.y -= threshold.y;
        else
            position.y += threshold.y;
        // ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ
        if (freezeZ || Mathf.Abs(difference.z) <= threshold.z)
            position.z = transform.position.z;
        else if (difference.z > threshold.z)
            position.z -= threshold.z;
        else
            position.z += threshold.z;

        return position;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void ResetTarget()
    {
        target = mainTarget;
    }
}
