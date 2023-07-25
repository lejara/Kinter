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
        set { this._state = value; }
    }

    [SerializeField]
    DebugSettings debugSettings;

    [Header("Information")]

    [ReadOnly]
    [SerializeField]
    States _state;

    void OnEnable()
    {
        _state = States.InMenus;

        //NOTE: It's assumed the excution order of DebugSettings runs first before this script
        //This is done by changing Unity's "Script Execution Order settings"
        if (debugSettings.skipMainMenu)
        {
            _state = States.Playing;
        }

    }

}
