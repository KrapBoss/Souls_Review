using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class SoundNoName
{
    public AudioClip clip;
    public float volume = 1.0f;
}

[System.Serializable]
public class Sound
{
    public string name = null;
    public AudioClip clip;
    public float volume = 1.0f;
}
[System.Serializable]
public class DistanceSound
{
    public Sound sound;
    public float heardDistance; //해당 소리를 들을 수 있는 범위
    [Range(0.0f,1.0f)]public float minVolume;     //거리가 0이 되었을 때도 들리는 최소 소리 0~1;
}
[System.Serializable]
public class FootSound
{
    public string name;
    public AudioClip[] clip_foots;
    public float volume_foot;
    public AudioClip clip_jump;
    public float volume_jump;
}
public class AudioManager : MonoBehaviour
{
    //Get instace to Singleton;
    public static AudioManager instance;

    [Header("FOOT 사운드 설정")]
    public FootSound[] footSounds;
    public AudioSource[] footSource; //사운드 재생을 위한 리소스
    private int _foot_clipIdx = 0;    // 사운드 중복을 예방
    private float _foot_volume = 0.5f;//상태에 따른 발자국 소리가 다르기 때문

    [Space]
    [Header("배경음")]
    public Sound[] bgmSound;
    public AudioSource[] BgmSources = new AudioSource[2];
    string currentBgmName = "";

    [Space]
    [Header("효과음")]// Sound에 따른 직접 생성
    public Sound[] effectiveSound;
    AudioSource[] effectiveSource;

    [Space]
    [Header("거리에 따른 효과음")]// Sound에 따른 직접 생성
    public DistanceSound[] distanceSound;
    AudioSource[] distanceSource;

