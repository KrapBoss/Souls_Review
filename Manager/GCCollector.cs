using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCCollector : MonoBehaviour
{
    private int frameCount = 0;

    void Update()
    {
        frameCount++;
        if (frameCount >= 30)
        {
            long memory = GC.GetTotalMemory(false);
#if UNITY_EDITOR
            Debug.Log("GC Check (every 30 frames): " + memory + " bytes");
#endif
            frameCount = 0;
        }
    }
}