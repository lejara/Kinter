using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

/// <summary>
/// MonoBehaviourSave is responsible to subscribe to SaveOrchestrator's events. It should provide its data to be saved. And load and reset when needed.
// It can also invoke save events
/// </summary>
public class MonoBehaviourSave : MonoBehaviour
{
    [SerializeField] SaveOrchestrator saveOrchestrator;

    /// <summary>
    /// A self reset can happen on the use case where you would like to reset this gameobject without making all other subscribers reset as well.
    /// </summary>
    [ButtonMethod]
    public void ResetSelf()
    {
        ResetSelf(true);
    }

    /// <summary>
    /// A self reset can happen on the use case where you would like to reset this gameobject without making all other subscribers reset as well.
    /// </summary>
    public void ResetSelf(bool save)
    {
        OnSelfReset(ref saveOrchestrator.saveData);
        if (save)
        {
            saveOrchestrator.Save();
        }
    }

    public void Save()
    {
        saveOrchestrator.Save();
    }

    protected virtual void OnEnable()
    {
        saveOrchestrator.onSave += OnSave;
        saveOrchestrator.onLoad += OnLoad;
        saveOrchestrator.onReset += OnReset;

        //NOTE: on fresh boot, the saveData will be empty until the first save is called.
        //For all subscribers we don't make them load saveData
        if (saveOrchestrator.saveExist)
        {
            //Make sure we are loaded
            OnLoad(saveOrchestrator.saveData);
        }
    }

    protected virtual void OnDisable()
    {
        saveOrchestrator.onSave -= OnSave;
        saveOrchestrator.onLoad -= OnLoad;
        saveOrchestrator.onReset -= OnReset;
    }

    /// <summary>
    /// Event, must mutate the param SaveData for it to be saved.
    /// </summary>
    protected virtual void OnSave(ref SaveData data) { }
    /// <summary>
    /// Event, must overrite its data to whats on SaveData
    /// </summary>
    protected virtual void OnLoad(SaveData data) { }
    /// <summary>
    /// Event if need be, must reset to a defualt state and provide its new data to SaveData. OnReset is called when all subscribers should reset
    /// </summary>
    protected virtual void OnReset(ref SaveData data) { }
    /// <summary>
    /// Event if need be, a self reset can happen on the use case where you would like to reset this gameobject only.
    /// Just like OnReset, you must mutate the param SaveData with the new data
    /// </summary>
    protected virtual void OnSelfReset(ref SaveData data) { }
}


// protected override void OnSave(ref SaveData data) { base.OnSave(ref data);  }

// protected override void OnLoad(SaveData data) { base.OnLoad(data); }

// protected override void OnReset(ref SaveData data) { base.OnReset(ref data); }

// protected override void OnSelfReset(ref SaveData data) { base.OnSelfReset(ref data); }