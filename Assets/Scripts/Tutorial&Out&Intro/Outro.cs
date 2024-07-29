using Cinemachine;
using CustomUI;
using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

//마지막 엔딩 씬
public class Outro : MonoBehaviour
{
    [SerializeField] Animator animator_Ghost;
    [SerializeField] Animator animator_Door;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] GameObject hat;

    //마지막
    [SerializeField] Transform demon;

    [SerializeField] GameObject[] particles;

    //아웃트로 시작 시 제거할 오브젝트
    [SerializeField] GameObject[] destroyItem;

    [SerializeField] AudioClip Devil1;
    [SerializeField] AudioClip CloseDoor;
    [SerializeField] AudioClip Joker1;
    [SerializeField] AudioClip BGM;
    AudioSource source;

    EndingDance cs_EndingDance;

    private void Start()
    {
        hat.SetActive(false);
        demon.gameObject.SetActive(false);

        source = GetComponent<AudioSource>();

        for (int i = 0; i < particles.Length; i++) { particles[i].SetActive(false); }

        cs_EndingDance = FindObjectOfType<EndingDance>();
        cs_EndingDance.gameObject.SetActive(false);
    }

    public void OutroStart(Action endAction = null)
    {
        //아이템 및 카메라 손전등 제거
        PlayerEvent.instance.Init();

        //Fade
        Fade.FadeSetting(true, 4.0f, Color.black);

        //데몬 활성
        demon.gameObject.SetActive(true);

        //모든 귀신 비활성화
        EventManager.instance.DeActiveGhost();

        //엔딩 댄스 활성화합니다.
        cs_EndingDance.gameObject.SetActive(true);
        cs_EndingDance.Init();

        StartCoroutine(OutroCoutine(endAction));
    }

    IEnumerator OutroCoutine(Action endAction)
    {
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(false);
        }

        yield return new WaitUntil(() => Fade.b_faded);
        //카메라 변경
        GameManager.Instance.CameraChange(3);


        //페이드 아웃 대기
        Fade.FadeSetting(false, 1.0f, Color.black);

        for (int i = 0; i < destroyItem.Length; i++)
        {
            Destroy(destroyItem[i]);
        }

        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "Outtro1"), 5.0f);
        UI.staticUI.ShowLine();


        //오르골 스포너 제거
        EventManager.instance.DestroyMusicboxSpawner();

        source.volume = DataSet.Instance.SettingValue.Volume* 0.7f;
        source.clip = BGM;
        source.Play();

        //애니메이션 재생시간 초기화
        animator_Ghost.Rebind();
        animator_Ghost.SetTrigger("Step1");
        Debug.Log("Ending : Step1 : 앞으로 진행한다.");

        particles[0].SetActive(true);
        // 애니메이션  대기
        yield return new WaitUntil(() => (animator_Ghost.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));


        source.PlayOneShot(Joker1, DataSet.Instance.SettingValue.Volume*1.5f);

        //애니메이션 재생시간 초기화
        animator_Ghost.Rebind();
        animator_Ghost.SetTrigger("Step2");
        Debug.Log("Ending : Step2 : 손을 뻗는다.");

        // 애니메이션  대기
        yield return new WaitUntil(() => (animator_Ghost.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));
        particles[1].SetActive(true);
        yield return new WaitForSeconds(0.5f);


        //애니메이션 재생시간 초기화
        animator_Ghost.Rebind();
        animator_Ghost.SetTrigger("Step3");
        Debug.Log("Ending : Step3 : 손을 뻗는다.");
        // 애니메이션  대기
        yield return new WaitUntil(() => (animator_Ghost.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));
        yield return new WaitForSeconds(1.0f);


        animator_Door.Rebind();
        animator_Door.SetTrigger("Close");
        Debug.Log("Ending : Close Door");
        source.PlayOneShot(CloseDoor, DataSet.Instance.SettingValue.Volume* 1.5f);
        yield return new WaitUntil(() => (animator_Door.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));

        //애니메이션 비활성화
        animator_Door.enabled = false;
        animator_Ghost.enabled = false;

        //파티클 비활성화
        for (int i = 0; i < particles.Length; i++) { particles[i].SetActive(false); }

        //마지막 위치 설정
        hat.SetActive(true);
        virtualCamera.transform.parent = transform;
        demon.localPosition = new Vector3(1.09000003f, -0.901530862f, 0.899999976f);
        virtualCamera.transform.localPosition = new Vector3(1.24199963f, 0.947000027f, 0.381999969f);
        virtualCamera.transform.localRotation = Quaternion.Euler(21.4896736f, 286.150482f, 6.06662893f);
        hat.GetComponent<Rigidbody>().AddForce(Vector3.right * 8.0f, ForceMode.Impulse);
        hat.GetComponent<Rigidbody>().AddTorque(Vector3.right * 20.0f, ForceMode.Impulse);
        source.PlayOneShot(Devil1, DataSet.Instance.SettingValue.Volume * 1.4f);
        yield return new WaitForSeconds(4.0f);

        //플레이어 시점 전환
        GameManager.Instance.CameraChange(0);
        PlayerEvent.instance.isDead = false;
        GameManager.Instance.DontMove = false;
        
        //마지막 대사
        AudioManager.instance.PlayEffectiveSound("Sigh", 1.5f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "End"), 7.0f);
        UI.staticUI.ShowLine();

        //모바일 컨트롤러 활성화
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(true);
        }

        //마지막 공지 지문
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "SoulsClear"),true, 15.0f);

        //음악끄기
        source.Stop();

        //마지막 동작
        if (endAction != null) endAction();
    }

    IEnumerator Shaking(float n)
    {
        Vector3 m_position = virtualCamera.gameObject.transform.position;

        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        float time = 0.0f;
        while (time < n)
        {
            float t = Random.Range(-0.1f, 0.1f);
            virtualCamera.transform.position = m_position + new Vector3(-t, t, t);
            yield return waitTime;

            time += 0.1f;
        }
    }
}
