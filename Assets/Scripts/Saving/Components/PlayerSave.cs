using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Passes player data to orchestrator for saving and loading
/// Also invokes saves calls, depending on ammount of distance walked, if falling, stunned and when it first landed
/// </summary>
public class PlayerSave : MonoBehaviourSave
{
    [Header("Save Settings")]
    public float velocityFallThreshold;
    public float walkDistance;
    [Range(0, 270)] public float fallAngle;


    [Header("References")]
    public Transform playerSpawn;
    [SerializeField] DebugSettings debugSettings;

    PlayerController playerController;
    Rigidbody rb;

    bool _lastLanded;
    bool _lastStunned;
    bool _didAirSave;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (playerController.isLanded != _lastLanded)
        {
            if (playerController.isLanded)
            {
                OnLand();
            }
            else
            {
                InAir();
            }
        }
        if (playerController.isStunned != _lastStunned && playerController.isStunned)
        {
            OnStunned();
        }
        _lastLanded = playerController.isLanded;
        _lastStunned = playerController.isStunned;
    }



    void OnLand()
    {
        // print("land save");
        _didAirSave = false;
        Save();
        StartCoroutine(WhileOnLand());
    }

    void InAir()
    {
        StartCoroutine(WhileInAir());
    }

    void OnStunned()
    {
        // print("Stun Save");
        Save();
    }

    IEnumerator WhileOnLand()
    {
        Vector3 lastSavePosition = transform.position;
        while (playerController.isLanded)
        {
            if (Vector3.Distance(lastSavePosition, transform.position) > walkDistance)
            {
                lastSavePosition = transform.position;
                Save();
            }
            yield return null;
        }
    }

    IEnumerator WhileInAir()
    {
        while (!playerController.isLanded && !playerController.isStunned && !_didAirSave)
        {
            if (rb.velocity.magnitude < velocityFallThreshold)
            {

                yield return null;
                continue;
            }

            if (playerController.isSwinging)
            {
                yield return null;
                continue;
            }
            // print(rb.velocity.normalized);
            if (rb.velocity.normalized.y <= Mathf.Cos(fallAngle * Mathf.Deg2Rad))
            {
                Save();
                // print("fall save");
                _didAirSave = true;
            }
            yield return null;
        }
    }

    protected override void OnSave(ref SaveData data)
    {
        base.OnSave(ref data);
        data.playerData.position = transform.position;
        data.playerData.isStunned = playerController.isStunned;
        data.playerData.velocity = rb.velocity;
    }
    protected override void OnLoad(SaveData data)
    {
        base.OnLoad(data);

        //Don't load empty values
        if (data.playerData.position == Vector3.zero)
        {
            if (!debugSettings.stopSaveLoading)
            {
                //Go to spawn if we can load saves and have no save
                transform.position = playerSpawn.position;
            }
            return;
        }


        StartCoroutine(Delay(() =>
        {
            transform.position = data.playerData.position;
            rb.velocity = data.playerData.velocity;
            playerController.isStunned = data.playerData.isStunned;
        }));
    }

    //TOOD: remove dumb. but i need to get this working :(
    IEnumerator Delay(Action f)
    {

        yield return new WaitForSeconds(2f);
        f?.Invoke();
    }

    protected override void OnReset(ref SaveData data)
    {
        base.OnReset(ref data);

        data.playerData.position = playerSpawn.position;
        transform.position = playerSpawn.position;
        playerController.Reset();
        data.playerData.isStunned = playerController.isStunned;
        data.playerData.velocity = rb.velocity;
    }


}
