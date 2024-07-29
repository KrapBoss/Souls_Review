using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 도서관 이벤트 시 처음 보여주는 안내문자
/// 이 안내문은 인트로가 종료된 후에 보여준다.
/// </summary>
public class GameStartedLineTrigger : MonoBehaviour
{
    [Header("Tip => //게임이 시작된 후 보여질 것인가?")]
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

