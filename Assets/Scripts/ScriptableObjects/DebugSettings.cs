using System;
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

    [Header("Checkpoint Options")]

#if UNITY_EDITOR
    [Multiline]
    public string checkpointDescirption = "";
#endif

    [Tooltip("Checkpoint state, toggle to change or use key")]
    public bool checkpoint;
    public KeyCode checkpointActivateKey = KeyCode.Alpha2;
    public KeyCode addPointKey = KeyCode.R;
    public KeyCode teleportToPointKey = KeyCode.T;
    [HideInInspector][NonSerialized] public Vector3 lastCheckpoint;

    [Header("God Mode Options")]

#if UNITY_EDITOR
    [Multiline]
    public string godModeDescirption = "";
#endif

    [Tooltip("God mode state, toggle to change or use key")]
    public bool godMode;
    public KeyCode godModeActivateKey = KeyCode.Alpha1;
    public KeyCode godModeBoostKey = KeyCode.Space;
    public float godModeSpeed;
    public float godModeBoostSpeed;
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
