using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//배터리의 손상도를 회복해줍니다.
public class BatteryRecovery : ExpandObject
{
    public override bool Func(string name = null)
    {
        PlayerEvent.instance.BatteryRecovery();
        //획득 사운드
        AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f, true);

        Destroy(gameObject);
        return true;
    }
}
