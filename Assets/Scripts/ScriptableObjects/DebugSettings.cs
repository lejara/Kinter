using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "DebugSettings")]
public class DebugSettings : ScriptableObject
{
    [Header("Menus")]
    [Tooltip("Go Straight to playing the character")]
    public bool skipMainMenu;

    [Header("Save System")]
    [Tooltip("The game will not save")]
    public bool stopSaving;

    [Tooltip("The game will not load a save and will not place player to spawn")]
    public bool stopSaveLoading;


    [Header("Player Debug Tools")]
    //TODO: comment these
    //TODO: add keybind to active them while only in editor
    public bool godMode;
    public bool checkpoint;
    public bool isPlayerInDebug { get { return this.godMode || this.checkpoint; } }

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

    [ButtonMethod]
    void SetProduction()
    {
        //PRODUCTION SETTINGS
        skipMainMenu = false;
        stopSaving = false;
        stopSaveLoading = false;
        godMode = false;
        checkpoint = false;
    }

}
