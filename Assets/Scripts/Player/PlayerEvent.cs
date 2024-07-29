using System;
using UnityEngine;
using StarterAssets;
using Unity.Mathematics;
using CustomUI;
using Unity.VisualScripting;

/// <summary>
/// 플레이어에서 발생한 이벤트에 대한 정보를 갱신하고 전달한다.
/// </summary>

// 플레이어의 현재 동작 상태를 나타낸다.
public enum EnumPlayerEvent
{
    HoldOnBreath = 0,
    Idle,
    Sit,
    Walk,
    Jump,
    Run
}
//플레이어의 상태 변환에 따라 거리 값을 저장할 변수
[System.Serializable]
public struct StructPlayerEvent
{
    public EnumPlayerEvent s_event;
    public float s_distance;

    public StructPlayerEvent(EnumPlayerEvent e, float d)
    {
        s_event = e;
        s_distance = d;
    }
}

public class PlayerEvent : MonoBehaviour
{
    public static PlayerEvent instance { get; private set; }
    public bool isDead { get; set; }
    //비디오를 보여주고 있는가?
    public bool showVideo { get; set; }

    [Space]
    [Header("플레이어 이벤트")]
    [SerializeField]
    StructPlayerEvent[] s_PlayerEvent =
    {
            new StructPlayerEvent(EnumPlayerEvent.HoldOnBreath, 0.0f),
            new StructPlayerEvent(EnumPlayerEvent.Idle, 1.0f),
            new StructPlayerEvent(EnumPlayerEvent.Sit, 2.0f),
            new StructPlayerEvent(EnumPlayerEvent.Walk, 3.0f),
            new StructPlayerEvent(EnumPlayerEvent.Run, 4.0f),
            new StructPlayerEvent(EnumPlayerEvent.Jump, 5.0f)
    };   //플레이어 각 이벤트별 소리 전달 범위를 가질 변수
    [SerializeField] float EventTimeOver = 0.5f;                 //플레이어 이벤트 소리 전달을 위한 쿨타임
    private float _apply_EventTimeOver;                 //실제 쿨타임으로 적용시킬 값

    [Space]
    [Header("몬스터 거리 측정")]
    [SerializeField] float m_EstimateDistance = 10f;        //몬스터와의 판정 거리
    //[SerializeField] string m_BGM = "Horror";               //몬스터가 가까이 있으면 재생되는 사운드
    public bool m_isPlaying = false; //브금이 재생중인가?


    [Space]
    [Header("몬스터 거리 측정")]
    public bool CanHoldOnBreath = true; //숨을 참을 수 있게 해주겠다.
    [SerializeField] float holdOnBreathTimeOut = 8.0f;    // 숨을 참을 수 있는 시간
    private float _holdOnBreathTimeOut;

    [Space]
    [Header("오르골 변수")]
    public LayerMask FlashDisturbeObject;       //사진찍을 때 방해물이 되는 오브젝트 탐지하는 것.


    [Space]
    [Header("카메라")]
    public bool cameraEquipState = false;           //카메라 장착 상태를 나타냅니다.
    public float cam_UsePerSec =0.1f;     //장착시 초당 감소양 0.1f;
    public float cam_TakePicture = 15;   //사용 시 감소양     3.0f; 
    public float camBattery = 100;         //현재 배터리
    public float cam_PermanentDamageRate = 0.3f; //촬영 시 카메라 배터리 영구 손상 비율
    public float camPermanentDamage = 0; //영구 손상된 배터리
    private CameraController _cameraController;


    //이벤트 처리를 위함.
    public StarterAssetsInputs _input;

    public float Timer;

    // 플레이어 죽음을 담을 함수
    //public Action Action_PlayerDead { get; set; }

    //플레이어  다시 태어날 경우 초기화 되는 것들
    public Action Action_Init { get; set; }


    //카메라 촬영 플래쉬 터트리는 시간
    float flashTimeout;

    AudioSource _source;//심장소리를 내기 위함

    FirstPersonController _controller;

