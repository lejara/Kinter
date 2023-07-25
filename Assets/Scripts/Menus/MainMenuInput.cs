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

    public void ShowMenu()
    {
        _gameState.state = States.InMenus;
        OnEnterMenus.Invoke();
    }

    public void HideMenu()
    {
        _gameState.state = States.Playing;
        OnExitMenus.Invoke();
    }

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
                HideMenu();
            }
            else if (_gameState.state == States.Playing)
            {

                ShowMenu();
            }
        }
    }
}
