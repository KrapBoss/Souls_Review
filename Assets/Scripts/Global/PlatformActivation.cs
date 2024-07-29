using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��� ���� Ȱ��ȭ�� ��Ȱ��ȭ�� �����մϴ�.
public class PlatformActivation : MonoBehaviour
{
    [SerializeField] bool IsPc;

    public void Awake()
    {
        //Debug.Log($"{IsPc} + {GameConfig.IsPc()}");
        gameObject.SetActive(!(IsPc ^ GameConfig.IsPc()));
    }
}
