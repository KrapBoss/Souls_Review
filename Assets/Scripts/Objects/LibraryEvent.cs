using CustomUI;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

//도서관에서 이벤트가 발생하고 영혼의 해방과 악귀의 해방이 이루어진다.
public class LibraryEvent : MonoBehaviour
{
    //도서관 이벤트가 활성화된 상태인지를 나타냅니다.
    public static bool Activation;

    public GameObject BookShelf;
    
    public int ActiveTotemNum;//토템 활성화할 개수를 지정한다.

    //이벤트를 실행해야 되는 시간을 설정합니다.
    public float Duration = 60.0f;
    float applyDuration = 0;

    //이벤트가 진행 중인가요?
    public bool isActivation;

    //클립
    public AudioClip SoulsScream;

    //귀신
    public GameObject ghost;
    // 악귀 스폰 위치
    public Transform GhostSpawnPosition;
    //파티클
    public GameObject openParticle;

    int ActivatedTotem;//활성화된 토템의 개수를 세아린다.

    public GameObject[] go_totems;

    public Transform tf_totemLamp;
    public GameObject go_totem;

    public AudioClip clip_GasLeak;
    AudioSource source;

    //문 제어를 위한 것
    public Door[] door;

    //가스 효과
    public GameObject go_FxSmoke;
    public GameObject go_FxHard;

    public GameObject go_Paper;

    bool first, second;

    bool clear;

    short deadCount =0;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        Init();
        ghost.SetActive(false);
        openParticle.SetActive(false);
        go_Paper.SetActive(false);

        //테스팅
        //Activation = true;

