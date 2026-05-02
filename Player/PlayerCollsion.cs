using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollsion : MonoBehaviour
{
    //천장에 부딪히고 있음을 나타낸다.
    public bool CeilingCollsion = false;

    public string tag_ceil = "Ceiling";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tag_ceil))
        {
            CeilingCollsion = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tag_ceil))
        {
            CeilingCollsion = false;
        }
    }
}
