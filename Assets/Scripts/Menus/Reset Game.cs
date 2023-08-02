using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    [SerializeField] SaveOrchestrator _saveOrchestrator;


    public void DoReset()
    {
        _saveOrchestrator.Reset();
    }
}
