using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDebugUI : MonoBehaviour
{
    [SerializeField] GameObject _panel;
    [SerializeField] TMP_Text _textList;
    [SerializeField] DebugSettings _debugSettings;

    void Awake()
    {
#if !UNITY_EDITOR
    gameObject.SetActive(false);
#endif
    }

    void Update()
    {
        _panel.SetActive(false);

        if (_debugSettings.isPlayerInDebug)
        {
            _panel.SetActive(true);
        }
        _textList.text = "";

        if (_debugSettings.godMode)
        {
            _textList.text += $"GodMode {Environment.NewLine}";
        }
        if (_debugSettings.checkpoint)
        {
            _textList.text += $"Checkpoint {Environment.NewLine}";
        }

    }
}
