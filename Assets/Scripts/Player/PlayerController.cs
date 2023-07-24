using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]
    public float sidewayMoveSpeed;                      // Basic movement speed 
    public bool isLanded;                               // Booleans for preventing player swinging multiple time
    public bool isSwinging;                             // Same as above
    [SerializeField] private float horizontalForce;     // Horizontal force to put player swing
    [SerializeField] private float maxSwingDistance;    // How long you can latch
    [SerializeField] private Transform swingStartPoint; // A starting point from the player, slightly above the character
    private Rigidbody playerRb;                         // Player rigidbody, used for movement and momentum

    [Header("References")]
    public float gravity;
    [SerializeField] private Transform swingTargetPoint; // Serialized for testing
    private SpringJoint joint;
    
    
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
        
        #region Moving

        // Movement for sideway only (A/D), this is our basic movement
        if (isLanded && !isSwinging)
        {
            SidewayMoving(horizontalInput);
        }
        
        #endregion
    }

    private void SidewayMoving(float horizontalInput)
    {
        playerRb.velocity = new Vector3(horizontalInput * sidewayMoveSpeed, playerRb.velocity.y, 0);
        /* Can have character flip here based on the direction of velocity. */
    }

    private void Swinging(float horizontalInput)
    {

    }
}
