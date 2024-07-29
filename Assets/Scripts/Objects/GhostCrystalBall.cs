using CustomUI;
using UnityEngine;
/// <summary>
/// 주변 Layer : Ghost를 탐색한다.
/// 탐색한 고스트 중 가장 가까운 고스트를 가져와서 고스트의 색깔과 효과를 표시한다
/// 거리가 가까울 수록 색과 효과를 추가한다.
/// 만약 탐색되는 오브젝트가 없다면 기본 색을 지정한다.
/// 스스로 지속적으로 발현되는 오브젝트이다.
/// </summary>
public class GhostCrystalBall : EquipAutoObject
{    
    
    //영혼 탐색을 위한 데이터입니다.
    class SearchData
    {
        public Color Color;
        public float DistanceA;
        public void Init()
        {
            Color = Color.grey;
            DistanceA = 0.0f;        //탐지된 몬스터의 거리 // 0~1f 의 알파 비율
        }
        public void Set(float disA, Color c)
        {
            DistanceA = disA;
            c.a = 0.3f + DistanceA * 0.7f;//기본적으로 거리에 따라 눈에 보이게 하는 값을 지정
            Color = c;
        }
    }
    SearchData data = new SearchData();
    //영혼사이 최소 거리값
    float MinDistance;


    [Header("고스트 탐지 변수")]
    [Tooltip("탐지 범위별로 파티클 시스템을 적용할 것들")]
    public ParticleSystem[] ParticlesStep;
    public Light ParticleLight;

    [Tooltip("탐지 범위")]
    public float range;

    [Tooltip("탐지할 레이어")]
    public LayerMask layer;

    //탐지 적용시간
    public float searchTimeout = 0.1f;
    float applySearchTimeout;

    AudioSource source;

    ParticleSystem.MainModule mainModule;

    //활성화를 시킨 횟수를 지정합니다.
    short ActivationCount = 0;

    public override bool GrabOn(Transform parent)
    {
        //팁 보여주는 횟수 제한
        if (ActivationCount < 1)
        {
            ActivationCount++;
            if (GameConfig.IsPc())
            {
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "TapIsTip"), false);
            }
            else
            {
                UI.semiStaticUI.ShowVideo(VideoType.BALL);
            }
        }

        //지속시간이 0.5f 이하일 경우 그랩하지 못한다.
        if (applyDuration < duration * 0.5f)
        {
            AudioManager.instance.PlayEffectiveSound("Beep", 1.0f);
            return false;
        }

        applySearchTimeout = 0.0f; // 타임머 초기화

        ParticleLight.enabled = true;


        data.Init();

        //탐색
        SearchObject();
        //색 지정
        ParticleActive(); 
        //초기화
        applySearchTimeout = searchTimeout;


        GRAB = true;

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public override bool GrabOff(Vector3 position, Vector3 force)
    {
        //파티클 제거
        for(int i = 0; i < ParticlesStep.Length; i++)
        {
            ParticlesStep[i].Stop();
        }

        //라이트 제거
        ParticleLight.enabled = false;

        data.Init();

        //비활성
        GRAB = false;

        return true;
    }

    protected override void Init()
    {
        base.Init();
        source = GetComponent<AudioSource>();
        ParticleLight.enabled = false;
        range = DataSet.Instance.GameDifficulty.CrystalBallRange;
    }

    private void Update()
    {
        if (GRAB)   // 현재 플레이어가 잡고 있다.
        {
            if (!source.isPlaying)
            {
                source.Play();
                source.volume = 0.0f;
            }

            applySearchTimeout -= Time.deltaTime;
            //applyDuration -= Time.deltaTime;

            if (applySearchTimeout <= 0.0f)
            {
                //Debug.Log($"고스트 탐지중");

                //오브젝트 탐지
                SearchObject();

                //색 지정
                ParticleActive();

                //타임아웃 시간
                applySearchTimeout = searchTimeout;
            }
        }
        else
        {
            if (source.isPlaying) source.Stop();
            //지속 시간 충전
            if (applyDuration < duration)
            {
                applyDuration += Time.deltaTime;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if(GRAB)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(transform.position, range);
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(transform.position, range * 0.666f);
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(transform.position, range * 0.333f);
    //    }
    //}

    void ParticleActive()
    {
        int _range = (int)Mathf.Floor(data.DistanceA / 0.333f);
        //Debug.Log($"탐색 범위에 따른 파티클 활성 개수 {data.DistanceA} / {0.333f} = (int){_range}");

        //사운드 지정
        if (data.Color.Equals(Color.gray))
        {
            source.volume = 0;
            _range = 0; // 활성화된 개체가 하나도 없음.
        }
        //지정된 오브젝트가 있는 경우에만 소리가 활성화됨
        else
        {
            source.volume = DataSet.Instance.SettingValue.Volume * data.DistanceA * 0.2f;
        }
        
        //탐색 범위에 따른 활성 파티클의 개수에 따라 색을 변경 후 활성화 한다.
        for( int i = 0; i <= _range; i++)
        {
            mainModule = ParticlesStep[i].main;
            mainModule.startColor = data.Color;
            ParticleLight.color = data.Color;

            if (!ParticlesStep[i].isPlaying) ParticlesStep[i].Play();
        }
        //나머지는 꺼주기
        for(int i = ParticlesStep.Length-1; i >_range && _range >= 0  ; i--)
        {
            if (ParticlesStep[i].isPlaying) ParticlesStep[i].Stop();
        }
    }



    //가장 가까운 Soul 한개를 반환한다.
    void SearchObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range,layer);

        Collider _collider = null;  // 가장 거리가 짧은 것을 탐지해 저장

        data.Init();    //초기값 초기화

        if (colliders.Length > 0 )
        {
            Debug.Log($"주위 고스트를 탐색했습니다. {colliders.Length}");

            MinDistance = Mathf.Infinity;    // 가장 작은 거리값을 저장한다.

            //감지된 컬라이더로 비교합니다.
            foreach (Collider collider in colliders)
            {
                //NavMesh를 이용한 거리값
                float distance = Calculator.GetPathDistance(collider.transform.position, transform.position);

                //탐지 거리 안에 존재한다.
                if(distance < range)
                {
                    //기존 거리값보다 작다면 저장한다.
                    if (distance < MinDistance)
                    {
                        _collider = collider;
                        MinDistance = distance;
                    }
                }
            }

            //Debug.LogWarning($"Crystar : 현재 가장 작은 거리값 제공 : {MinDistance}");

            //입력된 디스턴스가 없다.
            if (MinDistance == Mathf.Infinity) { MinDistance = 0.0f; }

            //Debug.Log($"고스트가 범위에 들어와 값이 입력되었습니다. : Min : {MinDistance}");
            //거리를 0~1로 지정 /고스트와의 거리가 가까울수록 DistancA가 1에 수렴
            //영혼의 색을 지정한다.
            Color c = (_collider != null) ? _collider.gameObject.GetComponent<Soul>().SoulColor : Color.gray;

            data.Set(((range - MinDistance) / range), c);
        }
    }
}
