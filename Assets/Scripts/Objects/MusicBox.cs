using CustomUI;
using JetBrains.Annotations;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public interface IMusicBox
{
    //활성화하면서 활성화가 가능한 상태인지 판단한다.
    public bool ActiveMusicbox(in MusicBox musicBox= null, bool empty = false);

    //오르골이 비활성화 되었을 때
    public void DeActiveMusicbox();

    public Vector3 GetPosition();
}

public class MusicBox : GlobalSound
{
    [Space]
    [Header("AttractObject Optional")]
    public GameObject particle;

    public bool isPlay;               //오르골 활성화

    public float duration = 20.0f;    //아이템 지속시간

    public float Range = 6.0f;       //귀신 탐지 범위
    public LayerMask searchLayer;     //탐지할 귀신의 레이어

    public float PlayerRange = 15.0f;         //플레이어에게 정보를 넘길 거리값

    [Space]
    public GameObject go_cover;
    public GameObject go_deco;
    public GameObject go_hadle;
    public float rotateSpeed;
    //Soul soul = null;
    IMusicBox imusicBox;


    [Space]
    public GameObject fx_explosion;
    public GameObject go_root;          //뮤직박스 제일 상위 오브젝트
    bool destroy;

    //승천 중임을 나타냄.
    bool ascending;

