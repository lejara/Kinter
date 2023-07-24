using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Slider))]
public class AudioSlider : MonoBehaviour
{
    public AudioMixerGroup mixerGroup;

    [SerializeField]
    AudioMixer mixer;

    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        slider.onValueChanged.AddListener((float value) =>
        {
            mixer.SetFloat(mixerGroup.name, Mathf.Log10(value) * 20);
        });

        slider.minValue = 0.0001f;
    }


}
