using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // 정적 멤버로 인스턴스를 갖고 있는다.    
    private static T _instance;    // 전역적인 접근점을 제공한다.   
    public static T Instance { get { return _instance; } }

    protected void Awake()
    {
        if(_instance == null)
        {
            _instance = GetComponent<T>();
        }
        else
        {
            if (_instance == this)
            {

            }
        }
    }
}


public class SingletonIterator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
