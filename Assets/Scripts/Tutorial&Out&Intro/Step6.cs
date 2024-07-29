using CustomUI;
using System.Collections;
using UnityEngine;

/// <summary>
/// 조커가 플레이어를 덮칩니다.
/// </summary>
public class Step6 : MonoBehaviour
{
    //public GameObject Character;
    //public Transform Neck;
    //public Vector3[] NeckRotZ = new Vector3[2];

    //public AudioSource source;

    //[Header("두번째 계획")]
    //public GameObject go_Joker;
    //public AudioClip clip_Fx;

    //public Rigidbody rig_Hat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //모자를 떨어트림
            //rig_Hat.gameObject.SetActive(true);
            //rig_Hat.AddForce(Vector3.forward, ForceMode.Impulse);

            //배경음
            //source.volume = DataSet.Instance.SettingValue.Volume * 0.7f;
            //source.Play();

            //동작 방어
            GetComponent<BoxCollider>().enabled = false;

            //움직임 실시
            StartCoroutine(RotateCroutine());

            /*
            Character.SetActive(true);
            PlayerEvent.instance.StopPlayerAnimation();

            success = true;

            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f,true);
            source.Play();

            //카메라 켜져있으면 사용불가하게 만들기
            if (PlayerEvent.instance.cameraEquipState) FindObjectOfType<CameraController>().CameraOnOff("CameraUnequip", 0f, false);

            PlayerEvent.instance.isDead = true;//조작이 불가하게 만들기 위함.

            StartCoroutine(RotateCroutine());

            eventCollider.enabled = false;
            */
        }
    }

    IEnumerator RotateCroutine()
    {
        //대사 지연
        yield return new WaitForSeconds(2.0f);

        //대사 : 아우...뭐 별거 없네.?
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6_Line"), 7);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6-1_Line"), 7);
        UI.staticUI.ShowLine();
        AudioManager.instance.PlayEffectiveSound("Sigh", 1.0f);

        //잠시 대기 -> 집안을 확인
        yield return new WaitForSeconds(10.0f);

        //알림 띄워주기
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6-1_Line_Tip"),true, 15.0f);//사운드
        AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.0f);

        //다음 스텝
        GameManager.Instance.SetIntroStep(7);

        //까꿍 소리
        //source.PlayOneShot(clip_Fx, DataSet.Instance.SettingValue.Volume);

        //시간차 공격
        //yield return new WaitForSeconds(2.0f);

        /*
        //기본 위치로 수정
        PlayerEvent.instance.SetDefault();

        //조작이 불가하게 만들기 위함.
        PlayerEvent.instance.isDead = true;

        //조커 활성
        go_Joker.SetActive(true);

        //조커를 플레이어 뒤로 이동
        go_Joker.transform.position = PlayerEvent.instance.GetPosition() +
            (-PlayerEvent.instance.transform.forward * .25f);

        //조커를 플레이어를 바라보게 하기
        Vector3 joker_dir = PlayerEvent.instance.GetPosition() - go_Joker.transform.position;
        go_Joker.transform.rotation = Quaternion.LookRotation(joker_dir);

        //현재 플레이어의 보는 각도
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();

        //대상을 향한 방향을 가져옵니다.
        Vector3 dir = go_Joker.transform.position - PlayerEvent.instance.GetPosition();
        Quaternion look = Quaternion.LookRotation(dir);

        //목표 회전 최종값
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);

        //회전
        float t = 0;
        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }
        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        //효과음
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", .8f);


        //퍽 사운드
        AudioManager.instance.PlayEffectiveSound("HitTheHuman", 1.5f);

        //삐이 소리
        AudioManager.instance.PlayEffectiveSound("Tinnitus", 1.0f);

        //화면 물들인다.
        Fade.FadeSetting(true, 2.0f, new Color(0.4f, 0, 0));

        //시간차
        yield return new WaitForSeconds(1.0f);

        //조작이 불가하게 만들기 위함.
        PlayerEvent.instance.isDead = false;

        //인트로 다음
        GameManager.Instance.SetIntroStep(7);
        */


        /*
        float t = 0;
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();
        Vector2 targetRot = new Vector2(0, 180.0f);

        AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.2f);
        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
        EventManager.instance.CameraShake(0.5f, 0.1f);

        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }

        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.2f);

        Character.transform.localPosition = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 4f;
        Neck.position = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 3.8f;
        Neck.position = new Vector3(Neck.position.x, PlayerEvent.instance.GetViewHeight() + 0.1f, Neck.position.z);
        Neck.transform.localRotation = Quaternion.Euler(NeckRotZ[0]);

        EventManager.instance.CameraShake(4.8f, 0.025f);
        PlayerEvent.instance.isDead = false;//조작 가능하도록 변경


        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "Tutorial2"), 5);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(5.0f);

        PlayerEvent.instance.StopPlayerAnimation();
        PlayerEvent.instance.isDead = true;//조작이 불가하게 만들기 위함.
        //변수 초기화
        t = 0;
        currentRot = PlayerEvent.instance.GetRotationCameraView();
        targetRot = new Vector2(0, 180.0f);


        EventManager.instance.CameraShake(0.5f, 0.1f);

        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }

        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.2f);
        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
        AudioManager.instance.PlayEffectiveSound("Gasp", 1.0f);
        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        Character.transform.localPosition = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 0.5f;
        Neck.position = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 0.35f;
        Neck.position = new Vector3(Neck.position.x, PlayerEvent.instance.GetViewHeight(), Neck.position.z);
        Neck.transform.localRotation = Quaternion.Euler(NeckRotZ[1]);

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.SetIntroStep(7);
        */
    }
}