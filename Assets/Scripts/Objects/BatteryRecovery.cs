using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���͸��� �ջ󵵸� ȸ�����ݴϴ�.
public class BatteryRecovery : ExpandObject
{
    public override bool Func(string name = null)
    {
        PlayerEvent.instance.BatteryRecovery();
        //ȹ�� ����
        AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f, true);

        Destroy(gameObject);
        return true;
    }
}
