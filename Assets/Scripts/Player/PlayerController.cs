using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]

    [ReadOnly] public bool isLanded;
    [ReadOnly] public bool isStunned;
    public float gravity;
    public float sidewayMoveSpeed;

    [Tooltip("How big the angle should be on its sides to count as a stun")]
    [Range(0f, 180f)] public float maxStunAngle;

    [Tooltip("Minimum velocity magnitude needed to apply a stun")]
    public float minVelocityStunThreshold;

    [Tooltip("WIll multiply on top of existing velocity when the player makes a stun contact")]
    public float stunBounceMultiplier;

    [Header("Grapple Attributes")]

    [ReadOnly] public bool isGrappling;
    [ReadOnly] public bool isSwinging;
    [ReadOnly] public bool isRetracting;
    public float grappleTravelTime;
    public float grappleRetractTime;
    public float maxGrappleDistance;
    public LayerMask platfromLayer;
    public AnimationCurve grappleTravelMotion;
    public AnimationCurve grappleRetractMotion;
    [Tooltip("Decay scale for how much time to take off based on grapple length")]
    public AnimationCurve grappleRetractTimeDecay;


    [Header("Swing Attributes")]

    public float horizontalForce;
    [Tooltip("When latching, how much of its existing velocity it should lose")]
    [Range(0f, 1f)] public float latchVelocityFalloff;


    [Header("Spring Joint Settings")]

    public float jointMaxDistance;
    public float jontMinDistance;
    public float spring;
    public float damper;
    public float massScale;


    [Header("References")]

    public Transform grappleStartPoint;
    public Transform grappleEndPoint;
    public Rigidbody playerRb;

    [SerializeField] GameObject grappleTarget;
    [SerializeField] GameState gameState;
    [SerializeField] DebugSettings debugSettings;

    //Events
    public Action OnGrappleShoot;
    public Action OnGrappleLatch;
    public Action<float> WhileSwinging;
    public Action OnGrappleDetach;
    public Action OnCannotShootGrapple;

    public Action OnLanded;
    public Action OnAir;
    public Action<float> WhileInAir;
    public Action<float> WhileOnLand;

    public Action<Vector3> OnStun;
    public Action OnStunExit;

    [HideInInspector] public Vector3 lastVelocity;

    float horizontalInput;
    SpringJoint joint;

    LineRenderer lineRenderer;

    public void Reset()
    {
        StunnedOff();
        playerRb.velocity = Vector3.zero;
        if (isSwinging || isGrappling)
        {
            DetachGrapple();
        }
    }

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
        //Make sure we keep the lineRenderer start point up to date
        lineRenderer.SetPosition(0, grappleStartPoint.position);

        //It is assumed grappleEndPoint is not a transfrom child of this object.
        //We need to keep it synced when its not in use
        if (!isSwinging && !isGrappling && !isRetracting)
        {
            SetGrapplePosition(grappleStartPoint.position);
        }

        #region Input

        //Only allow input if we are playing and not in debug
        if (gameState.state != States.Playing || debugSettings.godMode)
        {
            horizontalInput = 0;
            return;
        }


        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isGrappling && !isSwinging && !isRetracting && !isStunned)
        {
            StartCoroutine(ShootGrapple());
        }
        else if ((!Input.GetKey(KeyCode.Mouse0) ||
                (grappleTarget && !grappleTarget.GetComponentInParent<PlatformsBehavior>().isValid))
                && isSwinging)
        {
            DetachGrapple();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnCannotShootGrapple?.Invoke();
        }

        #endregion
    }

    void FixedUpdate()
    {
        lastVelocity = playerRb.velocity;

        #region On Land or Air Logic
        bool _lastIsLanded = isLanded;

        isLanded = GroundCheck() && !isSwinging;
        if (_lastIsLanded != isLanded)
        {
            if (isLanded)
            {
                OnLanded?.Invoke();
            }
            else
            {
                OnAir?.Invoke();
            }
        }

        #endregion

        #region While On Land Or In Air Logic
        if (isLanded)
        {
            StunnedOff();
            SidewayMoving(horizontalInput);
            WhileOnLand?.Invoke(horizontalInput);
        }
        else
        {
            WhileInAir?.Invoke(horizontalInput);
        }
        #endregion

        #region Swing Logic
        if (isSwinging)
        {
            if (grappleTarget)
            {
                SetGrapplePosition(grappleTarget.transform.TransformPoint(joint.connectedAnchor));
            }
            Swing(horizontalInput);
        }
        #endregion

    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isLanded)
        {
            Stun(collision);
        }
    }

    #region Move Methods

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

    #endregion

    #region Stun Method

    void Stun(Collision collision)
    {

        //We only apply a stun if the player is moving a significant ammount
        if (lastVelocity.magnitude < minVelocityStunThreshold)
        {
            return;
        }

        //Check angle, we only want to stun when it makes contact on its sides.
        //Note: sides are in world space
        float desiredSideDotProduct = Mathf.Cos(maxStunAngle * Mathf.Deg2Rad);
        int index = collision.contacts.FirstIndex((point) =>
            (Vector3.Dot(Vector3.right, point.normal) <= desiredSideDotProduct ||
            Vector3.Dot(Vector3.left, point.normal) <= desiredSideDotProduct));

        if (index == -1)
        {
            return;
        }

        //Make the player bounce off on what they made contact with
        playerRb.velocity = lastVelocity.magnitude * stunBounceMultiplier * collision.GetContact(index).normal;
        if (isSwinging || isGrappling)
        {
            DetachGrapple();
        }
        isStunned = true;
        OnStun?.Invoke(playerRb.velocity);

    }

    void StunnedOff()
    {
        if (isStunned)
        {
            isStunned = false;
            OnStunExit?.Invoke();
        }
    }

    #endregion

    #region Grapple Methods

    private IEnumerator ShootGrapple()
    {

        isGrappling = true;
        lineRenderer.enabled = true;
        OnGrappleShoot?.Invoke();

        float normTime = 0;
        Vector3 target = grappleStartPoint.position + (Vector3.up * maxGrappleDistance);
        while (normTime < 1.0f && GetGrappleDistance() < maxGrappleDistance)
        {
            // Edge case, stop shooting the grapple if we got stunned
            if (isStunned)
            {
                //We wont detach here. we let the stun method handle it
                yield break;
            }

            //Move Grapple
            SetGrapplePosition(Vector3.Lerp(grappleStartPoint.position, target, grappleTravelMotion.Evaluate(normTime)));
            Vector3 shootingDir = (grappleEndPoint.position - grappleStartPoint.position).normalized;

            if (Physics.Raycast(grappleEndPoint.position, shootingDir, out RaycastHit hit, 0.12f, platfromLayer))
            {
                if (!hit.transform.gameObject.GetComponentInParent<PlatformsBehavior>().isValid)
                {
                    DetachGrapple();
                }
                else
                {
                    LatchGrapple(hit);
                }
                yield break;
            }

            // Edge case, detach if the grapple will phase through a collider. 
            // This can happend when the player is shooting to a non-valid platform
            if (Physics.Raycast(grappleStartPoint.position,
                shootingDir,
                out RaycastHit _,
                GetGrappleDistance() - 1f))
            {
                DetachGrapple();
                yield break;
            }

            normTime += Time.deltaTime / grappleTravelTime;
            yield return null;
        }

        DetachGrapple();

    }

    private void LatchGrapple(RaycastHit hit)
    {
        isSwinging = true;
        isLanded = false;
        playerRb.velocity *= (1 - latchVelocityFalloff);

        SetGrapplePosition(hit.point);

        // Joint Setup
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        if (hit.transform.gameObject.GetComponentInParent<PlatformsBehavior>().type == PlatformType.Normal)
        {
            joint.connectedAnchor = hit.point;
        }
        else
        {
            joint.connectedBody = hit.rigidbody;
            joint.connectedAnchor = hit.transform.InverseTransformPoint(hit.point);
            joint.enableCollision = true;

            // Grappled Object Setup
            grappleTarget = joint.connectedBody.transform.gameObject;
            grappleTarget.GetComponentInParent<PlatformsBehavior>().isLatched = true;
        }

        float distance = GetGrappleDistance();

        joint.maxDistance = jointMaxDistance * distance;
        joint.minDistance = jontMinDistance * distance;
        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        OnGrappleLatch?.Invoke();
    }

    private void DetachGrapple()
    {
        isSwinging = false;
        isGrappling = false;

        if (joint)
        {
            if (grappleTarget)
            {
                // We use grappleTarget here as destroyed platform would be deactivated and can't use 
                // joint.connectBody which is the rigidbody of the platform.
                grappleTarget.GetComponentInParent<PlatformsBehavior>().isLatched = false;
                grappleTarget = null;
            }
            Destroy(joint);
        }

        OnGrappleDetach?.Invoke();
        StartCoroutine(RetractGrapple());
    }

    private IEnumerator RetractGrapple()
    {
        isRetracting = true;

        float normTime = 0;
        //Scale down the time based on the length of the grapple
        float totalTime = grappleRetractTimeDecay.Evaluate(GetGrappleDistance() / maxGrappleDistance) * grappleRetractTime;
        Vector3 startingPosition = grappleEndPoint.position;

        while (normTime < 1.0f)
        {
            //Move Grapple
            SetGrapplePosition(Vector3.Lerp(startingPosition, grappleStartPoint.position, grappleRetractMotion.Evaluate(normTime)));

            normTime += Time.deltaTime / totalTime;
            yield return null;
        }

        lineRenderer.enabled = false;
        isRetracting = false;

    }

    private void Swing(float horizontalInput)
    {
        playerRb.AddForce(horizontalInput * horizontalForce * Vector3.right, ForceMode.Force);
        WhileSwinging?.Invoke(horizontalInput);
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
