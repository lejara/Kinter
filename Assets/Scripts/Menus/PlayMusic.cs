using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(AudioSource))]
public class PlayMusic : MonoBehaviour
{
    [ReadOnly] public bool played = false;

    AudioSource _audioSource;
    [SerializeField] DebugSettings debugSettings;
    // Start is called before the first frame update
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (!played && !debugSettings.skipMainMenu)
        {
            _audioSource.Play();
            played = true;
        }

    }
}
