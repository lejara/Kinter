using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: test in production
[RequireComponent(typeof(PlayerController))]
public class PlayerDebug : MonoBehaviour
{

    [SerializeField] DebugSettings _debugSettings;
    PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
#if !UNITY_EDITOR
    this.enabled = false;
#endif
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
            //TODO: make sure you reset the player controller
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _debugSettings.checkpoint = !_debugSettings.checkpoint;
            //TODO: checkpoints save should persist across play
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
        StartCoroutine(WhileCheckpoint());
    }

    IEnumerator WhileCheckpoint()
    {
        while (_debugSettings.checkpoint)
        {
            yield return null;
        }

    }
    void OnCheckpointDeactive()
    {

    }
}
