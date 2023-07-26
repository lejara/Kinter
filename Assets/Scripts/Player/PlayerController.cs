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
    public float grappleTravelTime;
    public float grappleDistance;
    public LayerMask platfromLayer;
    public AnimationCurve grappleTravelCurve;


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
        isLanded = true;
        isSwinging = false;
        Physics.gravity = new(Physics.gravity.x, gravity, Physics.gravity.z);

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.state != States.Playing)
        {
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        lineRenderer.SetPosition(0, grappleStartPoint.position);

        #region Grapple


        if (Input.GetKeyDown(KeyCode.Mouse0) && !isGrappling && !isSwinging)
        {
            StartCoroutine(ShootGrapple());
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && isSwinging)
        {
            DetachGrapple();
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

    private IEnumerator ShootGrapple()
    {

        isGrappling = true;
        lineRenderer.enabled = true;

        float normTime = 0;
        Vector3 target = grappleStartPoint.position + new Vector3(0, grappleDistance, 0);
        while (normTime < 1.0f &&
                Vector3.Distance(grappleStartPoint.position, grappleEndPoint.position) < grappleDistance)
        {
            //Move Grapple
            grappleEndPoint.position = Vector3.Lerp(grappleStartPoint.position, target, grappleTravelCurve.Evaluate(normTime));
            lineRenderer.SetPosition(1, grappleEndPoint.position);

            if (Physics.Raycast(grappleEndPoint.position, Vector3.up, out RaycastHit hit, 0.1f, platfromLayer))
            {
                LatchGrapple(hit.point);
                yield break;
            }

            // Edge case, let detach if the grapple will phase through a collider. 
            // This can happend when the player is shooting to a non-valid platfrom
            if (Physics.Raycast(grappleStartPoint.position,
                Vector3.up,
                out RaycastHit _,
                Vector3.Distance(grappleStartPoint.position, grappleEndPoint.position)))
            {
                //MAYBE: could do some fancy animation based on the raycast hit
                DetachGrapple();
                yield break;
            }


            normTime += Time.deltaTime / grappleTravelTime;
            yield return null; //allow for an Update
        }

        DetachGrapple();

    }

    private void LatchGrapple(Vector3 point)
    {
        isSwinging = true;
        isLanded = false;

        // Joint Setup
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = point;

        float distance = Vector3.Distance(grappleStartPoint.position, point);
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
        lineRenderer.enabled = false;
        grappleEndPoint.position = grappleStartPoint.position;
        lineRenderer.SetPosition(1, grappleEndPoint.position);

        Destroy(joint);
    }

    private void Swing(float horizontalInput)
    {
        playerRb.AddForce(horizontalInput * horizontalForce * Vector3.right, ForceMode.Force);
    }
}
