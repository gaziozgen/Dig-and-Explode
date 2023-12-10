using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartLookObject : MonoBehaviour
{
    [SerializeField] string key;
    void Start()
    {
        // Start Smartlook
        SmartlookUnity.SetupOptionsBuilder builder = new SmartlookUnity.SetupOptionsBuilder(key);
        SmartlookUnity.Smartlook.SetupAndStartRecording(builder.Build());
    }
}
