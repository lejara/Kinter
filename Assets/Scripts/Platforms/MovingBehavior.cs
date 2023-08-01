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
    float time;

    [Header("Reference")]

    [Tooltip("Object reference that we are going to move")]
    public GameObject movingObject;
    [SerializeField] Transform destinationLocation;
    [SerializeField] Transform startingLocation;

    // Start is called before the first frame update
    void Start()
    {
        time = timeToWait;
        shouldGoBack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove) 
        {
            Move();
        }
        else if (!shouldMove && Vector3.Distance(movingObject.transform.position, startingLocation.position) > 0)
        {
            // Cool down
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                ResetPosition();
            }
        }
    }

    #region Moving Methods
    private void Move()
    {
        if (time != timeToWait) time = timeToWait;
        // Move position
        Transform realDes = shouldGoBack ? startingLocation : destinationLocation;
        var actualSpeed = speed * Time.deltaTime;
        movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, realDes.position, actualSpeed);

        if (Vector3.Distance(movingObject.transform.position, destinationLocation.position) < 0.001f && !shouldGoBack)
        { 
            shouldGoBack = true; 
        }
        else if (Vector3.Distance(movingObject.transform.position, startingLocation.position) < 0.001f && shouldGoBack)
        {
            shouldGoBack = false;
        }
    }

    private void ResetPosition()
    {
        var actualSpeed = speed * Time.deltaTime;
        movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, startingLocation.position, actualSpeed);
    }

    #endregion

    #region Utils

    public void SetMovingStart(bool start)
    {
        shouldMove = start;
    }

    #endregion
}
