using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��� F
/// ȹ�� �� : ī�޶� �Ǵ� �÷��� ����Ʈ ��� ȹ��
/// 
/// </summary>
public class GetItemUse : ExpandObject
{
    public bool mCamera;//ī�޶� Ȱ��ȭ�ΰ�?
    public bool mFlash;//�÷��� ����Ʈ Ȱ��ȭ�ΰ�?
    public bool mMeter;
    public bool mCrystal;

    public bool showTip;

    public override bool Func(string name = null)
    {
        if (_progressObject)
        {
            if (_progressObject.ActionObject())//���� ������ ���¸� ��Ÿ����.
            {
                ItemActive();

                if (mCamera) { GameManager.Instance.DontUseCamera = false; }
                else if (mFlash) { GameManager.Instance.DontUseFlashLight = false; }
                else if (mMeter) { GameManager.Instance.DontUseGhostMeter = false; }
                else if (mCrystal) { GameManager.Instance.DontUseGhostBall = false; }

                //ȹ�� ����
                AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f, true);

                Hide();

                return true;
            }
        }
        else
        {
            ItemActive();

            Hide();
            //ȹ�� ����
            AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f, true);
            //�츮��
            return true;
        }

        return false;
    }

    void Hide()
    {
        gameObject.layer = 0;
        gameObject.SetActive(false);
    }

    //�������� ��밡���ϵ��� Ȱ��ȭ���ݴϴ�.
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
        //�����
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
