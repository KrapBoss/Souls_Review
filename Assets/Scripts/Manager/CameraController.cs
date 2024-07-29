using Cinemachine;
using CustomUI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

//플레이어가 하단에 존재하는 카메라에 대한 값을 조절합니다.
public class CameraController : MonoBehaviour
{
    Animator _anim;

    public float fov_default = 50;
    public float fov_aroundMonster = 35;

    bool _cameraEquipTimeOut = false;   //카메라 작동 오류 방지

    public GameObject go_FlashLight;

    [Header("상황에 따른 컬링 마스크")]
    public LayerMask layer_UnEquipCam;
    public LayerMask layer_EquipCam;

    public CinemachineVirtualCamera virtualCamera;

    Camera m_Cam;

    VideoController m_Player;

    //0 : Lense / 1: Video
    [SerializeField] Material[] materia;
    [SerializeField] TMP_Text video_text;

    private int[] hash_state = new int[]
    {
            Animator.StringToHash("UnEquipCamera"),//0
            Animator.StringToHash("EquipCamera"),   //1
            Animator.StringToHash("ShowVideo"),     //2
            Animator.StringToHash("HideVideo")      //3
    };


    private void Start()
    {
        m_Cam = GetComponent<Camera>();

        _anim = gameObject.GetComponentInChildren<Animator>();

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        m_Player = FindObjectOfType<VideoController>();

        //event
        EventManager.instance.Action_AroundMonster += AroundMonster;

        go_FlashLight.SetActive(false);

        //손전등을 꺼준다.
        PlayerEvent.instance.Action_Init += () =>
        {
            if (PlayerEvent.instance.cameraEquipState) EquipCamera();
            if (go_FlashLight.activeSelf) go_FlashLight.SetActive(false);
            if (PlayerEvent.instance.showVideo) { VideoPlayer(); }
        };
    }

    // 카메라의 장착
    public bool EquipCamera()
    {
        //카메라 촬영을 위한 대기시간
        if (_cameraEquipTimeOut) return false;

        _cameraEquipTimeOut = true;

        //카메라 장착에 대한 사운드와 활성 정보 애님 재생 방향을 표시한다.
        if (PlayerEvent.instance.cameraEquipState) CameraOnOff("CameraUnequip", 0f, false);
        else CameraOnOff("CameraEquip", 1f, true);
        return true;
    }

    //카메라 장착/해제
    public void CameraOnOff(string soundName, float ratioAnim, bool active)
    {
        //사운드 출력
        AudioManager.instance.PlayEffectiveSound(soundName, 1.0f);

        //카메라 활성 옵션의 경우
        if (active)
        {
            //비디오 해제
            if (PlayerEvent.instance.showVideo) VideoPlayer();
        }
        //애니메이션 재생시간 초기화
        _anim.Rebind();
        _anim.SetTrigger(hash_state[(int)ratioAnim]);

        //카메라 장착 애니메이션 대기 딜레이
        StartCoroutine(EquipCameraCroutine(ratioAnim, active));
    }

    // 일정 시간 대기 후 현재 적용된 카메라 효과를 제거한다.
    IEnumerator EquipCameraCroutine(float t, bool equip)
    {
        //아이템을 장착하고 있다면 떨어트린다.
        if (equip)
        {
            PlayerEvent.instance.GrabOff();
        }

        //카메라 애니메이션 장착 대기
        yield return new WaitUntil(() => (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= t));

        //카메라 장착
        if (equip)
        {
            //Debug.Log("카메라를 장착합니다.");
            m_Cam.cullingMask = layer_EquipCam;
        }
        //카메라 해제
        else
        {
            //Debug.Log("카메라를 해제합니다.");
            m_Cam.cullingMask = layer_UnEquipCam;
        }

        //카메라 장착 여부 판단
        PlayerEvent.instance.cameraEquipState = equip; // 카메라 장착 여부를 저장한다.
        EventManager.instance.Action_CameraEquip(equip);

        //손전등이 장착 중이라면 카메라 장착 시 해제합니다.
        if (PlayerEvent.instance.cameraEquipState)
        {
            FlashLight(false);
        }
        //카메라를 장착해제했으면 플래쉬라이트를 킵니다.
        else
        {
            if (!PlayerEvent.instance.isDead) FlashLight(true);
        }

        _cameraEquipTimeOut = false;
    }

    //비디오를 보여줄 것인가?
    public void VideoPlayer(VideoType number = VideoType.None)
    {
        //비디오를 보고 있지 않습니다.
        if (!PlayerEvent.instance.showVideo)
        {
            if (number.Equals(VideoType.None)) { Debug.LogWarning("비디오 없음"); return; }

            Debug.Log($"아이템을 장착 {number} : {PlayerEvent.instance.showVideo}");

            //카메라를 장착하고 있을 카메라를 해제합니다.
            if (PlayerEvent.instance.cameraEquipState)
            {
                EquipCamera();
            }

            //카메라 장착 소리
            AudioManager.instance.PlayEffectiveSound("CameraEquip", 1.0f);

            string txt;
            switch (number)
            {
                case VideoType.METER: // Meter
                    txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "Meter");
                    break;
                case VideoType.BALL: // Ball
                    txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "Chrystal");
                    break;
                case VideoType.MUSICBOX: //Box
                    txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "MusicBox");
                    break;
                default:
                    txt = "이런....오류가 났네? ㅋㅋㅋ 이런 제엔장!";
                    break;
            }

            video_text.text = txt;

            //장착 애니메이션
            _anim.SetTrigger(hash_state[2]);


            m_Player.Play(number);
            //비디오 플레이
            //m_Player.clip = videoClip[number];
            //m_Player.Play();
            PlayerEvent.instance.showVideo = true;
        }
        else
        {
            //비디오를 보고 있는 경우에만 작동함.
            if (!PlayerEvent.instance.showVideo) return;

            AudioManager.instance.PlayEffectiveSound("CameraUnequip", 1.0f);

            //장착해제 애니메이션
            _anim.SetTrigger(hash_state[3]);

            //비디오 플레이
            m_Player.Stop();

            PlayerEvent.instance.showVideo = false;
        }
    }




    //몬스터가 주변에 존재하면 참조받은 카메라의 FOV를 감소시킨다.
    public void AroundMonster(float t)
    {
        float _fov = fov_default - ((fov_default - fov_aroundMonster) * t);
        if (virtualCamera.m_Lens.FieldOfView != _fov)
        {
            virtualCamera.m_Lens.FieldOfView = _fov;
        }
    }

    //카메라 하단에 자식으로 있는 Light를 조정
    public void FlashLight(bool active)
    {
        //서로 원하는 상태가 다른 경우와 사용 가능한 상태일 경우에만 활성화
        if (!active.Equals(go_FlashLight.activeSelf) && !GameManager.Instance.DontUseFlashLight)
        {
            go_FlashLight.SetActive(active);

            //딸깍 소리 내주기
            AudioManager.instance.PlayEffectiveSound("LightOnOff", 1.0f);
        }
    }
}