    [Space]
    [Header("UI 효과음")]
    public Sound[] uiSound;
    AudioSource[] uiSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        Destroy(instance);
        instance = null;
        Setting.Action_SoundChanger -= SoundChangeEvent;
    }

    private void Start()
    {
        //오디오 할당
        GameObject _temp_go = transform.Find("EffectiveSound").gameObject;

        BgmSources = transform.Find("BGMSound").GetComponents<AudioSource>();

        effectiveSource = new AudioSource[effectiveSound.Length];
        for (int i =0 ; i< effectiveSound.Length; i++)
        {
            AudioSource _source = _temp_go.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.clip = effectiveSound[i].clip;
            effectiveSource[i] = _source;
        }


        _temp_go = transform.Find("DistanceSound").gameObject;
        distanceSource = new AudioSource[distanceSound.Length];
        for(int i = 0 ; i< distanceSound.Length; i++)
        {
            AudioSource _source = _temp_go.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.clip = distanceSound[i].sound.clip;
            distanceSource[i] = _source;
        }

        _temp_go = transform.Find("UISound").gameObject;
        uiSource = new AudioSource[uiSound.Length];
        for(int i = 0 ;i< uiSound.Length; i++)
        {
            AudioSource _source = _temp_go.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.clip = uiSound[i].clip;
            uiSource[i] = _source;
        }

        //사운드 변경 이벤트 할당
        Setting.Action_SoundChanger += SoundChangeEvent;
    }

    //사운드 재 설정 후 적용을 위함
    public void SoundChangeEvent()
    {
        Debug.Log("사운드 매니저 사운드가 변경됩니다.");

        //BGM
        BgmSources[_bgmSourceIndex].volume = DataSet.Instance.SettingValue.BGMVoulme * bgmSound[currentBGMIndex].volume;
    }


    #region Foot
    public void PlayFootSound(string name, bool jump =false)
    {
        for (int i = 0; i < footSounds.Length; i++)
        {
            if (footSounds[i].name.Equals(name))
            {
                //점프 사운드인가?
                if (jump)
                {
                    FootSoundPlay(footSounds[i].clip_jump, footSounds[i].volume_jump);
                    return;
                }

                FootSoundPlay(footSounds[i].clip_foots[Random.Range(0, footSounds[i].clip_foots.Length)], footSounds[i].volume_foot);
                return;
            }
        }

        Debug.LogWarning("FOOT 사운드가 없습니다.");
    }

    void FootSoundPlay(AudioClip _clip , float _volume)
    {
        footSource[_foot_clipIdx].clip = _clip;
        footSource[_foot_clipIdx].volume = _volume * _foot_volume * DataSet.Instance.SettingValue.Volume;
        footSource[_foot_clipIdx].Play();
        _foot_clipIdx = ((_foot_clipIdx + 1) % footSource.Length);
    }

    public void SetFootVolume(float _volume)   // 사운드를 전체적으로 줄인다.
    {
        _foot_volume = _volume;
    }
    #endregion


    #region BGM
    int _bgmSourceIndex = 0;//현재 재생되고 있는 BGM의 인덱스 번호를 나타낸다.
    int currentBGMIndex;    //현재 인덱스
    public void PlayBGMSound(string name)
    {
        if (currentBgmName.Equals(name))
            return;//기존 사운드와 같다면, 재생시키지 않음

        for (int i = 0; i < bgmSound.Length; i++)
        {
            if (bgmSound[i].name.Equals(name))
            {
                //Debug.Log($"BGM 사운드를 찾았습니다. {bgmSound[i].name} == {name}");

                if(currentBgmName.Equals(""))//만약 현재 배경음이 비어있다면? 현재 인덱스로 그냥 재생
                {
                    BGMAllStop();

                    currentBGMIndex = i;
                    StartCoroutine(BGMFadeIn(i));
                }
                else // 배경음이 이미 재생되고 있는게 있다면?
                {
                    StopBGMFadeOut();
                    _bgmSourceIndex = ++_bgmSourceIndex % 2; // 앞에 있던 값을 먼저 중지시킨 후에 재생한다.
                    StartCoroutine(BGMFadeIn(i));
                }

                currentBgmName = bgmSound[i].name; //현재 재생되는 BGM의 이름을 넣는다.

                return;
            }
        }

        Debug.LogWarning($"BGM 사운드가 없습니다. {name}");
    }
    
    IEnumerator BGMFadeIn(int _soundIndex)  //BGM 사운드 FadeIns
    {
        Debug.LogWarning($"BGM 사운드를 재생합니다. 현재 사운드 트랙 인덱스 {_soundIndex}");

        float t = 0;

        BgmSources[_bgmSourceIndex].volume = t;
        BgmSources[_bgmSourceIndex].clip = bgmSound[_soundIndex].clip;
        BgmSources[_bgmSourceIndex].Play();

        while (t < 1.0f)
        {
            BgmSources[_bgmSourceIndex].volume = t* DataSet.Instance.SettingValue.BGMVoulme * bgmSound[_soundIndex].volume;
            t += Time.deltaTime*0.5f;
            yield return null;
        }

        BgmSources[_bgmSourceIndex].volume = DataSet.Instance.SettingValue.BGMVoulme * bgmSound[_soundIndex].volume;
    }

    public void StopBGMFadeOut() //현재 재생 중인 배경음을 중지시킨다.
    {
        if (currentBgmName.Equals("")) return;  //이미 비어있다면, 실행하지 않는다.
        StartCoroutine(StopBGMFadeOutCroutine(_bgmSourceIndex));
    }
    IEnumerator StopBGMFadeOutCroutine(int _currentSourceIndex)    //배경음을 FadeOut 시킨다.
    {
        Debug.LogWarning($"BGM 사운드를 중지시킵니다. / 현재 소스 인덱스 {_currentSourceIndex}");

        currentBgmName = ""; // 사운드가 비었음을 나타낸다.
        float t = BgmSources[_currentSourceIndex].volume;

        while(t > 0.0f)
        {
            BgmSources[_currentSourceIndex].volume = t;
            t -= Time.unscaledDeltaTime * 0.7f;
            yield return null;
        }

        BgmSources[_currentSourceIndex].Stop();
    }

    void BGMAllStop() //모든 BGM을 종료
    {
        StopAllCoroutines();
        for(int i = 0; i < BgmSources.Length; i++)
        {
            BgmSources[i].Stop();
        }
    }
    #endregion

    #region EffectiveSound
    public void PlayEffectiveSound(string name, float volume, bool overwrite = false)
    {
        for (int i = 0; i < effectiveSound.Length; i++)
        {
            if (effectiveSound[i].name.Equals(name))
            {
                effectiveSource[i].volume = volume * effectiveSound[i].volume * DataSet.Instance.SettingValue.Volume;

                if(overwrite) effectiveSource[i].Play();
                else
                if (!effectiveSource[i].isPlaying) effectiveSource[i].Play();

                return;
            }
        }
        Debug.Log($"AUDIOMANAGER :: EFFECTIVE : {name} 이 없습니다.");
    }
    public void StopEffectiveSound(string name)
    {
        for (int i = 0; i < effectiveSound.Length; i++)
        {
            if (effectiveSound[i].name.Equals(name))
            {
                effectiveSource[i].Stop();
            }
        }
    }
    #endregion

    #region DistanceSound
    //매개변수 : 검색 이름 / 기본적으로 적용할 볼륨 크기 = 1 / 소리 근원지  / 층수 차이가나도 사운드를 들려줄 것인가
    //지속적으로 매 프레임 호출하면 안되고, 순간적인 이벤트가 발생했을 때만 사용
    public float PlayDistanceSound(string name, float volume, Vector3 from, bool floorGap = false)
    {
        for (int i = 0; i < distanceSound.Length; i++)
        {
            if (distanceSound[i].sound.name.Equals(name))
            {
                //층수 차이가 나면 사운드를 들려주지 않겠다.
                if (!floorGap)
                {
                    //층수차이 체크
                    if (!Calculator.GetBetweenHeight(from, PlayerEvent.instance.GetPosition()))
                    {
                        return -1;
                    }
                }
                //현재 위치 값 차이를 구한다.
                float distance = Vector3.Distance(from, PlayerEvent.instance.GetPosition());

                if (distance > distanceSound[i].heardDistance) return -1;

                //플레이어와의거리를 들을 수 있는 범위로 나누어 현재 거리 값의 크기를 0~1사이로 구한다.
                float disVolume = 1 - Mathf.Clamp(distance / distanceSound[i].heardDistance, 0.0f, 1.0f);
                if (disVolume < distanceSound[i].minVolume) { disVolume = distanceSound[i].minVolume; }

                distanceSource[i].volume = volume * distanceSound[i].sound.volume * disVolume * DataSet.Instance.SettingValue.Volume;
                //Debug.Log($"거리사운드 볼륨 :: {disVolume} // {distanceSource[i].volume}");

                if (!distanceSource[i].isPlaying) distanceSource[i].Play();

                return distanceSource[i].clip.length;
            }
        }

        Debug.Log($"DistanceSound :: {name} 사운드가 존재하지 않음");
        return -1;
    }

    public void StopDistanceSound(string name)
    {
        for (int i = 0; i < distanceSound.Length; i++)
        {
            if (distanceSound[i].sound.name.Equals(name))
            {
                distanceSource[i].Stop();
            }
        }
    }
    #endregion


    #region UISound
    public void PlayUISound(string name, float volume)
    {
        for (int i = 0; i < uiSound.Length; i++)
        {
            if (uiSound[i].name.Equals(name))
            {
                uiSource[i].volume = volume * uiSound[i].volume * DataSet.Instance.SettingValue.Volume;
                if (!uiSource[i].isPlaying)
                {
                    uiSource[i].Play();
                    return;
                }
            }
        }
        Debug.Log($"{name} 에 해당하는 사운드가 없습니다.");
    }
    #endregion
}