    DeadScene _deadScene;
    bool lastChace=false;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(instance); instance = this; }

        showVideo = false;
        isDead = false;
    }

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        _source = GetComponent<AudioSource>();
        _source.volume = DataSet.Instance.SettingValue.Volume * 1.2f;

        _controller = GetComponent<FirstPersonController>();

        _cameraController = FindObjectOfType<CameraController>();

        _deadScene = FindObjectOfType<DeadScene>();

        EventManager.instance.Action_AroundMonster += PlayHeartSound;


        //카메라값 설정
        cam_UsePerSec = DataSet.Instance.GameDifficulty.BatterryUsePerSec;
        cam_PermanentDamageRate = DataSet.Instance.GameDifficulty.BatteryPermanentDamageRate;

        //숨참기 시간 설정
        holdOnBreathTimeOut = DataSet.Instance.GameDifficulty.HoldOnBreath;
        _holdOnBreathTimeOut = holdOnBreathTimeOut;
    }

    private void Update()
    {
        //인트로가 종료되고 나서 게임 타이머가 시작됨.
        if(GameManager.Instance.GameStarted)Timer += Time.unscaledDeltaTime;

        if (isDead || GameManager.Instance.FrozenGame || GameManager.Instance.DontMove) return;

        AroundMonster();
        CheckHoldOnBreath();

        CameraBattery();

        if (_apply_EventTimeOver > 0.0f) { _apply_EventTimeOver -= Time.deltaTime; }
        if (flashTimeout > 0.0f) { flashTimeout -= Time.deltaTime; }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, s_PlayerEvent[1].s_distance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, s_PlayerEvent[2].s_distance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, s_PlayerEvent[3].s_distance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, s_PlayerEvent[4].s_distance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, s_PlayerEvent[5].s_distance);
    }
