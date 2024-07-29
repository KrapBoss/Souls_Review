using CustomUI;
using UnityEngine;

/// <summary>
/// 음악을 들려줍니다.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : ExpandObject
{
    static short PlayCount;

    [Header("0번 플레이 / 1번 중지 / 2~ 브금")]
    public AudioClip[] clip_Bgm;

    AudioSource _source;

    bool play;

    public override bool Func(string name = null)
    {
        if (clip_Bgm.Length >2)
        {
            Debug.Log("음악을 재생합니다.");

            

            //플레이 중이다.
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
            Debug.Log("음악을 없습니다.");
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
