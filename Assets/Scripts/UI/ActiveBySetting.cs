using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 셋팅값에 따른 활성화를 결정합니다.
/// </summary>
public class ActiveBySetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (DataSet.Instance.SettingValue.IntroClear)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
