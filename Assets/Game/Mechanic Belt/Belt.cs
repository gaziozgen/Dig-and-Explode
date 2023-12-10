using FateGames.Core;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : MonoBehaviour
{
    [SerializeField] IntVariable beltLevel;
    [SerializeField] SellPoint sellPoint;
    [SerializeField] GameObject[] defaultBelts;
    [SerializeField] GameObject[] machines;
    [SerializeField] float[] stopPositions;
    public PathCreator Path;
    [HideInInspector] public List<Mine> Mines;

    int currentLevel = 0;

    private void Awake()
    {
        beltLevel.OnValueChanged.AddListener((int bafore, int after) => UpdateLevel());
    }

    public void UpdateMinePoses(float distance)
    {
        for (int i = Mines.Count - 1; i >= 0; i--)
        {
            Mine mine = Mines[i];
            mine.Distance += distance;
            if (mine.Distance > Path.path.length)
            {
                Mines.Remove(mine);
                sellPoint.AddMine(mine);
                DiggerMachine.Instance.DequeueTypeList();
            }
            else
            {
                for (int machineIndex = 0; machineIndex < currentLevel; machineIndex++)
                {
                    if (mine.Distance >= stopPositions[machineIndex] && mine.Level < machineIndex + 1)
                        mine.SetLevel(mine.Level + 1);
                }
                mine.transform.SetPositionAndRotation(Path.path.GetPointAtDistance(mine.Distance), Path.path.GetRotationAtDistance(mine.Distance));
            }
        }
    }

    private void UpdateLevel()
    {
        if (beltLevel.Value > currentLevel)
        {
            while (beltLevel.Value > currentLevel)
            {
                defaultBelts[currentLevel].SetActive(false);
                machines[currentLevel].SetActive(true);
                currentLevel++;
            }
        }
    }
}
