using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FowSetter : MonoBehaviour
{
    [SerializeField] float targetHorizontalFov = 35;


    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = 2.0f * Mathf.Atan(Mathf.Tan(targetHorizontalFov * Mathf.Deg2Rad / 2) / cam.aspect) * Mathf.Rad2Deg;
    }
}
