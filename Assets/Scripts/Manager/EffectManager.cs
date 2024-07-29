using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
/// <summary>
/// return the consistency object through a method
/// </summary>
/// 

[System.Serializable]class ParticleObject
{
    public string Name;
    public GameObject TheGo;
    public float Timeout = 0.5f;        //��ȯ�ð�

    //Ÿ�� �ƿ� �ð�
    [HideInInspector]public IEnumerator crou_timeout = null;
    public void SetPosition(Vector3 position)
    {
        TheGo.SetActive(true);
        TheGo.transform.position = position;

        crou_timeout = TimeoutCroutine();
    }

    IEnumerator TimeoutCroutine()//�ð� ���� �� ������Ʈ �ʱ�ȭ�� ����.
    {
        yield return new WaitForSeconds(Timeout);
        TheGo.transform.position = Vector3.zero;
        TheGo.SetActive(false);
        crou_timeout = null;
    }
}

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField] private ParticleObject[] particleObj;

    private void Awake()
    {
        //Debug.Log("EffectManager ����");
        if (instance == null) instance = this;

        for (int i = 0; i < particleObj.Length; i++)
        {
            particleObj[i].TheGo.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        Destroy(instance);
        instance = null;
    }


    //��ƼŬ �̸��� ���� ��ƼŬ�� ��ġ�� ȿ���� �ݿ��Ѵ�.
    public bool ActiveParticle(string name, Vector3 position)
    {
        for (int i = 0; i < particleObj.Length; i++)
        {
            if (particleObj[i].Name.Equals(name))
            {
                if (particleObj[i].crou_timeout != null) return false;
                particleObj[i].SetPosition(position);
                StartCoroutine(particleObj[i].crou_timeout);
                return true;
            }
        }
        Debug.LogWarning($"EffectManager :: �ش� ��ƼŬ�� �����ϴ�.");
        return false;
    }
}
