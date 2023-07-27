using System.Collections;
using UnityEngine;

public class PlatformsBehavior : MonoBehaviour
{
    public enum PlatformType
    {
        Normal,
        Notgrappable,
        Breakable,
        Uneven,
        Moving,
        Rotating
    }
    public PlatformType type;

    [Tooltip("Indicate whether this platform should activate its own script at the start or after being latched")]
    public bool shouldActive;
    public bool isLatched;

    void Update()
    {
        if (!shouldActive && isLatched) TypeHandler();
    }

    private void TypeHandler() 
    { 
        switch(type)
        {
            case PlatformType.Breakable:
                break;
            case PlatformType.Moving:
                break;
            case PlatformType.Rotating: 
                break;
            default:
                break;
        }
    }
}
