using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//여기서 발자국 소리를 담당.
//발자국 소리는 사운드를 통합적으로 관리하여 담당하는 곳에서 출력한다.
public class PlayerEventSound : MonoBehaviour
{
    RaycastHit _hit;
    [FormerlySerializedAs("Distance_y")] public float distanceY = 2.0f; // 레이저 쏘는 거리
    [FormerlySerializedAs("Layer")] public LayerMask layer;

    public float testRange;

    bool _isJump = false;    //점프가 실행되면 발걸음 소리 딜레이

    [FormerlySerializedAs("_dead1")] [Header("플레이어 애니메이션 사운드 지정")]
    public AudioClip dead1;
    [FormerlySerializedAs("_dead2")] public AudioClip dead2;
    [FormerlySerializedAs("_respawn1")] public AudioClip respawn1;

    [FormerlySerializedAs("clip_paint")] public AudioClip clipPaint;

    AudioSource _source;

    private IEnumerator Start()
    {
        _source = GetComponent<AudioSource>();
        yield return new WaitForSeconds(1.0f);  // 현재 땅에 대한 위치 정보를 넘겨주기 위함.
        FootIK();
    }

    public void FootIK()
    {
        //Debug.DrawRay(transform.position, Vector3.down * Distance_y,Color.blue, 0.3f);
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, distanceY, layer))
        {
            if (_isJump) { _isJump = false; return; }
            //Debug.Log($"발자국 소리가 났음 {_hit.transform.tag}");
            AudioManager.instance.PlayFootSound($"{_hit.transform.tag}");
        }
    }

    public void FootJump()
    {
        //Debug.DrawRay(transform.position, Vector3.down * Distance_y,Color.blue, 0.3f);
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, distanceY, layer))
        {
            //Debug.Log($"점프 발자국 소리가 났음 {Hit.transform.tag}");
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