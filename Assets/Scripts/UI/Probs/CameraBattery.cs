using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraBattery : MonoBehaviour
{
    [SerializeField] Image img_BatteryCharge;
    [SerializeField] Image img_BatteryPermanentDamage;
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text text;

    int remain, soul;
    int previouslySoul;

    IEnumerator Start()
    {
        yield return null;
        yield return null;

        canvas.enabled = false;
        text.enabled = false;
    }

    public void SetBattery(float charge, float damged)
    {
        if (GameManager.Instance.DontUseCamera)
        {
            canvas.enabled = false;
        }
        else
        {
            canvas.enabled = true; 
            
            if (charge > PlayerEvent.instance.cam_TakePicture * 1.0)
                img_BatteryCharge.color = Color.white;
            else
                img_BatteryCharge.color = Color.blue;

            img_BatteryCharge.fillAmount = charge * 0.01f;
            img_BatteryPermanentDamage.fillAmount = damged * 0.01f;

            //Debug.Log($"���͸��� �����մϴ�.{GameManager.Instance.GameStarted} {LibraryEvent.Activation}");

            //��Ʈ�� ���� �Ŀ� ���� �ܷ��� ������Ʈ�մϴ�
            if (GameManager.Instance.GameStarted)
            {
                text.enabled = true;

                //���͸��� �ܷ��� ���� ���� �Կ� Ƚ���� ���� ��ȥ�� ǥ�����ݴϴ�.
                float PermanentDamage = (DataSet.Instance.GameDifficulty.BatteryPermanentDamageRate * 15.0f);
                float RemainTakePictureBattery = 100.0f - damged;
                float RemainChance = (RemainTakePictureBattery - (15 - PermanentDamage + 1f)) / PermanentDamage;

                int _remain = Mathf.FloorToInt(RemainChance);

                //Debug.Log($"CameraBattery :: ���� ��ȸ  : {RemainChance} // ���� ���͸� : {RemainTakePictureBattery}");
                
                if (LibraryEvent.Activation) { soul = GameManager.Instance.GetRemainSouls();}
                else { soul = 0; }

                if (!_remain.Equals(remain) || soul.Equals(GameManager.Instance.GetRemainSouls()) || !soul.Equals(previouslySoul))
                {
                    remain = _remain;
                    previouslySoul = soul;

                    text.text = $"{remain} | {soul}";
                }
            }
            
        }
    }
}
