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

    [Header("More Settings")]

    public float swingDelayTime;
    public float distanceThresholdToPlayShoot;
    public float playLandMagThreshold;

    [Header("References")]
    [SerializeField] AudioSource _channel_one;
    [SerializeField] AudioSource _channel_two;

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
        _playerController.OnGrappleShoot = () =>
        {
            //Only play grapple shoot sound if its a long distance the grapple will travel
            if (Physics.Raycast(_playerController.grappleStartPoint.position, Vector3.up, out RaycastHit hit, _playerController.maxGrappleDistance))
            {

                if (Vector3.Distance(_playerController.grappleStartPoint.position, hit.point) > distanceThresholdToPlayShoot)
                {
                    Play(grappleShooting);
                }

            }
            else
            {
                Play(grappleShooting);
            }

        };
        _playerController.OnGrappleLatch = () => { Play(grappleLatched); };

        _playerController.OnGrappleDetach = () => { Play(grappleRetract); };
        // _playerController.OnCannotShootGrapple = () => { if (Time.frameCount % 2 == 0) Play(cannotShootGrapple); };

        // _playerController.OnStun = (vel) => { print("stunned"); };

        _playerController.OnLanded = () =>
        {
            if (_playerController.lastVelocity.magnitude > playLandMagThreshold)
            {
                Play(landed);
            }
        };
        // _playerController.OnAir = () => { print("On Air"); };
        // _playerController.WhileInAir = (input) => { print(" air"); };
        // _playerController.WhileOnLand = (input) => { print("landed"); };
        _playerController.WhileSwinging = (input) =>
        {
            if (input > -0.1f && input < 0.1f)
            {
                _canPlaySwing = true;
                return;
            }

            if (_inSwingSoundDelay)
            {
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
            }
            else if (input > 0 && swingDirection.x > 0)
            {
                Play(swingForward);
            }
            _canPlaySwing = false;
            StartCoroutine(DelaySwingSound());

        };
    }

    void Play(AudioClipData clip)
    {
        if (clip == null)
        {
            return;
        }
        AudioSource audioSource;
        if (_channel_one.isPlaying)
        {
            audioSource = _channel_two;
        }
        else if (_channel_two.isPlaying)
        {
            audioSource = _channel_one;
        }
        else
        {
            audioSource = _channel_one;
        }

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
