using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ���� �� �� �ִ�. ���� ���谡 �ʿ��ϴٸ�, Ÿ�� ������Ʈ�� �����Ѵ�.
/// </summary>
public class Drawer : ExpandObject
{
    [Space]
    [Header("������ ��ȣ�ۿ� ��ҵ�")]
    public bool isSliderDrawer; //���� �����̴��� ���� �����δٸ� true // ���� �Ϲ�������ó�� �����ٸ� false
    public float targetValue;    // �Ϲݹ��� Rotation y�� ȸ�� ��  //�����̴� �����̶�� ������ ������ ��
    Vector3 init_trasform;     //�ʱ� �Է� ���� ��Ÿ����.
    Vector3 from;
    Vector3 to;

    bool isActivating;      // ���� ������ �ִ� ���̴�.
    bool isOpened;          //���� ���ȴ�.
    float t_value;          //���� ����
    protected override void Init()
    {
        base.Init();
        TargetObjectName = "Self";  //������Ʈ�� ������ ��ȣ�ۿ����� ��Ÿ��.
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

        if (isOpened)   //���� �����ִ� ����
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

        if (isOpened)   //���� �����ִ� ����
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
