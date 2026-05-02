using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 최종적으로 열어야 되는 문이다.
/// 하위 모든 오브젝트들의 상호작용이 완료되어야 사용이 가능하다.
/// </summary>
public class GoalDoor : ExpandObject
{
    ProgressObjectManager _progressManager;

    bool activated = false;//동작이 완료되었음을 나타낸다.
    
    //시작 좌표
    public Transform StartPosition;

    public override bool Func(string name = null)
    {
        if (_progressManager.ActionObject() && !activated)//모든 하위 오브젝트들의 이벤트가 완료되었다.
        {
            //목표 오브젝트 이름과 같다면?
            if (name.Equals(TargetObjectName))
            {
                //문이 열림
                StartCoroutine(OpenDoorCroutine());

                StartCoroutine(FuncCroutine());
                return true;
            }
            else
            {
                _progressManager.ShowNextAction();
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    IEnumerator OpenDoorCroutine()
    {
        float t = 0;
        float targetY = transform.localRotation.eulerAngles.y + 90.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0,Mathf.Lerp(transform.localRotation.eulerAngles.y, targetY,t),0);
            yield return null;
        }
    }

    IEnumerator FuncCroutine()
    {
        activated = true;

        //그랩 아이템 제거
        PlayerEvent.instance.DestroyGrabedItem();

        //플레이어 애니메이션 제거
        PlayerEvent.instance.StopPlayerAnimation();

        //모든 동작 불가
        GameManager.Instance.DontMove = true;

        //사운드
        AudioManager.instance.PlayEffectiveSound("DoorOpen_Common", 1.0f);

        //화면 가리기
        Fade.FadeSetting(true, 1.0f, Color.black);
        yield return new WaitForSeconds(1.1f);
        yield return new WaitForSeconds(2.0f);

        //인트로 시작
        GameManager.Instance.SetIntroStep(0);

        //화면 페이드 아웃
        Fade.FadeSetting(false, 1.0f, Color.black);

        //모든 동작 가능
        GameManager.Instance.DontMove = false;

        PlayerEvent.instance.GrabOff();
        PlayerEvent.instance.SetPostition(StartPosition.position);
        PlayerEvent.instance.SetRotation(Quaternion.Euler(0, 180.0f, 0));

        //튜토리얼 삭제
        Destroy(FindObjectOfType<Tutorial>().gameObject);
    }

    protected override void Init()
    {
        base.Init();

        _progressManager = GetComponent<ProgressObjectManager>();
    }
}
