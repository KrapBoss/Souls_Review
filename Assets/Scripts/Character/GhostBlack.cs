using CustomUI;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

/// <summary>
/// �����ֱ⸶�� �÷��̾� ��ó�� ����
/// </summary>
public class GhostBlack : MonoBehaviour
{
    //Ȱ��ȭ ����
    public static bool activation= false;

    [Header("Ȱ��ȭ�ֱ�")]
    public float ActiveCycle = 60.0f;

    [Header("���� �̺�Ʈ �߻� �ֱ�")]
    public float occurSoundCycle = 4.0f;

    //���� ����� �ð�
    public float exposeLightTime = 5.0f;

    public AudioClip clip_Footstep;

    public GameObject go_ParticleEmber;

    public AudioSource source1;
    public AudioSource source2;
    public AudioSource source3_Walk;

    [Space, Header("�Ÿ��� ������ ����")]
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

    //���� ���ϰ� �ֳ���?
    private bool _hitTheLight= false;
    //���� ����� �ð�
    private float _applyExposeLightTime;

    private void Awake()
    {
        _ghostEffect = GetComponentInChildren<GhostEffect>();
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<BoxCollider>();

        //��Ʈ ���� ��Ÿ���� �ð��� �����մϴ�.
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

    //���� �ð� ���� ����Ǹ� �������.
    void ExposeTheLight()
    {
        if (!_hitTheLight) return;

        if(_applyExposeLightTime < exposeLightTime)
        {
            _applyExposeLightTime += Time.deltaTime;
        }
        //���ó��
        else
        {
            GhostDeActive();
        }
    }


    //�����ð��� ������ �ش� ������Ʈ�� Ȱ��ȭ �ǵ��� �Ѵ�.
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

                    //����
                    source1.PlayOneShot(clip_Footstep, DataSet.Instance.SettingValue.Volume * 1.0f);

                    //��ƼŬ
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

            //���� �Ǳ� Ȱ��ȭ
            GhostActive();

            Debug.LogWarning("���� �ǱͰ� Ȱ��ȭ�˴ϴ�.");
        }
    }

    //�������� �Ÿ��� 
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

        //Ȱ��
        _ghostEffect.gameObject.SetActive(true);
        _collider.enabled = true;

        //��� ����
        _agent.SetDestination(PlayerEvent.instance.GetPosition());

        //���� ���� ����
        _ghostEffect.UpdateNextMonement();

        //����
        source1.volume = DataSet.Instance.SettingValue.Volume*0.7f;
        source1.pitch = 0.8f;
        source1.Play();

        source2.volume = DataSet.Instance.SettingValue.Volume*0.7f;
        source2.Play();

        source3_Walk.volume = DataSet.Instance.SettingValue.Volume*1.2f;
        source3_Walk.Play();

        //���� ����ȭ
        _ghostEffect.rot = 10.0f;
    }

    //�ͽ� ��Ȱ��
    void GhostDeActive()
    {
        activation = false;

        //��Ȱ��
        _ghostEffect.gameObject.SetActive(false);
        go_ParticleEmber.SetActive(false);

        //UI ��Ȱ��
        UI.activityUI.AroundBlack(0);

        //���� ����
        source1.Stop();
        source1.pitch = 2.0f;

        source2.Stop();

        source3_Walk.Stop();

        //���� ���� ���� ��Ȱ��
        _hitTheLight = false;

        _collider.enabled = false;
    }


    //�ͽ��� Ȱ��ȭ�Ǿ� �ִ� ��� ������ �����Ѵ�.
    void GhostEvent()
    {
        //���� ����
        if (!source1.isPlaying)
        {
            source1.volume = DataSet.Instance.SettingValue.Volume * 1.2f;
            source1.Play();
        }

        //���� ���ϰ� �ִ� ��� ������
        if (_hitTheLight)
        {
            _agent.speed = 0;

            //���ڱ� �Ҹ� ����
            source3_Walk.Stop();
        }
        //�������� ��� ����
        else
        {
            _agent.speed = 8;
        }

        //���� �̺�Ʈ�� �߻���Ų��.
        if (_applyOccurSoundCycle > 0)
        {
            _applyOccurSoundCycle -= Time.deltaTime;
        }
        else
        {
            Debug.LogWarning("���� �ǱͰ� ���� �̺�Ʈ�� �߻����׽��ϴ�.");

            _applyOccurSoundCycle = occurSoundCycle;

            //���� �߻�
            EventManager.instance.EventSound(PlayerEvent.instance.transform, 20.0f);

            //�߰� ��� ����
            _agent.SetDestination(PlayerEvent.instance.GetPosition());
        }

        //���� ���ϰ� ���� ���� ���� �������� ������Ʈ
        if (!_hitTheLight)
        {
            //��ǥ������ �ָ� �����ϴ� ��쿡�� �������� �ο���

            if (_agent.stoppingDistance >= _agent.remainingDistance)
            {
                _ghostEffect.rot = 0.0f;
                source3_Walk.volume = 0;
            }
            //��ǥ�������� �Ÿ��� �����ִ� ���
            else
            {
                ///���ڱ� �Ҹ� ǥ��
                if(!source3_Walk.isPlaying)source3_Walk.Play();
                source3_Walk.volume = DataSet.Instance.SettingValue.Volume * 1.3f;

                _ghostEffect.rot = 10.0f;

                //���� ������ �����մϴ�.
                _ghostEffect.UpdateNextMonement();
            }
        }
    }

    //���� �����
    public void HitTheLight()
    {
        _hitTheLight = true;

        source1.pitch = 3.0f;

        //�ݷ��� ������
        _ghostEffect.rot = 20.0f;

        //���� ����Ǳ� ����
        _applyExposeLightTime = 0.0f;

        go_ParticleEmber.SetActive(true);
    }

    //���� ���
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
