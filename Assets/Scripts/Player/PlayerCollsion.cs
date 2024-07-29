using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollsion : MonoBehaviour
{
    //õ�忡 �ε����� ������ ��Ÿ����.
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
