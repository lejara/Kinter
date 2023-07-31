using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugSettings")]
public class DebugSettings : ScriptableObject
{
    [Tooltip("Go Straight to playing the character")]
    public bool skipMainMenu;
    [Tooltip("The game will not save")]
    public bool stopSaving;

    void OnEnable()
    {
#if !UNITY_EDITOR
        //PRODUCTION SETTINGS
        skipMainMenu = false;
        stopAutoSaving = false;
#endif
    }

}
