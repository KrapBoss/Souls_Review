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
            Debug.Log("도서관 이벤트가 활성화되지 않았습니다.");

            //지문 표시
            string line = LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation");
            UI.topUI.ShowNotice(line, false);

            return false;
        }

        if (TargetObjectName.Equals(name))
        {
            if (!source.isPlaying) source.Play();

            Debug.Log("WireBox 활성화");
            fx_Success.SetActive(false);
            fx_Failed.SetActive(false);
            UI.semiStaticUI.WireBox(Success);
            return true;
        }
        return false;
    }

    //성공했을 경우
    void Success(bool success)
    {
        if(success)
        {
            // 안보이기
            gameObject.layer = 0;
            foreach(MeshRenderer filter in GetComponentsInChildren<MeshRenderer>())
            {
                filter.enabled = false;
            }

            //펑 사운드
            source.PlayOneShot(clip_Success, DataSet.Instance.SettingValue.Volume * 1.7f);

            //클리어 파티클
            fx_Success.SetActive(true);

            //기본 파티클 제거
            fx_Default.SetActive(false);

            //코루틴으로 오브젝트를 제거합니다.
            IEnumerator enumerator = Delay();
            CoroutineHandler.StartCorou(enumerator);
        }
        else
        {
            //파직 사운드
            source.PlayOneShot(clip_Failed, DataSet.Instance.SettingValue.Volume * 1.7f);

            //파직 파티클
            fx_Failed.SetActive(true);
        }
    }

    IEnumerator Delay()
    {
        Destroy(fx_Default);


        yield return new WaitForSeconds(1.0f);

        //클리어가 모두 완료 되었다면?
        if (WireBox.SuccessCount > 2)
        {
            Debug.LogWarning("배전함 영혼을 활성화합니다.");

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
