using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ð��� ���� Ȱ��ȭ�� �����մϴ�.
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
