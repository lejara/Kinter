using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(MultiAimConstraint))]
public class BodyAimConstraint : MonoBehaviour
{
    [SerializeField] float weight;
    [SerializeField] PlayerController _playerController;

    MultiAimConstraint _multiAimConstraint;

    // Start is called before the first frame update
    void Awake()
    {
        _multiAimConstraint = GetComponent<MultiAimConstraint>();
    }

    void Start()
    {
        _multiAimConstraint.weight = 0;

        _playerController.OnGrappleLatch += () =>
        {
            _multiAimConstraint.weight = weight;
        };

        _playerController.OnGrappleDetach += () =>
 {
     _multiAimConstraint.weight = 0;
 };
    }
}
