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

    [Tooltip("The game will not load a save and will not place player to spawn")]
    public bool stopSaveLoading;

    void Awake()
    {
#if !UNITY_EDITOR
    SetProduction();
#endif
    }

    void OnEnable()
    {
#if !UNITY_EDITOR
    SetProduction();
#endif
    }

    void SetProduction()
    {
        //PRODUCTION SETTINGS
        skipMainMenu = false;
        stopSaving = false;
        stopSaveLoading = false;
    }

}
