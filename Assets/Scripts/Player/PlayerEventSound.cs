using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//���⼭ ���ڱ� �Ҹ��� ���.
//���ڱ� �Ҹ��� ���带 ���������� �����Ͽ� ����ϴ� ������ ����Ѵ�.
public class PlayerEventSound : MonoBehaviour
{
    RaycastHit _hit;
    [FormerlySerializedAs("Distance_y")] public float distanceY = 2.0f; // ������ ��� �Ÿ�
    [FormerlySerializedAs("Layer")] public LayerMask layer;

    public float testRange;

    bool _isJump = false;    //������ ����Ǹ� �߰��� �Ҹ� ������

    [FormerlySerializedAs("_dead1")] [Header("�÷��̾� �ִϸ��̼� ���� ����")]
    public AudioClip dead1;
    [FormerlySerializedAs("_dead2")] public AudioClip dead2;
    [FormerlySerializedAs("_respawn1")] public AudioClip respawn1;

    [FormerlySerializedAs("clip_paint")] public AudioClip clipPaint;

    AudioSource _source;

    private IEnumerator Start()
    {
        _source = GetComponent<AudioSource>();
        yield return new WaitForSeconds(1.0f);  // ���� ���� ���� ��ġ ������ �Ѱ��ֱ� ����.
        FootIK();
    }

    public void FootIK()
    {
        //Debug.DrawRay(transform.position, Vector3.down * Distance_y,Color.blue, 0.3f);
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, distanceY, layer))
        {
            if (_isJump) { _isJump = false; return; }
            //Debug.Log($"���ڱ� �Ҹ��� ���� {_hit.transform.tag}");
            AudioManager.instance.PlayFootSound($"{_hit.transform.tag}");
        }
    }

    public void FootJump()
    {
        //Debug.DrawRay(transform.position, Vector3.down * Distance_y,Color.blue, 0.3f);
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, distanceY, layer))
        {
            //Debug.Log($"���� ���ڱ� �Ҹ��� ���� {Hit.transform.tag}");
            AudioManager.instance.PlayFootSound($"{_hit.transform.tag}", true);
            _isJump = true;
        }
    }

    public void SoundPaint()
    {
        _source.volume = DataSet.Instance.SettingValue.Volume;
        _source.clip = clipPaint;
        _source.Play();
    }

    public void DeadAnimation1()
    {
        _source.clip = dead1;
        _source.volume = DataSet.Instance.SettingValue.Volume * 1.5f;
        _source.Play();
    }
    public void DeadAnimation2()
    {
        _source.clip = dead2;
        _source.volume = DataSet.Instance.SettingValue.Volume;
        _source.Play();
    }
    public void RespawnAnimation()
    {
        _source.clip = respawn1;
        _source.volume = DataSet.Instance.SettingValue.Volume;
        _source.Play();
    }
}