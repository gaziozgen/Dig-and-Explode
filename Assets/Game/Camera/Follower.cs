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
    [SerializeField] float targetHorizontalFov = 35;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = 2.0f * Mathf.Atan(Mathf.Tan(targetHorizontalFov * Mathf.Deg2Rad / 2) / cam.aspect) * Mathf.Rad2Deg;
    }

    private void Update()
    {

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
        if (!target) return;
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

        transform.position = position;
    }
}
