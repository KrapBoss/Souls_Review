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
    public float heardDistance; //�ش� �Ҹ��� ���� �� �ִ� ����
    [Range(0.0f,1.0f)]public float minVolume;     //�Ÿ��� 0�� �Ǿ��� ���� �鸮�� �ּ� �Ҹ� 0~1;
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

    [Header("FOOT ���� ����")]
    public FootSound[] footSounds;
    public AudioSource[] footSource; //���� ����� ���� ���ҽ�
    private int _foot_clipIdx = 0;    // ���� �ߺ��� ����
    private float _foot_volume = 0.5f;//���¿� ���� ���ڱ� �Ҹ��� �ٸ��� ����

    [Space]
    [Header("�����")]
    public Sound[] bgmSound;
    public AudioSource[] BgmSources = new AudioSource[2];
    string currentBgmName = "";

    [Space]
    [Header("ȿ����")]// Sound�� ���� ���� ����
    public Sound[] effectiveSound;
    AudioSource[] effectiveSource;

    [Space]
    [Header("�Ÿ��� ���� ȿ����")]// Sound�� ���� ���� ����
    public DistanceSound[] distanceSound;
    AudioSource[] distanceSource;

    [Space]
    [Header("UI ȿ����")]
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
        //����� �Ҵ�
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

        //���� ���� �̺�Ʈ �Ҵ�
        Setting.Action_SoundChanger += SoundChangeEvent;
    }

    //���� �� ���� �� ������ ����
    public void SoundChangeEvent()
    {
        Debug.Log("���� �Ŵ��� ���尡 ����˴ϴ�.");

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
                //���� �����ΰ�?
                if (jump)
                {
                    FootSoundPlay(footSounds[i].clip_jump, footSounds[i].volume_jump);
                    return;
                }

                FootSoundPlay(footSounds[i].clip_foots[Random.Range(0, footSounds[i].clip_foots.Length)], footSounds[i].volume_foot);
                return;
            }
        }

        Debug.LogWarning("FOOT ���尡 �����ϴ�.");
    }

    void FootSoundPlay(AudioClip _clip , float _volume)
    {
        footSource[_foot_clipIdx].clip = _clip;
        footSource[_foot_clipIdx].volume = _volume * _foot_volume * DataSet.Instance.SettingValue.Volume;
        footSource[_foot_clipIdx].Play();
        _foot_clipIdx = ((_foot_clipIdx + 1) % footSource.Length);
    }

    public void SetFootVolume(float _volume)   // ���带 ��ü������ ���δ�.
    {
        _foot_volume = _volume;
    }
    #endregion


    #region BGM
    int _bgmSourceIndex = 0;//���� ����ǰ� �ִ� BGM�� �ε��� ��ȣ�� ��Ÿ����.
    int currentBGMIndex;    //���� �ε���
    public void PlayBGMSound(string name)
    {
        if (currentBgmName.Equals(name))
            return;//���� ����� ���ٸ�, �����Ű�� ����

        for (int i = 0; i < bgmSound.Length; i++)
        {
            if (bgmSound[i].name.Equals(name))
            {
                //Debug.Log($"BGM ���带 ã�ҽ��ϴ�. {bgmSound[i].name} == {name}");

                if(currentBgmName.Equals(""))//���� ���� ������� ����ִٸ�? ���� �ε����� �׳� ���
                {
                    BGMAllStop();

                    currentBGMIndex = i;
                    StartCoroutine(BGMFadeIn(i));
                }
                else // ������� �̹� ����ǰ� �ִ°� �ִٸ�?
                {
                    StopBGMFadeOut();
                    _bgmSourceIndex = ++_bgmSourceIndex % 2; // �տ� �ִ� ���� ���� ������Ų �Ŀ� ����Ѵ�.
                    StartCoroutine(BGMFadeIn(i));
                }

                currentBgmName = bgmSound[i].name; //���� ����Ǵ� BGM�� �̸��� �ִ´�.

                return;
            }
        }

        Debug.LogWarning($"BGM ���尡 �����ϴ�. {name}");
    }
    
    IEnumerator BGMFadeIn(int _soundIndex)  //BGM ���� FadeIns
    {
        Debug.LogWarning($"BGM ���带 ����մϴ�. ���� ���� Ʈ�� �ε��� {_soundIndex}");

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

    public void StopBGMFadeOut() //���� ��� ���� ������� ������Ų��.
    {
        if (currentBgmName.Equals("")) return;  //�̹� ����ִٸ�, �������� �ʴ´�.
        StartCoroutine(StopBGMFadeOutCroutine(_bgmSourceIndex));
    }
    IEnumerator StopBGMFadeOutCroutine(int _currentSourceIndex)    //������� FadeOut ��Ų��.
    {
        Debug.LogWarning($"BGM ���带 ������ŵ�ϴ�. / ���� �ҽ� �ε��� {_currentSourceIndex}");

        currentBgmName = ""; // ���尡 ������� ��Ÿ����.
        float t = BgmSources[_currentSourceIndex].volume;

        while(t > 0.0f)
        {
            BgmSources[_currentSourceIndex].volume = t;
            t -= Time.unscaledDeltaTime * 0.7f;
            yield return null;
        }

        BgmSources[_currentSourceIndex].Stop();
    }

    void BGMAllStop() //��� BGM�� ����
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
        Debug.Log($"AUDIOMANAGER :: EFFECTIVE : {name} �� �����ϴ�.");
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
    //�Ű����� : �˻� �̸� / �⺻������ ������ ���� ũ�� = 1 / �Ҹ� �ٿ���  / ���� ���̰����� ���带 ����� ���ΰ�
    //���������� �� ������ ȣ���ϸ� �ȵǰ�, �������� �̺�Ʈ�� �߻����� ���� ���
    public float PlayDistanceSound(string name, float volume, Vector3 from, bool floorGap = false)
    {
        for (int i = 0; i < distanceSound.Length; i++)
        {
            if (distanceSound[i].sound.name.Equals(name))
            {
                //���� ���̰� ���� ���带 ������� �ʰڴ�.
                if (!floorGap)
                {
                    //�������� üũ
                    if (!Calculator.GetBetweenHeight(from, PlayerEvent.instance.GetPosition()))
                    {
                        return -1;
                    }
                }
                //���� ��ġ �� ���̸� ���Ѵ�.
                float distance = Vector3.Distance(from, PlayerEvent.instance.GetPosition());

                if (distance > distanceSound[i].heardDistance) return -1;

                //�÷��̾���ǰŸ��� ���� �� �ִ� ������ ������ ���� �Ÿ� ���� ũ�⸦ 0~1���̷� ���Ѵ�.
                float disVolume = 1 - Mathf.Clamp(distance / distanceSound[i].heardDistance, 0.0f, 1.0f);
                if (disVolume < distanceSound[i].minVolume) { disVolume = distanceSound[i].minVolume; }

                distanceSource[i].volume = volume * distanceSound[i].sound.volume * disVolume * DataSet.Instance.SettingValue.Volume;
                //Debug.Log($"�Ÿ����� ���� :: {disVolume} // {distanceSource[i].volume}");

                if (!distanceSource[i].isPlaying) distanceSource[i].Play();

                return distanceSource[i].clip.length;
            }
        }

        Debug.Log($"DistanceSound :: {name} ���尡 �������� ����");
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
        Debug.Log($"{name} �� �ش��ϴ� ���尡 �����ϴ�.");
    }
    #endregion
}