using UnityEngine;

//오브젝트 충돌 시 소리가 나게 하기 위한 것이다.
//소리가 나면 이벤트 함수에 소리가 난 곳을 전달한다.
public class ObjectCollsion : MonoBehaviour
{
    AudioSource _source;
    [Tooltip("물건을 던져 충돌 시 발생하는 사운드 감지범위")]
    public float Distance = 3.0f; // defaultSetting =2

    [Header("클립을 넣는다면, 자동으로 충돌 시 해당 클립이 재생된다. 안 넣으면, 기본 AudioSource가 재생")]
    public AudioClip clip;//충돌 시 발생하는 소음

    public bool SoundDelay = true;

    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        if(source == null ) 
        { 
            source= transform.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        _source = source;

        _source.minDistance = 0.5f;
        _source.maxDistance = Distance;

        if (SoundDelay)
        {
            CanPlay = false;
            Invoke("SoundTimeOut", 1.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"{collision.gameObject.name}");

        if( collision.gameObject.layer != DataSet.Instance.Layers.ObjectNonCollsion)
        {
            PlaySound();
        }
    }


    bool CanPlay = false;

    void PlaySound()
    {
        if (CanPlay)
        {
            CanPlay = false;

            EventManager.instance.EventSound(transform, Distance);

            //Debug.Log("충돌 사운드 " + gameObject.name);

            _source.volume = DataSet.Instance.SettingValue.Volume;

            if (clip != null)_source.PlayOneShot(clip);
            else { _source.Play(); }
            
            Invoke("SoundTimeOut", 0.3f);
        }
    }

    void SoundTimeOut()
    {
        CanPlay = true;
    }
}
