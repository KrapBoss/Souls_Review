using CustomUI;
using UnityEngine;

public class ShowTimer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCollision"))
        {
            float clearTime = PlayerEvent.instance.Timer;
            float hour = clearTime / 3600;
            float minute = clearTime % 3600 / 60;
            float second = clearTime % 3600 % 60;
            //클리어 시간 지정
            string diff = (DataSet.Instance.SettingValue.CurrentDiff == 0) ? "[Easy]" : (DataSet.Instance.SettingValue.CurrentDiff == 1) ? "[Normal]" : "[Hard]";

            string time= $"{diff}" +
                $"{string.Format("{0:00}", hour)} : {string.Format("{0:00}", minute)} : {string.Format("{0:00.#}", second)}";

            UI.topUI.ShowNotice(time,true);

            AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f, true);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
