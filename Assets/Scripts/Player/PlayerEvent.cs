using System;
using UnityEngine;
using StarterAssets;
using Unity.Mathematics;
using CustomUI;
using Unity.VisualScripting;

/// <summary>
/// �÷��̾�� �߻��� �̺�Ʈ�� ���� ������ �����ϰ� �����Ѵ�.
/// </summary>

// �÷��̾��� ���� ���� ���¸� ��Ÿ����.
public enum EnumPlayerEvent
{
    HoldOnBreath = 0,
    Idle,
    Sit,
    Walk,
    Jump,
    Run
}
//�÷��̾��� ���� ��ȯ�� ���� �Ÿ� ���� ������ ����
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
    //������ �����ְ� �ִ°�?
    public bool showVideo { get; set; }

    [Space]
    [Header("�÷��̾� �̺�Ʈ")]
    [SerializeField]
    StructPlayerEvent[] s_PlayerEvent =
    {
            new StructPlayerEvent(EnumPlayerEvent.HoldOnBreath, 0.0f),
            new StructPlayerEvent(EnumPlayerEvent.Idle, 1.0f),
            new StructPlayerEvent(EnumPlayerEvent.Sit, 2.0f),
            new StructPlayerEvent(EnumPlayerEvent.Walk, 3.0f),
            new StructPlayerEvent(EnumPlayerEvent.Run, 4.0f),
            new StructPlayerEvent(EnumPlayerEvent.Jump, 5.0f)
    };   //�÷��̾� �� �̺�Ʈ�� �Ҹ� ���� ������ ���� ����
    [SerializeField] float EventTimeOver = 0.5f;                 //�÷��̾� �̺�Ʈ �Ҹ� ������ ���� ��Ÿ��
    private float _apply_EventTimeOver;                 //���� ��Ÿ������ �����ų ��

    [Space]
    [Header("���� �Ÿ� ����")]
    [SerializeField] float m_EstimateDistance = 10f;        //���Ϳ��� ���� �Ÿ�
    //[SerializeField] string m_BGM = "Horror";               //���Ͱ� ������ ������ ����Ǵ� ����
    public bool m_isPlaying = false; //����� ������ΰ�?


    [Space]
    [Header("���� �Ÿ� ����")]
    public bool CanHoldOnBreath = true; //���� ���� �� �ְ� ���ְڴ�.
    [SerializeField] float holdOnBreathTimeOut = 8.0f;    // ���� ���� �� �ִ� �ð�
    private float _holdOnBreathTimeOut;

    [Space]
    [Header("������ ����")]
    public LayerMask FlashDisturbeObject;       //�������� �� ���ع��� �Ǵ� ������Ʈ Ž���ϴ� ��.


    [Space]
    [Header("ī�޶�")]
    public bool cameraEquipState = false;           //ī�޶� ���� ���¸� ��Ÿ���ϴ�.
    public float cam_UsePerSec =0.1f;     //������ �ʴ� ���Ҿ� 0.1f;
    public float cam_TakePicture = 15;   //��� �� ���Ҿ�     3.0f; 
    public float camBattery = 100;         //���� ���͸�
    public float cam_PermanentDamageRate = 0.3f; //�Կ� �� ī�޶� ���͸� ���� �ջ� ����
    public float camPermanentDamage = 0; //���� �ջ�� ���͸�
    private CameraController _cameraController;


    //�̺�Ʈ ó���� ����.
    public StarterAssetsInputs _input;

    public float Timer;

    // �÷��̾� ������ ���� �Լ�
    //public Action Action_PlayerDead { get; set; }

    //�÷��̾�  �ٽ� �¾ ��� �ʱ�ȭ �Ǵ� �͵�
    public Action Action_Init { get; set; }


    //ī�޶� �Կ� �÷��� ��Ʈ���� �ð�
    float flashTimeout;

    AudioSource _source;//����Ҹ��� ���� ����

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


        //ī�޶� ����
        cam_UsePerSec = DataSet.Instance.GameDifficulty.BatterryUsePerSec;
        cam_PermanentDamageRate = DataSet.Instance.GameDifficulty.BatteryPermanentDamageRate;

        //������ �ð� ����
        holdOnBreathTimeOut = DataSet.Instance.GameDifficulty.HoldOnBreath;
        _holdOnBreathTimeOut = holdOnBreathTimeOut;
    }

    private void Update()
    {
        //��Ʈ�ΰ� ����ǰ� ���� ���� Ÿ�̸Ӱ� ���۵�.
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

    //�⺻ ������ �ٽ� �մϴ�.
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
    float gaspTime = 100; //����ڰ� �����⸦ �����ð�.
    bool playedBreathSound = false;
    public void CheckHoldOnBreath()
    {
        //����� ������ ���ɴϴ�.
        _holdOnBreath = _input.holdOnBreath;

        //if (!CanHoldOnBreath) _holdOnBreath = false;

        if (_holdOnBreath && CanHoldOnBreath) // �� ���� Ű�� ������.
        {
            //������ �Ҹ��� �߻���Ų��.
            if (!playedInputHold)
            {
                if (gaspTime > 2.0f)//�����⸦ ���� �ð��� 2�� �̻��� ��쿡�� �����Ѵ�.
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
                Debug.Log("�� ���� �Ѱ�ġ");
            }
        }
        else                    //������ �ð� ����
        {
            if (_holdOnBreathTimeOut < holdOnBreathTimeOut)
            {
                _holdOnBreathTimeOut += Time.deltaTime * 0.8f;
                gaspTime += Time.deltaTime;

                //������ �ð��� 0.3���� ũ�� �ٽ� ����
                if (_holdOnBreathTimeOut > holdOnBreathTimeOut * 0.3f) CanHoldOnBreath = true;

                //���� ����� �Ҹ��� �߻���Ų��.
                if (!playedUninputHold)
                {
                    if (playedBreathSound)   //�տ��� ���� �����״ٸ�, ����´�.
                    {
                        AudioManager.instance.PlayEffectiveSound("GaspRelief", 0.9f);
                        playedBreathSound = false;
                    }
                    playedUninputHold = true;
                    playedInputHold = false;
                }
            }
            else                //�� ���� �ð���  �����Ǿ� �ִٸ� �ٽ� ������ ����
            {
                _holdOnBreathTimeOut = holdOnBreathTimeOut;
                CanHoldOnBreath = true;
                gaspTime = 100;
                //Debug.Log("�� ���� ȸ��");
            }
        }

        UI.activityUI.SetBreathGage(_holdOnBreathTimeOut / holdOnBreathTimeOut, CanHoldOnBreath);


        if (!m_isPlaying)//���Ϳ� ���� ����Ҹ� �÷��� ������ ���� ���
        {
            if (_holdOnBreathTimeOut < holdOnBreathTimeOut) // ���� ������ �������� �����ϰ� ���� ���
            {
                if (!_source.isPlaying) _source.Play();
                _source.pitch = 0.8f + 1 - (_holdOnBreathTimeOut / holdOnBreathTimeOut);
                //Debug.Log("������ ���̓G�� ���� �Ҹ� ������");
            }
            else//������ �������� �������� ���
            {
                if (_source.isPlaying) _source.Stop();
                //Debug.Log("������ ������ ������� �ʴ´�.");
            }
        }
    }

    float t;
    void AroundMonster()     //���Ͱ� �ֺ��� ������, ����Ҹ��� ��Ÿ ȿ���� �����Ѵ�.
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
            return;
        }
        t = 0.1f;

        //���Ϳ� �÷��̾��� �Ÿ����� ���ɴϴ�.
        float distance = Calculator.GetPathDistance(GetPosition(), EventManager.instance.GetMonsterPosition());
        //Debug.Log($"���� �÷��̾� �̺�Ʈ�� ���� üũ ������ �Ǵ��ϰ� �ֽ��ϴ�.{m_EstimateDistance} // {distance}");

        //���� Ž�� ���� ���� ���Ͱ� �����մϴ�.
        if (distance < m_EstimateDistance)
        {
            if (!m_isPlaying)   //����� ��������� ���� ���
            {
                //Debug.Log("���� ���� ���");
                //AudioManager.instance.PlayBGMSound(m_BGM);
                m_isPlaying = true;

                //if (!_source.isPlaying) _source.Play();
            }
            EventManager.instance.Action_AroundMonster(math.clamp(1 - (distance / m_EstimateDistance), 0, 1));//�Ÿ� ���� ���� ������ �����Ѵ�.
            //_source.pitch = 1.0f + 1.0f - (distance / m_EstimateDistance);
        }
        //���Ͱ� Ž�� ���� ���� �������� �ʽ��ϴ�.
        else
        {
            //���� BGM�� ��� ���� ��� �����մϴ�.
            if (m_isPlaying)    
            {
                //AudioManager.instance.StopBGMFadeOut();
                EventManager.instance.Action_AroundMonster(0);//�Ÿ� ���� ���� ������ �����Ѵ�.

                m_isPlaying = false;
            }
        }

    }

    //����Ҹ��� ����ش�.
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

    EnumPlayerEvent _previous_playerEvent = EnumPlayerEvent.Idle;  //���� �̺�Ʈ�� ����
    //�÷��̾� ���忡 ���� �̺�Ʈ�� �������ش�.
    public void PlayerEventSound(EnumPlayerEvent _event)
    {
        //Debug.Log($"Current Player State {_event}");
        //���� �̺�Ʈ ���� �ٸ��ٸ� �ٷ� �Ҹ��� �ش��ϴ� �Ÿ����� �����Ѵ�.
        if (!_previous_playerEvent.Equals(_event))
        {
            _previous_playerEvent = _event;
            _apply_EventTimeOver = 0.0f;
            //Debug.Log($"{_event} /  ������ �ٸ� �Ҹ����� �ԷµǾ����ϴ�.");
        }
        //�Ÿ��� ������ �����̸� �Ǵ��Ѵ�.
        if (_apply_EventTimeOver > 0.0f) { return; }


        //!�߿�! ���� �÷��̾� ���¿� ���� �Ҹ� ���� ���ް�
        EventManager.instance.EventSound(transform, GetDistaceFromPlayerEvent(_event));

        // �ð�
        _apply_EventTimeOver = EventTimeOver;
    }

    

    //�� �̺�Ʈ�� �Ҹ� ������ �����´�.
    //�ش� �̺�Ʈ�� ���� ��� �Ҹ� ���� �Էµ��� �ʴ´�. = return 0
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

    //ī�޶��� ���� Y ���� ��
    public float GetViewHeight() { return _controller.CameraWrap.transform.position.y; }


    #region ----------------------------------------------�÷��̾� ����-------------------------------------------------------

    //�÷��̾ �������Ѵ�
    //������ �� ��� ���� Ƚ���� �������� �ʴ´�.
    public void PlayerFaint(Action action = null)
    {
        //������ ǥ���մϴ�.
        isDead = true;

        //�׾��� ��� �⺻���� �缳���� �����մϴ�.
        Action_Init();
        //������ ����
        FlashLightEquip(false);

        //���� ȿ���� �����ݴϴ�
        _controller.Faint(action);
    }

    //���Ϳ��� ������ DeadScene�� �Ѿ �״� ����� ���Դϴ�.
    public void PlayerDeadSecene(bool white)
    {
        if (_deadScene == null)
        {
            Debug.LogError("Player Event :: Non DeadScnenScript");
            return;
        }

        Debug.LogError("�÷��̾� ���");

        AudioManager.instance.StopBGMFadeOut();

        int remainChance = GameManager.Instance.CountingChance();

        //ī�޶� ����
        GameManager.Instance.CameraChange(1);

        _controller.SetDefaultTransform();

        //�⺻ ����
        Action_Init();
        //������ ����
        FlashLightEquip(false);

        //�׾��ٰ� �˸�
        isDead = true;

        //��ȸ ���� ����
        if (remainChance > 0)
        {
            Debug.Log("����");

            if (white)
            {
                //����
                _deadScene.PaintWhite(Spawn);
                EventManager.instance.Action_AroundMonster(1);
            }
            else
            {
                //���� ����
                _deadScene.PaintBlack(Spawn);
                EventManager.instance.Action_AroundMonster(0.3f);
                UI.activityUI.AroundBlack(1.0f);
            }
        }
        //����
        else
        {
            Debug.Log("�� ����");

            //�����
            if (!GameConfig.IsPc())
            {
                //������ ���� �һ� ��� ���� ��쿡��
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

    //������ ��ȸ�� �־�����.
    public void LastChance(bool white)
    {
        Debug.Log($" ������ ��ȸ ���� : {!lastChace}");

#if UNITY_ANDROID
        if(GoogleManager.instance.ShowAd(() => {
            if (white)
            {
                //����
                _deadScene.PaintWhite(Spawn);
                EventManager.instance.Action_AroundMonster(1);
            }
            else
            {
                //���� ����
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
        //������� ��쿡�� ���ϴ�.
        if (!GameConfig.IsPc())
        {
            UI.topUI.LastChanceActive(false);

            //���� ������, ���� �� ���� �� Ÿ��Ʋ�� �����ش�.
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

    //�÷��̾� ������Ŵ
    public void Spawn()
    {
        //ȭ�� ������
        Fade.FadeSetting(true, 0.5f, Color.black);

        //ī�޶� ��ü
        GameManager.Instance.CameraChange(0);

        //������ Ȱ��ȭ
        FlashLightEquip(true);
        
        //����� �����ֱ�
        AudioManager.instance.PlayBGMSound("AmbientScary");

        //������
        _controller.Spawn(1.0f, ShowText);
    }
    #endregion--------------------------------------------------------------------------------------


    #region -------------ī�޶�--------------

    float BatteryDelayDelta = 0.1f;
    //ī�޶� ���͸� ���� ������Ʈ
    void CameraBattery()
    {
        float PermanentDamage = (cam_TakePicture * cam_PermanentDamageRate);
        float RemainTakePictureBattery = 100.0f - camPermanentDamage;
        float RemainChance = (RemainTakePictureBattery - (15 - PermanentDamage + 1f)) / PermanentDamage;

        /*Debug.LogWarning($"PlayerEvent ���� ī�޶� ���͸� ���Ƚ�� : {RemainChance}" +
            $"\n ���� ��ȥ : {GameManager.Instance.GetRemainSouls()}");*/

        //ī�޶� ���� ���� ���
        if (cameraEquipState)
        {
            camBattery -= cam_UsePerSec*Time.deltaTime;

            //ī�޶� ���͸��� 0.1 ������ ��� ���� ����
            if (camBattery <= 0.1f)
            {
                SetCameraUnEquip();
            }
        }
        //ī�޶� ������ �ƴ� ���
        else
        {
            //�ʴ� ī�޶� ���͸� ��������
            if (camBattery < (100.0f - camPermanentDamage))
            {
                camBattery += cam_UsePerSec * 0.5f * Time.deltaTime;
            }
        }

        //UI ������Ʈ���� �ǵ����� ������
        BatteryDelayDelta += Time.deltaTime;

        if (BatteryDelayDelta > 0.1f)
        {
            //UI ������Ʈ
            UI.staticUI.SetBattery(camBattery, camPermanentDamage);
            BatteryDelayDelta = 0;
        }
    }

    //���͸� ȸ�� �޼���
    public void BatteryRecovery()
    {
        if (camPermanentDamage > 0) camPermanentDamage = Mathf.Clamp(camPermanentDamage - DataSet.Instance.GameDifficulty.CameraRecovery, 0, 100);
        AudioManager.instance.PlayEffectiveSound("CameraUnequip", 1.0f);
    }

    //ī�޶� ���� ����
    public void SetCameraUnEquip()
    {
        if (cameraEquipState) _cameraController.EquipCamera();
    }
    //ī�޶� ����
    public bool CameraEquip()
    {
        //��� �Ұ�
        if(GameManager.Instance.DontUseCamera || (camBattery < 3.0f)) return false;

        //ī�޶� ����
        _cameraController.EquipCamera();

        return true;
    }

    //ī�޶�� �Կ��Ͽ� ��ȥ�� ��õ��ŵ�ϴ�.
    RaycastHit hit;
    public void CameraTakePicture()
    {
        if (flashTimeout > 0.0f) return;
        if (camBattery < cam_TakePicture) return;

        EventManager.instance.Action_CameraFlash();// ȿ���� ����.
        AudioManager.instance.PlayEffectiveSound("CameraShutter", 1.0f);

        //ī�޶� ���͸� �뷮 ����
        camBattery -= cam_TakePicture;
        camPermanentDamage += cam_TakePicture * cam_PermanentDamageRate;

        //��õ ������ ��ȥ�� ���ɴϴ�.
        Soul soul = EventManager.instance.GetSoul(transform.position + new Vector3(0, GetViewHeight() * 0.5f, 0));
        if (soul != null)
        {
            //��ȥ �Կ��� �������� Ȯ���մϴ�.
            if (Physics.Linecast(_controller.CameraWrap.position, soul.GetPosition() + new Vector3(0, 0.8f, 0), out hit, FlashDisturbeObject))
            {
                Debug.Log($"��õ�� �����߽��ϴ�. �ε��� ��ֹ� : {hit.transform.name}");
                return;
            }

            //���ع� ������Ʈ Ž���Ȱ� ���� ��쿡�� �����Ѵ�.
            soul.Ascention();

            //�ȵ��� �Ѽ� �Ҹ�
            AudioManager.instance.PlayEffectiveSound("Sigh", 0.8f);
        }
        flashTimeout = 0.3f;



        //-----------���� ī�޶� �Կ� Ƚ���� ���մϴ�.-----------
        //��Ʈ�ΰ� ���� ������ ��쿡 �ش��Ѵ�.
        if (GameManager.Instance.GameStarted)
        {
            float PermanentDamage = (cam_TakePicture * cam_PermanentDamageRate);
            float RemainTakePictureBattery = 100.0f - camPermanentDamage;
            float RemainChance = (RemainTakePictureBattery - (15- PermanentDamage + 1f)) / PermanentDamage;
            //Debug.LogWarning($"Player ���� ī�޶� ���͸� ���Ƚ�� : {RemainChance}" +
            //    $"\n ���� ��ȥ : {GameManager.Instance.GetRemainSouls()}");

            if (GameManager.Instance.GetRemainSouls() > RemainChance)
            {
               // Debug.LogWarning($"���͸��� ��� ����߽��ϴ�. : {RemainChance}");

                isDead = true;
                Time.timeScale = 1.0f;

                UI.topUI.ShowNoSignal();
            }
        }
    }

    //���� ����
    public void ShowVideo(VideoType type)
    {
        //������� ���
        if (!GameConfig.IsPc())
        {
            UI.semiStaticUI.ShowVideo(type);
        }
        //PC�� ���
        else
        {
            _cameraController.VideoPlayer(type);
        }
    }

    #endregion
    #region---------------������ --------------------
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




    //���� ����� ���� ������ ǥ���� �ش�.
    void ShowText()
    {
        //���� ǥ��
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

    //������ ����Ʈ����
    public void GrabOff()
    {   
        _controller.GrabOff(0.0f);
    }

    //��� ���� �븮��
    public void DestroyGrabedItem()
    {
        _controller.DestroyGrabedItem();
    }

    public void SetDefault()
    {
        _controller.SetDefaultTransform();
    }
}