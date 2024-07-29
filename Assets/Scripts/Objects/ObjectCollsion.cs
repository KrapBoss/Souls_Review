using UnityEngine;

//������Ʈ �浹 �� �Ҹ��� ���� �ϱ� ���� ���̴�.
//�Ҹ��� ���� �̺�Ʈ �Լ��� �Ҹ��� �� ���� �����Ѵ�.
public class ObjectCollsion : MonoBehaviour
{
    AudioSource _source;
    [Tooltip("������ ���� �浹 �� �߻��ϴ� ���� ��������")]
    public float Distance = 3.0f; // defaultSetting =2

    [Header("Ŭ���� �ִ´ٸ�, �ڵ����� �浹 �� �ش� Ŭ���� ����ȴ�. �� ������, �⺻ AudioSource�� ���")]
    public AudioClip clip;//�浹 �� �߻��ϴ� ����

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

            //Debug.Log("�浹 ���� " + gameObject.name);

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
