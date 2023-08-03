using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipData
{
    public AudioClip clip;
    public float pitch = 1;

}

[RequireComponent(typeof(PlayerController))]
public class PlayerSoundController : MonoBehaviour
{
    [Header("Audio Clips")]

    public AudioClipData airTime;
    public AudioClipData landed;

    public AudioClipData grappleShooting;
    public AudioClipData grappleLatched;
    public AudioClipData grappleRetract;

    public AudioClipData swingForward;

    public AudioClipData cannotShootGrapple;

    [SerializeField] AudioSource _channel_one;
    [SerializeField] AudioSource _channel_two;

    [Header("More Settings")]

    public float swingDelayTime;

    PlayerController _playerController;

    bool _inSwingSoundDelay = false;
    bool _canPlaySwing = true;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        // _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        // _playerController.OnGrappleShoot = () => { Play(grappleShooting); };
        _playerController.OnGrappleLatch = () => { Play(grappleLatched); };

        _playerController.OnGrappleDetach = () => { Play(grappleRetract); };
        // _playerController.OnCannotShootGrapple = () => { if (Time.frameCount % 2 == 0) Play(cannotShootGrapple); };

        _playerController.OnLanded = () => { Play(landed); };
        _playerController.OnAir = () => { print("On Air"); };
        // _playerController.WhileInAir = (input) => { print(" air"); };
        // _playerController.WhileOnLand = (input) => { print("landed"); };
        _playerController.WhileSwinging = (input) =>
        {
            if (_inSwingSoundDelay)
            {
                return;
            }

            if (input == 0)
            {
                _canPlaySwing = true;
                return;
            }

            if (!_canPlaySwing)
            {
                return;
            }

            Vector3 swingDirection = (_playerController.grappleEndPoint.position - _playerController.grappleStartPoint.position).normalized;
            if (input < 0 && swingDirection.x < 0)
            {

                Play(swingForward);
                print("play");
            }
            else if (input > 0 && swingDirection.x > 0)
            {
                Play(swingForward);
                print("play");
            }
            print(swingDirection);
            _canPlaySwing = false;
            StartCoroutine(DelaySwingSound());

        };

        _playerController.OnStun = (vel) => { print("stunned"); };

    }

    void Play(AudioClipData clip)
    {
        if (clip == null)
        {
            return;
        }
        AudioSource audioSource;
        // if (_channel_one.isPlaying)
        // {
        //     audioSource = _channel_two;
        // }
        // else if (_channel_two.isPlaying)
        // {
        //     audioSource = _channel_one;
        // }
        // else
        // {
        //     audioSource = _channel_one;
        // }
        audioSource = _channel_one;

        audioSource.pitch = clip.pitch;
        audioSource.clip = clip.clip;
        audioSource.Play();
    }

    IEnumerator DelaySwingSound()
    {
        _inSwingSoundDelay = true;
        yield return new WaitForSeconds(swingDelayTime);
        _inSwingSoundDelay = false;
    }
}
