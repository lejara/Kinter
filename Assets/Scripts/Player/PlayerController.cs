using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class PlayerController : MonoBehaviour
{

    [Header("Grapple Attributes")]

    [ReadOnly]
    public bool isGrappling;
    [ReadOnly]
    public bool isSwinging;
    public float grappleTravelTime;
    public float maxSwingDistance;
    public AnimationCurve grappleTravelCurve;

    [Header("Player Attributes")]
    [ReadOnly]
    public bool isLanded;
    public float gravity;
    public float sidewayMoveSpeed;
    public float horizontalForce;


    [Header("References")]
    [SerializeField] LineRenderer lineRenderer;     // A LineRenderer to draw swinging rope
    [SerializeField] GameObject swingStartPoint;
    [SerializeField] GameObject swingTargetIndicator;

    float horizontalInput;
    Vector3 predictionPoint;
    Vector3 swingPoint;
    SpringJoint joint;
    Rigidbody playerRb;
    Coroutine grappleCoroutine;


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
        horizontalInput = Input.GetAxis("Horizontal");
        isLanded = GroundCheck() && !isSwinging;


        #region Swinging

        //REDO
        CheckForSwingingPoint();
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isSwinging)
        {
            SwingingStart();
            Debug.Log("Hit with" + swingPoint + "and isSwinging is set to: " + isSwinging);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && isSwinging)
        {
            SwingingStop();
            Debug.Log("Now it releases, and isSwinging set back to: " + isSwinging);
        }
        // /REDO


        #endregion
    }

    void FixedUpdate()
    {
        // Movement for sideway only (A/D), this is our basic movement
        if (isLanded)
        {
            SidewayMoving(horizontalInput);
        }

        if (isSwinging)
        {
            Swinging(horizontalInput);
        }
    }

    private bool GroundCheck()
    {
        LayerMask layer = LayerMask.GetMask("Platform");
        Vector3 squareExtents = new(GetComponent<BoxCollider>().bounds.extents.x, 0, GetComponent<BoxCollider>().bounds.extents.z);
        return Physics.BoxCast(GetComponent<BoxCollider>().bounds.center, squareExtents,
                                Vector3.down, out _, Quaternion.identity, GetComponent<BoxCollider>().bounds.extents.y + 0.1f, layer);
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
            if (!swingTargetIndicator.activeSelf)
            {
                swingTargetIndicator.SetActive(true);
            }
        }
        else
        {
            predictionPoint = Vector3.zero;
            if (swingTargetIndicator.activeSelf)
            {
                swingTargetIndicator.SetActive(false);
            }
        }
    }

    private void SwingingStart()
    {
        if (joint || isSwinging || predictionPoint == Vector3.zero) return;

        isSwinging = true;
        isLanded = false;

        if (swingTargetIndicator.activeSelf) swingTargetIndicator.SetActive(false);

        if (!lineRenderer.enabled) lineRenderer.enabled = true;

        // Joint Setup
        swingPoint = predictionPoint;
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distance = Vector3.Distance(swingStartPoint.transform.position, swingPoint);
        joint.maxDistance = 0.6f * distance;
        joint.minDistance = 0.3f * distance;
        joint.spring = 8f;
        joint.damper = 7f;
        joint.massScale = 3f;

        lineRenderer.positionCount = 2;
    }

    private void SwingingStop()
    {
        isSwinging = false;
        lineRenderer.enabled = false;
        Destroy(joint);
    }

    private void Swinging(float horizontalInput)
    {
        playerRb.AddForce(horizontalInput * horizontalForce * Vector3.right, ForceMode.Force);
        lineRenderer.SetPosition(0, swingStartPoint.transform.position);
        lineRenderer.SetPosition(1, swingPoint);
    }
}
