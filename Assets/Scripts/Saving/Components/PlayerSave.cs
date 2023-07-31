using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSave : MonoBehaviourSave
{

    [SerializeField] SaveOrchestrator saveState;
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
    protected override void OnLoad(ref SaveData data)
    {
        base.OnLoad(ref data);
        transform.position = data.playerData.position;
    }
    protected override void OnReset()
    {
        base.OnReset();
        transform.position = _startingPosition;
    }


}
