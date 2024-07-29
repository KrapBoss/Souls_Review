using CustomUI;
using UnityEngine;
/// <summary>
/// �ֺ� Layer : Ghost�� Ž���Ѵ�.
/// Ž���� ��Ʈ �� ���� ����� ��Ʈ�� �����ͼ� ��Ʈ�� ����� ȿ���� ǥ���Ѵ�
/// �Ÿ��� ����� ���� ���� ȿ���� �߰��Ѵ�.
/// ���� Ž���Ǵ� ������Ʈ�� ���ٸ� �⺻ ���� �����Ѵ�.
/// ������ ���������� �����Ǵ� ������Ʈ�̴�.
/// </summary>
public class GhostCrystalBall : EquipAutoObject
{    
    
    //��ȥ Ž���� ���� �������Դϴ�.
    class SearchData
    {
        public Color Color;
        public float DistanceA;
        public void Init()
        {
            Color = Color.grey;
            DistanceA = 0.0f;        //Ž���� ������ �Ÿ� // 0~1f �� ���� ����
        }
        public void Set(float disA, Color c)
        {
            DistanceA = disA;
            c.a = 0.3f + DistanceA * 0.7f;//�⺻������ �Ÿ��� ���� ���� ���̰� �ϴ� ���� ����
            Color = c;
        }
    }
    SearchData data = new SearchData();
    //��ȥ���� �ּ� �Ÿ���
    float MinDistance;


    [Header("��Ʈ Ž�� ����")]
    [Tooltip("Ž�� �������� ��ƼŬ �ý����� ������ �͵�")]
    public ParticleSystem[] ParticlesStep;
    public Light ParticleLight;

    [Tooltip("Ž�� ����")]
    public float range;

    [Tooltip("Ž���� ���̾�")]
    public LayerMask layer;

    //Ž�� ����ð�
    public float searchTimeout = 0.1f;
    float applySearchTimeout;

    AudioSource source;

    ParticleSystem.MainModule mainModule;

    //Ȱ��ȭ�� ��Ų Ƚ���� �����մϴ�.
    short ActivationCount = 0;

    public override bool GrabOn(Transform parent)
    {
        //�� �����ִ� Ƚ�� ����
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

        //���ӽð��� 0.5f ������ ��� �׷����� ���Ѵ�.
        if (applyDuration < duration * 0.5f)
        {
            AudioManager.instance.PlayEffectiveSound("Beep", 1.0f);
            return false;
        }

        applySearchTimeout = 0.0f; // Ÿ�Ӹ� �ʱ�ȭ

        ParticleLight.enabled = true;


        data.Init();

        //Ž��
        SearchObject();
        //�� ����
        ParticleActive(); 
        //�ʱ�ȭ
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
        //��ƼŬ ����
        for(int i = 0; i < ParticlesStep.Length; i++)
        {
            ParticlesStep[i].Stop();
        }

        //����Ʈ ����
        ParticleLight.enabled = false;

        data.Init();

        //��Ȱ��
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
        if (GRAB)   // ���� �÷��̾ ��� �ִ�.
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
                //Debug.Log($"��Ʈ Ž����");

                //������Ʈ Ž��
                SearchObject();

                //�� ����
                ParticleActive();

                //Ÿ�Ӿƿ� �ð�
                applySearchTimeout = searchTimeout;
            }
        }
        else
        {
            if (source.isPlaying) source.Stop();
            //���� �ð� ����
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
        //Debug.Log($"Ž�� ������ ���� ��ƼŬ Ȱ�� ���� {data.DistanceA} / {0.333f} = (int){_range}");

        //���� ����
        if (data.Color.Equals(Color.gray))
        {
            source.volume = 0;
            _range = 0; // Ȱ��ȭ�� ��ü�� �ϳ��� ����.
        }
        //������ ������Ʈ�� �ִ� ��쿡�� �Ҹ��� Ȱ��ȭ��
        else
        {
            source.volume = DataSet.Instance.SettingValue.Volume * data.DistanceA * 0.2f;
        }
        
        //Ž�� ������ ���� Ȱ�� ��ƼŬ�� ������ ���� ���� ���� �� Ȱ��ȭ �Ѵ�.
        for( int i = 0; i <= _range; i++)
        {
            mainModule = ParticlesStep[i].main;
            mainModule.startColor = data.Color;
            ParticleLight.color = data.Color;

            if (!ParticlesStep[i].isPlaying) ParticlesStep[i].Play();
        }
        //�������� ���ֱ�
        for(int i = ParticlesStep.Length-1; i >_range && _range >= 0  ; i--)
        {
            if (ParticlesStep[i].isPlaying) ParticlesStep[i].Stop();
        }
    }



    //���� ����� Soul �Ѱ��� ��ȯ�Ѵ�.
    void SearchObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range,layer);

        Collider _collider = null;  // ���� �Ÿ��� ª�� ���� Ž���� ����

        data.Init();    //�ʱⰪ �ʱ�ȭ

        if (colliders.Length > 0 )
        {
            Debug.Log($"���� ��Ʈ�� Ž���߽��ϴ�. {colliders.Length}");

            MinDistance = Mathf.Infinity;    // ���� ���� �Ÿ����� �����Ѵ�.

            //������ �ö��̴��� ���մϴ�.
            foreach (Collider collider in colliders)
            {
                //NavMesh�� �̿��� �Ÿ���
                float distance = Calculator.GetPathDistance(collider.transform.position, transform.position);

                //Ž�� �Ÿ� �ȿ� �����Ѵ�.
                if(distance < range)
                {
                    //���� �Ÿ������� �۴ٸ� �����Ѵ�.
                    if (distance < MinDistance)
                    {
                        _collider = collider;
                        MinDistance = distance;
                    }
                }
            }

            //Debug.LogWarning($"Crystar : ���� ���� ���� �Ÿ��� ���� : {MinDistance}");

            //�Էµ� ���Ͻ��� ����.
            if (MinDistance == Mathf.Infinity) { MinDistance = 0.0f; }

            //Debug.Log($"��Ʈ�� ������ ���� ���� �ԷµǾ����ϴ�. : Min : {MinDistance}");
            //�Ÿ��� 0~1�� ���� /��Ʈ���� �Ÿ��� �������� DistancA�� 1�� ����
            //��ȥ�� ���� �����Ѵ�.
            Color c = (_collider != null) ? _collider.gameObject.GetComponent<Soul>().SoulColor : Color.gray;

            data.Set(((range - MinDistance) / range), c);
        }
    }
}
