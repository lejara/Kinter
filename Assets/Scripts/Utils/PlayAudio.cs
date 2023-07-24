using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudio : MonoBehaviour
{
    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [ContextMenu("Play")]
    void Play()
    {
        audioSource.Play();
    }

    [ContextMenu("Stop")]
    void Stop()
    {
        audioSource.Stop();
    }

    [ContextMenu("Pause")]
    void Pause()
    {
        audioSource.Pause();
    }
}
