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
            //Debug.Log("검정이 Enter");

            if(cs_GhostBlack ==null)
                cs_GhostBlack = other.GetComponent<GhostBlack>();

            cs_GhostBlack.HitTheLight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GhostBlack"))
        {
            Debug.Log("검정이 Exit");

            cs_GhostBlack.EscapeTheLight();
        }
    }

    private void OnDisable()
    {
        //Debug.Log("플래쉬 라이트 비활성화");
        if(cs_GhostBlack != null)
            cs_GhostBlack.EscapeTheLight();
    }
}
