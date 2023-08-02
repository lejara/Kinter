using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public DateTime date;
    public PlayerSaveData playerData;
    public SettingsSaveData settingsData;
    public DebugSaveData debugSaveData;

    public SaveData()
    {
        playerData = new PlayerSaveData();
        settingsData = new SettingsSaveData();
        debugSaveData = new DebugSaveData();
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public bool isStunned;
    public Vector3 position = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
}

[System.Serializable]
public class SettingsSaveData
{
    public float musicSlider = -1;
    public float sfxSlider = -1;
}

[System.Serializable]
public class DebugSaveData
{
    public Vector3 lastCheckpointPos = Vector3.zero;
}