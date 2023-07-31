using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "SaveOrchestrator")]
public class SaveOrchestrator : ScriptableObject
{
    public delegate void SaveDataEventWrite(ref SaveData saveData);
    public delegate void SaveDataEventRead(SaveData saveData);

    public bool verbose;
    public bool saveExist { get { return File.Exists(this._path); } }
    public string fileName;

    //Note: Should not be setting this
    [ReadOnly] public SaveData saveData; //TODO: don't serialize  
    public SaveDataEventWrite onSave;
    public SaveDataEventRead onLoad;
    public SaveDataEventWrite onReset;


    string _path;



    void OnEnable()
    {
        _path = Path.Combine(Application.persistentDataPath, fileName);
        Load();
    }


    [ButtonMethod]
    public void Save()
    {
        Log("Save");
        onSave?.Invoke(ref saveData);
        WriteToFile(saveData);
    }

    [ButtonMethod]
    public void Load()
    {
        if (!saveExist)
        {
            Save();
            return;
        }

        Log("Load");
        saveData = LoadFromFile(saveData);
        onLoad?.Invoke(saveData);
    }

    //A reset 
    [ButtonMethod]
    public void Reset()
    {
        Log("Reset");
        onReset?.Invoke(ref saveData);
        WriteToFile(saveData);
    }

    //Will delete
    [ButtonMethod]
    public void Clear()
    {
        Log("Clear");
        Reset();
        File.Delete(_path);
    }


    void WriteToFile(SaveData data)
    {
        //TODO (remove): C:\Users\Lejara\AppData\LocalLow\DefaultCompany\Kinter
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
            JsonUtility.FromJsonOverwrite(File.ReadAllText(_path), saveData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load from file with exception {e}");
        }
        return saveData;
    }

    void Log(string msg)
    {
        if (verbose)
        {
            Debug.Log(msg);
        }
    }
}
