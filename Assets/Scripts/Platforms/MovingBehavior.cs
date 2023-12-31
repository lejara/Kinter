using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBehavior : MonoBehaviour
{
    [Header("Attribute")]

    public float speed;
    public bool shouldMove;
    [SerializeField] bool shouldGoBack;
    [Tooltip("A time we wait before starting to go back to staring point")]
    [SerializeField] float timeToWait;
    [Tooltip("A time we wait when it reaches the destination")]
    [SerializeField] float timeOnDestination;
    float timeWaitCoolDown;
    float timeDestCoolDown;

    [Header("Reference")]

    [Tooltip("Object reference that we are going to move")]
    public GameObject movingObject;
    [SerializeField] Transform destinationLocation;
    [SerializeField] Transform startingLocation;
    Rigidbody movingRb;

    // Start is called before the first frame update
    void Start()
    {
        timeWaitCoolDown = timeToWait;
        timeDestCoolDown = 0;
        shouldGoBack = false;
        movingRb = movingObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            if (timeDestCoolDown > 0)
            {
                timeDestCoolDown -= Time.deltaTime;
            }
            else
            {
                timeDestCoolDown = 0;
                Move();
            }
        }
        else if (!shouldMove && Vector3.Distance(movingObject.transform.position, startingLocation.position) > 0)
        {
            // Cool down
            timeDestCoolDown = timeDestCoolDown != 0 ? 0 : timeDestCoolDown;
            if (timeWaitCoolDown > 0)
            {
                timeWaitCoolDown -= Time.deltaTime;
            }
            else
            {
                ResetPosition();
            }
        }
        else
        {
            if (timeDestCoolDown != 0) timeDestCoolDown = 0;
            if (timeWaitCoolDown != timeToWait) timeWaitCoolDown = timeToWait;
            shouldGoBack = false;
        }
    }

    #region Moving Methods
    private void Move()
    {
        if (timeWaitCoolDown != timeToWait) timeWaitCoolDown = timeToWait;
        // Move position
        Transform realDes = shouldGoBack ? startingLocation : destinationLocation;
        var actualSpeed = speed * Time.deltaTime;
        movingRb.MovePosition(Vector3.MoveTowards(movingObject.transform.position, realDes.position, actualSpeed));

        if (Vector3.Distance(movingObject.transform.position, destinationLocation.position) < 0.001f && !shouldGoBack)
        {
            shouldGoBack = true;
            timeDestCoolDown = timeOnDestination;
        }
        else if (Vector3.Distance(movingObject.transform.position, startingLocation.position) < 0.001f && shouldGoBack)
        {
            shouldGoBack = false;
            timeDestCoolDown = timeOnDestination;
        }
    }

    private void ResetPosition()
    {
        var actualSpeed = speed * Time.deltaTime;
        movingRb.MovePosition(Vector3.MoveTowards(movingObject.transform.position, startingLocation.position, actualSpeed));
    }

    #endregion

    #region Utils

    public void SetMovingStart(bool start)
    {
        shouldMove = start;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startingLocation.position, destinationLocation.position);
    }

    #endregion
}
