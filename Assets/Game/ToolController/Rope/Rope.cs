using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] ObiRope rope;
    [SerializeField] ObiSolver solver;
    [SerializeField] float baseThickness = 0.15f;
    [SerializeField] float waveThickness = 0.1f;
    [SerializeField] float waveSpeed = 1;
    [SerializeField] float waveLength = 0;
    [SerializeField] float minWaveProduceInterval = 1;

    private float nextWaveProduceTime = -1;
    private readonly List<float> wavePositions = new();


    private void Update()
    {
        UpdateRopeToCurrentWaves();

        if (Input.GetKeyDown(KeyCode.W)) AddWave();
    }

    public void AddWave()
    {
        if (nextWaveProduceTime < Time.time)
        {
            nextWaveProduceTime = Time.time + minWaveProduceInterval;
            wavePositions.Add(-waveLength / 2);
        }
    }

    private void UpdateRopeToCurrentWaves()
    {
        for (int i = 0; i < rope.solverIndices.Length; i++)
            solver.principalRadii[i] = Vector3.right * (baseThickness + AdditionalScale(i));
        UpdateWavePositions();
    }

    private float AdditionalScale(float pos)
    {
        float additionalScale = 0;
        float halfWaveLength = waveLength / 2;
        for (int i = 0; i < wavePositions.Count; i++)
        {
            if (pos > wavePositions[i] - halfWaveLength && pos < wavePositions[i] + halfWaveLength)
            {
                float deg = 90 + (wavePositions[i] - pos) / halfWaveLength * 90;
                additionalScale += Mathf.Sin(deg * Mathf.Deg2Rad) * waveThickness;
            }
        }
        return additionalScale;
    }

    private void UpdateWavePositions()
    {
        for (int i = 0; i < wavePositions.Count; i++)
        {
            wavePositions[i] += Time.deltaTime * waveSpeed;

            if (wavePositions[i] > rope.solverIndices.Length)
            {
                wavePositions.RemoveAt(i);
                i--;
            }
        }
    }
}
