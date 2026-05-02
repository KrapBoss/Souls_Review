using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class IntroBox : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] Transform endPosition;

    [SerializeField] int lineCount = 9;

    /// <summary> 인트로를 시작합니다. </summary>
    public void StartIntoBox()
    {
        animator.SetTrigger("START");

        if (GameConfig.IsPc())
        {
            GameManager.Instance.CursorHide();
        }

        GameManager.Instance.cs_MapObject.MainDoor.SetActive(false);

        //손전등 끄기
        PlayerEvent.instance.FlashLightEquip(false);

    }


    public void Skip()
    {
        Debug.Log("인트로 박스 제거");
        Destroy(gameObject);
    }

    public void EndIntro()
    {
        Debug.Log("인트로 종료");

        // 플레이어 위치 조정
        PlayerEvent.instance.SetPostition(endPosition.position);
        PlayerEvent.instance.SetRotation(endPosition.rotation);


        //밤으로 변경
        RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
        //AudioManager.instance.PlayEffectiveSound("Sigh", 1.0f, true);

        // 후, 하 , 사운드
        VoiceLineManager.Instance.PlayRandomHansungReact();

        //아이템 놓기
        AudioManager.instance.PlayBGMSound("AmbientScary");

        // 화면 다시 보이도록 변경하기
        Fade.FadeSetting(false, 1.0f, Color.black);

        GameManager.Instance.SetIntroStep(3);
        GameManager.Instance.cs_MapObject.MainDoor.SetActive(true);
        GameManager.Instance.cs_MapObject.MainDoorLeft.isLocked = (false);
        GameManager.Instance.cs_MapObject.MainDoorRight.isLocked = (false);

        PlayerEvent.instance.FlashLightEquip(true);

        Skip();
    }
}