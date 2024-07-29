using CustomUI;
using JetBrains.Annotations;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public interface IMusicBox
{
    //Ȱ��ȭ�ϸ鼭 Ȱ��ȭ�� ������ �������� �Ǵ��Ѵ�.
    public bool ActiveMusicbox(in MusicBox musicBox= null, bool empty = false);

    //�������� ��Ȱ��ȭ �Ǿ��� ��
    public void DeActiveMusicbox();

    public Vector3 GetPosition();
}

public class MusicBox : GlobalSound
{
    [Space]
    [Header("AttractObject Optional")]
    public GameObject particle;

    public bool isPlay;               //������ Ȱ��ȭ

    public float duration = 20.0f;    //������ ���ӽð�

    public float Range = 6.0f;       //�ͽ� Ž�� ����
    public LayerMask searchLayer;     //Ž���� �ͽ��� ���̾�

    public float PlayerRange = 15.0f;         //�÷��̾�� ������ �ѱ� �Ÿ���

    [Space]
    public GameObject go_cover;
    public GameObject go_deco;
    public GameObject go_hadle;
    public float rotateSpeed;
    //Soul soul = null;
    IMusicBox imusicBox;


    [Space]
    public GameObject fx_explosion;
    public GameObject go_root;          //�����ڽ� ���� ���� ������Ʈ
    bool destroy;

    //��õ ������ ��Ÿ��.
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
        // �������� ���������� ��Ÿ��.
        if (isPlay && !destroy)
        {
            go_deco.transform.Rotate(Vector3.up,rotateSpeed*Time.deltaTime*0.5f);
            go_hadle.transform.Rotate(-Vector3.left,rotateSpeed*Time.deltaTime);

            //���Ͱ� �ֺ��� �����Ѵٸ� �������� ����
            if(Vector3.Distance(EventManager.instance.GetMonsterPosition(), transform.position) < Range * 0.5f)
            {
                if(Calculator.GetBetweenHeight(EventManager.instance.GetMonsterPosition(), transform.position))
                {
                    if(!ascending) Explosion();
                    return;
                }
            }

            //���� ��ȥ�� ���ٸ� ���������� Ž���մϴ�.
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
            
            //�Ÿ��� �Ǵ��Ͽ� ��ȥ�� ������ ������ ����ٸ�, ��Ȱ��ȭ��ŵ�ϴ�.
            if(Vector3.Distance(transform.position, imusicBox.GetPosition()) > Range*1.1f)
            {
                DeleteIMusicBox();
            }
        }
    }

    static short ActivationCount = 0;
    public override bool GrabOn(Transform parent)
    {
        //��ȣ�ۿ��� �����ϴٸ� ���̾ ����
        if (!TYPE.Equals(InteractionType.Expand))
        {
            go_root.layer = 30;
            go_cover.layer = 30;
            go_hadle.layer = 30;
            go_deco.layer = 30;
        }

        //�� �����ִ� Ƚ�� ����
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

        TYPE = InteractionType.None; //Ÿ�� �������� �׷� �Ұ��ϰ� �Ѵ�.
        gameObject.layer = DataSet.Instance.Layers.Default; 
        if (Childs != null)
        {
            for (int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.Default;
            }
        }

        isPlay = true;

        //�ڽ��� ����
        OpenMusicBox();

        //������ ���̵��ΰ� FX Ȱ��ȭ�Ѵ�.
        FadeInSound();
        particle.SetActive(true);

        //���� ���
        AudioSource _source= GetComponent<AudioSource>();
        _source.volume = DataSet.Instance.SettingValue.Volume;
        _source.loop = true; _source.Play();
        _source.maxDistance = PlayerRange;

        Destroy(GetComponent<ObjectCollsion>());

        //�̺�Ʈ �Ŵ������� ���� ���� ������ �Ѱ��ش�.
        EventManager.instance.ActiveTheHightestPrioritySound(GetObjectInfo());

        //���� ��Ʈ �˻� �ڷ�ƾ ���
        //Searchsoul();

        //���� �ð� ���� �ش� ������ ���� //1ȸ�� ������
        Invoke("Explosion", duration);

        return true; // �ٴڿ� ���� �����.
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

    //�ڿ������� �Ҹ� ������ ������.
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

    //��ȥ�� Ž��
    bool SearchSoul()
    {
        //���� ��Ʈ�� Ž���Ѵ�.
        Collider[] colliders;

        //��ȥ Ž��
        colliders = Physics.OverlapSphere(transform.position, Range, searchLayer);

        //��ȥ�� ������ ���и� ��ȯ
        if (colliders.Length == 0) return false;

        for (int i =0; i < colliders.Length; i++)
        {
            //�������̰� ũ�ٸ� Ȱ��ȭ ��ų �� ����.
            if(Calculator.GetBetweenHeight(transform.position, colliders[i].transform.position))
            {
                //Ȱ��ȭ ��ŵ�ϴ�.
                imusicBox = colliders[i].GetComponent<IMusicBox>();

                //������� ���� Ȱ��ȭ�� �Ұ����� �������� �Ǵ��մϴ�.
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

    //��ȥ�� ��õ�� �߽��ϴ�.
    public void AscendedSoul()
    {
        //Debug.Log("�ͽ��� ��õ�Ͽ� �������� ����� ���ߴ�.");
        ascending = true;
        Explosion();
    }

    //������ ���ϰ� �ı���Ų��.
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
    //���� �ð� ���� �� ����
    void DestroyMusicBox()
    {
        StopAllCoroutines();

        EventManager.instance.DeActiveTheHightestPrioritySound(GetObjectInfo()); // �켱 ���� ������Ʈ ����

        EventManager.instance.MusicBoxDestroy();

        Destroy(this.gameObject);
    }

    //��ȥ ������ �����մϴ�.
    void DeleteIMusicBox()
    {
        //���� �������� �ı��ȴٸ� ��Ȱ��ȭ�մϴ�.
        if (imusicBox != null) imusicBox.DeActiveMusicbox();

        imusicBox = null;
    }
}
