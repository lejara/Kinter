using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class BreakingBehavior : MonoBehaviour
{
    [Header("Attributes")]
    public bool startBreaking;
    [Tooltip("How long would this object break")]
    [SerializeField] float breakingTime;
    [SerializeField] float recoverTime;
    [SerializeField] float notificationTime;

    [Header("Reference")]
    [Tooltip("Object reference that we are going to break")]
    [SerializeField] GameObject breakingObject;

    // Update is called once per frame
    void Update()
    {
        if (startBreaking)
        {
            StartCoroutine(Breaking());
        }
        
    }

    public void SetBreakingStart(bool start)
    {
        startBreaking = start;
    }

    private IEnumerator Breaking()
    {
        float currTime = 0f;
        while (currTime < breakingTime)
        {
            if (!startBreaking)
            {
                yield break;
            }
            currTime += Time.deltaTime;
            yield return null;
        }

        GetComponent<PlatformsBehavior>().SetValid(false);
        breakingObject.SetActive(false);
        
        yield return new WaitForSeconds(recoverTime);
        
        startBreaking = false;
        GetComponent<PlatformsBehavior>().SetValid(true);
        breakingObject.SetActive(true);
    }
}
