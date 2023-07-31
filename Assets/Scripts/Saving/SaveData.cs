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
}

[System.Serializable]
public class PlayerSaveData
{
    public Vector3 position = Vector3.zero;
}

[System.Serializable]
public class SettingsSaveData
{
    public float musicSlider = -1;
    public float sfxSlider = -1;
}