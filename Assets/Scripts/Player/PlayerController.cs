using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]
    [ReadOnly]
    public bool isLanded;
    public float gravity;
    public float sidewayMoveSpeed;

    [Header("Grapple Attributes")]

    [ReadOnly]
    public bool isGrappling;
    [ReadOnly]
    public bool isSwinging;
    [ReadOnly]
    public bool isRetracting;
    public float grappleTravelTime;
    public float grappleRetractTime;
    public float maxGrappleDistance;
    public LayerMask platfromLayer;
    public AnimationCurve grappleTravelCurve;
    public AnimationCurve grappleRetractCurve;
    [Tooltip("Growth scale for how much time to take off based on grapple length")]
    public AnimationCurve grappleTimeRetractCurve;


    [Header("Swing Attributes")]
    public float horizontalForce;


    [Header("References")]
    [SerializeField] Transform grappleStartPoint;
    [SerializeField] Transform grappleEndPoint;
    [SerializeField] GameState gameState;

    float horizontalInput;
    Vector3 predictionPoint;
    SpringJoint joint;
    Rigidbody playerRb;
    LineRenderer lineRenderer;


    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }
    void Start()
    {
        Physics.gravity = new(Physics.gravity.x, gravity, Physics.gravity.z);

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;

        //Tests. Make sure we have the correct hierarchy.
        Debug.Assert(transform.GetChildsWhere((childT) => (childT == grappleStartPoint)).Count == 1,
        "grappleStartPoint must be a transfom child of this object");

        Debug.Assert(transform.GetChildsWhere((childT) => (childT == grappleEndPoint)).Count == 0,
        "grappleEndPoint must not be a transfrom child of this object");

    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, grappleStartPoint.position);

        if (gameState.state != States.Playing)
        {
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");

        #region Grapple

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isGrappling && !isSwinging && !isRetracting)
        {
            StartCoroutine(ShootGrapple());
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && isSwinging)
        {
            DetachGrapple();
        }

        //It is assumed grappleEndPoint is not a transfrom child of this object.
        //We need to keep it synced when its not in use
        if (!isSwinging && !isGrappling && !isRetracting)
        {
            SetGrapplePosition(grappleStartPoint.position);
        }

        #endregion
    }

    void FixedUpdate()
    {
        isLanded = GroundCheck() && !isSwinging;

        // Movement for sideway only (A/D), this is our basic movement
        if (isLanded)
        {
            SidewayMoving(horizontalInput);
        }

        if (isSwinging)
        {
            Swing(horizontalInput);
        }
    }

    private bool GroundCheck()
    {
        Vector3 squareExtents = new(GetComponent<BoxCollider>().bounds.extents.x, 0, GetComponent<BoxCollider>().bounds.extents.z);
        return Physics.BoxCast(GetComponent<BoxCollider>().bounds.center, squareExtents,
                                Vector3.down, out _, Quaternion.identity, GetComponent<BoxCollider>().bounds.extents.y + 0.1f);
    }

    private void SidewayMoving(float horizontalInput)
    {
        playerRb.velocity = new Vector3(horizontalInput * sidewayMoveSpeed, playerRb.velocity.y, 0);
        /* Can have character flip here based on the direction of velocity. */
    }

    #region Grapple Methods

    private IEnumerator ShootGrapple()
    {

        isGrappling = true;
        lineRenderer.enabled = true;

        float normTime = 0;
        Vector3 target = grappleStartPoint.position + (Vector3.up * maxGrappleDistance);
        while (normTime < 1.0f && GetGrappleDistance() < maxGrappleDistance)
        {
            //Move Grapple
            SetGrapplePosition(Vector3.Lerp(grappleStartPoint.position, target, grappleTravelCurve.Evaluate(normTime)));
            Vector3 shootingDir = (grappleEndPoint.position - grappleStartPoint.position).normalized;

            if (Physics.Raycast(grappleEndPoint.position, shootingDir, out RaycastHit hit, 0.1f, platfromLayer))
            {
                LatchGrapple(hit.point);
                yield break;
            }

            // Edge case, lets detach if the grapple will phase through a collider. 
            // This can happend when the player is shooting to a non-valid platform
            if (Physics.Raycast(grappleStartPoint.position,
                shootingDir,
                out RaycastHit _,
                GetGrappleDistance()))
            {
                //MAYBE: could do some fancy animation based on the raycast hit
                DetachGrapple();
                yield break;
            }


            normTime += Time.deltaTime / grappleTravelTime;
            yield return null;
        }

        DetachGrapple();

    }

    private void LatchGrapple(Vector3 point)
    {
        isSwinging = true;
        isLanded = false;

        SetGrapplePosition(point);

        // Joint Setup
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = point;

        float distance = GetGrappleDistance();
        //TODO: make these fields 
        joint.maxDistance = 0.6f * distance;
        joint.minDistance = 0.3f * distance;
        joint.spring = 8f;
        joint.damper = 7f;
        joint.massScale = 3f;
    }


    private void DetachGrapple()
    {
        isSwinging = false;
        isGrappling = false;
        Destroy(joint);

        StartCoroutine(RetractGrapple());
    }


    private IEnumerator RetractGrapple()
    {
        isRetracting = true;

        float normTime = 0;
        //Lets scale down the time based on the length of the grapple
        float totalTime = grappleTimeRetractCurve.Evaluate(GetGrappleDistance() / maxGrappleDistance) * grappleRetractTime;
        Vector3 startingPosition = grappleEndPoint.position;

        while (normTime < 1.0f)
        {
            //Move Grapple
            SetGrapplePosition(Vector3.Lerp(startingPosition, grappleStartPoint.position, grappleRetractCurve.Evaluate(normTime)));

            normTime += Time.deltaTime / totalTime;
            yield return null;
        }

        lineRenderer.enabled = false;
        isRetracting = false;

    }

    private void Swing(float horizontalInput)
    {
        playerRb.AddForce(horizontalInput * horizontalForce * Vector3.right, ForceMode.Force);
    }


    #endregion

    #region Utils

    private float GetGrappleDistance()
    {
        return Vector3.Distance(grappleStartPoint.position, grappleEndPoint.position);
    }

    private void SetGrapplePosition(Vector3 pos)
    {
        grappleEndPoint.position = pos;
        lineRenderer.SetPosition(1, grappleEndPoint.position);
    }
    #endregion
}
