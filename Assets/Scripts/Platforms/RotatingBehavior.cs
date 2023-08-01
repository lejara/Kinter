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
    float time;

    [Header("Reference")]

    [Tooltip("Object reference that we are going to rotate")]
    public GameObject rotatingObject;
    [SerializeField] Quaternion desiredAngle;
    [SerializeField] Quaternion startingAngle;

    void Start()
    {
        time = timeToWait;
        shouldTurnBack = false;
        startingAngle = rotatingObject.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRotate)
        {
            Rotate();
        }
        else if (!shouldRotate && rotatingObject.transform.rotation != startingAngle)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                ResetRotation();
            }
        }
    }

    private void Rotate()
    {
        if (time != timeToWait) time = timeToWait;
        Quaternion realAngle = shouldTurnBack ? startingAngle : desiredAngle;
        var actualSpeed = angleSpeed * Time.deltaTime;
        rotatingObject.transform.rotation = Quaternion.RotateTowards(rotatingObject.transform.rotation, realAngle, actualSpeed);

        if (rotatingObject.transform.rotation == desiredAngle && !shouldTurnBack)
        {
            shouldTurnBack = true;
        }
        else if (rotatingObject.transform.rotation == startingAngle && shouldTurnBack)
        {
            shouldTurnBack = false;
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
