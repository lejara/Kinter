using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotatingBehavior : MonoBehaviour
{
    [Header("Attribute")]

    public float angleSpeed;
    public bool shouldRotate;
    [SerializeField] bool shouldTurnBack;
    [SerializeField] float timeToWait;
    [SerializeField] float timeOnRotation;
    float timeWaitCoolDown;
    float timeRotaCoolDown;

    [Header("Reference")]

    [Tooltip("Object reference that we are going to rotate")]
    public GameObject rotatingObject;
    [SerializeField] Quaternion desiredAngle;
    [SerializeField] Quaternion startingAngle;

    void Start()
    {
        timeWaitCoolDown = timeToWait;
        timeRotaCoolDown = 0;
        shouldTurnBack = false;
        startingAngle = rotatingObject.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRotate)
        {
            if (timeRotaCoolDown > 0)
            {
                timeRotaCoolDown -= Time.deltaTime;
            }
            else
            {
                timeRotaCoolDown = 0;
                Rotate();
            }
        }
        else if (!shouldRotate && rotatingObject.transform.rotation != startingAngle)
        {
            timeRotaCoolDown = timeRotaCoolDown != 0 ? 0 : timeRotaCoolDown; 
            if (timeWaitCoolDown > 0)
            {
                timeWaitCoolDown -= Time.deltaTime;
            }
            else
            {
                ResetRotation();
            }
        }
        else
        {
            if (timeRotaCoolDown != 0) timeRotaCoolDown = 0;
            if (timeWaitCoolDown != timeToWait) timeWaitCoolDown = timeToWait;
            shouldTurnBack = false;
        }
    }

    private void Rotate()
    {
        if (timeWaitCoolDown != timeToWait) timeWaitCoolDown = timeToWait;
        Quaternion realAngle = shouldTurnBack ? startingAngle : desiredAngle;
        var actualSpeed = angleSpeed * Time.deltaTime;
        rotatingObject.transform.rotation = Quaternion.RotateTowards(rotatingObject.transform.rotation, realAngle, actualSpeed);

        if (rotatingObject.transform.rotation == desiredAngle && !shouldTurnBack)
        {
            shouldTurnBack = true;
            timeRotaCoolDown = timeOnRotation;
        }
        else if (rotatingObject.transform.rotation == startingAngle && shouldTurnBack)
        {
            shouldTurnBack = false;
            timeRotaCoolDown = timeOnRotation;
        }
    }

    private void ResetRotation()
    {
        var actualSpeed = angleSpeed * Time.deltaTime;
        rotatingObject.transform.rotation = Quaternion.RotateTowards(rotatingObject.transform.rotation, startingAngle, actualSpeed);
    }

    public void SetRotatingStart(bool start)
    {
        shouldRotate = start;
    }
}
