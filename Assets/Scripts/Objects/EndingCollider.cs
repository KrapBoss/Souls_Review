using CustomUI;
using UnityEngine;

public class EndingCollider : MonoBehaviour
{
    public string txt_line;
    public GameObject m_light;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCollision"))
        {
            UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main",txt_line), 10.0f);
            UI.staticUI.ShowLine();
            m_light.SetActive(true);
            AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f, true);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
