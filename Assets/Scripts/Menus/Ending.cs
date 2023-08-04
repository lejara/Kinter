using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class Ending : MonoBehaviour
{
    public UnityEvent OnEnterEnding;
    [SerializeField]
    GameState _gameState;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _gameState.state == States.Playing)
        {
            ShowEnding();
        }
    }

    [ButtonMethod]
    public void ShowEnding()
    {
        _gameState.state = States.Ending;
        OnEnterEnding.Invoke();
    }
}
