using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    GameState _gameState;

    public void Play()
    {
        _gameState.state = States.Playing;
    }
}
