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

    [Tooltip("Delay to apply after each swing sound")]
    public float swingDelay;
    [Tooltip("Distance Threshold to when shooting a grapple should play its sound. We don't want to play the sound if the grapple will travel a short distance")]
    public float distanceThresholdForGrappleShoot;
    [Tooltip("Landing threshold for player's last velocity mag. Play a the landing sound if over. We don't want to play it on a short landing")]
    public float LandingThreshold;
    [Tooltip("How far should the ray shoot to check how close a floor is. If hits, the airTime sound will not play")]
    public float inAirRayDistance;
    [Tooltip("How fast (velocity mag) the player should be to allow for a ray test for airTime sound")]
    public float inAirThreshold;

    [Header("References")]
    [SerializeField] AudioSource _channel_one;
    [SerializeField] AudioSource _channel_two;

    PlayerController _playerController;

    bool _inSwingSoundDelay = false;
    bool _canPlaySwing = true;
    bool _playedInAirSound = false;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        _playerController.OnGrappleShoot = () =>
        {
            //Only play grapple shoot sound if its a long distance the grapple will travel
            if (Physics.Raycast(_playerController.grappleStartPoint.position, Vector3.up, out RaycastHit hit, _playerController.maxGrappleDistance))
            {

                if (Vector3.Distance(_playerController.grappleStartPoint.position, hit.point) > distanceThresholdForGrappleShoot)
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


        _playerController.OnLanded = () =>
        {
            _playedInAirSound = false;
            if (_playerController.lastVelocity.magnitude > LandingThreshold)
            {
                Play(landed);
            }
        };

        _playerController.WhileInAir = (input) =>
        {
            if (!_playedInAirSound &&
            !_playerController.isSwinging &&
            _playerController.playerRb.velocity.y < 0 &&
            _playerController.playerRb.velocity.magnitude > inAirThreshold &&
            !Physics.Raycast(_playerController.playerRb.position, Vector3.down, out RaycastHit _, inAirRayDistance))
            {
                _playedInAirSound = true;
                Play(airTime);
            }
        };

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

        // _playerController.OnCannotShootGrapple = () => { if (Time.frameCount % 2 == 0) Play(cannotShootGrapple); };
        // _playerController.OnStun = (vel) => { print("stunned"); };
        // _playerController.OnAir = () => { print("On Air"); };
        // _playerController.WhileOnLand = (input) => { print("landed"); };
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
        yield return new WaitForSeconds(swingDelay);
        _inSwingSoundDelay = false;
    }
}