    float searchTimeout = 0.0f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, Range);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, PlayerRange);
    }

    private void Update()
    {
        // 오르골이 동작중임을 나타냄.
        if (isPlay && !destroy)
        {
            go_deco.transform.Rotate(Vector3.up,rotateSpeed*Time.deltaTime*0.5f);
            go_hadle.transform.Rotate(-Vector3.left,rotateSpeed*Time.deltaTime);

            //몬스터가 주변에 존재한다면 오르골을 제거
            if(Vector3.Distance(EventManager.instance.GetMonsterPosition(), transform.position) < Range * 0.5f)
            {
                if(Calculator.GetBetweenHeight(EventManager.instance.GetMonsterPosition(), transform.position))
                {
                    if(!ascending) Explosion();
                    return;
                }
            }

            //만약 영혼이 없다면 지속적으로 탐지합니다.
            if (imusicBox == null)
            {
                searchTimeout -= Time.deltaTime;
                if (searchTimeout < 0.0f)
                {
                    SearchSoul();
                    searchTimeout = 0.1f;
                }
                return;
            }
            
            //거리를 판단하여 영혼이 오르골 범위를 벗어났다면, 비활성화시킵니다.
            if(Vector3.Distance(transform.position, imusicBox.GetPosition()) > Range*1.1f)
            {
                DeleteIMusicBox();
            }
        }
    }

    static short ActivationCount = 0;
    public override bool GrabOn(Transform parent)
    {
        //상호작용이 가능하다면 레이어를 변경
        if (!TYPE.Equals(InteractionType.Expand))
        {
            go_root.layer = 30;
            go_cover.layer = 30;
            go_hadle.layer = 30;
            go_deco.layer = 30;
        }

        //팁 보여주는 횟수 제한
        if (ActivationCount < 1)
        {
            ActivationCount+=1;

            if (GameConfig.IsPc())
            {
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "TapIsTip"), false);
            }
            else
            {
                UI.semiStaticUI.ShowVideo(VideoType.MUSICBOX);
            }
        }

        return base.GrabOn(parent);
    }

    public override bool GrabOff(Vector3 position, Vector3 force)
    {
        go_root.layer = 0;
        go_cover.layer = 0;
        go_hadle.layer = 0;
        go_deco.layer = 0;
        return base.GrabOff(position, force);
    }

    public override bool Func()
    {
        if (isPlay) return false;

        TYPE = InteractionType.None; //타입 변경으로 그랩 불가하게 한다.
        gameObject.layer = DataSet.Instance.Layers.Default; 
        if (Childs != null)
        {
            for (int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.Default;
            }
        }

        isPlay = true;

        //박스를 연다
        OpenMusicBox();

        //사운드의 페이드인과 FX 활성화한다.
        FadeInSound();
        particle.SetActive(true);

        //사운드 재생
        AudioSource _source= GetComponent<AudioSource>();
        _source.volume = DataSet.Instance.SettingValue.Volume;
        _source.loop = true; _source.Play();
        _source.maxDistance = PlayerRange;

        Destroy(GetComponent<ObjectCollsion>());

        //이벤트 매니저한테 전역 사운드 변수를 넘겨준다.
        EventManager.instance.ActiveTheHightestPrioritySound(GetObjectInfo());

        //주위 고스트 검색 코루틴 사용
        //Searchsoul();

        //일정 시간 이후 해당 아이템 삭제 //1회성 아이템
        Invoke("Explosion", duration);

        return true; // 바닥에 놓게 만든다.
    }

    void OpenMusicBox()
    {
        StartCoroutine(OpenMusicBoxCoroutine());
    }

    IEnumerator OpenMusicBoxCoroutine()
    {
        float t =0.0f;
        while (t < 1.0f)
        {
            float r = Mathf.Lerp(0, 90,t);
            float y = Mathf.Lerp(0.03f, 0.12f, t);


            go_deco.transform.localPosition = new Vector3(0,y,0);
            go_cover.transform.localRotation = Quaternion.Euler(-90+r, 0, 0);

            t += Time.deltaTime *2;
            yield return null;
        }
    }

    //자연스러운 소리 증가를 우위함.
    void FadeInSound()
    {
        StartCoroutine(FadeInSoundCoroutine());
    }

    WaitForSeconds fadeTime = new WaitForSeconds(0.1f);
    IEnumerator FadeInSoundCoroutine()
    {
        _source.volume = 0;
        while (_source.volume < 1.0f)
        {
            _source.volume += 0.1f;
            yield return fadeTime;
        }
    }

    //영혼을 탐색
    bool SearchSoul()
    {
        //주위 고스트를 탐지한다.
        Collider[] colliders;

        //영혼 탐색
        colliders = Physics.OverlapSphere(transform.position, Range, searchLayer);

        //영혼이 없으면 실패를 반환
        if (colliders.Length == 0) return false;

        for (int i =0; i < colliders.Length; i++)
        {
            //높이차이가 크다면 활성화 시킬 수 없다.
            if(Calculator.GetBetweenHeight(transform.position, colliders[i].transform.position))
            {
                //활성화 시킵니다.
                imusicBox = colliders[i].GetComponent<IMusicBox>();

                //오르골로 인한 활성화가 불가능한 상태임을 판단합니다.
                if (!imusicBox.ActiveMusicbox(this))
                {
                    imusicBox = null;
                    return false;
                }
                break;
            }
        }
        return true;
    }

    //영혼이 승천을 했습니다.
    public void AscendedSoul()
    {
        //Debug.Log("귀신이 승천하여 오르골의 기능을 다했다.");
        ascending = true;
        Explosion();
    }

    //쓰임을 다하고 파괴시킨다.
    void Explosion()
    {
        destroy = true;

        fx_explosion.SetActive(true);

        AudioManager.instance.PlayDistanceSound("BreakMusicbox", 1.0f, transform.position,true);

        EventManager.instance.EventSound(PlayerEvent.instance.transform, 50);

        go_root.SetActive(false);

        DeleteIMusicBox();

        Invoke("DestroyMusicBox", 1.0f);
    }
    //제한 시간 종료 후 제거
    void DestroyMusicBox()
    {
        StopAllCoroutines();

        EventManager.instance.DeActiveTheHightestPrioritySound(GetObjectInfo()); // 우선 순위 오브젝트 제거

        EventManager.instance.MusicBoxDestroy();

        Destroy(this.gameObject);
    }

    //영혼 참조를 제거합니다.
    void DeleteIMusicBox()
    {
        //만약 오르골이 파괴된다면 비활성화합니다.
        if (imusicBox != null) imusicBox.DeActiveMusicbox();

        imusicBox = null;
    }
}
