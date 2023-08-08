using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
#if (UNITY_WEBGL)
        Application.OpenURL("https://leption.itch.io/iron-jungle");
#else
        Application.Quit();
#endif
    }
}
