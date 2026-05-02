using CustomUI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 찰스가 나타나며, 주인공을 없앱니다.
/// </summary>
public class Step11 : MonoBehaviour
{
    public GameObject go_chals;
    public GameObject go_Target;
    public float angleThreshold = 10f; // 허용 오차 (5도)
    public float rotateTime = 0.5f;      //플레이어가 찰스를 보는 시간
    public float positionMultiple = 2.0f;      //찰스의 위치값
    bool activated; // 플레이어가 이벤트를 활성화 시킴
    bool looked;    // 찰스를 바라봤음

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            //너구나 이 쥐새끼가
            //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro13_Line"), 5);
            //UI.staticUI.ShowLine();

            VoiceLineManager.Instance.EnterLine(new() { clip = "chals_TutorialLibrary1", delay = 0, line = "chals_TutorialLibrary1", reverb = true });
            VoiceLineManager.Instance.ShowLine();

            //적 활성화 및 바라보는 방향 조정
            go_chals.SetActive(false);

            activated = true;
        }
    }

    private void Update()
    {
        if (activated) // 이벤트 활성화 및 찰스가 활성화 되었다면?
        {
            if (go_chals.activeSelf)
            {
                go_chals.transform.LookAt(PlayerEvent.instance.transform);
            }
        }


        if (activated && !looked)
        {
            // 플레이어가 찰스를 바라보고 있는지 판단
            Vector3 directionToB = (go_chals.transform.position - PlayerEvent.instance.transform.position).normalized;
            float angle = Vector3.Angle(PlayerEvent.instance.transform.forward, directionToB);


            if (angle <= angleThreshold)
            {
                looked = true;
                StartCoroutine(ActionCroutine());
                Debug.Log("찰스를 바라봤음");
            }
            else
            {
                Debug.Log($"찰스를 바라보고 있지 않음 {angle}");
            }
        }
    }


    IEnumerator ActionCroutine()
    {
        yield return null;

        //없나...? 대사
        VoiceLineManager.Instance.EnterLine(new() { clip = $"Intro13_Line_1", delay = 0, line = $"Intro13_Line_1", reverb = false });
        VoiceLineManager.Instance.ShowLine(true);
        //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro13_Line_1"), 5);
        //UI.staticUI.ShowLine();
        Debug.Log("찰스를 바라봤음2");

        yield return new WaitForSeconds(5.0f);

        //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro13_Line_2"), 5);
        //UI.staticUI.ShowLine();

        VoiceLineManager.Instance.EnterLine(new() { clip = "chals_TutorialLibrary2", delay = 0, line = "chals_TutorialLibrary2", reverb = true });
        VoiceLineManager.Instance.EnterLine(new() { clip = "chals_laugh2", delay = .5f, line = string.Empty, reverb = true }, nonTranslate : true);
        VoiceLineManager.Instance.ShowLine();

        //적 활성화 및 바라보는 방향 조정
        go_chals.SetActive(true);
        RenderSettings.ambientLight = Color.black;

        //yield return new WaitForSeconds(2.0f);

        PlayerEvent.instance.FlashLightEquip(false);

        //모든게 어두워진 이후 밝아진다.
        RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
        //효과음
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f, true);
        //카메라 흔들기
        EventManager.instance.CameraShake(5.0f, 0.025f);

        //플레이어를 기본 상태 및 조작 불가 상태로 변경
        PlayerEvent.instance.SetDefault();
        PlayerEvent.instance.isDead = true;
        //적 활성화 및 바라보는 방향 조정
        go_chals.SetActive(true);

        // 플레이어가 찰스를 바라봄
        Vector3 dir = go_Target.transform.position - (PlayerEvent.instance.GetPosition() + new Vector3(0, 1.0f, 0));//+ new Vector3(0, 1.0f, 0));
        Quaternion look = Quaternion.LookRotation(dir);
        //목표 회전 최종값
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);
        PlayerEvent.instance.SetCameraView(targetRot);
        Debug.Log("찰스를 바라봤음3");

        yield return new WaitForSeconds(0.25f);

        //효과음
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f, true);

        go_chals.SetActive(false);

        RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();

        yield return new WaitForSeconds(0.25f);

        Debug.Log("찰스를 바라봤음4");
        try
        {
            go_chals.SetActive(true);

            RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
            //효과음
            AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f, true);
            go_chals.transform.position = PlayerEvent.instance.transform.position + PlayerEvent.instance.transform.forward * positionMultiple;

            // 플레이어가 찰스를 바라봄
            dir = go_Target.transform.position - (PlayerEvent.instance.GetPosition() + new Vector3(0, 1.0f, 0));//+ new Vector3(0, 1.0f, 0));
            look = Quaternion.LookRotation(dir);
            //목표 회전 최종값
            targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);
            PlayerEvent.instance.SetCameraView(targetRot);
            //카메라 흔들기
            EventManager.instance.CameraShake(5.0f, 0.05f);

        }
        catch(Exception e)
        {

            GameManager.Instance.SetIntroStep(-1);
        }


        yield return new WaitForSeconds(0.2f);

        Debug.Log("찰스를 바라봤음5");

        // 플레이어 조작 활성화
        PlayerEvent.instance.isDead = false;

        //인트로 종료
        GameManager.Instance.SetIntroStep(-1);
        Debug.Log("찰스를 바라봤음6");
    }
}
