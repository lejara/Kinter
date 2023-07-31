using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBehavior : MonoBehaviour
{
    [Header("Attribute")]
    
    public float speed;
    public bool shouldMove;
    [SerializeField] bool shouldGoBack;

    [Header("Reference")]
    [Tooltip("Object reference that we are going to move")]
    public GameObject movingObject;
    [SerializeField] Transform destinationLocation;
    [SerializeField] Transform startingLocation;

    // Start is called before the first frame update
    void Start()
    {
        shouldGoBack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldMove) Move();
    }

    private void Move()
    {
        // Move position
        Transform realDes = shouldGoBack ? startingLocation : destinationLocation;
        var actualSpeed = speed * Time.deltaTime;
        movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, realDes.position, actualSpeed);

        if (Vector3.Distance(movingObject.transform.position, destinationLocation.position) < 0.001f)
        { 
            shouldGoBack = true; 
        }
        else if (Vector3.Distance(movingObject.transform.position, startingLocation.position) < 0.001f)
        {
            shouldGoBack = false;
        }

    }

    private IEnumerator ResetPosition()
    {
        yield return null;
    }

    public void SetMovingStart(bool start)
    {
        shouldMove = start;
    }
}
