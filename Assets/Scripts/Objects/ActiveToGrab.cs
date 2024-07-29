using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�׷��� �� ��� ������Ʈ�� Ư�� ������Ʈ�� Ȱ��ȭ ��Ų��.
public class ActiveToGrab : InteractionObjects
{
    // �׷� �� Ȱ��ȭ��ų ������Ʈ
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
