using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class MonoBehaviourSave : MonoBehaviour
{
    [SerializeField] SaveOrchestrator saveOrchestrator;

    [ButtonMethod]
    public void ResetSelf()
    {
        ResetSelf(true);
    }

    public void ResetSelf(bool save)
    {
        OnSelfReset(ref saveOrchestrator.saveData);
        if (save)
        {
            saveOrchestrator.Save();
        }
    }

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

    //TOOD: comment these
    protected virtual void OnSave(ref SaveData data) { }
    protected virtual void OnLoad(SaveData data) { }
    protected virtual void OnReset(ref SaveData data) { }
    protected virtual void OnSelfReset(ref SaveData data) { }
}