        go_FxHard.SetActive(false);
    }

    private void Update()
    {
        //활성화 되어 있다면 사운드 속도를 증가
        if (isActivation)
        {
            applyDuration += Time.deltaTime;

            float ratio = (applyDuration / Duration);

            source.pitch = 0.8f + ratio;

            if(!first && ratio > 0.5f)
            {
                first = true;
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryLine1"), 4.0f);
                UI.staticUI.ShowLine();
            }
            else
                if (!second && ratio > 0.95f)
            {
                second = true;
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryLine2"), 4.0f);
                UI.staticUI.ShowLine();
            }
            else
                if(ratio > 1.0f)
            {
                StartCoroutine(GameOverCroutine());
            }
        }
    }

    //도서관 이벤트 진행 설정을 초기 상채로 만든다.
    public void Init()
    {
        go_FxSmoke.SetActive(false);

        //
        Activation = false;

        source.pitch = 1;

        first = false;
        second = false;

        //ghost.SetActive(false);
        //토템 활성화
        go_totem.transform.position = (tf_totemLamp.position);
        go_totem.transform.localRotation =  Quaternion.Euler(0, 0, 0);


        //초반 위치 설정
        BookShelf.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //현재 활성화된 토템 개수
        ActivatedTotem = 0;

        //모두 비활성화
        for (int i = 0; i < go_totems.Length; i++)
        {
            go_totems[i].GetComponent<Totem>().LightOFF();
            go_totems[i].SetActive(false);
        }

        //첫번째는 활성화
        go_totems[0].SetActive(true);

        //활성화된 영혼의 수를 가져옴
        ActiveTotemNum = EventManager.instance.GetActivationSouls();

        //지정한 개수만큼 활성화시킵니다.
        for (int i = 1; i < ActiveTotemNum; i++)
        {
            while (true)
            {
                int activeNum = Random.Range(0, go_totems.Length);
                //주소 2번 전 까지는 모두 고정형
                if (!go_totems[activeNum].activeSelf)
                {
                    go_totems[activeNum].SetActive(true);
                    break;
                }
            }
        }
    }

    //토템이 활성화 될 경우 넘겨받게 된다.
    public void ActiveTotem()
    {
        ActivatedTotem++;

        //활성화된 토템의 개수와 활성화된 토템의 개수가 같다면, 문을 연다.
        if(ActiveTotemNum == ActivatedTotem)
        {
            clear = true;

            //사운드 및 효과
            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
            EffectManager.instance.ActiveParticle("ScreenDistortion", PlayerEvent.instance.GetPosition());

            PlayerEvent.instance.Action_Init();
            PlayerEvent.instance.StopPlayerAnimation();

            source.pitch = 1;

            StartCoroutine(OpenBookShelfCroutine());
        }
    }

    //문을 개방합니다.
    IEnumerator OpenBookShelfCroutine()
    {
        //사운드 제거와 동작 중지
        source.Stop();
        isActivation = false;

        //움직임이 불가능하도록 변경
        GameManager.Instance.DontMove = true;

        //화면 가리기
        Fade.FadeSetting(true, 0.5f, Color.black);
        yield return new WaitForSeconds(0.6f);

        //커서 제거
        UI.activityUI.DeActiveCursor();

        //카메라 변경
        GameManager.Instance.CameraChange(2);

        //화면 보이기
        Fade.FadeSetting(false, 0.5f, Color.black);
        yield return new WaitForSeconds(0.6f);

        //대충 문 열리는 효과
        EventManager.instance.CameraShake(3.0f, 0.025f);
        source.Stop();


        //영혼 빠져나가는 파티클 활성화와 사운드 효과
        openParticle.SetActive(true);
        source.PlayOneShot(SoulsScream, DataSet.Instance.SettingValue.Volume *1.3f);

        float t = 0;
        while(t < 1.0f)
        {
            BookShelf.transform.localRotation = Quaternion .Euler(new Vector3(0,Mathf.Lerp(0,95,t),0));
            t += Time.deltaTime*0.333f;
            yield return null;
        }

        Debug.LogWarning("Library 이벤트가 완료되었습니다.");


        Debug.LogWarning("-----------영혼과 악마를 활성화합니다.-------------");

        openParticle.SetActive(true);
        ghost.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        //움직임이 가능하도록 설정
        GameManager.Instance.DontMove = false;
        GameManager.Instance.CameraChange(0);

        //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryLine3"), 4.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "Shift"), 5.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryClear"), 10.0f);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(1.0f);
        EventManager.instance.ActiveSouls(GhostSpawnPosition.position);

        //다음 목표 제시
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryClear"),true, 15.0f);

        //문 잠금 해제
        DoorLock(false);
        go_FxSmoke.SetActive(false);
        //AudioManager.instance.PlayEffectiveSound("JokerLaugh3", 1.0f);

        //영혼 종이 활성화
        go_Paper.SetActive(true);

        //하드 모드 시 붉은 색이 남음
        if (DataSet.Instance.SettingValue.CurrentDiff == 2)
        {
            go_FxHard.SetActive(true);
            source.PlayOneShot(clip_GasLeak, DataSet.Instance.SettingValue.Volume * 1.2f);
        }

        Activation = true;
        this.enabled = false;
    }

    //도서관 이벤트의 시작을 알립니다.
    public void EventStart()
    {
        //클리어 했을 경우 시작 안 함.
        if (clear) return;

        isActivation = true;

        //안개 활성화
        go_FxSmoke.SetActive(true);

        //1. 사운드 시작
        source.pitch = 1.0f;
        source.volume = DataSet.Instance.SettingValue.Volume;
        source.Play();

        //AudioManager.instance.PlayEffectiveSound("JokerLaugh3", 1.0f);
        AudioManager.instance.PlayEffectiveSound("JokerLaugh3", 0.8f);
        source.PlayOneShot(clip_GasLeak, DataSet.Instance.SettingValue.Volume*1.2f); 
        AudioManager.instance.PlayEffectiveSound("DoorClose_Metal", 1.5f);
        EventManager.instance.CameraShake(1.5f, 0.1f);

        //문잠금
        DoorLock(true);

        //2. 대사 시작
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", $"LibraryStart{Random.Range(0,3)}"), 4.0f);
        UI.staticUI.ShowLine();
    }

    short maxLife = 3;
    //플레이어가 이벤트 진행 도중에 죽었을 경우에 초기화를 위함.
    IEnumerator GameOverCroutine()
    {
        if (isActivation)
        {
            isActivation = false;

            //타이머 초기화
            applyDuration = 0;

            //사운드 종료
            source.Stop();

            PlayerEvent.instance.isDead = true;

            /*
            //찰스를 소환합니다.
            PlayerEvent.instance.SetCameraView(new Vector2(-30.0f,0));
            EventManager.instance.CameraShake(0.5f, 0.1f);
            ghost.SetActive(true);
            ghost.transform.position = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 0.5f;
            ghost.transform.rotation = Quaternion.LookRotation(PlayerEvent.instance.GetPosition() - ghost.transform.position);
            AudioManager.instance.PlayEffectiveSound("JokerLaugh1", 0.8f);
            AudioManager.instance.PlayEffectiveSound("HitTheHuman", 1.2f);
            yield return new WaitForSeconds(0.2f);
            */

            //마지막 목숨일 경우 마지막 대사를 표출
            if(deadCount == maxLife)
            {
                //쓰러지기 전에 대사
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryFinally"), 4.0f);
                UI.staticUI.ShowLine();
            }
            else
            {
                //쓰러지기 전에 대사
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryPaint"), 4.0f);
                UI.staticUI.ShowLine();
            }


            //삐이 소리
            AudioManager.instance.PlayEffectiveSound("Tinnitus", 0.6f);

            //화면을 붉게 물들인다.
            Fade.FadeSetting(true, 2.0f, new Color(1.0f, 0, 0));

            //플레이어 애니메이션 상태를 Idle로 변경합니다.
            PlayerEvent.instance.StopPlayerAnimation();

            yield return new WaitForSeconds(1.0f);

            //3번 죽었을 경우 타이틀 씬으로 이동
            if (deadCount == maxLife)
            {
//MobileOption
                SceneLoader.LoadLoaderScene("TitleScene");
            }
            else
            {
                //졸도 효과
                PlayerEvent.instance.PlayerFaint(DeadDelay);

                //이벤트 변수 초기화
                Init();



                //문 잠금 해제
                DoorLock(false);


                //죽은 횟수 적립
                deadCount++;
            }
        }
    }

    public void DeadDelay()
    {
        switch (deadCount)
        {
            //LibraryDead1-1
            case 1:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryDead1-1"), 3.0f);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "Lamp"), 5.0f);
                UI.staticUI.ShowLine();
                break;

            case 2:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryDead2-1"), 3.0f);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "Candle"), 5.0f);
                UI.staticUI.ShowLine();
                break;
            case 3:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryDead3-1"), 3.0f);
                UI.staticUI.ShowLine();
                break;
            default:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryFinally"), 5.0f);
                UI.staticUI.ShowLine();
                break;
        }

        ghost.SetActive(false);
    }

    //문을 잠금
    void DoorLock(bool Lock)
    {
        //문을 잠글 경우
        if (Lock)
        {
            foreach(var d in door)
            {
                //문이 열려있는 경우
                if (d.OpenCloseCheck)
                {
                    d.Func();
                }
                d.isLocked = true;
            }
        }
        //문을 열 경우
        else
        {
            foreach (var d in door)
            {
                d.isLocked = false;
            }
        }
    }
    private void OnDestroy()
    {
        Activation = false;
    }
}
