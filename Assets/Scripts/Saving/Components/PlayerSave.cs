using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSave : MonoBehaviourSave
{

    [SerializeField] Transform playerSpawn;
    [SerializeField] DebugSettings debugSettings;

    void Start()
    {
        if (!debugSettings.stopSaveLoading)
        {
            transform.position = playerSpawn.position;
        }
    }

    protected override void OnSave(ref SaveData data)
    {
        base.OnSave(ref data);
        data.playerData.position = transform.position;
    }
    protected override void OnLoad(SaveData data)
    {
        base.OnLoad(data);

        //Don't load empty values
        if (data.playerData.position == Vector3.zero)
        {
            return;
        }
        transform.position = data.playerData.position;
    }
    protected override void OnReset(ref SaveData data)
    {
        base.OnReset(ref data);
        data.playerData.position = playerSpawn.position;
        transform.position = playerSpawn.position;
    }


}
