using Cinemachine;
using CustomUI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    //아이템 사용 가능 변수
    public bool DontUseCamera = false;
    public bool DontUseFlashLight = false;
    public bool DontUseGhostBall = false;
    public bool DontUseGhostMeter = false;

    //인트로가 끝나고 게임이 시작 됨
    public bool GameStarted = false;

    //게임 내 오브젝트가 동작하지 못하도록 한다.
    public bool DontMove { get; set; } = false;

    //게임의 기능 중지, Time.delta에 따른
    Action action_frozen;

    //게임이 일시정시 상태로 변경된다.
    public bool FrozenGame { get; private set; } = false;

    ///For Progress Game
    //남은 리트라이 가능 횟수 0이 되면 게임이 끝난다.
    int _remainChance = 3;
    //승천시킨 영혼의 개수
    int _ascendedSoul = 0;
    //총 영혼 수
    int _allSoulsCount;

    //0 : Player, 1 : Dead, 2 : Library
    public List<CinemachineVirtualCamera> virtualCamera;

    public Tutorial cs_Tutorial;
    public IntroSystem cs_IntroSystem;
    public Outro cs_Outro;

    private void Awake()
    {
        if (Instance == null)
        {
            //Debug.LogWarning("게임매니저 싱글톤 없음");
            Instance = this;
        }
        else
        {
            //Debug.LogWarning("게임매니저 싱글톤 있음");
            Destroy(Instance);
        }

        GameStarted = false;
    }
    private void OnDestroy()
    {
        Destroy(Instance);
        Instance = null;
    }
    IEnumerator Start()
    {
        //모든 영혼을 비활성화
        EventManager.instance.DeactiveSouls();

        //게임 시작 시 존재하는 영혼들의 수를 가져온다.
        _allSoulsCount = EventManager.instance.GetActivationSouls(); 
        Debug.LogWarning("현재 존재하는 영혼의 수 : " + _allSoulsCount);

        //모바일일 경우
        if (!GameConfig.IsPc())
        {
            //아이템 UI를 비활성화합니다.
            UI.mobileControllerUI.AllDeactiveItem();
        }

        //대기
        yield return null;

        //초기 셋팅
        Setting.LoadInitializeSetting();

        CursorHide();

        cs_Outro.gameObject.SetActive(false);

        //yield return new WaitForSeconds(2.0f); 
        //////test
        //PlayerEvent.instance.PlayerDeadSecene(false);
    }

    //게임 시작
    public void GameStart(bool tutorial)
    {
        if (tutorial) TutorialStart();
        else SkipTutorial();
    }

    #region Intro && Tutorial
    public void TutorialStart()
    {
        //Debug.Log("튜토리얼 시작");

        //초반 조작 금지
        PlayerEvent.instance.isDead = true;
        DontUseCamera = true;
        DontUseFlashLight = true;
        DontUseGhostBall = true;
        DontUseGhostMeter = true;

        IntroSystem.Active = false;

        Tutorial tutorial = FindObjectOfType<Tutorial>();
        tutorial.TutorialStart();
    }

    //게임 튜토리얼과 인트로를 건너뛰고 게임이 시작됩니다.
    public void SkipTutorial()
    {
        //Debug.Log("튜토리얼 스킵");

        //초반 조작 금지
        PlayerEvent.instance.isDead = true;

        //사용 금지
        DontUseCamera = true;
        DontUseFlashLight = true;
        DontUseGhostBall = true;
        DontUseGhostMeter = true;

        //밤으로 변경
        RenderSettings.ambientLight =DataSet.Instance.GetDefaultColor();

        //인트로를 클리어했음을 나타냄.
        DataSet.Instance.SettingValue.IntroClear = true;
        DataSet.Instance.Save();

        //스폰 효과
        PlayerEvent.instance.PlayerFaint(
            () => {
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "Paint1"), 6);
                UI.staticUI.ShowLine();

                PlayerEvent.instance.camBattery = 100;
                PlayerEvent.instance.camPermanentDamage = 0;
            }
         ) ;

        //튜토리얼 인트로 제거
        if(cs_Tutorial)cs_Tutorial.Skip();
        if(cs_IntroSystem)cs_IntroSystem.Skip();

        // 배경음
        AudioManager.instance.PlayBGMSound("AmbientScary");

        //모바일일 경우
        if (!GameConfig.IsPc())
        {
            //아이템 UI를 비활성화합니다.
            UI.mobileControllerUI.AllDeactiveItem();
        }

        //게임 시작을 알림
        GameStarted = true;

        //플레이어 속도 지정
        //FindObjectOfType<FirstPersonController>().MoveSpeed = 3.5f;
    }

    //게임이 모두 끝났을 경우
    public void GameEnd()
    {
        Debug.Log("게임 엔딩");

        //시간 저장
        float _time = PlayerEvent.instance.Timer;
        //5분 이상의 기록이 존재할 경우
        if (DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff] > 300)
        {
            //현재 시간이 이전 기록보다 크다면?
            if(DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff] < _time)
            {
                //이전 기록을 기록한다.
                _time = DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff];
            }
        }
        DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff] = _time;
        DataSet.Instance.Save();

        //모든 움직임 금지
        DontMove = true;
        PlayerEvent.instance.isDead = true;

        //애니메이션 재생을 위함
        Time.timeScale = 1;

        //아웃트로 활성화
        cs_Outro.gameObject.SetActive(true);
        cs_Outro.OutroStart();

        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.2f);

        //플레이어를 가만히 있게한다
        PlayerEvent.instance.StopPlayerAnimation();

        //동작 초기화
        PlayerEvent.instance.Action_Init();
    }

    public void SetIntroStep(int step)
    {
        cs_IntroSystem.gameObject.SetActive(true);
        cs_IntroSystem.IntroStep(step);
    }
    #endregion

    //게임 정지와 관련하여 함수를 가지고 있다가 실행한다.
    public void FrozenOnGame(Action action = null)
    {
        Time.timeScale = 0.0f;
        FrozenGame = true;
        if (action != null) action_frozen = action;
    }
    public void FrozenOffGame()
    {
        Time.timeScale = 1.0f;
        FrozenGame = false;
        if (action_frozen != null) action_frozen();
        action_frozen = null;

        CursorHide();
    }

    //승천한 영혼의 수를 카운트합니다.
    public void Ascention()
    {
        _ascendedSoul += 1;
        //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", $"Ascended{_ascendedSoul}"), 5.0f);
        //UI.staticUI.ShowLine();


        if(_ascendedSoul >= _allSoulsCount)
        {
            Debug.Log("모든 영혼을 정화했습니다.");
            GameEnd();
            return;
        }

        if (_ascendedSoul >= Mathf.Floor(_allSoulsCount*0.5f))
        {
            Debug.Log("영혼 절반을 정화했습니다. 검은 악귀를 활성화 합니다.");
            EventManager.instance.ActiveGhostBlack();
        }

        //유령의 속도가 빨라지고, 소리를 지른다.
        EventManager.instance.AscendedSoul();
    }


    //CursorSetting
    public void CursorShow()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined; // 구역 제한
    }
    public void CursorHide()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;  //보이지 않기
    }

    //남은 찬스를 지정
    public int CountingChance() { _remainChance -= 1; return _remainChance; }

    public int RemainChance() { return _remainChance; }

    public float GetRemainSoulsRatio()
    {
        //0~ 1 비율로 남은 영혼들을 비율을 반환
        return (_allSoulsCount-_ascendedSoul) / (float)_allSoulsCount;
    }
    public int GetRemainSouls() { return _allSoulsCount - _ascendedSoul; }

    //카메라 전환
    public void CameraChange(int num)
    {
        if (virtualCamera.Count < 1) { Debug.LogError("변경을 위한 가상 카메라가 없습니다."); return; }

        for (int i = 0; i < virtualCamera.Count; i++)
        {
            virtualCamera[i].Priority = virtualCamera.Count - i;
        }

        if (num < virtualCamera.Count)
        {
            virtualCamera[num].Priority = virtualCamera.Count + 1;
        }
    }
}
