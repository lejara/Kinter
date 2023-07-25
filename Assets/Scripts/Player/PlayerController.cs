using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]
    public float sidewayMoveSpeed;                            // Basic movement speed 
    public bool isLanded;                                     // Booleans for preventing player swinging multiple time
    public bool isSwinging;                                   // Same as above
    [SerializeField] private float horizontalForce;           // Horizontal force to put player swing
    [SerializeField] private float maxSwingDistance;          // How long you can latch
    private Rigidbody playerRb;                               // Player rigidbody, used for movement and momentum

    [Header("References")]
    public float gravity;
    [SerializeField] private GameObject swingStartPoint;      // A starting point from the player, slightly above the character
    [SerializeField] private GameObject swingTargetIndicator; // Serialized for testing
    private Vector3 predictionPoint;                          // A Vector to store location for potantial swinging point
    private Vector3 swingPoint;                               // A Vector to store location of the Target
    private SpringJoint joint;                                // Joint
    
    
    // Start is called before the first frame update
    void Start()
    {
        isLanded = true;
        isSwinging = false;
        Physics.gravity = new(Physics.gravity.x, gravity, Physics.gravity.z);
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        isLanded = GroundCheck();
        
        #region Moving

        // Movement for sideway only (A/D), this is our basic movement
        if (isLanded && !isSwinging)
        {
            SidewayMoving(horizontalInput);
        }

        #endregion

        #region Swinging
        
        CheckForSwingingPoint();

        #endregion
    }

    private bool GroundCheck()
    {
        LayerMask layer = LayerMask.GetMask("Platform");
        return Physics.Raycast(GetComponent<BoxCollider>().bounds.center, Vector3.down, out _, GetComponent<BoxCollider>().bounds.extents.y + 0.1f, layer);
    }

    private void SidewayMoving(float horizontalInput)
    {
        playerRb.velocity = new Vector3(horizontalInput * sidewayMoveSpeed, playerRb.velocity.y, 0);
        /* Can have character flip here based on the direction of velocity. */
    }

    // A method to check for valid grapple points, player can only shoot grapple upwards
    private void CheckForSwingingPoint()
    {
        if (joint || isSwinging) return;

        LayerMask layer = LayerMask.GetMask("Platform");
        Vector3 hitPoint;

        if (Physics.Raycast(swingStartPoint.transform.position, Vector3.up, out RaycastHit directHit, maxSwingDistance, layer))
        {
            hitPoint = directHit.point;
        }
        else
        {
            hitPoint = Vector3.zero;
        }

        // A valid hit point found
        if (hitPoint != Vector3.zero)
        {
            swingTargetIndicator.transform.position = hitPoint;
            predictionPoint = hitPoint;
            if(!swingTargetIndicator.activeSelf)
            {
                swingTargetIndicator.SetActive(true);
            }
            Debug.Log("Hit with" + directHit.point);
        }
        else
        {
            predictionPoint = Vector3.zero;
            if (swingTargetIndicator.activeSelf)
            {
                swingTargetIndicator.SetActive(false);
            }
            Debug.Log("Missed!");
        }

    }
}
