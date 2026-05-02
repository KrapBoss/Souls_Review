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
    public float Timeout = 0.5f;        //반환시간

    //타임 아웃 시간
    [HideInInspector]public IEnumerator crou_timeout = null;
    public void SetPosition(Vector3 position)
    {
        TheGo.SetActive(true);
        TheGo.transform.position = position;

        crou_timeout = TimeoutCroutine();
    }

    IEnumerator TimeoutCroutine()//시간 종료 후 오브젝트 초기화를 위함.
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
        //Debug.Log("EffectManager 생성");
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


    //파티클 이름에 따라 파티클의 위치와 효과를 반영한다.
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
        Debug.LogWarning($"EffectManager :: 해당 파티클이 없습니다.");
        return false;
    }
}
