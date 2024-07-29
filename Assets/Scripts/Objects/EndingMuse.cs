using CustomUI;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndingMuse : ExpandObject
{
    public AudioSource source;
    public AudioSource source_15;

    public EndingDance cs_EndingDance;

    public AudioClip clip_Tak;
    public AudioClip clip_Music;

    //�� �����
    public Color color_PointClub;
    public Color color_PointWaving;

    public GameObject go_Fx;
    public GameObject go_Fx2;

    public override bool Func(string name = null)
    {
        gameObject.layer = 0;

        //��� ���� ����
        PlayerEvent.instance.Init();
        PlayerEvent.instance.isDead = true;
        GameManager.Instance.DontUseCamera = true;
        GameManager.Instance.DontUseFlashLight = true;
        GameManager.Instance.DontUseGhostBall = true;
        GameManager.Instance.DontUseGhostMeter = true;
        //�浹ü �� Ȱ��ȭ
        cs_EndingDance.Colliders.SetActive(true);

        //�ð� �帣���� ����
        Time.timeScale = 1.0f;

        //���� ũ����
        UI.staticUI.EnterLine("<color=#2222ff>Thank you for Playing</color>", 15.0f);
        UI.staticUI.ShowLine();

        go_Fx.SetActive(false);
        go_Fx2.SetActive(true);

        //Ź �Ҹ��� �Բ� ���� ����.
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        cs_EndingDance.light_Spot.enabled = false;
        cs_EndingDance.light_Point.enabled = false;

        StartCoroutine(MuseCroutine());

        return true;
    }

    IEnumerator MuseCroutine()
    {
        //ĳ���͵� Ȱ��ȭ
        cs_EndingDance.go_Characters.SetActive(true);
       

        yield return new WaitForSeconds(2.0f);

        //��ƼŬ Ȱ��ȭ


        //������ Ȱ��ȭ
        PlayerEvent.instance.isDead = false;
        //Ź
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        //�� Ȱ��ȭ
        cs_EndingDance.light_Point.enabled = true;
        cs_EndingDance.ChangeColor(color_PointWaving);
        //�λ�
        cs_EndingDance.PlayAnimation("Waving");
        yield return new WaitForSeconds(3.0f);

        //Ź
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        cs_EndingDance.light_Point.enabled = false;

        //AudioManager.instance.PlayEffectiveSound("JokerLaugh1", 0.7f);

        //�����
        source_15.volume = DataSet.Instance.SettingValue.BGMVoulme * 1.3f;
        source_15.Play();

        //yield return new WaitForSeconds(.0f);

        //�뷡 ����
        //source.volume = DataSet.Instance.SettingValue.Volume * 0.8f;
        //source.Play();

        yield return new WaitForSeconds(0.5f);

        source.Stop();

        //�� ����
        cs_EndingDance.light_Point.enabled = true;
        cs_EndingDance.ChangeColor(color_PointClub);
        //Ź
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        //�� ����
        cs_EndingDance.PlayAnimation("Dance");
    }
}
