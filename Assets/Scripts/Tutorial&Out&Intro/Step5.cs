
using CustomUI;
using System.Collections;
using UnityEngine;

/// <summary>
/// 문이 닫히고 캐릭터가 문쪽을 바라봅니다.
/// </summary>
public class Step5 : MonoBehaviour
{
    //삐에로 이벤트 실행
    public Door door_left;
    public Door door_right;

    public GameObject go_Target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //쾅 소리
            AudioManager.instance.PlayEffectiveSound("DoorClose_Metal", 1.5f);
            
            //카메라 떨림
            EventManager.instance.CameraShake(0.7f, 0.25f);

            //화면 놀란 효과
            Fade.FadeSetting(false, 0.5f, Color.black);

            //문이 열려있다면, 닫는다.
            if (door_left.OpenCloseCheck) door_left.Func();
            if(door_right.OpenCloseCheck) door_right.Func();

            StartCoroutine(RotateCroutine());
        }
    }

    IEnumerator RotateCroutine()
    {
        //조작이 불가하게 만들기 위함.
        PlayerEvent.instance.isDead = true;

        ////잠시 대기
        yield return new WaitForSeconds(1.0f);

        //기본 위치로 수정
        PlayerEvent.instance.SetDefault();

        //현재 플레이어의 보는 각도
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();

        //대상을 향한 방향을 가져옵니다.
        Vector3 dir = go_Target.transform.position - (PlayerEvent.instance.GetPosition()+new Vector3(0,1.0f,0));
        Quaternion look = Quaternion.LookRotation(dir);

        //목표 회전 최종값
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);

        //회전
        float t = 0;
        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 8;
            yield return null;
        }
        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        //움직임 활성
        PlayerEvent.instance.isDead = false;

        //대사를 띄웁니다.
        //어ㅓ...? 뭐...뭐야!!
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro5_Line"), 7);
        UI.staticUI.ShowLine();

        //다음 스텝 실행
        GameManager.Instance.SetIntroStep(6);
    }
}
