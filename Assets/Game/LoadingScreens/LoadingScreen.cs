using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI percentText;
    [SerializeField] Slider slider;
    int targetPercent = 0;

    private void Update()
    {
        slider.value = Mathf.MoveTowards(slider.value, targetPercent, Time.deltaTime * 100);
        percentText.text = $"{Mathf.FloorToInt(slider.value)}%";
    }
    public void SetPercent(int percent)
    {
        Debug.Log(percent);
        targetPercent = percent;
    }
}
