using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �̺�Ʈ �� ó�� �����ִ� �ȳ�����
/// �� �ȳ����� ��Ʈ�ΰ� ����� �Ŀ� �����ش�.
/// </summary>
public class GameStartedLineTrigger : MonoBehaviour
{
    [Header("Tip => //������ ���۵� �� ������ ���ΰ�?")]
    public bool GameStarted = true;

    public string Key;
    public float time = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameStarted)
            {
                if (GameManager.Instance.GameStarted)
                {
                    UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", Key), time);
                    UI.staticUI.ShowLine();
                    Destroy(this.gameObject);
                }
            }
            else
            {
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", Key), time);
                UI.staticUI.ShowLine();
                Destroy(this.gameObject);
            }
        }
        
    }

}

