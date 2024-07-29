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

    //각 색상들
    public Color color_PointClub;
    public Color color_PointWaving;

    public GameObject go_Fx;
    public GameObject go_Fx2;

    public override bool Func(string name = null)
    {
        gameObject.layer = 0;

        //모든 설정 끄기
        PlayerEvent.instance.Init();
        PlayerEvent.instance.isDead = true;
        GameManager.Instance.DontUseCamera = true;
        GameManager.Instance.DontUseFlashLight = true;
        GameManager.Instance.DontUseGhostBall = true;
        GameManager.Instance.DontUseGhostMeter = true;
        //충돌체 들 활성화
        cs_EndingDance.Colliders.SetActive(true);

        //시간 흐르도록 설정
        Time.timeScale = 1.0f;

        //최초 크레딧
        UI.staticUI.EnterLine("<color=#2222ff>Thank you for Playing</color>", 15.0f);
        UI.staticUI.ShowLine();

        go_Fx.SetActive(false);
        go_Fx2.SetActive(true);

        //탁 소리와 함께 불을 끈다.
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        cs_EndingDance.light_Spot.enabled = false;
        cs_EndingDance.light_Point.enabled = false;

        StartCoroutine(MuseCroutine());

        return true;
    }

    IEnumerator MuseCroutine()
    {
        //캐릭터들 활성화
        cs_EndingDance.go_Characters.SetActive(true);
       

        yield return new WaitForSeconds(2.0f);

        //파티클 활성화


        //움직임 활성화
        PlayerEvent.instance.isDead = false;
        //탁
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        //빛 활성화
        cs_EndingDance.light_Point.enabled = true;
        cs_EndingDance.ChangeColor(color_PointWaving);
        //인사
        cs_EndingDance.PlayAnimation("Waving");
        yield return new WaitForSeconds(3.0f);

        //탁
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        cs_EndingDance.light_Point.enabled = false;

        //AudioManager.instance.PlayEffectiveSound("JokerLaugh1", 0.7f);

        //배경음
        source_15.volume = DataSet.Instance.SettingValue.BGMVoulme * 1.3f;
        source_15.Play();

        //yield return new WaitForSeconds(.0f);

        //노래 시작
        //source.volume = DataSet.Instance.SettingValue.Volume * 0.8f;
        //source.Play();

        yield return new WaitForSeconds(0.5f);

        source.Stop();

        //빛 변경
        cs_EndingDance.light_Point.enabled = true;
        cs_EndingDance.ChangeColor(color_PointClub);
        //탁
        source.PlayOneShot(clip_Tak, DataSet.Instance.SettingValue.Volume * 1.2f);
        //춤 시작
        cs_EndingDance.PlayAnimation("Dance");
    }
}
