using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현관문 앞에서 Wheel의 사용법을 알려준다.
public class Step4 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetIntroStep(5);
        }
    }
}
