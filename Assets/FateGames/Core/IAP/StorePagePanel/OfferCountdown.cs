using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfferCountdown : MonoBehaviour
{
    [SerializeField] int initialSeconds = 42;
    [SerializeField] int initialMinutes = 23;
    [SerializeField] int initialHours = 2;
    [SerializeField] TMPro.TextMeshProUGUI countdownText;
    int seconds, minutes, hours;
    private void Awake()
    {
        seconds = initialSeconds;
        minutes = initialMinutes;
        hours = initialHours;
    }

    private void Start()
    {
        InvokeRepeating(nameof(Countdown), 1, 1);
    }

    void Countdown()
    {
        if (seconds > 0)
            seconds--;
        else if (minutes > 0)
        {
            minutes--;
            seconds = 59;
        }
        else if (hours > 0)
        {
            hours--;
            minutes = 59;
            seconds = 59;
        }
        countdownText.text = $"{hours}:{minutes}:{seconds}";
    }
}
