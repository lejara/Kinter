using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Util component. Will trigger a unity event depending on selected in state.
/// </summary>
public class GameStateListener : MonoBehaviour
{

    [Tooltip("On what state should the events trigger")]
    public States inState;
    public UnityEvent onInState;
    public UnityEvent onOutState;

    [SerializeField]
    GameState _gameState;

    void Start()
    {
        if (Time.frameCount == 1)
        {
            StateChanged(_gameState.state);
        }
    }
    void OnEnable()
    {
        _gameState.onStateChange += StateChanged;
        //Note: Prevent race cons. Awake and OnEnable are not in sync across scripts
        if (Time.frameCount > 1)
        {
            //Call the State Change anyways to check the active state
            StateChanged(_gameState.state);
        }

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
