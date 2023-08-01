using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: test in production
[RequireComponent(typeof(PlayerController))]
public class PlayerDebug : MonoBehaviour
{
    [SerializeField] DebugSettings _debugSettings;
    [SerializeField] GameObject _checkPointMarkerPrefab;

    GameObject _marker;
    PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
#if !UNITY_EDITOR
    this.enabled = false;
#endif
    }

    void Start()
    {
        if (_debugSettings.godMode)
        {
            OnGodModeActive();
        }
        if (_debugSettings.checkpoint)
        {
            OnCheckpointActive();
        }

    }


    void Update()
    {
        if (Input.GetKeyDown(_debugSettings.godModeActivateKey))
        {
            _debugSettings.godMode = !_debugSettings.godMode;

            if (_debugSettings.godMode)
            {
                OnGodModeActive();
            }
            else
            {
                OnGodModeDeactive();
            }
        }

        if (Input.GetKeyDown(_debugSettings.checkpointActivateKey))
        {
            _debugSettings.checkpoint = !_debugSettings.checkpoint;

            if (_debugSettings.checkpoint)
            {
                OnCheckpointActive();
            }
            else
            {
                OnCheckpointDeactive();
            }
        }

    }

    void OnGodModeActive()
    {
        playerController.Reset();
        playerController.GetComponent<Rigidbody>().isKinematic = true;
        playerController.GetComponent<Collider>().enabled = false;

        StartCoroutine(WhileGodMode());
    }

    IEnumerator WhileGodMode()
    {
        Func<float, float> getSpeed = (float axis) =>
        {
            if (Input.GetKey(_debugSettings.godModeBoostKey))
            {
                return axis * _debugSettings.godModeBoostSpeed * Time.deltaTime;
            }
            return axis * _debugSettings.godModeSpeed * Time.deltaTime;
        };

        while (_debugSettings.godMode)
        {

            playerController.transform.Translate(new Vector3(getSpeed(Input.GetAxis("Horizontal")), getSpeed(Input.GetAxis("Vertical"))));
            yield return null;
        }

    }

    void OnGodModeDeactive()
    {
        playerController.GetComponent<Rigidbody>().isKinematic = false;
        playerController.GetComponent<Collider>().enabled = true;
    }

    void OnCheckpointActive()
    {
        _marker = Instantiate(_checkPointMarkerPrefab, _debugSettings.lastCheckpoint, Quaternion.identity);
        StartCoroutine(WhileCheckpoint());
    }

    IEnumerator WhileCheckpoint()
    {
        while (_debugSettings.checkpoint)
        {
            if (Input.GetKeyDown(_debugSettings.teleportToPointKey) && _debugSettings.lastCheckpoint != Vector3.zero)
            {
                playerController.Reset();
                transform.position = _debugSettings.lastCheckpoint;
            }

            if (Input.GetKeyDown(_debugSettings.addPointKey))
            {
                _debugSettings.lastCheckpoint = transform.position;
                _marker.transform.position = _debugSettings.lastCheckpoint;
            }
            yield return null;
        }

    }
    void OnCheckpointDeactive()
    {
        GameObject.Destroy(_marker);
    }
}
