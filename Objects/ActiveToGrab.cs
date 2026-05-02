using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//그랩을 할 경우 오브젝트의 특정 오브젝트를 활성화 시킨다.
public class ActiveToGrab : InteractionObjects
{
    // 그랩 시 활성화시킬 오브젝트
    public GameObject[] go_willActive;

    [SerializeField]
    public ItemShowText itemShowText;

    protected override void Init()
    {
        base.Init();

        ChildsActive(false);
    }

    public override bool GrabOn(Transform parent)
    {
        itemShowText.ShowText();
        ChildsActive(true);
        return base.GrabOn(parent);
    }

    public override bool GrabOff(Vector3 position, Vector3 force)
    {
        ChildsActive(false);
        return base.GrabOff(position, force);
    }

    void ChildsActive(bool _bool)
    {
        if (go_willActive != null)
        {
            for (int i = 0; i < go_willActive.Length; i++) { go_willActive[i].SetActive(_bool); }
        }
    }
}
