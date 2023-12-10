using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GiantDiggerPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject button;
    [SerializeField] Digger digger;

    public void Open()
    {
        button.SetActive(false);
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
        button.SetActive(true);
    }

    public void TryGet()
    {
        AdManager.Instance.ShowRewardedAd($"giantDigger", () => { }, () =>
        {
            Get();
        });
    }

    private void Get()
    {
        panel.SetActive(false);
        digger.SwichGiantDigger(() =>
        {
            button.SetActive(true);
        });
    }
}
