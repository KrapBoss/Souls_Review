using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 문을 열 수 있다. 만약 열쇠가 필요하다면, 타겟 오브젝트가 존재한다.
/// </summary>
public class Drawer : ExpandObject
{
    [Space]
    [Header("서랍장 상호작용 요소들")]
    public bool isSliderDrawer; //문이 슬라이더를 따라 움직인다면 true // 문이 일반현관문처럼 열린다면 false
    public float targetValue;    // 일반문을 Rotation y축 회전 값  //슬라이더 형식이라면 앞으로 나오는 값
    Vector3 init_trasform;     //초기 입력 값을 나타낸다.
    Vector3 from;
    Vector3 to;

    bool isActivating;      // 문이 열리고 있는 중이다.
    bool isOpened;          //문이 열렸다.
    float t_value;          //보간 비율
    protected override void Init()
    {
        base.Init();
        TargetObjectName = "Self";  //오브젝트가 스스로 상호작용함을 나타냄.
        init_trasform = isSliderDrawer ?  transform.localPosition : transform.localRotation.eulerAngles;

    }

    public override bool Func()
    {
        return Func("");
    }

    public override bool Func(string name = null)
    {
        if(isActivating) { return false; }

        if (isSliderDrawer)
        {
            OpenSliderDrawer();
        }
        else
        {
            OpenRotationDrawer();
        }
        return true;
    }

    public void OpenSliderDrawer()
    {
        isActivating = true;
        t_value = 0;

        if (isOpened)   //문이 열려있는 상태
        {
            from = init_trasform + transform.forward * targetValue;
            to = init_trasform;
        }
        else
        {
            from = init_trasform;
            to = init_trasform + transform.forward * targetValue;
        }

        StartCoroutine(OpenSliderDrawerCroutine());
    }


    IEnumerator OpenSliderDrawerCroutine()
    {
        if(isOpened) AudioManager.instance.PlayEffectiveSound("DrawerClose", 0.7f, true);
        else AudioManager.instance.PlayEffectiveSound("DrawerOpen", 0.7f,true);
        while(t_value <= 1.0f)
        {
            transform.localPosition = Vector3.Lerp(from, to, t_value);
            t_value += Time.deltaTime * 3;

            yield return null;
        }

        isOpened = !isOpened;
        isActivating = false;
    }


    public void OpenRotationDrawer()
    {
        isActivating = true;
        t_value = 0;

        if (isOpened)   //문이 열려있는 상태
        {
            from = init_trasform + new Vector3(0,targetValue,0);
            to = init_trasform;
        }
        else
        {
            from = init_trasform;
            to = init_trasform + new Vector3(0, targetValue, 0);
        }
        StartCoroutine(OpenRotationDrawerCroutine());
    }

    IEnumerator OpenRotationDrawerCroutine()
    {
        string _sound;
        if (isOpened) _sound = "NonDrawerClose";
        else _sound = "NonDrawerOpen";
        AudioManager.instance.PlayEffectiveSound(_sound, 0.7f, true);

        while (t_value <= 1.0f)
        {
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(from, to, t_value));
            t_value += Time.deltaTime * 3;

            yield return null;
        }

        isOpened = !isOpened;
        isActivating = false;
    }
}
