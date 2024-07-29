using Cinemachine;
using CustomUI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

//�÷��̾ �ϴܿ� �����ϴ� ī�޶� ���� ���� �����մϴ�.
public class CameraController : MonoBehaviour
{
    Animator _anim;

    public float fov_default = 50;
    public float fov_aroundMonster = 35;

    bool _cameraEquipTimeOut = false;   //ī�޶� �۵� ���� ����

    public GameObject go_FlashLight;

    [Header("��Ȳ�� ���� �ø� ����ũ")]
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

        //�������� ���ش�.
        PlayerEvent.instance.Action_Init += () =>
        {
            if (PlayerEvent.instance.cameraEquipState) EquipCamera();
            if (go_FlashLight.activeSelf) go_FlashLight.SetActive(false);
            if (PlayerEvent.instance.showVideo) { VideoPlayer(); }
        };
    }

    // ī�޶��� ����
    public bool EquipCamera()
    {
        //ī�޶� �Կ��� ���� ���ð�
        if (_cameraEquipTimeOut) return false;

        _cameraEquipTimeOut = true;

        //ī�޶� ������ ���� ����� Ȱ�� ���� �ִ� ��� ������ ǥ���Ѵ�.
        if (PlayerEvent.instance.cameraEquipState) CameraOnOff("CameraUnequip", 0f, false);
        else CameraOnOff("CameraEquip", 1f, true);
        return true;
    }

    //ī�޶� ����/����
    public void CameraOnOff(string soundName, float ratioAnim, bool active)
    {
        //���� ���
        AudioManager.instance.PlayEffectiveSound(soundName, 1.0f);

        //ī�޶� Ȱ�� �ɼ��� ���
        if (active)
        {
            //���� ����
            if (PlayerEvent.instance.showVideo) VideoPlayer();
        }
        //�ִϸ��̼� ����ð� �ʱ�ȭ
        _anim.Rebind();
        _anim.SetTrigger(hash_state[(int)ratioAnim]);

        //ī�޶� ���� �ִϸ��̼� ��� ������
        StartCoroutine(EquipCameraCroutine(ratioAnim, active));
    }

    // ���� �ð� ��� �� ���� ����� ī�޶� ȿ���� �����Ѵ�.
    IEnumerator EquipCameraCroutine(float t, bool equip)
    {
        //�������� �����ϰ� �ִٸ� ����Ʈ����.
        if (equip)
        {
            PlayerEvent.instance.GrabOff();
        }

        //ī�޶� �ִϸ��̼� ���� ���
        yield return new WaitUntil(() => (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= t));

        //ī�޶� ����
        if (equip)
        {
            //Debug.Log("ī�޶� �����մϴ�.");
            m_Cam.cullingMask = layer_EquipCam;
        }
        //ī�޶� ����
        else
        {
            //Debug.Log("ī�޶� �����մϴ�.");
            m_Cam.cullingMask = layer_UnEquipCam;
        }

        //ī�޶� ���� ���� �Ǵ�
        PlayerEvent.instance.cameraEquipState = equip; // ī�޶� ���� ���θ� �����Ѵ�.
        EventManager.instance.Action_CameraEquip(equip);

        //�������� ���� ���̶�� ī�޶� ���� �� �����մϴ�.
        if (PlayerEvent.instance.cameraEquipState)
        {
            FlashLight(false);
        }
        //ī�޶� �������������� �÷�������Ʈ�� ŵ�ϴ�.
        else
        {
            if (!PlayerEvent.instance.isDead) FlashLight(true);
        }

        _cameraEquipTimeOut = false;
    }

    //������ ������ ���ΰ�?
    public void VideoPlayer(VideoType number = VideoType.None)
    {
        //������ ���� ���� �ʽ��ϴ�.
        if (!PlayerEvent.instance.showVideo)
        {
            if (number.Equals(VideoType.None)) { Debug.LogWarning("���� ����"); return; }

            Debug.Log($"�������� ���� {number} : {PlayerEvent.instance.showVideo}");

            //ī�޶� �����ϰ� ���� ī�޶� �����մϴ�.
            if (PlayerEvent.instance.cameraEquipState)
            {
                EquipCamera();
            }

            //ī�޶� ���� �Ҹ�
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
                    txt = "�̷�....������ ����? ������ �̷� ������!";
                    break;
            }

            video_text.text = txt;

            //���� �ִϸ��̼�
            _anim.SetTrigger(hash_state[2]);


            m_Player.Play(number);
            //���� �÷���
            //m_Player.clip = videoClip[number];
            //m_Player.Play();
            PlayerEvent.instance.showVideo = true;
        }
        else
        {
            //������ ���� �ִ� ��쿡�� �۵���.
            if (!PlayerEvent.instance.showVideo) return;

            AudioManager.instance.PlayEffectiveSound("CameraUnequip", 1.0f);

            //�������� �ִϸ��̼�
            _anim.SetTrigger(hash_state[3]);

            //���� �÷���
            m_Player.Stop();

            PlayerEvent.instance.showVideo = false;
        }
    }




    //���Ͱ� �ֺ��� �����ϸ� �������� ī�޶��� FOV�� ���ҽ�Ų��.
    public void AroundMonster(float t)
    {
        float _fov = fov_default - ((fov_default - fov_aroundMonster) * t);
        if (virtualCamera.m_Lens.FieldOfView != _fov)
        {
            virtualCamera.m_Lens.FieldOfView = _fov;
        }
    }

    //ī�޶� �ϴܿ� �ڽ����� �ִ� Light�� ����
    public void FlashLight(bool active)
    {
        //���� ���ϴ� ���°� �ٸ� ���� ��� ������ ������ ��쿡�� Ȱ��ȭ
        if (!active.Equals(go_FlashLight.activeSelf) && !GameManager.Instance.DontUseFlashLight)
        {
            go_FlashLight.SetActive(active);

            //���� �Ҹ� ���ֱ�
            AudioManager.instance.PlayEffectiveSound("LightOnOff", 1.0f);
        }
    }
}
