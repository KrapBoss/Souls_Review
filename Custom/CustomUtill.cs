using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 원하는 기능 정의
/// </summary>
public class CustomUtill : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public static class _debug
{
    public static void Log(string content)
    {
#if UNITY_EDITOR
        Debug.Log(content);
#endif
    }
    public static void LogWarning(string content)
    {
#if UNITY_EDITOR
        Debug.LogWarning(content);
#endif
    }
    public static void LogError(string content)
    {
#if UNITY_EDITOR
        Debug.LogError(content);
#endif
    }
}