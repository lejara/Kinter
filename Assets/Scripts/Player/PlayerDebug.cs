using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerDebug : MonoBehaviourSave
{

    [SerializeField] DebugSettings _debugSettings;
    [SerializeField] GameObject _checkPointMarkerPrefab;

    GameObject _marker;
    PlayerController playerController;
    Vector3 _lastCheckpoint;

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

        if (_debugSettings.teleportOnStart && _debugSettings.checkpoint)
        {
            TeleportToCheckpoint();
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
        _marker = Instantiate(_checkPointMarkerPrefab, _lastCheckpoint, Quaternion.identity);
        StartCoroutine(WhileCheckpoint());
    }

    IEnumerator WhileCheckpoint()
    {
        while (_debugSettings.checkpoint)
        {
            //Teleport
            if (Input.GetKeyDown(_debugSettings.teleportToPointKey))
            {
                TeleportToCheckpoint();
            }

            //Add checkpoint
            if (Input.GetKeyDown(_debugSettings.addPointKey))
            {
                _lastCheckpoint = transform.position;
                _marker.transform.position = _lastCheckpoint;
                this.Save();
            }
            yield return null;
        }

    }

    void TeleportToCheckpoint()
    {
        if (_lastCheckpoint == Vector3.zero)
        {
            return;
        }

        playerController.Reset();
        transform.position = _lastCheckpoint;
    }
    void OnCheckpointDeactive()
    {
        GameObject.Destroy(_marker);
    }

    #region Save And Loading

    protected override void OnSave(ref SaveData data)
    {
        base.OnSave(ref data);
        data.debugSaveData.lastCheckpointPos = _lastCheckpoint;
    }

    protected override void OnLoad(SaveData data)
    {
        base.OnLoad(data);
        if (data.debugSaveData.lastCheckpointPos == Vector3.zero)
        {
            return;
        }
        _lastCheckpoint = data.debugSaveData.lastCheckpointPos;

    }

    protected override void OnSelfReset(ref SaveData data)
    {
        base.OnSelfReset(ref data);
        _lastCheckpoint = Vector3.zero;
    }

    #endregion
}
