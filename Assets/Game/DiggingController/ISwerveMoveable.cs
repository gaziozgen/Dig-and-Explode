using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISwerveMoveable 
{
    public void Move(Swerve swerve);
    public void OnStable();
}
