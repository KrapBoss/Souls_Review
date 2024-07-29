using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//토템 오브젝트
//활성화되면 이펙트를 발생시킨다.
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

            //파티클 효과
            go_fx.SetActive(true);

            //촛불 효과
            go_fxCandle.SetActive(true);

            //사운드 효과
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
