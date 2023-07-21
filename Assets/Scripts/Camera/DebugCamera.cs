using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    public float speed;
    public float boostSpeed;

    void Update()
    {
        transform.Translate(new Vector3(getSpeed(Input.GetAxis("Horizontal")), getSpeed(Input.GetAxis("Vertical"))));
    }

    float getSpeed(float axis)
    {
        //boosting
        if (Input.GetKey(KeyCode.Space))
        {
            return axis * boostSpeed;
        }
        return axis * speed;
    }
}
