using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerWorldContraint : MonoBehaviour
{
    public float cameraBoundsOffset;
    public BoxCollider boundaries;
    public CinemachineVirtualCamera playerVCam;


    float _camHalfHeight;
    float _camHalfWidth;

    Rigidbody _playerRB;
    GameObject _followDummy;
    Camera _cam;

    void Awake()
    {
        _playerRB = GetComponent<Rigidbody>();
        _cam = Camera.main;

        if (boundaries == null)
        {
            return;
        }

        //Replace player vcam follow to a dummy so we can control the vcam
        _followDummy = Instantiate(new GameObject(), _playerRB.position, Quaternion.identity);
        _followDummy.name = "player-follow-dummy";
        _followDummy.transform.parent = this.transform.parent;

        playerVCam.Follow = _followDummy.transform;
    }

    void Start()
    {
        float camDepth = Mathf.Abs(_cam.transform.position.z);
        float halfFieldOfView = _cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        _camHalfHeight = camDepth * Mathf.Tan(halfFieldOfView);
        _camHalfWidth = _cam.aspect * _camHalfHeight;

        Debug.Assert(boundaries != null, "Boundaries is empty. will not contrain player and its VCamera");
    }

    void Update()
    {
        RestrictCamera();
    }

    void FixedUpdate()
    {
        RestrictPlayer();
    }

    void RestrictCamera()
    {
        if (boundaries == null)
        {
            return;
        }

        Vector3 min = boundaries.bounds.min;
        min.x += cameraBoundsOffset;
        Vector3 max = boundaries.bounds.max;
        max.x -= cameraBoundsOffset;

        float x = Mathf.Clamp(_playerRB.position.x, min.x + _camHalfWidth, max.x - _camHalfWidth);

        _followDummy.transform.position = new Vector3(x, _playerRB.position.y, _playerRB.position.z);
    }

    void RestrictPlayer()
    {
        if (boundaries == null)
        {
            return;
        }
        Vector3 playerPos = _playerRB.transform.position;
        playerPos.x = Mathf.Clamp(playerPos.x, boundaries.bounds.min.x, boundaries.bounds.max.x);
        _playerRB.MovePosition(playerPos);
    }

}
