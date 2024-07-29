using CustomUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���� ������ �÷��̾�� ���� �̺�Ʈ�� ���õ� ������ �޴´�.
//ĳ���Ϳ� ������ �̺�Ʈ�� Ȯ���Ѵ�.
public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public Transform GlobalSoundTarget { get; private set; }   // ���� ������ �Ҹ� �̺�Ʈ�� �����ϱ� ����.
    public Transform LocalSoundTarget { get; private set; }


    //���� ������Ʈ ������ ��Ƶδ� ����
    List<ObjectInfo> objInfo = new List<ObjectInfo>();

    //�Ƿ��� ��ġ��ǥ
    Ghost cs_Ghost;

    //��ȥ��
    [SerializeField]GameObject GhostWrap;
    [SerializeField]GameObject SoulWrap;
    //���� �Ǳ�
    [SerializeField]GameObject go_GhostBlack;

    //ī�޶� ������ ���� ������ �����Ѵ�. //ī�޶� ��Ʈ�ѷ����� ȣ��ȴ�.
    public Action<bool> Action_CameraEquip { get; set; }

    //���Ͱ� �÷��̾� �ֺ��� ��ȸ�� ��� ����Ǵ� �׼�
    //���Ϳ� ������ 1 , �ִ� 0�� ��ȯ
    public Action<float> Action_AroundMonster { get; set; }

    CameraShaking cameraShaking;                    //ī�޶� ���� ȿ��

    public Action Action_CameraFlash { get; set; }  // ī�޶� �Կ� �̺�Ʈ


    //��õ ������ ��ȥ�� ����
    List<Soul> list_Soul = new List<Soul>();

    MusicboxSpawn musicboxSpawn;

    private void Awake()
    {
        if (instance == null) instance = this;

        cameraShaking = FindObjectOfType<CameraShaking>();

        cs_Ghost = FindObjectOfType<Ghost>();

        musicboxSpawn= FindObjectOfType<MusicboxSpawn>();

    }
    private void OnDestroy()
    {
        Destroy(instance);
        instance = null;
    }
    //�۷ι� ���� ����Ʈ�� ������Ʈ�� �����ϴ��� �Ǵ�
    bool FindObject(string Name)
    {
        foreach (var n in objInfo)
        {
            if (n.Name.Equals(Name)) { return true; }
        }
        return false;
    }

    public void DeActiveGhost()
    {
        go_GhostBlack.SetActive(false);
        GhostWrap.SetActive(false);
        SoulWrap.SetActive(false);
    }

    //�������带 ������ ���� �����Ѵ�.
    public void ActiveTheHightestPrioritySound(ObjectInfo obj)
    {
        // ���� ������Ʈ�� �����ϸ� ��Ȱ��ȭ
        if (FindObject(obj.Name)) return;

        CancleLocalSoundDelay();//���� ���� �����̰� ���� ��� ����

        //�̹� ���� ��� �ܼ��� �߰�
        if (objInfo.Count > 0)
        {
            objInfo.Add(obj);
        }
        //�ƹ��͵� ���� ��� Ÿ�ټ���
        else
        {
            GlobalSoundTarget = obj.Trans;
            objInfo.Add(obj);
        }
    }

    //�۷ι� ���带 ������.
    public void DeActiveTheHightestPrioritySound(ObjectInfo obj)
    {
        //Debug.Log($"���� ������Ʈ ��Ȱ��ȭ ���� {objInfo.Count}");

        //���� ������Ʈ�� �����Ѵٸ�, ���� �̸��� ������Ʈ�� �����Ѵ�.
        for (int i = 0; i < objInfo.Count; i++)
        {
            if (objInfo[i].Name.Equals(obj.Name))
            {
                objInfo.RemoveAt(i);
                //Debug.Log($"���� ������Ʈ ��Ȱ��ȭ �Ϸ� {objInfo.Count}");
                break;
            }
        }

        //������Ʈ�� �����ϸ� �ٸ� ������Ʈ�� Ÿ������ �����Ѵ�.
        if (objInfo.Count > 0)
        {
            GlobalSoundTarget = objInfo[0].Trans;
            //Debug.Log("���ο� ���� ����� ��ü�Ѵ�!!");
        }
        //����� ���� ���尡 ���ٸ� ������ �����Ѵ�.
        else
        {
            GlobalSoundTarget = null;
            //Debug.Log("���ο� ���� ����� ����!!!");
        }
    }

    #region------LocalSound--------
    IEnumerator EventSoundTimeout; // Ÿ�Ӿƿ� �ڷ�ƾ ����
    //�Ҹ��� ���� �̺�Ʈ �߻����� �Ÿ��� ���� �޴´�
    //���޹��� ��ġ�� ���� �Ÿ��� ������� ���� �Ÿ��� ���Ͱ� ���� �߻����� �ȿ� �ִٸ� ���Ϳ��� ���ο� ��ġ�� �������ش�.
    //���������� �����۰� ������ �Ÿ��� _distace �Ű��������� �۴ٸ�, ���ο� Transform�� ���Ź޾� Ÿ���� �����ϰ� �ȴ�
    public bool EventSound(Transform _target, float _distance)
    {
        //�۷ι� ���尡 ������ ��� �������� ����.
        if (GlobalSoundTarget != null) return false;

        //���� ���� ���� ���� �� �������� ����.
        //if (LocalSoundTarget) return false;

        //�Ҹ��� ��������� ���� _distance
        //�Ҹ��� �߻��� ������ _target
        if ((GetMonsterPosition() - _target.position).magnitude < (_distance* DataSet.Instance.GameDifficulty.SoundSensitivity))
        {
            //Debug.Log(string.Format($"EventSound :: �Ҹ��� ���� ���������� Ž���˴ϴ�. {transform.name} :: {_distance}"));

            //���� Ÿ�� ����
            LocalSoundTarget = _target;

            //���� ������ ���� �� ���� ����
            CancleLocalSoundDelay();
            EventSoundTimeout = EventSoundCroutine();
            StartCoroutine(EventSoundTimeout);

            //Debug.Log("EventManager :: �Ͻ����� ���� �̺�Ʈ �߻� �Է� ����");

            return true;    // ����
        }

        return false;// ����
    }

    //���� ���尡 �����Ǿ��� ��, ������ �� ���带 �����մϴ�.
    IEnumerator EventSoundCroutine() // �̺�Ʈ ���� Ÿ�� ���� �ð� = 1��
    {
        yield return new WaitForSeconds(1);
        LocalSoundTarget = null;
        EventSoundTimeout = null;

        //Debug.Log("EventMAnager :: ���� �ٿ��� TimeOut �ʱ�ȭ");
    }

    // ���� ������ ��ǥ���� ������ ������ �����Ѵ�.
    public Vector3 GetLocalSoundTarget()
    {
        //������ ��ġ�� ���ٸ�, Zero�� ��ȯ
        if (LocalSoundTarget == null) return Vector3.zero;

        Vector3 target = LocalSoundTarget.position;
        LocalSoundTarget = null;

        CancleLocalSoundDelay();

        Debug.Log("EventMAnager :: ���� �ٿ��� ��ȯ �� �ʱ�ȭ");

        return target;
    }

    //���û��� ������ �ڷ�ƾ�� �����Ѵ�.
    void CancleLocalSoundDelay()
    {
        if (EventSoundTimeout != null) StopCoroutine(EventSoundTimeout);
        EventSoundTimeout = null;
    }
    #endregion


    #region ------------��ȥ ��õ�� ���� ����--------------
    public void SaveAscentionSoul(in Soul _soul)
    {
        for (int i = 0; i < list_Soul.Count; i++)
        {
            if (list_Soul[i] == _soul)
            {
                Debug.Log("��õ ������ ��ȥ�� �̹� �����մϴ�.");
                return;
            }
        }

        list_Soul.Add(_soul);
    }
    public void DeleteAscentionSoul(in Soul _soul)
    {
        for (int i = 0; i < list_Soul.Count; i++)
        {
            if (list_Soul[i] == _soul)
            {
                Debug.Log("��õ���� ��ȥ ��Ͽ��� �����մϴ�.");
                list_Soul.RemoveAt(i);
                return;
            }
        }
    }

    //���� �Է¿��� ���� �Ÿ����� ����� ���� ��ȯ�մϴ�.
    public Soul GetSoul(Vector3 currentPosition)
    {
        if (list_Soul.Count < 1) { return null; }

        float minDistance = Mathf.Infinity;
        Soul _soul = null;
        for (int i = 0; i < list_Soul.Count; i++)
        {
            //ī�޶� ���ο� �����Ѵ�.
            if (Calculator.ObjectInViewport(list_Soul[i].GetPosition() + new Vector3(0, 0.8f, 0)))
            {
                Debug.Log($"ī�޶� ���ο� ��ȥ�� ��ġ�մϴ�.");

                //���� �Ÿ��� ����� ��ȥ�� �����´�.
                float dis = Vector3.Distance(currentPosition, list_Soul[i].GetPosition());
                if (dis < minDistance)
                {
                    minDistance = dis;
                    _soul = list_Soul[i];
                }
            }
        }
        return _soul;
    }
    #endregion


    public Vector3 GetMonsterPosition()
    {
        if (cs_Ghost && GhostWrap.activeSelf) return cs_Ghost.transform.position;
        else return new Vector3(0, Mathf.Infinity, 0);
    }

    //�ǱͿ� ��ȥ�� Ȱ��ȭ
    public void ActiveSouls(Vector3 ghostPosition)
    {
        Debug.LogWarning("�ǱͿ� ��ȥ���� Ȱ��ȭ�˴ϴ�.");
        GhostWrap.SetActive(true);
        cs_Ghost.Activation(ghostPosition);
        SoulWrap.SetActive(true);
    } 

    public void DeactiveSouls()
    {
        Debug.LogWarning("�ǱͿ� ��ȥ���� ��Ȱ��ȭ�˴ϴ�.");
        GhostWrap.SetActive(false);
        SoulWrap.SetActive(false);
        //���� �Ǳ͸� ��Ȱ��ȭ�մϴ�.
        go_GhostBlack.SetActive(false);
    }

    #region ī�޶�
    //ī�޶� ����
    public void CameraShake(float t, float force) { cameraShaking.Shake(t, force); }
    
    #endregion


    //�������� ������ �˸�
    public void MusicBoxDestroy()
    {
        musicboxSpawn.MusicBoxDestroy();
    }

    //���� �����ִ� ��ȥ�� �� ��ȯ
    public int GetActivationSouls()
    {
        Debug.LogError($"EventManager : ���� ��ȥ ���� ��ȯ�մϴ�. {SoulWrap.transform.childCount}");
        return SoulWrap.transform.childCount;
    }

    //������ ������ ����
    public void DestroyMusicboxSpawner()
    {
        Destroy(musicboxSpawn.gameObject); 
    }

    public void ActiveGhostBlack()
    {
        //��Ʈ�� Ȱ��ȭ�� �ȵǰ�, ���̵��� ������ �ƴ� ��쿡�� �Ǳ� Ȱ��ȭ
        if(!go_GhostBlack.activeSelf && DataSet.Instance.SettingValue.CurrentDiff != 0)
        {
            Debug.LogWarning("���� �Ǳ͸� Ȱ��ȭ�մϴ�.");

            go_GhostBlack.SetActive(true);

            AudioManager.instance.PlayEffectiveSound("GhostBlack", 1.0f);

            //���
            //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "ActiveBlack"), 4.0f);
            //UI.staticUI.ShowLine();

            AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.0f);

            if(DataSet.Instance.SettingValue.CurrentDiff > 0)
            {
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "ActiveBlack2"), false);
            }
        }
    }

    //��ȥ�� ��õ�ϸ� �ӵ��� �����մϴ�.
    public void AscendedSoul()
    {
        cs_Ghost.AscensededSoul();
    }
}
