using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSave : MonoBehaviour
{
    [SerializeField] SaveOrchestrator saveState;

    protected virtual void OnEnable()
    {
        saveState.onSave += OnSave;
        saveState.onLoad += OnLoad;
        saveState.onReset += OnReset;
    }

    protected virtual void OnDisable()
    {
        saveState.onSave -= OnSave;
        saveState.onLoad -= OnLoad;
        saveState.onReset -= OnReset;
    }

    protected virtual void OnSave(ref SaveData data) { }
    protected virtual void OnLoad(SaveData data) { }
    protected virtual void OnReset() { }
}