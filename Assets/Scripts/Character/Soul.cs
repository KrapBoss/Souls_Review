using CustomUI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Soul : MonoBehaviour, IMusicBox
{
    //������ �ִ� ��ȥ �� ��� Ȱ��ȭ
    public bool DontMove;

    //-----------------------------
    [Header("��ȥ �ֺ� Ž�� ����")]
    public float RangeMonster = 5.0f;

    //��ȸ�� �ϰ� �Ǵ� ���� �̸�
    [Header("��ȸ��")]
    public string[] traversalLocationName;

    //�߰� ����
    [Header("�߰� ����")]
    public bool[] chaseState = new bool[3];
    public float[] chaseDelayTime = new float[3];

    [Header("�⺻ �ӵ��� ���� �ӵ�")]
    public float defaultSpeed;
    public float runSpeed;

    [Header("��ȥ ���� ����")]
    public Color SoulColor = Color.yellow;

    [Header("�� ���¿� ���� ���̾� ����")]
    public LayerType layer_Default;
    public LayerType layer_ViewSoul;
    public LayerType layer_Ascention;

    [Header("�ð��� ����")]
    public GameObject particle_Soul;
    public GameObject[] particles_Ascended;

    [Header("���� ������ų ��ƼŬ")]
    public ParticleSystem[] particles;

    [Header("�ͽ����κ��� �������ϴ�.")]
    public bool ChasingByGhost;

    public AudioSource source_Whisper;
    public AudioSource source_Divine;

    //�ذ� ���
    public GameObject go_Soul;

    /////Private
    ///
    //��ǥ����
    Vector3 target;

    //Nav
    NavMeshAgent nav;

    //��õ ����
    bool Ascending = false;

    //������ Ÿ��
    float deleyTime;

    //������ ����
    MusicBox musicbox;
    bool isEmpty; // �������� ����ִ����� ��Ÿ����, ������� ��� �������� ����.

    // �ִϸ��̼� ������ ����
    Animator anim;

    private void Awake()
    {
        Debug.Log($"��ȥ�� �����մϴ�.  {transform.name}");

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

        //�������� �ʴ� ��ȥ�� ��� ���� ������Ʈ�� �����մϴ�.
        if (DontMove)
        {
            Destroy(nav);
        }

        //���������� ���θ� ����
        ChasingByGhost = DataSet.Instance.GameDifficulty.SoulRun;

        //���� ����
        Setting.Action_SoundChanger += SoundChange;
        SoundChange();
    }

    void Update()
    {
        if (Ascending || GameManager.Instance.DontMove || DontMove) return;

        //1. ����
        if (CheckMonster(0))
        {
            //Debug.Log($"{name} :: ���Ͱ� �ֺ��� �־� ����Ĩ�ϴ�.");
        }
        //2. ������
        else if (CheckMusicBox(1))
        {
            //Debug.Log($"{name} :: ������ �߰�");
        }
        //3. ��ȸ�� ����
        else
        {
            Travarsal(2);
            //Debug.Log($"{name} :: �⺻ ��ȸ");
        }


        if(target != nav.destination) nav.SetDestination(target);
    }

    //���Ͱ� �ֺ��� �����ϴ��� �Ǵ� �� �����մϴ�.
    bool CheckMonster(int index)
    {
        //�ͽſ��� �ѱ�� ���� ������� �ʾ�����, ����ġ�� �ʴ´�.
        if (!ChasingByGhost)
        {
            return false;
        }

        //�̹� ���� ���̴�.
        if (chaseState[index])
        {
            //������ ������ �Ǵ�
            if (ArriveCheck())
            {
                //���� ����
                return false;
            }

            return true;
        }
        else
        {
            //���Ϳ��� �Ÿ� �Ǵ�
            if (Calculator.GetBetweenHeight(transform.position, EventManager.instance.GetMonsterPosition()))
            {
                if (Vector3.Distance(transform.position, EventManager.instance.GetMonsterPosition()) < RangeMonster)
                {
                    //�̹� �߰� ���°� �ƴ� ��쿡�� ���� ����� ������Ʈ �մϴ�.
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

    //�����񿡼��� �Է��� ���´ٸ� �����մϴ�.
    bool CheckMusicBox(int index)
    {
        if (musicbox)
        {
            //�߰� ���°� �ƴ� ��쿡 �߰ݴ�� ����
            if (!chaseState[index])
            {
                target = musicbox.transform.position;
                nav.speed = defaultSpeed;
                ChaseStateChange(index);
                deleyTime = chaseDelayTime[index];
            }

            //������ �����մϴ�.
            return true;
        }

        return false;
    }

    //���� ��ȸ
    void Travarsal(int index)
    {
        // �̹� �߰� ���̴�.
        if (chaseState[index])
        {
            //�������� ������ ���� �ʾ��� ��� �߰��� �����մϴ�.
            if (!ArriveCheck())
            {
                return;
            }
        }

        //�߰����� �ƴϴ�.
        target = GetMapTarget();
        nav.speed = defaultSpeed;
        ChaseStateChange(index);
        deleyTime = chaseDelayTime[index];
        //Debug.Log($"��ȸ�� ���� ���� ���ð� : {deleyTime} /  ");
    }

    //���� ���¸� �����մϴ�.
    void ChaseStateChange(int index)
    {
        for(int i = 0; i< chaseState.Length; i++)
        {
            chaseState[i] = false;
        }

        chaseState[index] = true;
    }

    //������ �Ǵ��մϴ�.
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

    //�� �Ŵ������� ��ǥ�� ���ɴϴ�.
    Vector3 GetMapTarget()
    {
        Vector3 t = MapManager.instance.GetLocationByName(traversalLocationName[Random.Range(0, traversalLocationName.Length)]);
        //Debug.Log($"��ȸ�� ���� ���� ���ð� : {deleyTime} /  {t}");
        return t;
    }


    //�ش� ��ȥ�� ������ ������ �������� ��ȯ
    public bool CanAscention()
    {
        if (chaseState[0] || Ascending) return false;
        else return true;
    }


    // ��õ
    public bool Ascention()
    {
        //��õ �Ұ����ϸ� �������� ����
        if (!CanAscention()) return false;
        //��õ�� ������ ��� ����
        else
        {
            Debug.Log("��õ�մϴ�.");

            go_Soul.SetActive(false);

            //1, ��õ �ִϸ��̼�
            particle_Soul.transform.localRotation = Quaternion.LookRotation(particle_Soul.transform.forward, Vector3.up);
            anim.SetTrigger("Ascension");//��õ �ִϸ��̼� ����
            
            //#��õ ȿ��
            for(int i = 0;i < particles_Ascended.Length; i++)
            {
                particles_Ascended[i].SetActive(true);
            }

            //���� ���
            source_Divine.volume =DataSet.Instance.SettingValue.Volume;
            source_Divine.Play();
            //source.clip = clip_Ascention.clip;
            //source.volume = clip_Ascention.volume * DataSet.Instance.SettingValue.Volume;
            //source.Play();

            //2. ���� ��Ȱ��ȭ �� ��õ �ÿ��� ����� ���̵��� �ϱ�
            Ascending = true;

            //3. ������ �ı�
            if(!isEmpty) musicbox.AscendedSoul();

            //4. �̺�Ʈ �Ŵ������� ����
            EventManager.instance.DeleteAscentionSoul(this);

            //5. ��ȥ���� ǥ��
            GameManager.Instance.Ascention();

            //6. ī�޶� ����
            PlayerEvent.instance.SetCameraUnEquip();

            //7. ��õ ȿ��
            EffectManager.instance.ActiveParticle("UpStar", transform.position);

            //����
            Invoke("Destroy", 5.0f);

            return true;
        }
    }

    #region ------------For MusicBox------------
    //MusicBox�� ���� �������̽�
    public bool ActiveMusicbox(in MusicBox _musicBox = null , bool empty = false)
    {
        //��õ�� ������ �����̹Ƿ� ���¸� �����մϴ�.
        if (CanAscention())
        {
            Debug.Log($"��ȥ �������� Ȱ��ȭ�Ǿ����ϴ�.");
            //��õ �� ����ϱ� ���� ����

            isEmpty = empty;
            if (!isEmpty) musicbox = _musicBox;

            //ī�޶�� �� �� �ִ� ���̾�� ����
            SoulSetLayer(layer_ViewSoul);

            //�̺�Ʈ �Ŵ����� �߰�
            EventManager.instance.SaveAscentionSoul(this);

            return true;
        }
        //��õ�� �Ұ����� ����
        else
        {
            Debug.Log($"�������� Ȱ��ȭ���� ���߽��ϴ�.");
            return false;
        }
    }

    //������� ���� ������ ���´�.
    public void DeActiveMusicbox()
    {
        Debug.Log($"�������� DeActiveMusicbox()");

        //���̾� ����
        SoulSetLayer(layer_Default);

        // �̺�Ʈ �Ŵ������� ����
        EventManager.instance.DeleteAscentionSoul(this);

        //���� ����
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

    //�������� ��ȥ�� ���̾� ����
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
        Debug.LogWarning($"{name} ��ȥ�� ���ŵ˴ϴ�.");

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
        Debug.LogWarning($"{name} ��ȥ �Ҹ� ���� ���.");
        source_Whisper.volume = DataSet.Instance.SettingValue.Volume * 0.8f;
    }

    private void OnDestroy()
    {
        Setting.Action_SoundChanger -= SoundChange;
        Debug.LogError("���� Soul " + name);
    }
}