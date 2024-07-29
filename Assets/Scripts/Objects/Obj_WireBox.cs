using CustomUI;
using System.Collections;
using UnityEngine;

//TargetObjectName = Hammer
public class Obj_WireBox : ExpandObject
{
    public GameObject fx_Success;
    public GameObject fx_Failed;
    public GameObject fx_Default;

    public AudioClip clip_Success;
    public AudioClip clip_Failed;

    public Transform tf_SoulSpawnPoint;
    public Soul soul;

    AudioSource source;

    protected override void Init()
    {
        base.Init();
        source = GetComponent<AudioSource>();
        source.volume = DataSet.Instance.SettingValue.Volume * 0.3f;
        Setting.Action_SoundChanger += SoundChange;
    }

    public override bool Func(string name = null)
    {
        if (!LibraryEvent.Activation)
        {
            Debug.Log("������ �̺�Ʈ�� Ȱ��ȭ���� �ʾҽ��ϴ�.");

            //���� ǥ��
            string line = LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation");
            UI.topUI.ShowNotice(line, false);

            return false;
        }

        if (TargetObjectName.Equals(name))
        {
            if (!source.isPlaying) source.Play();

            Debug.Log("WireBox Ȱ��ȭ");
            fx_Success.SetActive(false);
            fx_Failed.SetActive(false);
            UI.semiStaticUI.WireBox(Success);
            return true;
        }
        return false;
    }

    //�������� ���
    void Success(bool success)
    {
        if(success)
        {
            // �Ⱥ��̱�
            gameObject.layer = 0;
            foreach(MeshRenderer filter in GetComponentsInChildren<MeshRenderer>())
            {
                filter.enabled = false;
            }

            //�� ����
            source.PlayOneShot(clip_Success, DataSet.Instance.SettingValue.Volume * 1.7f);

            //Ŭ���� ��ƼŬ
            fx_Success.SetActive(true);

            //�⺻ ��ƼŬ ����
            fx_Default.SetActive(false);

            //�ڷ�ƾ���� ������Ʈ�� �����մϴ�.
            IEnumerator enumerator = Delay();
            CoroutineHandler.StartCorou(enumerator);
        }
        else
        {
            //���� ����
            source.PlayOneShot(clip_Failed, DataSet.Instance.SettingValue.Volume * 1.7f);

            //���� ��ƼŬ
            fx_Failed.SetActive(true);
        }
    }

    IEnumerator Delay()
    {
        Destroy(fx_Default);


        yield return new WaitForSeconds(1.0f);

        //Ŭ��� ��� �Ϸ� �Ǿ��ٸ�?
        if (WireBox.SuccessCount > 2)
        {
            Debug.LogWarning("������ ��ȥ�� Ȱ��ȭ�մϴ�.");

            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f, true);

            soul.gameObject.SetActive(true);
            soul.transform.localPosition = tf_SoulSpawnPoint.position;
            soul.ActiveMusicbox(null, true);
        }

        Destroy(source);
        Destroy(fx_Failed);
        Destroy(fx_Success);
        Destroy(this);

        yield return -1;
    }

    public void SoundChange()
    {
        source.volume = DataSet.Instance.SettingValue.Volume * 0.3f;
    }
    private void OnDestroy()
    {
        Setting.Action_SoundChanger -= SoundChange;
    }
}
