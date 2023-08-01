using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{

    [SerializeField] DebugSettings _debugSettings;

    void Awake()
    {
#if !UNITY_EDITOR
    this.enabled = false;
#endif
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _debugSettings.godMode = !_debugSettings.godMode;
            //TODO: make sure you reset the player controller
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _debugSettings.checkpoint = !_debugSettings.checkpoint;
            //TODO: checkpoints save should persist across play
        }
    }
}
