using CustomUI;
using System;
using System.Collections;
using UnityEngine;

enum InputNames
{
    Sit,
    HoldOnBreath,
    Jump,
    Grab,
    Throwing,
    Func,
    CameraView
}

[System.Serializable]
public struct ObjectsActiveInfo
{
    public GameObject obj;
    public bool active;
}
[System.Serializable]
public class StepObjectInfo // 단계별 오브젝트 활성 비활성 정보를 담는다.
{
    public ObjectsActiveInfo[] StepObejcts;

    public void SetObjects()
    {
        for (int i = 0; i < StepObejcts.Length; i++)
        {
            StepObejcts[i].obj.SetActive(StepObejcts[i].active);
        }
    }
}

public class IntroSystem : MonoBehaviour
{
    public StepObjectInfo[] StepObejcts;//단계별 오브젝트 활성화정보를 지정

    public GameObject[] obj_delete; // 삭제할 오브젝트를 지정함.

    /// <summary> 오르골  </summary>
    public MusicBox cs_musicBox;
    /// <summary> 영혼 </summary>
    public Soul cs_soul;

    /// <summary> 잠금을 진행할 문 </summary>
    public Door[] cs_doors;

    public static bool Active = false; // 인트로가 활성화됨을 나타냄.

    public void IntroStep(int step)
    {
        if (step > StepObejcts.Length - 1)
        {
            Debug.Log("Intro :: 다음 단계가 없습니다.");
            step = -1;
        }

        Debug.Log($"Intro를 실행합니다.:: 다음 단계가 없습니다. {step}");

        Active = true;

        //제한이나 해당 스텝을 준비한다.
        switch (step)
        {
            case 0://플레이어 트랜스폼 초기화 및 카메라 및 가림막 활성화
                Debug.LogWarning("Intro Step 0 :: 플레이어 활성화 및 카메라 줍기");

                //밤으로 변경
                RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
                // 후, 하 , 사운드
                VoiceLineManager.Instance.PlayRandomHansungReact();

                //아이템 놓기
                AudioManager.instance.PlayBGMSound("AmbientScary");
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "EnterTheMansion"),true);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro1"), 3);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro2"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[0].SetObjects();
                break;
            case 1://문을 열어야 된다.
                //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro3"), 5);
                //UI.staticUI.ShowLine();
                StepObejcts[1].SetObjects();
                break;
            case 2://망치를 가지러 가야한다.
                //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro4"), 5);
                //UI.staticUI.ShowLine();
                StepObejcts[2].SetObjects();
                break;
            case 3://저택안으로 진입을 시작하기 전
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro5"), 5);
                UI.staticUI.ShowLine();

                VoiceLineManager.Instance.ClearVoice();         // 음성대화 종료
                GameManager.Instance.DontUseFlashLight = false; //우선 플레쉬 라이트만 사용합니다.
                GameManager.Instance.DontUseCamera = true;
                GameManager.Instance.DontUseGhostBall = true;
                GameManager.Instance.DontUseGhostMeter = true;

                if (!GameConfig.IsPc()) UI.mobileControllerUI.Show();

                VoiceLineManager.Instance.PlayRandomHansungReact();

                PlayerEvent.instance.isDead = false;

                StepObejcts[3].SetObjects();
                break;
            case 4:// 저택 진입한 이후 부딪힌 컬라이더
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6"), 5);
                UI.staticUI.ShowLine();
                // 후, 하 , 사운드
                VoiceLineManager.Instance.PlayRandomHansungReact();


                StepObejcts[4].SetObjects();
                break;
            case 5: //문에 대한 사용 법을 알려줍니다.
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro7"), 5);
                UI.staticUI.ShowLine();


