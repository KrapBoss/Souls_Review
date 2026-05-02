using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//배터리의 손상도를 회복해줍니다.
public class BatteryRecovery : ExpandObject
{
    public override bool Func(string name = null)
    {
        // 사용이 불가한 경우
        if (!PlayerEvent.instance.BatteryRecovery()){
            UI.activityUI.SetItemText(LocalLanguageSetting.Instance.GetLocalText(LocalizationTable.ItemTable, $"{NAME}_cantRecover"));
            return false;
        }
        
        //획득 사운드
        AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f, true);

        //획득한 아이템을 보여줍니다.
        ShowText();

        Destroy(gameObject);
        return true;
    }
}
