using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]
    public float sidewayMoveSpeed;
    public bool isLanded;
    public bool isSwinging;
    [SerializeField] private float horizontalForce;
    [SerializeField] private float maxSwingDistance;
    [SerializeField] private Transform swingStartPoint;     
    private Rigidbody playerRb;

    [Header("References")]
    [SerializeField] private Transform swingTargetPoint;    // Serialized for testing
    [SerializeField] private float gravity;
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
    }

    private void Swinging(float horizontalInput)
    {

    }
}
