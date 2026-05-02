using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플랫에 따라 활성화와 비활성화를 구분합니다.
public class PlatformActivation : MonoBehaviour
{
    [SerializeField] bool IsPc;
    [SerializeField] Canvas m_Canvas;

    public void Awake()
    {
        //Debug.Log($"{IsPc} + {GameConfig.IsPc()}");
        gameObject.SetActive(!(IsPc ^ GameConfig.IsPc()));

        if (m_Canvas != null) m_Canvas.enabled = true;
    }
}
