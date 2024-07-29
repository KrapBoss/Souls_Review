using CustomUI;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

/// <summary>
/// 일정주기마다 플레이어 근처에 등장
/// </summary>
public class GhostBlack : MonoBehaviour
{
    //활성화 여부
    public static bool activation= false;

    [Header("활성화주기")]
    public float ActiveCycle = 60.0f;

    [Header("사운드 이벤트 발생 주기")]
    public float occurSoundCycle = 4.0f;

    //빛에 노출된 시간
    public float exposeLightTime = 5.0f;

    public AudioClip clip_Footstep;

    public GameObject go_ParticleEmber;

    public AudioSource source1;
    public AudioSource source2;
    public AudioSource source3_Walk;

    [Space, Header("거리값 측정을 위함")]
    public float distance = 5.0f;
    public float DelayTime = 0.1f;
    float _delayTime = 0.0f;

    private GhostEffect _ghostEffect;
    private float _applyActiveCycle;
    [SerializeField]private float _applyCurrentCycle;

    private float _applyOccurSoundCycle;

    private NavMeshAgent _agent;

    private bool _premonitorySymptom;

    private BoxCollider _collider;

    //빛에 당하고 있나요?
    private bool _hitTheLight= false;
    //빛에 노출된 시간
    private float _applyExposeLightTime;

    private void Awake()
    {
        _ghostEffect = GetComponentInChildren<GhostEffect>();
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<BoxCollider>();

        //고스트 블랙이 나타나는 시간을 지정합니다.
        ActiveCycle = DataSet.Instance.GameDifficulty.GhostBlackCycleTime;

        //test
        //ActiveCycle = 5.0f;

        _applyActiveCycle = Random.Range(ActiveCycle * 0.7f, ActiveCycle * 1.2f);
        _applyCurrentCycle = _applyActiveCycle;

        _ghostEffect.gameObject.SetActive(false);
        go_ParticleEmber.SetActive(false);
        _collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerEvent.instance.isDead || GameManager.Instance.DontMove) {
            source3_Walk.Stop();
            return; }

