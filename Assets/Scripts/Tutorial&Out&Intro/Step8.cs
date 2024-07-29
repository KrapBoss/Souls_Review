using CustomUI;
using StarterAssets;
using System.Collections;
using UnityEngine;


//흰둥이 등장.
//죽음

public class Step8 : MonoBehaviour
{
    public GameObject go_Air;
    public GameObject go_Target;

    public GameObject go_HeartBeat;

    public GameObject go_Block;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Step 8 Start");

            go_Block.SetActive(true);

            GetComponent<BoxCollider>().enabled = false;

            go_Air.GetComponent<AudioSource>().volume = DataSet.Instance.SettingValue.Volume*0.7f;

            StartCoroutine(ActionCroutine());
        }
    }


    IEnumerator ActionCroutine()
    {
        //사운드와 함께 뒤돌아 봅니다.

        //기본 위치로 수정
        PlayerEvent.instance.SetDefault();

        //조작이 불가하게 만들기 위함.
        PlayerEvent.instance.isDead = true;

        //흰둥이 활성화
        go_Air.SetActive(true);
        //현재 플레이어의 보는 각도
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();
        //대상을 향한 방향을 가져옵니다.
        Vector3 dir = go_Air.transform.position - PlayerEvent.instance.GetPosition() - new Vector3(0, -0.7f, 0);
        Quaternion look = Quaternion.LookRotation(dir);
        //목표 회전 최종값
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);
        //회전
        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime * 5;
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            yield return null;
        }
        //카메라 흔들기
        EventManager.instance.CameraShake(5.0f, 0.05f);
        //효과음
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f);

        //대사 : 뭐..뭐야!! 저거 tlqkf 빨리 나가자..
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro8_Line"), 5, true);
        //대사 : 아이씨!! 조졌다 이거...
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro8-1_Line"), 7);
        UI.staticUI.ShowLine();

        //--------------------------------------플레이어의 속도 증가!!!--------------------------------
        //FindObjectOfType<FirstPersonController>().MoveSpeed *= 1.3f;

        //사운드 On
        go_HeartBeat.SetActive(true);
        go_HeartBeat.GetComponent<AudioSource>().volume = DataSet.Instance.SettingValue.Volume * 0.8f;
        go_HeartBeat.GetComponent<AudioSource>().pitch = 1.5f;

        yield return new WaitForSeconds(1.0f);

        currentRot = PlayerEvent.instance.GetRotationCameraView();
        //대상을 향한 방향을 가져옵니다.
        dir = go_Target.transform.position - PlayerEvent.instance.GetPosition();
        look = Quaternion.LookRotation(dir);
        //목표 회전 최종값
        targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);
        //회전
        t = 0;
        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }
        go_Air.SetActive(false);

        PlayerEvent.instance.isDead = false;

        //다음 스텝
        GameManager.Instance.SetIntroStep(9);
    }
}
