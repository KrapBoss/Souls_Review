using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플랫에 따라 활성화와 비활성화를 구분합니다.
public class PlatformActivation : MonoBehaviour
{
    [SerializeField] bool IsPc;

    public void Awake()
    {
        //Debug.Log($"{IsPc} + {GameConfig.IsPc()}");
        gameObject.SetActive(!(IsPc ^ GameConfig.IsPc()));
    }
}
