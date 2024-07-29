using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� ������Ʈ
//Ȱ��ȭ�Ǹ� ����Ʈ�� �߻���Ų��.
public class Totem : ExpandObject
{
    public GameObject go_fxCandle;
    public GameObject go_fx;

    LibraryEvent events;

    protected override void Init()
    {
        base.Init();
        events= transform.GetComponentInParent<LibraryEvent>();
        go_fxCandle.SetActive(false);
        go_fx.SetActive(false);
    }

    public override bool Func(string name = null)
    {
        if (name.Equals(TargetObjectName) && events.isActivation)
        {
            events.ActiveTotem();

            //��ƼŬ ȿ��
            go_fx.SetActive(true);

            //�к� ȿ��
            go_fxCandle.SetActive(true);

            //���� ȿ��
            AudioSource source =   GetComponent<AudioSource>();
            source.volume = DataSet.Instance.SettingValue.Volume;
            source.Play();

            gameObject.layer = DataSet.Instance.Layers.Default;

            return true;
        }
        else return false;

    }
    public void LightOFF()
    {
        go_fx.SetActive(false);
        go_fxCandle.SetActive(false);
        gameObject.layer = DataSet.Instance.Layers.Object;
    }
}
