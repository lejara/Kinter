using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationController : MonoBehaviour
{
    public float rotationLookOffset;

    [Header("References")]
    public Transform root;
    public Animator animator;

    PlayerController _playerController;


    bool _canPlaySwing;
    Quaternion _leftLook;
    Quaternion _rightLook;


    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        //Cache Rotations
        //TODO: BAD BAD SO BAD. If u see this fix this. its trash.
        // Debug.Assert(root.rotation == Quaternion.identity, $"{gameObject.name} rotation must be 0 in all axis");
        _rightLook = root.rotation;
        _leftLook = Quaternion.Euler(root.rotation.eulerAngles.x, 40, root.rotation.eulerAngles.z);


        //Events
        _playerController.OnStun += (Vector3 vel) =>
        {
            if (vel.x < 0)
            {
                LookLeft();
            }
            else
            {
                LookRight();
            }
            animator.SetBool("isStunned", true);
        };

        _playerController.OnStunExit += () =>
        {

            animator.SetBool("isStunned", false);
        };

        _playerController.OnLanded += () =>
        {
            animator.SetTrigger("land");
        };

        _playerController.WhileOnLand += (float dir) =>
        {
            if (dir == 0)
            {
                animator.SetBool("isWalking", false);
                return;
            }

            animator.SetBool("isWalking", true);
            if (dir > 0)
            {
                LookLeft();
            }
            else
            {
                LookRight();
            }
            //Note: update method will handle isLanded
        };


        _playerController.WhileSwinging += (input) =>
        {
            if (input > -0.1f && input < 0.1f)
            {
                animator.SetBool("swing_push", false);
                _canPlaySwing = true;
                return;
            }

            if (!_canPlaySwing)
            {
                return;
            }

            //Look
            if (input > 0)
            {
                LookLeft();
            }
            else
            {
                LookRight();
            }

            _canPlaySwing = false;
            animator.SetBool("swing_push", true);
        };


    }




    void Update()
    {
        animator.SetBool("isLanded", _playerController.isLanded);
        animator.SetBool("isSwinging", _playerController.isSwinging);

    }

    [ButtonMethod]
    void LookLeft()
    {
        root.rotation = _leftLook;
    }

    [ButtonMethod]
    void LookRight()
    {
        root.rotation = _rightLook;
    }
}
