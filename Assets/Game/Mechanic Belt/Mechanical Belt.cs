using DG.Tweening;
using FateGames.Core;
using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltManager : MonoBehaviour
{
    [SerializeField] MechanicArm mechanicArm;
    [SerializeField] Belt[] belts;
    [SerializeField] Material beltMaterial;
    [SerializeField] float beltTexSpeedMultiplier = 1;

    [SerializeField] float[] beltSpeeds;
    [SerializeField] IntVariable beltSpeedLevel;
    [SerializeField] Transform startPos;

    float beltSpeed => beltSpeeds[beltSpeedLevel.Value];

    private void Update()
    {
        for (int i = 0; i < belts.Length; i++)
            belts[i].UpdateMinePoses(beltSpeed * Time.deltaTime);

        beltMaterial.mainTextureOffset = beltMaterial.mainTextureOffset + Vector2.up * beltSpeed * beltTexSpeedMultiplier * Time.deltaTime;
    }

    public IEnumerator TakeMines()
    {
        for (int i = mechanicArm.HoldedMines.Count - 1; i >= 0; i--)
        {
            Mine mine = mechanicArm.HoldedMines[i];
            mine.transform.parent = null;
            mechanicArm.HoldedMines.Remove(mine);

            mine.transform.DOMove(startPos.position, 0.2f);
            mine.transform.DORotate(startPos.forward, 0.2f).OnComplete(() =>
            {
                belts[mine.Category].Mines.Add(mine);
            });
            yield return new WaitForSeconds(1 / beltSpeed); // optimize adilebilir
        }
    }
}