                //공지 추가 표기
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "Tutorial_Door"),false);

                StepObejcts[5].SetObjects();
                break;
            case 6: // 이제... 탐지가 한번 사용해보자

                VoiceLineManager.Instance.EnterLine(new() { clip = $"Intro8_Line", delay = 0, line = $"Intro8_Line", reverb = false });
                VoiceLineManager.Instance.ShowLine(false);
                //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro8_Line"), 5);
                //UI.staticUI.ShowLine();

                // 후, 하 , 사운드
                //VoiceLineManager.Instance.PlayRandomHansungReact();

                GameManager.Instance.DontUseGhostMeter = false;
                if (!GameConfig.IsPc()) UI.mobileControllerUI.ActiveIcon(1, true);

                // 튜토리얼 영상을 보여줌 => 컬라이더로 처리
                //PlayerEvent.instance.ShowVideo(VideoType.METER);
                //GameManager.Instance.DontUseGhostMeter = false;

                //공지 추가 표기
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "howtousemeter"), false, delay: 3.0f);
                StepObejcts[6].SetObjects();
                break;
            case 7: // 보주 사용 법
                VoiceLineManager.Instance.EnterLine(new() { clip = $"Intro9_Line", delay = 0, line = $"Intro9_Line", reverb = false });
                VoiceLineManager.Instance.ShowLine(false);
                //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro9_Line"), 5);
                //UI.staticUI.ShowLine();

                // 튜토리얼 영상을 보여줌
                PlayerEvent.instance.showVideo = false;
                PlayerEvent.instance.ShowVideo(VideoType.BALL);
                GameManager.Instance.DontUseGhostBall = false;
                if (!GameConfig.IsPc()) UI.mobileControllerUI.ActiveIcon(2, true);

                //공지 추가 표기
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "howtousecrystal"), false, delay: 3.0f);
                StepObejcts[7].SetObjects();
                break;
            case 8: // 오르골 사용 튜토리얼 + 문에 물리적 제한을 추가

                VoiceLineManager.Instance.EnterLine(new() { clip = $"Intro10_Line", delay = 0, line = $"Intro10_Line", reverb = false });
                VoiceLineManager.Instance.ShowLine(false);
                //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro10_Line"), 5);
                //UI.staticUI.ShowLine();

                //문을 잠급니다.
                foreach (var item in cs_doors) item.Close();
                foreach (var item in cs_doors) item.isLocked = true;

                // 튜토리얼 영상을 보여줌
                PlayerEvent.instance.showVideo = false;
                PlayerEvent.instance.ShowVideo(VideoType.MUSICBOX);

                //오르골 사용 이후 액션 지정
                cs_musicBox.act_afterUse = () => GameManager.Instance.SetIntroStep(9);

                //공지 추가 표기
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "howtousemusicbox"), false, delay: 3.0f);

                StepObejcts[8].SetObjects();
                break;

            case 9: // 카메라 사용하도록 유도

                VoiceLineManager.Instance.EnterLine(new() { clip = $"Intro11_Line", delay = 0, line = $"Intro11_Line", reverb = false });
                VoiceLineManager.Instance.ShowLine(false);
                //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro11_Line"), 5);
                //UI.staticUI.ShowLine();

                //공지 추가 표기
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "howtousecamera"), false, delay:3.0f);

                // 영혼에 대한 해방 이벤트 등록 및 승천 후 인트로 다음 단계로 넘어감
                GameManager.Instance.DontUseCamera = false;
                if (!GameConfig.IsPc()) UI.mobileControllerUI.ActiveIcon(0, true);
                cs_soul.act_afterAscention = () => GameManager.Instance.SetIntroStep(10);

                StepObejcts[9].SetObjects();
                break;

            case 10: // 끝과 힘께 아름다운 빛들이 주변을 둘러싼다.
                VoiceLineManager.Instance.EnterLine(new() { clip = $"Intro12_Line", delay = 0, line = $"Intro12_Line", reverb = false });
                VoiceLineManager.Instance.ShowLine(true);

                //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro12_Line"), 5);
                //UI.staticUI.ShowLine();

                StepObejcts[10].SetObjects();
                break;

            case 11: // 찰스에게 죽음을 당함
                UI.staticUI.ShowLine();

                StepObejcts[11].SetObjects();
                break;

            case -1://모두 삭제 후 

                Debug.Log("종료");

                for (int i = 0; i < obj_delete.Length; i++)
                {
                    Destroy(obj_delete[i]);
                }

                foreach (var item in cs_doors) item.isLocked = false;

                //인트로가 끝나고 모든 오브젝트를 삭제합니다.
                GameManager.Instance.SkipTutorial();
                break;
        }
    }

    //스킵을 할 경우
    public void Skip()
    {
        //메인도어 활성화
       // StepObejcts[4].SetObjects();
        Active = true;
        Destroy(gameObject);
    }
}
