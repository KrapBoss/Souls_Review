using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ �տ��� Wheel�� ������ �˷��ش�.
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
