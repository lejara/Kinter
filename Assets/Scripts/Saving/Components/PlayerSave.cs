using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSave : MonoBehaviourSave
{
    Vector3 _startingPosition;
    void Awake()
    {
        _startingPosition = transform.position;
    }


    protected override void OnSave(ref SaveData data)
    {
        base.OnSave(ref data);
        data.playerData.position = transform.position;
    }
    protected override void OnLoad(SaveData data)
    {
        base.OnLoad(data);
        transform.position = data.playerData.position;
    }
    protected override void OnReset(ref SaveData data)
    {
        base.OnReset(ref data);
        data.playerData.position = _startingPosition;
        transform.position = _startingPosition;
    }


}
