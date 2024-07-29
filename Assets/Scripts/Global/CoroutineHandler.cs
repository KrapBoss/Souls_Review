using System;
using System.Collections;
using UnityEngine;

//�ڷ�ƾ�� ����� �� ���� ������ ����ؼ� ��������
public class CoroutineHandler : MonoBehaviour
{
    IEnumerator enumerator = null;

    private void Coroutine(IEnumerator coro)
    {
        //�� �̵� �ÿ��� �����ϱ� ����
        DontDestroyOnLoad(gameObject);

        enumerator = coro;

        StartCoroutine(coro);
    }

    void Update()
    {
        if (enumerator != null)
        {
            //yield return ���� -1�� ��� ����
            if(enumerator.Current!=null && enumerator.Current.Equals(-1))
            {
                Debug.LogWarning("�ڷ�ƾ �ڵ鷯�� �����մϴ�.");
                enumerator = null;
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("�ڷ�ƾ �ڵ鷯�� �����մϴ�.");
            Destroy(gameObject);
        }
    }

    public void Stop()
    {
        Debug.LogWarning("�ڷ�ƾ �ڵ鷯�� �����մϴ�.");
        StopCoroutine(enumerator);
        Destroy(gameObject);
    }

    //�ڷ�ƾ ������ ���� ������
    //�ݵ�� ���� �� -1�� ��ȯ�ϼ���.
    public static CoroutineHandler StartCorou(IEnumerator coro)
    {
        Debug.LogWarning("�ڷ�ƾ �ڵ鷯�� ����մϴ�.");
        
        GameObject obj = new GameObject("CoroutineHandler");
        CoroutineHandler handler = obj.AddComponent<CoroutineHandler>();

        if (handler)
        {
            handler.Coroutine(coro);
        }
        return handler;
    }

}
