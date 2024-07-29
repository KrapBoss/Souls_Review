using Cinemachine;
using CustomUI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    //������ ��� ���� ����
    public bool DontUseCamera = false;
    public bool DontUseFlashLight = false;
    public bool DontUseGhostBall = false;
    public bool DontUseGhostMeter = false;

    //��Ʈ�ΰ� ������ ������ ���� ��
    public bool GameStarted = false;

    //���� �� ������Ʈ�� �������� ���ϵ��� �Ѵ�.
    public bool DontMove { get; set; } = false;

    //������ ��� ����, Time.delta�� ����
    Action action_frozen;

    //������ �Ͻ����� ���·� ����ȴ�.
    public bool FrozenGame { get; private set; } = false;

    ///For Progress Game
    //���� ��Ʈ���� ���� Ƚ�� 0�� �Ǹ� ������ ������.
    int _remainChance = 3;
    //��õ��Ų ��ȥ�� ����
    int _ascendedSoul = 0;
    //�� ��ȥ ��
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
            //Debug.LogWarning("���ӸŴ��� �̱��� ����");
            Instance = this;
        }
        else
        {
            //Debug.LogWarning("���ӸŴ��� �̱��� ����");
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
        //��� ��ȥ�� ��Ȱ��ȭ
        EventManager.instance.DeactiveSouls();

        //���� ���� �� �����ϴ� ��ȥ���� ���� �����´�.
        _allSoulsCount = EventManager.instance.GetActivationSouls(); 
        Debug.LogWarning("���� �����ϴ� ��ȥ�� �� : " + _allSoulsCount);

        //������� ���
        if (!GameConfig.IsPc())
        {
            //������ UI�� ��Ȱ��ȭ�մϴ�.
            UI.mobileControllerUI.AllDeactiveItem();
        }

        //���
        yield return null;

        //�ʱ� ����
        Setting.LoadInitializeSetting();

        CursorHide();

        cs_Outro.gameObject.SetActive(false);

        //yield return new WaitForSeconds(2.0f); 
        //////test
        //PlayerEvent.instance.PlayerDeadSecene(false);
    }

    //���� ����
    public void GameStart(bool tutorial)
    {
        if (tutorial) TutorialStart();
        else SkipTutorial();
    }

    #region Intro && Tutorial
    public void TutorialStart()
    {
        //Debug.Log("Ʃ�丮�� ����");

        //�ʹ� ���� ����
        PlayerEvent.instance.isDead = true;
        DontUseCamera = true;
        DontUseFlashLight = true;
        DontUseGhostBall = true;
        DontUseGhostMeter = true;

        IntroSystem.Active = false;

        Tutorial tutorial = FindObjectOfType<Tutorial>();
        tutorial.TutorialStart();
    }

    //���� Ʃ�丮��� ��Ʈ�θ� �ǳʶٰ� ������ ���۵˴ϴ�.
    public void SkipTutorial()
    {
        //Debug.Log("Ʃ�丮�� ��ŵ");

        //�ʹ� ���� ����
        PlayerEvent.instance.isDead = true;

        //��� ����
        DontUseCamera = true;
        DontUseFlashLight = true;
        DontUseGhostBall = true;
        DontUseGhostMeter = true;

        //������ ����
        RenderSettings.ambientLight =DataSet.Instance.GetDefaultColor();

        //��Ʈ�θ� Ŭ���������� ��Ÿ��.
        DataSet.Instance.SettingValue.IntroClear = true;
        DataSet.Instance.Save();

        //���� ȿ��
        PlayerEvent.instance.PlayerFaint(
            () => {
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "Paint1"), 6);
                UI.staticUI.ShowLine();

                PlayerEvent.instance.camBattery = 100;
                PlayerEvent.instance.camPermanentDamage = 0;
            }
         ) ;

        //Ʃ�丮�� ��Ʈ�� ����
        if(cs_Tutorial)cs_Tutorial.Skip();
        if(cs_IntroSystem)cs_IntroSystem.Skip();

        // �����
        AudioManager.instance.PlayBGMSound("AmbientScary");

        //������� ���
        if (!GameConfig.IsPc())
        {
            //������ UI�� ��Ȱ��ȭ�մϴ�.
            UI.mobileControllerUI.AllDeactiveItem();
        }

        //���� ������ �˸�
        GameStarted = true;

        //�÷��̾� �ӵ� ����
        //FindObjectOfType<FirstPersonController>().MoveSpeed = 3.5f;
    }

    //������ ��� ������ ���
    public void GameEnd()
    {
        Debug.Log("���� ����");

        //�ð� ����
        float _time = PlayerEvent.instance.Timer;
        //5�� �̻��� ����� ������ ���
        if (DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff] > 300)
        {
            //���� �ð��� ���� ��Ϻ��� ũ�ٸ�?
            if(DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff] < _time)
            {
                //���� ����� ����Ѵ�.
                _time = DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff];
            }
        }
        DataSet.Instance.SettingValue.ClearTime[DataSet.Instance.SettingValue.CurrentDiff] = _time;
        DataSet.Instance.Save();

        //��� ������ ����
        DontMove = true;
        PlayerEvent.instance.isDead = true;

        //�ִϸ��̼� ����� ����
        Time.timeScale = 1;

        //�ƿ�Ʈ�� Ȱ��ȭ
        cs_Outro.gameObject.SetActive(true);
        cs_Outro.OutroStart();

        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.2f);

        //�÷��̾ ������ �ְ��Ѵ�
        PlayerEvent.instance.StopPlayerAnimation();

        //���� �ʱ�ȭ
        PlayerEvent.instance.Action_Init();
    }

    public void SetIntroStep(int step)
    {
        cs_IntroSystem.gameObject.SetActive(true);
        cs_IntroSystem.IntroStep(step);
    }
    #endregion

    //���� ������ �����Ͽ� �Լ��� ������ �ִٰ� �����Ѵ�.
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

    //��õ�� ��ȥ�� ���� ī��Ʈ�մϴ�.
    public void Ascention()
    {
        _ascendedSoul += 1;
        //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", $"Ascended{_ascendedSoul}"), 5.0f);
        //UI.staticUI.ShowLine();


        if(_ascendedSoul >= _allSoulsCount)
        {
            Debug.Log("��� ��ȥ�� ��ȭ�߽��ϴ�.");
            GameEnd();
            return;
        }

        if (_ascendedSoul >= Mathf.Floor(_allSoulsCount*0.5f))
        {
            Debug.Log("��ȥ ������ ��ȭ�߽��ϴ�. ���� �Ǳ͸� Ȱ��ȭ �մϴ�.");
            EventManager.instance.ActiveGhostBlack();
        }

        //������ �ӵ��� ��������, �Ҹ��� ������.
        EventManager.instance.AscendedSoul();
    }


    //CursorSetting
    public void CursorShow()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined; // ���� ����
    }
    public void CursorHide()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;  //������ �ʱ�
    }

    //���� ������ ����
    public int CountingChance() { _remainChance -= 1; return _remainChance; }

    public int RemainChance() { return _remainChance; }

    public float GetRemainSoulsRatio()
    {
        //0~ 1 ������ ���� ��ȥ���� ������ ��ȯ
        return (_allSoulsCount-_ascendedSoul) / (float)_allSoulsCount;
    }
    public int GetRemainSouls() { return _allSoulsCount - _ascendedSoul; }

    //ī�޶� ��ȯ
    public void CameraChange(int num)
    {
        if (virtualCamera.Count < 1) { Debug.LogError("������ ���� ���� ī�޶� �����ϴ�."); return; }

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