#endif

    //기본 설정을 다시 합니다.
    public void Init()
    {
        _holdOnBreath = false;
        playedUninputHold = false;
        playedInputHold = false;
        UI.activityUI.SetBreathGage(1 , true);

        Action_Init();
    }


    public bool playedInputHold = false, playedUninputHold = false;
    bool _holdOnBreath = false;
    float gaspTime = 100; //사용자가 숨참기를 누른시간.
    bool playedBreathSound = false;
    public void CheckHoldOnBreath()
    {
        //사용할 변수를 얻어옵니다.
        _holdOnBreath = _input.holdOnBreath;

        //if (!CanHoldOnBreath) _holdOnBreath = false;

        if (_holdOnBreath && CanHoldOnBreath) // 숨 참기 키를 눌렀다.
        {
            //숨쉬는 소리를 발생시킨다.
            if (!playedInputHold)
            {
                if (gaspTime > 2.0f)//숨참기를 누른 시간이 2초 이상일 경우에만 실행한다.
                {
                    AudioManager.instance.PlayEffectiveSound("Gasp", 0.9f);
                    playedBreathSound = true;
                }
                playedInputHold = true;
                playedUninputHold = false;
                gaspTime = 0;
            }

            _holdOnBreathTimeOut -= Time.deltaTime;
            gaspTime += Time.deltaTime;

            if (_holdOnBreathTimeOut < 0.0f)
            {
                _holdOnBreath = false;
                CanHoldOnBreath = false;
                Debug.Log("숨 참기 한계치");
            }
        }
        else                    //숨참는 시간 충전
        {
            if (_holdOnBreathTimeOut < holdOnBreathTimeOut)
            {
                _holdOnBreathTimeOut += Time.deltaTime * 0.8f;
                gaspTime += Time.deltaTime;

                //숨쉬기 시간이 0.3보다 크면 다시 가능
                if (_holdOnBreathTimeOut > holdOnBreathTimeOut * 0.3f) CanHoldOnBreath = true;

                //숨을 내뱉는 소리를 발생시킨다.
                if (!playedUninputHold)
                {
                    if (playedBreathSound)   //앞에서 숨을 들이켰다면, 내뱉는다.
                    {
                        AudioManager.instance.PlayEffectiveSound("GaspRelief", 0.9f);
                        playedBreathSound = false;
                    }
                    playedUninputHold = true;
                    playedInputHold = false;
                }
            }
            else                //숨 참는 시간이  충전되어 있다면 다시 숨참기 가능
            {
                _holdOnBreathTimeOut = holdOnBreathTimeOut;
                CanHoldOnBreath = true;
                gaspTime = 100;
                //Debug.Log("숨 참기 회복");
            }
        }

        UI.activityUI.SetBreathGage(_holdOnBreathTimeOut / holdOnBreathTimeOut, CanHoldOnBreath);


        if (!m_isPlaying)//몬스터에 따른 심장소리 플레이 중이지 않을 경우
        {
            if (_holdOnBreathTimeOut < holdOnBreathTimeOut) // 지금 숨참기 게이지가 변경하고 있을 경우
            {
                if (!_source.isPlaying) _source.Play();
                _source.pitch = 0.8f + 1 - (_holdOnBreathTimeOut / holdOnBreathTimeOut);
                //Debug.Log("숨참기 게이밎에 따른 소리 갱신중");
            }
            else//숨참기 게이지가 꽉차오른 경우
            {
                if (_source.isPlaying) _source.Stop();
                //Debug.Log("숨참기 게이지 변경되지 않는다.");
            }
        }
    }

    float t;
    void AroundMonster()     //몬스터가 주변에 있으면, 심장소리와 기타 효과를 적용한다.
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
            return;
        }
        t = 0.1f;

        //몬스터와 플레이어의 거리값을 얻어옵니다.
        float distance = Calculator.GetPathDistance(GetPosition(), EventManager.instance.GetMonsterPosition());
        //Debug.Log($"현재 플레이어 이벤트의 몬스터 체크 범위를 판단하고 있습니다.{m_EstimateDistance} // {distance}");

        //현재 탐지 범위 내에 몬스터가 존재합니다.
        if (distance < m_EstimateDistance)
        {
            if (!m_isPlaying)   //브금이 재생중이지 않을 경우
            {
                //Debug.Log("몬스터 사운드 재생");
                //AudioManager.instance.PlayBGMSound(m_BGM);
                m_isPlaying = true;

                //if (!_source.isPlaying) _source.Play();
            }
            EventManager.instance.Action_AroundMonster(math.clamp(1 - (distance / m_EstimateDistance), 0, 1));//거리 값에 따라 비율을 전달한다.
            //_source.pitch = 1.0f + 1.0f - (distance / m_EstimateDistance);
        }
        //몬스터가 탐지 범위 내에 존재하지 않습니다.
        else
        {
            //몬스터 BGM이 재생 중인 경우 종료합니다.
            if (m_isPlaying)    
            {
                //AudioManager.instance.StopBGMFadeOut();
                EventManager.instance.Action_AroundMonster(0);//거리 값에 따라 비율을 전달한다.

                m_isPlaying = false;
            }
        }

    }

    //심장소리를 들려준다.
    public void PlayHeartSound(float _pitch)
    {
        if (_pitch > 0.01f)
        {
            if (!_source.isPlaying) _source.Play();
            _source.pitch = 1.0f + _pitch;
        }
        else
        {
            _source.pitch = 1.0f;
            if (_source.isPlaying && m_isPlaying) _source.Stop();

            //AudioManager.instance.PlayBGMSound("AmbientScary");
        }
    }

    EnumPlayerEvent _previous_playerEvent = EnumPlayerEvent.Idle;  //이전 이벤트를 담음
    //플레이어 사운드에 따라 이벤트를 전달해준다.
    public void PlayerEventSound(EnumPlayerEvent _event)
    {
        //Debug.Log($"Current Player State {_event}");
        //이전 이벤트 값과 다르다면 바로 소리에 해당하는 거리값을 전달한다.
        if (!_previous_playerEvent.Equals(_event))
        {
            _previous_playerEvent = _event;
            _apply_EventTimeOver = 0.0f;
            //Debug.Log($"{_event} /  이전과 다른 소리값이 입력되었습니다.");
        }
        //거리값 전달의 딜레이를 판단한다.
        if (_apply_EventTimeOver > 0.0f) { return; }


        //!중요! 현재 플레이어 상태에 따른 소리 범위 전달값
        EventManager.instance.EventSound(transform, GetDistaceFromPlayerEvent(_event));

        // 시간
        _apply_EventTimeOver = EventTimeOver;
    }

    

    //각 이벤트별 소리 범위를 가져온다.
    //해당 이벤트가 없을 경우 소리 값이 입력되지 않는다. = return 0
    public float GetDistaceFromPlayerEvent(EnumPlayerEvent e)
    {
        for (int i = 0; i < s_PlayerEvent.Length; i++)
        {
            if (s_PlayerEvent[i].s_event.Equals(e)) return s_PlayerEvent[i].s_distance;
        }
        return 0;
    }

    public Vector3 GetPosition() { return transform.position; }
    public Vector2 GetRotationCameraView() { return _controller.GetRotationCameraView(); }
    public void SetPostition(Vector3 position)
    {
        _controller.SetPosition(position);
    }
    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void StopPlayerAnimation()
    {
        _controller.AnimationIdle();
    }
    public void SetCameraView(Vector2 vector2)
    {
        _controller.SetCameraView(vector2);
    }

    //카메라의 실제 Y 높이 값
    public float GetViewHeight() { return _controller.CameraWrap.transform.position.y; }


    #region ----------------------------------------------플레이어 죽음-------------------------------------------------------

    //플레이어가 기절을한다
    //기절을 할 경우 남은 횟수가 차감되지 않는다.
    public void PlayerFaint(Action action = null)
    {
        //죽음을 표시합니다.
        isDead = true;

        //죽었을 경우 기본적인 재설정을 실행합니다.
        Action_Init();
        //손전등 끄기
        FlashLightEquip(false);

        //기절 효과를 보여줍니다
        _controller.Faint(action);
    }

    //몬스터에게 잡히면 DeadScene로 넘어가 죽는 모션을 보입니다.
    public void PlayerDeadSecene(bool white)
    {
        if (_deadScene == null)
        {
            Debug.LogError("Player Event :: Non DeadScnenScript");
            return;
        }

        Debug.LogError("플레이어 사망");

        AudioManager.instance.StopBGMFadeOut();

        int remainChance = GameManager.Instance.CountingChance();

        //카메라 변경
        GameManager.Instance.CameraChange(1);

        _controller.SetDefaultTransform();

        //기본 셋팅
        Action_Init();
        //손전등 끄기
        FlashLightEquip(false);

        //죽었다고 알림
        isDead = true;

        //기회 존재 졸도
        if (remainChance > 0)
        {
            Debug.Log("졸도");

            if (white)
            {
                //졸도
                _deadScene.PaintWhite(Spawn);
                EventManager.instance.Action_AroundMonster(1);
            }
            else
            {
                //졸도 블랙이
                _deadScene.PaintBlack(Spawn);
                EventManager.instance.Action_AroundMonster(0.3f);
                UI.activityUI.AroundBlack(1.0f);
            }
        }
        //죽음
        else
        {
            Debug.Log("찐 죽음");

            //모바일
            if (!GameConfig.IsPc())
            {
                //마지막 광고 소생 사용 안한 경우에만
                if (!lastChace)
                {
                    UI.topUI.LastChance
                        (
                            ()=>LastChance(white),                     //yes
                            () => GoTitleAfterDead(white)   //no
                        ) ;

                    lastChace = true;

                    return;
                }
            }

            GoTitleAfterDead(white);
        }
    }

    //마지막 기회가 주어진다.
    public void LastChance(bool white)
    {
        Debug.Log($" 마지막 기회 여부 : {!lastChace}");

#if UNITY_ANDROID
        if(GoogleManager.instance.ShowAd(() => {
            if (white)
            {
                //졸도
                _deadScene.PaintWhite(Spawn);
                EventManager.instance.Action_AroundMonster(1);
            }
            else
            {
                //졸도 블랙이
                _deadScene.PaintBlack(Spawn);
                EventManager.instance.Action_AroundMonster(0);
            }}))
        {
            UI.topUI.LastChanceActive(false);
        }
#endif
    }
    public void GoTitleAfterDead(bool white)
    {

#if UNITY_ANDROID
        //모바일일 경우에만 끕니다.
        if (!GameConfig.IsPc())
        {
            UI.topUI.LastChanceActive(false);

            //광고가 있으면, 광고 후 죽음 후 타이틀로 보내준다.
            if (GoogleManager.instance.ShowAd(()=>DeadScenePaint(white)))
            {
                return;
            }
        }
#endif

        DeadScenePaint(white);
    }

    public void DeadScenePaint(bool white)
    {
        if (white)
        {
            _deadScene.PaintWhite(() => {
                SceneLoader.LoadLoaderScene("titleScene");
            });
        }
        else
        {
            _deadScene.PaintBlack(() => {
                SceneLoader.LoadLoaderScene("titleScene");
            });
        }        
    }

    //플레이어 스폰시킴
    public void Spawn()
    {
        //화면 가리기
        Fade.FadeSetting(true, 0.5f, Color.black);

        //카메라 교체
        GameManager.Instance.CameraChange(0);

        //손전등 활성화
        FlashLightEquip(true);
        
        //배경음 보여주기
        AudioManager.instance.PlayBGMSound("AmbientScary");

        //리스폰
        _controller.Spawn(1.0f, ShowText);
    }
    #endregion--------------------------------------------------------------------------------------


    #region -------------카메라--------------

    float BatteryDelayDelta = 0.1f;
    //카메라 배터리 관련 업데이트
    void CameraBattery()
    {
        float PermanentDamage = (cam_TakePicture * cam_PermanentDamageRate);
        float RemainTakePictureBattery = 100.0f - camPermanentDamage;
        float RemainChance = (RemainTakePictureBattery - (15 - PermanentDamage + 1f)) / PermanentDamage;

        /*Debug.LogWarning($"PlayerEvent 남은 카메라 배터리 사용횟수 : {RemainChance}" +
            $"\n 남은 영혼 : {GameManager.Instance.GetRemainSouls()}");*/

        //카메라 장착 중일 경우
        if (cameraEquipState)
        {
            camBattery -= cam_UsePerSec*Time.deltaTime;

            //카메라 배터리가 0.1 이하일 경우 장착 해제
            if (camBattery <= 0.1f)
            {
                SetCameraUnEquip();
            }
        }
        //카메라 장착이 아닌 경우
        else
        {
            //초당 카메라 배터리 충전비율
            if (camBattery < (100.0f - camPermanentDamage))
            {
                camBattery += cam_UsePerSec * 0.5f * Time.deltaTime;
            }
        }

        //UI 업데이트에는 의도적인 딜레이
        BatteryDelayDelta += Time.deltaTime;

        if (BatteryDelayDelta > 0.1f)
        {
            //UI 업데이트
            UI.staticUI.SetBattery(camBattery, camPermanentDamage);
            BatteryDelayDelta = 0;
        }
    }

    //배터리 회복 메서드
    public void BatteryRecovery()
    {
        if (camPermanentDamage > 0) camPermanentDamage = Mathf.Clamp(camPermanentDamage - DataSet.Instance.GameDifficulty.CameraRecovery, 0, 100);
        AudioManager.instance.PlayEffectiveSound("CameraUnequip", 1.0f);
    }

    //카메라 장착 해제
    public void SetCameraUnEquip()
    {
        if (cameraEquipState) _cameraController.EquipCamera();
    }
    //카메라 장착
    public bool CameraEquip()
    {
        //사용 불가
        if(GameManager.Instance.DontUseCamera || (camBattery < 3.0f)) return false;

        //카메라 장착
        _cameraController.EquipCamera();

        return true;
    }

    //카메라로 촬영하여 영혼을 승천시킵니다.
    RaycastHit hit;
    public void CameraTakePicture()
    {
        if (flashTimeout > 0.0f) return;
        if (camBattery < cam_TakePicture) return;

        EventManager.instance.Action_CameraFlash();// 효과를 위함.
        AudioManager.instance.PlayEffectiveSound("CameraShutter", 1.0f);

        //카메라 배터리 용량 감소
        camBattery -= cam_TakePicture;
        camPermanentDamage += cam_TakePicture * cam_PermanentDamageRate;

        //승천 가능한 영혼을 얻어옵니다.
        Soul soul = EventManager.instance.GetSoul(transform.position + new Vector3(0, GetViewHeight() * 0.5f, 0));
        if (soul != null)
        {
            //영혼 촬영이 가능한지 확인합니다.
            if (Physics.Linecast(_controller.CameraWrap.position, soul.GetPosition() + new Vector3(0, 0.8f, 0), out hit, FlashDisturbeObject))
            {
                Debug.Log($"승천에 실패했습니다. 부딪힌 장애물 : {hit.transform.name}");
                return;
            }

            //방해물 오브젝트 탐지된게 없는 경우에만 실행한다.
            soul.Ascention();

            //안도의 한숨 소리
            AudioManager.instance.PlayEffectiveSound("Sigh", 0.8f);
        }
        flashTimeout = 0.3f;



        //-----------남은 카메라 촬영 횟수와 비교합니다.-----------
        //인트로가 끝난 상태일 경우에 해당한다.
        if (GameManager.Instance.GameStarted)
        {
            float PermanentDamage = (cam_TakePicture * cam_PermanentDamageRate);
            float RemainTakePictureBattery = 100.0f - camPermanentDamage;
            float RemainChance = (RemainTakePictureBattery - (15- PermanentDamage + 1f)) / PermanentDamage;
            //Debug.LogWarning($"Player 남은 카메라 배터리 사용횟수 : {RemainChance}" +
            //    $"\n 남은 영혼 : {GameManager.Instance.GetRemainSouls()}");

            if (GameManager.Instance.GetRemainSouls() > RemainChance)
            {
               // Debug.LogWarning($"배터리를 모두 사용했습니다. : {RemainChance}");

                isDead = true;
                Time.timeScale = 1.0f;

                UI.topUI.ShowNoSignal();
            }
        }
    }

    //비디오 보기
    public void ShowVideo(VideoType type)
    {
        //모바일의 경우
        if (!GameConfig.IsPc())
        {
            UI.semiStaticUI.ShowVideo(type);
        }
        //PC의 경우
        else
        {
            _cameraController.VideoPlayer(type);
        }
    }

    #endregion
    #region---------------손전등 --------------------
    public bool FlashLightEquip(bool active)
    {
        if (cameraEquipState) return false;
        else
        {
            _cameraController.FlashLight(active);
            return true;
        }
    }


    #endregion




    //남은 목숨에 따라 지문을 표시해 준다.
    void ShowText()
    {
        //지문 표시
        switch (GameManager.Instance.RemainChance())
        {
            case 2:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "RemainLife2"), 3);
                UI.staticUI.ShowLine();
                //AudioManager.instance.PlayEffectiveSound("Sigh", 0.7f);
                break;
            case 1:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "RemainLife1"), 3);
                UI.staticUI.ShowLine();
                //AudioManager.instance.PlayEffectiveSound("Sigh", 1.2f);
                break;
            case 0:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "RemainLife0"), 3);
                UI.staticUI.ShowLine();
                //AudioManager.instance.PlayEffectiveSound("Sigh", 1.2f);
                break;
        }
    }


    public bool GetHoldOnBreath()
    {
        return _holdOnBreath;
    }

    //아이템 떨어트리기
    public void GrabOff()
    {   
        _controller.GrabOff(0.0f);
    }

    //대신 삭제 대리자
    public void DestroyGrabedItem()
    {
        _controller.DestroyGrabedItem();
    }

    public void SetDefault()
    {
        _controller.SetDefaultTransform();
    }
}