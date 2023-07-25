using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuInput : MonoBehaviour
{

    public UnityEvent OnExitMenus;
    public UnityEvent OnEnterMenus;

    [SerializeField]
    GameState _gameState;

    void Update()
    {
        CheckToggleMenus();
    }

    void CheckToggleMenus()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (_gameState.state == States.InMenus)
            {
                _gameState.state = States.Playing;
                OnExitMenus.Invoke();
            }
            else if (_gameState.state == States.Playing)
            {
                _gameState.state = States.InMenus;
                OnEnterMenus.Invoke();
            }
        }
    }
}
