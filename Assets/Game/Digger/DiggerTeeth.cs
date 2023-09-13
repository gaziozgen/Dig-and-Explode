using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class DiggerTeeth : FateMonoBehaviour
{
    [SerializeField] Transform diggerCenter;
    Vector3 offset;
    Vector3 initialEuler;
    private void Awake()
    {
        offset = diggerCenter.position - transform.position;
        initialEuler = transform.eulerAngles;
    }
    private void Update()
    {
        transform.position = diggerCenter.position + Quaternion.Euler(0, 0, diggerCenter.rotation.eulerAngles.z) * offset;
        transform.eulerAngles = initialEuler + diggerCenter.eulerAngles;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Diggable"))
        {
            Diggable diggable = other.GetComponent<Diggable>();
            diggable.GetDug();
        }
    }


}
