using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum AudioSliderType
{
    Music,
    SFX,
    Other,
}

[RequireComponent(typeof(Slider))]
public class AudioSliderSave : MonoBehaviourSave
{
    [Tooltip("Where should the value be stored in saveData")]
    public AudioSliderType audioSliderType;

    Slider slider;
    float _defualtValue;
    void Awake()
    {
        slider = GetComponent<Slider>();
        _defualtValue = slider.value;
    }

    protected override void OnSave(ref SaveData data)
    {
        base.OnSave(ref data);
        switch (audioSliderType)
        {
            case AudioSliderType.Music:
                data.settingsData.musicSlider = slider.value;
                break;
            case AudioSliderType.SFX:
                data.settingsData.sfxSlider = slider.value;
                break;
        }
    }

    protected override void OnLoad(SaveData data)
    {
        base.OnLoad(data);
        //Don't load empty values
        if (data.settingsData.musicSlider == -1 || data.settingsData.sfxSlider == -1)
        {
            return;
        }

        switch (audioSliderType)
        {
            case AudioSliderType.Music:
                slider.value = data.settingsData.musicSlider;
                break;
            case AudioSliderType.SFX:
                slider.value = data.settingsData.sfxSlider;
                break;
        }
    }

    protected override void OnReset(ref SaveData data)
    {
        base.OnReset(ref data);
        //Note: we want to keep the existing settings values when a global reset is called
    }

    protected override void OnSelfReset(ref SaveData data)
    {
        base.OnSelfReset(ref data);
        slider.value = _defualtValue;
    }

}

