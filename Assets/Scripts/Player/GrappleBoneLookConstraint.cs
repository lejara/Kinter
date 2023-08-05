using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(MultiAimConstraint))]
public class GrappleBoneLookConstraint : MonoBehaviour
{
    [SerializeField] PlayerController _playerController;
    MultiAimConstraint _multiPositionConstraint;


    void Awake()
    {
        _multiPositionConstraint = GetComponent<MultiAimConstraint>();
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
