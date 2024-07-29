using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    GhostBlack cs_GhostBlack = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GhostBlack"))
        {
            //Debug.Log("������ Enter");

            if(cs_GhostBlack ==null)
                cs_GhostBlack = other.GetComponent<GhostBlack>();

            cs_GhostBlack.HitTheLight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GhostBlack"))
        {
            Debug.Log("������ Exit");

            cs_GhostBlack.EscapeTheLight();
        }
    }

    private void OnDisable()
    {
        //Debug.Log("�÷��� ����Ʈ ��Ȱ��ȭ");
        if(cs_GhostBlack != null)
            cs_GhostBlack.EscapeTheLight();
    }
}
