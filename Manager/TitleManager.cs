using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        if (((Screen.width / Screen.height) > 1.8f) && ((Screen.width / Screen.height) < 1.72f))
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }
    }
}