        if (_ghostEffect.gameObject.activeSelf)
        {
            GhostEvent();
            ExposeTheLight(); 
            Effect();
        }
        else{
            ActiveCounter();
        }       
    }

    //일정 시간 빛에 노출되면 사라진다.
    void ExposeTheLight()
    {
        if (!_hitTheLight) return;

        if(_applyExposeLightTime < exposeLightTime)
        {
            _applyExposeLightTime += Time.deltaTime;
        }
        //사망처리
        else
        {
            GhostDeActive();
        }
    }


    //일정시간이 지나면 해당 오브젝트가 활성화 되도록 한다.
    void ActiveCounter()
    {   
        if (_applyCurrentCycle > 0)
        {
            _applyCurrentCycle -= Time.deltaTime;

            if (!_premonitorySymptom)
            {
                if (_applyCurrentCycle < _applyActiveCycle * 0.5f)
                {
                    _premonitorySymptom = true;

                    //사운드
                    source1.PlayOneShot(clip_Footstep, DataSet.Instance.SettingValue.Volume * 1.0f);

                    //파티클
                    EffectManager.instance.ActiveParticle("FootPrint", PlayerEvent.instance.GetPosition() + new Vector3(0, 0.25f, 0));
                }
            }
        }
        else
        {
            _applyActiveCycle = Random.Range(ActiveCycle * 0.7f, ActiveCycle *1.2f) * GameManager.Instance.GetRemainSoulsRatio() *2;
            _applyCurrentCycle = _applyActiveCycle;

            _premonitorySymptom= false;

            AudioManager.instance.PlayEffectiveSound("GhostBlack", 0.8f);

            //검은 악귀 활성화
            GhostActive();

            Debug.LogWarning("검은 악귀가 활성화됩니다.");
        }
    }

    //검은귀의 거리에 
    void Effect()
    {
        _delayTime += Time.deltaTime;
        if(DelayTime < _delayTime)
        {
            _delayTime = 0;
            float length = Calculator.GetPathDistance(transform.position, PlayerEvent.instance.GetPosition());

            if (length < distance) UI.activityUI.AroundBlack(length/ distance);
        }
    }

    void GhostActive()
    {
        activation = true;

        //활성
        _ghostEffect.gameObject.SetActive(true);
        _collider.enabled = true;

        //대상 추적
        _agent.SetDestination(PlayerEvent.instance.GetPosition());

        //다음 동작 지정
        _ghostEffect.UpdateNextMonement();

        //사운드
        source1.volume = DataSet.Instance.SettingValue.Volume*0.7f;
        source1.pitch = 0.8f;
        source1.Play();

        source2.volume = DataSet.Instance.SettingValue.Volume*0.7f;
        source2.Play();

        source3_Walk.volume = DataSet.Instance.SettingValue.Volume*1.2f;
        source3_Walk.Play();

        //떨림 정상화
        _ghostEffect.rot = 10.0f;
    }

    //귀신 비활성
    void GhostDeActive()
    {
        activation = false;

        //비활성
        _ghostEffect.gameObject.SetActive(false);
        go_ParticleEmber.SetActive(false);

        //UI 비활성
        UI.activityUI.AroundBlack(0);

        //사운드 종료
        source1.Stop();
        source1.pitch = 2.0f;

        source2.Stop();

        source3_Walk.Stop();

        //빛에 의한 노출 비활성
        _hitTheLight = false;

        _collider.enabled = false;
    }


    //귀신이 활성화되어 있는 경우 동작을 수행한다.
    void GhostEvent()
    {
        //사운드 실행
        if (!source1.isPlaying)
        {
            source1.volume = DataSet.Instance.SettingValue.Volume * 1.2f;
            source1.Play();
        }

        //빛에 당하고 있는 경우 느려짐
        if (_hitTheLight)
        {
            _agent.speed = 0;

            //발자국 소리 정지
            source3_Walk.Stop();
        }
        //정상적일 경우 빠름
        else
        {
            _agent.speed = 8;
        }

        //사운드 이벤트를 발생시킨다.
        if (_applyOccurSoundCycle > 0)
        {
            _applyOccurSoundCycle -= Time.deltaTime;
        }
        else
        {
            Debug.LogWarning("검은 악귀가 사운드 이벤트를 발생시켰습니다.");

            _applyOccurSoundCycle = occurSoundCycle;

            //사운드 발생
            EventManager.instance.EventSound(PlayerEvent.instance.transform, 20.0f);

            //추격 대상 지정
            _agent.SetDestination(PlayerEvent.instance.GetPosition());
        }

        //빛에 당하고 있지 않을 때만 움직임을 업데이트
        if (!_hitTheLight)
        {
            //목표지점이 멀리 존재하는 경우에만 움직임을 부여함

            if (_agent.stoppingDistance >= _agent.remainingDistance)
            {
                _ghostEffect.rot = 0.0f;
                source3_Walk.volume = 0;
            }
            //목표지점과의 거리가 남아있는 경우
            else
            {
                ///발자국 소리 표시
                if(!source3_Walk.isPlaying)source3_Walk.Play();
                source3_Walk.volume = DataSet.Instance.SettingValue.Volume * 1.3f;

                _ghostEffect.rot = 10.0f;

                //몸의 꺽임을 지정합니다.
                _ghostEffect.UpdateNextMonement();
            }
        }
    }

    //빛에 노출됨
    public void HitTheLight()
    {
        _hitTheLight = true;

        source1.pitch = 3.0f;

        //격렬한 움직임
        _ghostEffect.rot = 20.0f;

        //빛에 노출되기 시작
        _applyExposeLightTime = 0.0f;

        go_ParticleEmber.SetActive(true);
    }

    //빛을 벗어남
    public void EscapeTheLight()
    {
        _hitTheLight = false;

        source1.pitch = 0.8f;

        go_ParticleEmber.SetActive(false);
    }

    private void OnDestroy()
    {
        activation = false;
    }
}
