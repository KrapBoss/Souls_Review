using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// 모바일에서 마지막 찬스를 광고를 본 후 얻을 수 있다.
/// </summary>
public class LastChanceAds_Mobile : MonoBehaviour
{
    [SerializeField] Button btn_Yes;
    [SerializeField] Button btn_No;

    public void Show(UnityAction yes, UnityAction no)
    {
        btn_Yes.onClick.RemoveAllListeners(); 
        btn_No.onClick.RemoveAllListeners();

        btn_Yes.onClick.AddListener(yes);
        btn_No.onClick.AddListener(no);
    }
}
