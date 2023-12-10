using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDiggable
{
    public void AddForce(Vector3 force);

    public bool IsDug();

    public void GetDug(bool checkDug = true);

    public float Power();
}
