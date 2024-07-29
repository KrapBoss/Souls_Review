using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 사용 F
/// 획득 시 : 카메라 또는 플레쉬 라이트 기능 획득
/// 
/// </summary>
public class GetItemUse : ExpandObject
{
    public bool mCamera;//카메라 활성화인가?
    public bool mFlash;//플래쉬 라이트 활성화인가?
    public bool mMeter;
    public bool mCrystal;

    public bool showTip;

    public override bool Func(string name = null)
    {
        if (_progressObject)
        {
            if (_progressObject.ActionObject())//동작 가능한 상태를 나타낸다.
            {
                ItemActive();

                if (mCamera) { GameManager.Instance.DontUseCamera = false; }
                else if (mFlash) { GameManager.Instance.DontUseFlashLight = false; }
                else if (mMeter) { GameManager.Instance.DontUseGhostMeter = false; }
                else if (mCrystal) { GameManager.Instance.DontUseGhostBall = false; }

                //획득 사운드
                AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f, true);

                Hide();

                return true;
            }
        }
        else
        {
            ItemActive();

            Hide();
            //획득 사운드
            AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f, true);
            //띠리링
            return true;
        }

        return false;
    }

    void Hide()
    {
        gameObject.layer = 0;
        gameObject.SetActive(false);
    }

    //아이템을 사용가능하도록 활성화해줍니다.
    void ItemActive()
    {
        string txt = "";

        if (GameConfig.IsPc())
        {
            if (mCamera) { GameManager.Instance.DontUseCamera = false; txt = " [ V ]"; }
            else if (mFlash) { GameManager.Instance.DontUseFlashLight = false;  PlayerEvent.instance.FlashLightEquip(true); }
            else if (mMeter) { GameManager.Instance.DontUseGhostMeter = false; txt = " [ 1 ]"; }
            else if (mCrystal) { GameManager.Instance.DontUseGhostBall = false; txt = " [ 2 ]"; }
        }
        //모바일
        else
        {
            if (mCamera) { GameManager.Instance.DontUseCamera = false; txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "V"); UI.mobileControllerUI.ActiveIcon(0,true); }
            else if (mFlash) { GameManager.Instance.DontUseFlashLight = false; ; PlayerEvent.instance.FlashLightEquip(true); }
            else if (mMeter) { GameManager.Instance.DontUseGhostMeter = false; txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "1");  UI.mobileControllerUI.ActiveIcon(1, true); }
            else if (mCrystal) { GameManager.Instance.DontUseGhostBall = false; txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "2"); UI.mobileControllerUI.ActiveIcon(2, true); }
        }

        if (!txt.Equals("") && !SafeBox.Activation)
        {
            txt = txt + LocalLanguageSetting.Instance.GetLocalText("Tip", "FuncKey");
            UI.topUI.ShowNotice(txt, false);
        }
    }
}
