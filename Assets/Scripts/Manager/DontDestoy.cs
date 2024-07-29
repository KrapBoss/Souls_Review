using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoyGameObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
