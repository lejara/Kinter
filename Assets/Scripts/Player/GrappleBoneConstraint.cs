using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(MultiPositionConstraint))]
public class GrappleBoneConstraint : MonoBehaviour
{

    [SerializeField] PlayerController _playerController;
    MultiPositionConstraint _multiPositionConstraint;


    void Awake()
    {
        _multiPositionConstraint = GetComponent<MultiPositionConstraint>();
    }

    void Start()
    {
        _multiPositionConstraint.weight = 0;

        _playerController.OnGrappleDoneRetract += () =>
        {
            _multiPositionConstraint.weight = 0;
        };

        _playerController.OnGrappleShoot += () =>
        {
            _multiPositionConstraint.weight = 1;
        };
    }

}
