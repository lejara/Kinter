using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using MyBox;

/// <summary>
/// SaveOrchestrator is responsible to emit all save events and collect save data for it to be written or loaded 
/// </summary>
[CreateAssetMenu(fileName = "SaveOrchestrator")]
public class SaveOrchestrator : ScriptableObject
{
    public delegate void SaveDataEventWrite(ref SaveData saveData);
    public delegate void SaveDataEventRead(SaveData saveData);

    public bool verbose;
    public bool saveExist { get { return File.Exists(this._path) && !this.debugSettings.stopSaveLoading; } }
    public string fileName;


    /// <summary>
    /// Holds the latest save data. 
    /// Should only be used for reading and only allow mutation in MonoBehaviourSave for self resets.
    /// </summary>
    [HideInInspector][NonSerialized] public SaveData saveData;
    public SaveDataEventWrite onSave;
    public SaveDataEventRead onLoad;
    public SaveDataEventWrite onReset;


    string _path;
    [SerializeField] DebugSettings debugSettings;


    void OnEnable()
    {
        saveData = new SaveData();
        _path = Path.Combine(Application.persistentDataPath, fileName);
        Load();
    }

    /// <summary>
    /// Calls all subs to write their new values to saveData and then write a save
    /// </summary>
    [ButtonMethod]
    public void Save()
    {

        if (debugSettings.stopSaving)
        {
            return;
        }

        Log("Save");
        onSave?.Invoke(ref saveData);
        WriteToFile(saveData);
    }
    #region Invokers
    /// <summary>
    /// Loads save and calls all subs to load the values of saveData
    /// </summary>
    [ButtonMethod]
    public void Load()
    {
        //NOTE: on fresh boot the saveData will be empty until the first save is called.
        //For all subscribers we don't make them load saveData
        if (!saveExist)
        {
            return;
        }

        Log("Load");
        saveData = LoadFromFile(saveData);
        onLoad?.Invoke(saveData);
    }

    /// <summary>
    /// Calls all subs to reset to defualt values and then save
    /// </summary>
    [ButtonMethod]
    public void Reset()
    {
        Log("Reset");
        onReset?.Invoke(ref saveData);
        WriteToFile(saveData);
    }

    /// <summary>
    /// Calls all subs to reset to defualt values and then delete save
    /// </summary>
    [ButtonMethod]
    public void Clear()
    {
        Log("Clear");
        saveData = new SaveData();
        Reset();
        File.Delete(_path);
    }
    #endregion


    #region FileIO
    void WriteToFile(SaveData data)
    {
        try
        {
            File.WriteAllText(_path, JsonUtility.ToJson(data));
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save {_path} with exception {e}");
        }
    }

    SaveData LoadFromFile(SaveData saveData)
    {
        try
        {

            object boxedStruct = saveData;
            JsonUtility.FromJsonOverwrite(File.ReadAllText(_path), boxedStruct);
            saveData = (SaveData)boxedStruct;
            Log(File.ReadAllText(_path));
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load from file with exception {e}");
        }
        return saveData;
    }
    #endregion



    #region Debug
    void Log(string msg)
    {
        if (verbose)
        {
            Debug.Log(msg);
        }
    }

    [ButtonMethod]
    void PrintSaveData()
    {
        Debug.Log(JsonUtility.ToJson(saveData));
    }
    #endregion

}
