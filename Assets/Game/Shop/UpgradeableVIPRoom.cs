using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.Events;

public class UpgradeableVIPRoom : FateMonoBehaviour
{
    [SerializeField] UpgradeableVIPRoomOptionContainer[] levelOptionContainers;
    [SerializeField] BuyingZone[] upgradeZones;
    [SerializeField] int defaultCost = 5;
    [SerializeField] Transform cameraFocusTarget;
    [SerializeField] Transform centerPoint;
    [SerializeField] Transform cameraPoint;
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] Vector3 cameraEuler;
    [SerializeField] SoundEntity sound;
    [SerializeField] UnityEvent onVIPRoomUpgraded, onVIPRoomLocked;
    private int level = 0;
    private int lockedOptionIndex = 0;
    private int previewingLevel = 0;
    private int previewingOptionIndex = 0;
    UpgradeableVIPRoomOption selectedOption = null;

    Follower mainCameraController = null;
    public int Level => level;
    public int optionIndex => lockedOptionIndex;

    private void Start()
    {
        mainCameraController = Camera.main.GetComponent<Follower>();
        //cameraPoint.position = centerPoint.position + cameraOffset;
        for (int i = 0; i < levelOptionContainers.Length; i++)
        {
            UpgradeableVIPRoomOptionContainer container = levelOptionContainers[i];
            for (int j = 0; j < container.Options.Length; j++)
            {
                UpgradeableVIPRoomOption option = container.Options[j];
                option.Deactivate();
            }
        }
        selectedOption = levelOptionContainers[level].Options[lockedOptionIndex];
        selectedOption.Activate();
    }

    public void PreviewOption(int targetLevel, int optionIndex)
    {
        GameManager.Instance.PlaySoundOneShot(sound);
        previewingLevel = targetLevel;
        previewingOptionIndex = optionIndex;
        selectedOption.Deactivate();
        selectedOption = levelOptionContainers[targetLevel].Options[optionIndex];
        selectedOption.Activate();
        selectedOption.AnimatePreview();
    }

    public void LockOption(bool upgrade = true)
    {
        GameManager.Instance.PlaySoundOneShot(sound);
        lockedOptionIndex = previewingOptionIndex;
        level = previewingLevel;
        /*if (upgradeZones[level])
            upgradeZones[level].Activate();*/
        onVIPRoomLocked?.Invoke();
        if (upgrade)
        {
            mainCameraController.ResetTarget();
            //LevelSystem.Instance.AddXP(0, 3);
        }
    }

    public void Buy()
    {
        PreviewOption(1, 0);
        LockOption(false);
    }

    public void Upgrade()
    {
        if (level + 1 >= levelOptionContainers.Length || level < 0)
        {
            Debug.LogError("Level is not suitable for upgrade!", this);
            return;
        }
        UpgradeableVIPRoomOptionContainer optionContainer = levelOptionContainers[level + 1];
        Debug.Log(optionContainer, optionContainer);
        if (optionContainer.Options.Length == 0)
        {
            Debug.LogError("No option!", this);
            return;
        }
        level += 1;
        //LevelSystem.Instance.AddXPWithoutStars(3);

        for (int i = 0; i < optionContainer.Options.Length; i++)
        {
            int index = i;
            int targetLevel = level;
            UpgradeableVIPRoomOption option = optionContainer.Options[i];

            option.getAction = () => LockOption();

            option.previewAction = () => PreviewOption(targetLevel, index);
        }
        PreviewOption(level, 0);
        VIPRoomUpgradePanel.Instance.Show(optionContainer, level);
        onVIPRoomUpgraded?.Invoke();
        mainCameraController.SetTarget(cameraFocusTarget);

    }

    protected void OnValidate()
    {
        levelOptionContainers = GetComponentsInChildren<UpgradeableVIPRoomOptionContainer>(true);
    }
}
