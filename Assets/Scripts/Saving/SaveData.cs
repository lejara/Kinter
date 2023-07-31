using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SaveData
{
    public DateTime date;
    public PlayerSaveData playerData;
    public SettingsSaveData settingsData;
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 position;
}

[System.Serializable]
public struct SettingsSaveData
{
    public float musicSlider;
    public float sfxSlider;
}