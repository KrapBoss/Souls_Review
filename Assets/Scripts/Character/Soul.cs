using CustomUI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Soul : MonoBehaviour, IMusicBox
{
    //가만히 있는 영혼 일 경우 활성화
    public bool DontMove;

    //-----------------------------
    [Header("영혼 주변 탐지 범위")]
    public float RangeMonster = 5.0f;

    //순회를 하게 되는 구역 이름
    [Header("순회지")]
    public string[] traversalLocationName;

    //추격 상태
    [Header("추격 상태")]
    public bool[] chaseState = new bool[3];
    public float[] chaseDelayTime = new float[3];

    [Header("기본 속도와 도망 속도")]
    public float defaultSpeed;
    public float runSpeed;

    [Header("영혼 감지 색깔")]
    public Color SoulColor = Color.yellow;

    [Header("각 상태에 따른 레이어 설정")]
    public LayerType layer_Default;
    public LayerType layer_ViewSoul;
    public LayerType layer_Ascention;

    [Header("시각적 설정")]
    public GameObject particle_Soul;
    public GameObject[] particles_Ascended;

    [Header("색을 연동시킬 파티클")]
    public ParticleSystem[] particles;

    [Header("귀신으로부터 도망갑니다.")]
    public bool ChasingByGhost;

    public AudioSource source_Whisper;
    public AudioSource source_Divine;

    //해골 모양
    public GameObject go_Soul;

    /////Private
    ///
    //목표지점
    Vector3 target;

    //Nav
    NavMeshAgent nav;

    //승천 상태
    bool Ascending = false;

    //딜레이 타임
    float deleyTime;

    //오르골 변수
    MusicBox musicbox;
    bool isEmpty; // 오르골이 비어있는지를 나타내며, 비어있을 경우 추적하지 않음.

    // 애니메이션 실행을 위함
    Animator anim;

    private void Awake()
    {
        Debug.Log($"영혼을 설정합니다.  {transform.name}");

        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        SoulSetLayer(layer_Default);

        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.MainModule module = particles[i].main;
            module.startColor = SoulColor;
        }

        for (int i = 0; i < particles_Ascended.Length; i++)
        {
            particles_Ascended[i].SetActive(false);
        }

        //움직이지 않는 영혼의 경우 관련 오브젝트를 삭제합니다.
        if (DontMove)
        {
            Destroy(nav);
        }

        //도망가는지 여부를 설정
        ChasingByGhost = DataSet.Instance.GameDifficulty.SoulRun;

        //사운드 설정
        Setting.Action_SoundChanger += SoundChange;
        SoundChange();
    }

    void Update()
    {
        if (Ascending || GameManager.Instance.DontMove || DontMove) return;

        //1. 몬스터
        if (CheckMonster(0))
        {
            //Debug.Log($"{name} :: 몬스터가 주변에 있어 도망칩니다.");
        }
        //2. 오르골
        else if (CheckMusicBox(1))
        {
            //Debug.Log($"{name} :: 오르골 추격");
        }
        //3. 순회지 지정
        else
        {
            Travarsal(2);
            //Debug.Log($"{name} :: 기본 순회");
        }


        if(target != nav.destination) nav.SetDestination(target);
    }

    //몬스터가 주변에 존재하는지 판단 후 진행합니다.
    bool CheckMonster(int index)
    {
        //귀신에게 쫓기는 것을 허용하지 않았으면, 도망치지 않는다.
        if (!ChasingByGhost)
        {
            return false;
        }

        //이미 추적 중이다.
        if (chaseState[index])
        {
            //목적지 도착을 판단
            if (ArriveCheck())
            {
                //추적 종료
                return false;
            }

            return true;
        }
        else
        {
            //몬스터와의 거리 판단
            if (Calculator.GetBetweenHeight(transform.position, EventManager.instance.GetMonsterPosition()))
            {
                if (Vector3.Distance(transform.position, EventManager.instance.GetMonsterPosition()) < RangeMonster)
                {
                    //이미 추격 상태가 아닐 경우에만 추적 대상을 업데이트 합니다.
                    target = GetMapTarget();
                    nav.speed = runSpeed;
                    ChaseStateChange(index);
                    deleyTime = chaseDelayTime[index];
                    return true;
                }
            }
        }
        
        return false;
    }

    //오르골에서의 입력이 들어온다면 실행합니다.
    bool CheckMusicBox(int index)
    {
        if (musicbox)
        {
            //추격 상태가 아닐 경우에 추격대상 지정
            if (!chaseState[index])
            {
                target = musicbox.transform.position;
                nav.speed = defaultSpeed;
                ChaseStateChange(index);
                deleyTime = chaseDelayTime[index];
            }

            //추적을 지속합니다.
            return true;
        }

        return false;
    }

    //가본 순회
    void Travarsal(int index)
    {
        // 이미 추격 중이다.
        if (chaseState[index])
        {
            //목적지에 도착을 하지 않았을 경우 추격을 지속합니다.
            if (!ArriveCheck())
            {
                return;
            }
        }

        //추격중이 아니다.
        target = GetMapTarget();
        nav.speed = defaultSpeed;
        ChaseStateChange(index);
        deleyTime = chaseDelayTime[index];
        //Debug.Log($"순회지 지정 도착 대기시간 : {deleyTime} /  ");
    }

    //추적 상태를 변경합니다.
    void ChaseStateChange(int index)
    {
        for(int i = 0; i< chaseState.Length; i++)
        {
            chaseState[i] = false;
        }

        chaseState[index] = true;
    }

    //도착을 판단합니다.
    bool ArriveCheck()
    {
        if (nav.remainingDistance <= nav.stoppingDistance)
        {
            if(deleyTime > 0.0f)
            {
                deleyTime -= Time.deltaTime;
                return false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    //맵 매니저에서 좌표를 얻어옵니다.
    Vector3 GetMapTarget()
    {
        Vector3 t = MapManager.instance.GetLocationByName(traversalLocationName[Random.Range(0, traversalLocationName.Length)]);
        //Debug.Log($"순회지 지정 도착 대기시간 : {deleyTime} /  {t}");
        return t;
    }


    //해당 영혼이 성불이 가능한 상태인지 반환
    public bool CanAscention()
    {
        if (chaseState[0] || Ascending) return false;
        else return true;
    }


    // 승천
    public bool Ascention()
    {
        //승천 불가능하면 실행하지 않음
        if (!CanAscention()) return false;
        //승천이 가능할 경우 실행
        else
        {
            Debug.Log("승천합니다.");

            go_Soul.SetActive(false);

            //1, 승천 애니메이션
            particle_Soul.transform.localRotation = Quaternion.LookRotation(particle_Soul.transform.forward, Vector3.up);
            anim.SetTrigger("Ascension");//승천 애니메이션 시작
            
            //#승천 효과
            for(int i = 0;i < particles_Ascended.Length; i++)
            {
                particles_Ascended[i].SetActive(true);
            }

            //사운드 재생
            source_Divine.volume =DataSet.Instance.SettingValue.Volume;
            source_Divine.Play();
            //source.clip = clip_Ascention.clip;
            //source.volume = clip_Ascention.volume * DataSet.Instance.SettingValue.Volume;
            //source.Play();

            //2. 추적 비활성화 및 승천 시에도 모습이 보이도록 하기
            Ascending = true;

            //3. 오르골 파괴
            if(!isEmpty) musicbox.AscendedSoul();

            //4. 이벤트 매니저에서 제거
            EventManager.instance.DeleteAscentionSoul(this);

            //5. 영혼개수 표시
            GameManager.Instance.Ascention();

            //6. 카메라 끄기
            PlayerEvent.instance.SetCameraUnEquip();

            //7. 승천 효과
            EffectManager.instance.ActiveParticle("UpStar", transform.position);

            //삭제
            Invoke("Destroy", 5.0f);

            return true;
        }
    }

    #region ------------For MusicBox------------
    //MusicBox를 위한 인터페이스
    public bool ActiveMusicbox(in MusicBox _musicBox = null , bool empty = false)
    {
        //승천이 가능한 상태이므로 상태를 변경합니다.
        if (CanAscention())
        {
            Debug.Log($"영혼 오르골이 활성화되었습니다.");
            //승천 시 사용하기 위한 지정

            isEmpty = empty;
            if (!isEmpty) musicbox = _musicBox;

            //카메라로 볼 수 있는 레이어로 변경
            SoulSetLayer(layer_ViewSoul);

            //이벤트 매니저에 추가
            EventManager.instance.SaveAscentionSoul(this);

            return true;
        }
        //승천이 불가능한 상태
        else
        {
            Debug.Log($"오르골이 활성화되지 못했습니다.");
            return false;
        }
    }

    //오르골로 과의 연결을 끊는다.
    public void DeActiveMusicbox()
    {
        Debug.Log($"오르골이 DeActiveMusicbox()");

        //레이어 변경
        SoulSetLayer(layer_Default);

        // 이벤트 매니저에서 제거
        EventManager.instance.DeleteAscentionSoul(this);

        //참조 제거
        musicbox = null;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
    #endregion

    //보여지는 영혼의 레이어 변경
    void SoulSetLayer(LayerType _layer)
    {
        if (!Ascending)
        {
            particle_Soul.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(_layer);
            foreach (var go in particle_Soul.GetComponentsInChildren<Transform>())
            {
                go.gameObject.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(_layer);
            }
        }
    }

    void Destroy()
    {
        Debug.LogWarning($"{name} 영혼이 제거됩니다.");

        if(GameManager.Instance.GetRemainSouls() > 0)
        {
            string s = $"<color=#{ColorUtility.ToHtmlStringRGB(SoulColor)}>{LocalLanguageSetting.Instance.GetLocalText("Story", $"Soul_Line{Random.Range(1, 6)}")}</color>";
            UI.staticUI.EnterLine(s, 5.0f);
            UI.staticUI.ShowLine();
        }

        Destroy(gameObject);
    }

    public void SoundChange()
    {
        Debug.LogWarning($"{name} 영혼 소리 설정 등록.");
        source_Whisper.volume = DataSet.Instance.SettingValue.Volume * 0.8f;
    }

    private void OnDestroy()
    {
        Setting.Action_SoundChanger -= SoundChange;
        Debug.LogError("제거 Soul " + name);
    }
}