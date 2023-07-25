using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Util component. Will trigger a unity event depending on selected in state.
/// </summary>
public class GameStateListener : MonoBehaviour
{

    public States inState;
    public UnityEvent onInState;
    public UnityEvent onOutState;

    [SerializeField]
    GameState _gameState;

    void OnEnable()
    {
        _gameState.onStateChange += StateChanged;
        //Call the State Change anyways to check the active state
        StateChanged(_gameState.state);
    }

    void OnDisable()
    {
        _gameState.onStateChange -= StateChanged;
    }

    void StateChanged(States state)
    {
        if (state == inState)
        {
            onInState.Invoke();
        }
        else
        {
            onOutState.Invoke();
        }
    }
}
