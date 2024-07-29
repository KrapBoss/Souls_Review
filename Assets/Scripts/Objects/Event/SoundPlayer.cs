using CustomUI;
using UnityEngine;

/// <summary>
/// ������ ����ݴϴ�.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : ExpandObject
{
    static short PlayCount;

    [Header("0�� �÷��� / 1�� ���� / 2~ ���")]
    public AudioClip[] clip_Bgm;

    AudioSource _source;

    bool play;

    public override bool Func(string name = null)
    {
        if (clip_Bgm.Length >2)
        {
            Debug.Log("������ ����մϴ�.");

            

            //�÷��� ���̴�.
            if (play)
            {
                _source.Stop();
                _source.PlayOneShot(clip_Bgm[1], DataSet.Instance.SettingValue.BGMVoulme * 0.7f);

                play = false;
            }
            else
            {
                _source.PlayOneShot(clip_Bgm[0], DataSet.Instance.SettingValue.BGMVoulme * 0.7f);
                _source.clip = clip_Bgm[Random.Range(2, clip_Bgm.Length)];
                _source.volume = DataSet.Instance.SettingValue.BGMVoulme * 0.6f;
                _source.loop = true;
                _source.Play();

                play = true; 
                
                PlayCount++;
                if (PlayCount < 3)
                {
                    UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", $"SoundPlayer{Random.Range(0, 3)}"), 5.0f, true);
                    UI.staticUI.ShowLine();
                }
            }

            return true;
        }
        else
        {
            Debug.Log("������ �����ϴ�.");
            return false;
        }
    }

    protected override void Init()
    {
        if (!_source) _source = GetComponent<AudioSource>();

        Setting.Action_SoundChanger += SoundChange;

        PlayerEvent.instance.Action_Init += SoundStop;

        base.Init();
    }

    public void SoundChange()
    {
        _source.volume = DataSet.Instance.SettingValue.BGMVoulme * 0.6f;
    }
    public void SoundStop()
    {
        _source.Stop();
    }

    private void OnDestroy()
    {
        Setting.Action_SoundChanger -= SoundChange;
        PlayerEvent.instance.Action_Init -= SoundStop;
    }
}
