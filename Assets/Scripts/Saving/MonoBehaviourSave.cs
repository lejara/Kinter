using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSave : MonoBehaviour
{
    [SerializeField] SaveOrchestrator saveOrchestrator;

    protected virtual void OnEnable()
    {
        saveOrchestrator.onSave += OnSave;
        saveOrchestrator.onLoad += OnLoad;
        saveOrchestrator.onReset += OnReset;

        //Make sure we are loaded
        OnLoad(saveOrchestrator.saveData);
    }

    protected virtual void OnDisable()
    {
        saveOrchestrator.onSave -= OnSave;
        saveOrchestrator.onLoad -= OnLoad;
        saveOrchestrator.onReset -= OnReset;
    }

    protected virtual void OnSave(ref SaveData data) { }
    protected virtual void OnLoad(SaveData data) { }
    protected virtual void OnReset(ref SaveData data) { }
}