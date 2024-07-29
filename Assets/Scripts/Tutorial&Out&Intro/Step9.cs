
using CustomUI;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 집 밖으로 나가지 못하게 구울들이 집안에 잡아둡니다.
/// </summary>

public class Step9 : ExpandObject
{
    [Space]
    [Header("For Trigger")]
    public GameObject go_Target;

    public GameObject go_Goul1;
    public GameObject go_Goul2;

    public AudioClip clip_Locked;

    public BoxCollider col_TutorialDoor;

    private void OnEnable()
    {
        col_TutorialDoor.enabled = false;
        go_Goul1.SetActive(false);
        go_Goul2.SetActive(false);
    }

    IEnumerator ActionCroutine()
    {
        //구울 사운드
        source.volume = DataSet.Instance.SettingValue.Volume;
        source.Play();

        //대사 : 하아... 인생 ㅆ.....
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro9_Line"), 4, true);
        UI.staticUI.ShowLine();

        go_Goul1.SetActive(true);

        yield return new WaitForSeconds(4.0f);

        //카메라 흔들기
        EventManager.instance.CameraShake(6.0f, 0.1f);
        //효과음
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f);

        ///못 움직임
        PlayerEvent.instance.isDead = true;

        Vector3 currentRot = PlayerEvent.instance.GetRotationCameraView();
        //대상을 향한 방향을 가져옵니다.
        Vector3 dir = go_Target.transform.position - PlayerEvent.instance.GetPosition();
        Quaternion look = Quaternion.LookRotation(dir);
        //목표 회전 최종값
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);

        //회전
        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime * 5;
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            yield return null;
        }

        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro9-1_Line"), 4, true);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(1.5f);

        //한숨 받아들임.
        AudioManager.instance.PlayEffectiveSound("Sigh2", 1.0f);
        
        yield return new WaitForSeconds(1.0f);


        Fade.FadeSetting(true, 0.1f, Color.black);
        yield return new WaitForSeconds(0.15f);

        //구을이 달려듭니다.
        go_Goul2.SetActive(true);
        go_Goul1.SetActive(false);

        //UI 상호작용 가능 없애기
        UI.activityUI.SetInteractionUI(Vector3.zero, false);
        
        Fade.FadeSetting(false, 0.1f, Color.black);

        //구울이 달려듬과 함께 죽습니다.
        go_Goul2.GetComponent<NavMeshAgent>().SetDestination(PlayerEvent.instance.GetPosition());
        AudioManager.instance.PlayEffectiveSound("GhostScream",1.0f);

        yield return new WaitForSeconds(0.12f);

        Fade.FadeSetting(true, 1.0f, Color.black);

        yield return new WaitForSeconds(1.2f);

        GameManager.Instance.SetIntroStep(10);
    }

    AudioSource source;
    bool activation = false;
    public override bool Func(string name = null)
    {
        //사운드 // 문 잠긴 소리
        source = GetComponent<AudioSource>();
        source.PlayOneShot(clip_Locked, DataSet.Instance.SettingValue.Volume);

        //카메라 흔들기
        EventManager.instance.CameraShake(0.5f, 0.1f);

        //이벤트 제한
        if (activation) return true;

        activation = true;

        Debug.Log("Step 9 Start");

        StartCoroutine(ActionCroutine());

        return true;
    }
}
