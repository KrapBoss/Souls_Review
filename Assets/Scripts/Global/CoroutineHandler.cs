using System;
using System.Collections;
using UnityEngine;

//코루틴을 사용할 수 없는 곳에서 대신해서 실행해줌
public class CoroutineHandler : MonoBehaviour
{
    IEnumerator enumerator = null;

    private void Coroutine(IEnumerator coro)
    {
        //씬 이동 시에도 적용하기 위함
        DontDestroyOnLoad(gameObject);

        enumerator = coro;

        StartCoroutine(coro);
    }

    void Update()
    {
        if (enumerator != null)
        {
            //yield return 값이 -1일 경우 제거
            if(enumerator.Current!=null && enumerator.Current.Equals(-1))
            {
                Debug.LogWarning("코루틴 핸들러를 제거합니다.");
                enumerator = null;
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("코루틴 핸들러를 제거합니다.");
            Destroy(gameObject);
        }
    }

    public void Stop()
    {
        Debug.LogWarning("코루틴 핸들러를 제거합니다.");
        StopCoroutine(enumerator);
        Destroy(gameObject);
    }

    //코루틴 실행을 위한 대행자
    //반드시 종료 시 -1을 반환하세요.
    public static CoroutineHandler StartCorou(IEnumerator coro)
    {
        Debug.LogWarning("코루틴 핸들러를 사용합니다.");
        
        GameObject obj = new GameObject("CoroutineHandler");
        CoroutineHandler handler = obj.AddComponent<CoroutineHandler>();

        if (handler)
        {
            handler.Coroutine(coro);
        }
        return handler;
    }

}
