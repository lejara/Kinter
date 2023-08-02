using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


public enum States
{
    InMenus,
    Playing,
}

[CreateAssetMenu(fileName = "GameState")]
public class GameState : ScriptableObject
{
    public States state
    {
        get { return this._state; }
        set
        {
            onStateChange?.Invoke(value);
            this._state = value;
        }
    }
    public Action<States> onStateChange;

    [SerializeField]
    DebugSettings _debugSettings;

    [Header("Information")]

    [ReadOnly]
    [SerializeField]
    States _state;

    void OnEnable()
    {
#if !UNITY_EDITOR
    _debugSettings.SetProduction();
#endif
        _state = States.InMenus;

        if (_debugSettings.skipMainMenu)
        {
            _state = States.Playing;
        }

        onStateChange?.Invoke(_state);
    }

}
