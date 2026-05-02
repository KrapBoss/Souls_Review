using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct  Parts
{
    public Transform part;
    public float degree;
}

//각 몸의 부위 별 회전 각과 연관된 것들의 최대각을 지정할 수 있음.
[System.Serializable]
public class BodyPart
{
    public string partName;

    //감전 효과를 줄것인가?
    public bool CantTremble;
    //회전값을 업데이트 할 때 랜덤으로 회전값을 지정할 것인가요?
    public bool RandomRotation;
    public bool OnlyTremble;

    //각 파츠에 대한 부분
    public Parts[] parts;
    //초기 시작 시 회전값을 저장
    Vector3[] defaultRotates;
    //다음 변경될 동작을 저장한다.
    Vector3[] changeRotates;
    //각 로테이트의 시작값을 기본값으로 맞춘다.
    public void Initialize()
    {
        defaultRotates = parts.Select((g)=>g.part.transform.localRotation.eulerAngles).ToArray();

        changeRotates = new Vector3[parts.Length];
    }

    //각 파츠에 대해 다음 각도를 다음 각도를 저장한다.
    public void UpdatePartNextRotate()
    {
        if (parts.Length == 0 || OnlyTremble) return;

        for (int i = 0; i < parts.Length; i++)
        {
            //랜덤한 위치값을 지정하기 위한 값
            int[] randomAxis = new int[3] { -1, -1, -1 };

            //1. 가중치를 다르게 하기 위해 랜덤한 순서로 좌표를 선정한다.
            for (int index = 0; index < randomAxis.Length; index++)
            {
                while (true)
                {
                    //랜덤값을 선정
                    int axis = Random.Range(0, randomAxis.Length);

                    //해당 값이 존재하지 않을 경우에만 지정
                    if (!randomAxis.Contains(axis))
                    {
                        randomAxis[index] = axis;
                        break;
                    }
                }
            }

            //2. X Y Z에 대한 가중치를 설정한다.
            Vector3 rotateW = Vector3.zero;
            rotateW[randomAxis[0]] = Random.Range(0.5f, 0.8f);
            rotateW[randomAxis[1]] = Random.Range(0.2f, 1.0f - rotateW[randomAxis[0]]);
            rotateW[randomAxis[2]] = (1.0f - rotateW[randomAxis[0]] - rotateW[randomAxis[1]]);
            rotateW[0] *= (Random.Range(0, 2) == 0 ? 1 : -1);
            rotateW[1] *= (Random.Range(0, 2) == 0 ? 1 : -1);
            rotateW[2] *= (Random.Range(0, 2) == 0 ? 1 : -1);

            //Debug.Log($"{rotateW}");

            //3. 지정된 값에서 랜덤한 값으로 회전값을 지정한다.
            float r = 0.0f;
            if (RandomRotation)
            {
                r = Random.Range(parts[i].degree * 0.3f, parts[i].degree);
            }
            else
            {
                r = parts[i].degree;
            }
            rotateW = rotateW * r *  1.2f;

            //4. 오브젝트 로테이션 지정
            //parts[i].part.localRotation = Quaternion.Euler(defaultRotates[i] + rotateW);
            changeRotates[i] = rotateW;
            //Debug.Log($"{rotateW} == {changeRotates[i]} / Euler = {Quaternion.Euler(rotateW)}");

        }

        //Debug.Log($"GhostEffect : {partName}의 회전값 적용");
    }

    //변경된 회전값을 적용한다.
    public void UpdatePartRotate(float rotateRatio)
    {
        if (parts.Length == 0 || OnlyTremble) return;
        for (int i = 0; i < parts.Length; i++)
        {
            //Debug.Log($"{changeRotates[i]} * {rotateRatio} = {(changeRotates[i] * rotateRatio)}");
            parts[i].part.localRotation = Quaternion.Euler(defaultRotates[i] + (changeRotates[i] * rotateRatio));
        }
    }

    //각 파트들이 감전된 듯 튕기는 효과
    public void TrembleBody(float rot)
    {
        if (parts.Length == 0 || CantTremble) return;

        Vector3 rotate = Vector3.zero;
        rotate.x = Random.Range(0.0f, 1.0f);
        rotate.y = Random.Range(0.0f, 1.0f - rotate.x);
        rotate.z = 1.0f - rotate.x - rotate.y;
        rotate *= rot * (Random.Range(0,2) ==0? -1 :1);

        //허리 같은 경우
        parts[0].part.localRotation = Quaternion.Euler(defaultRotates[0] + changeRotates[0] + rotate);
    }
}

/// <summary>
/// 실질적으로 보여지는 움직임을 지정한 것
/// </summary>
public class GhostEffect : MonoBehaviour
{

    [SerializeField] BodyPart[] bodyParts;

    //다시 재생을 위한 대기시간
    float grudgeTimeout =0.0f;

    AudioSource source;

    //사운드 크기 최대 2.0f
    [SerializeField] float sound_GrudgeVolume;
    //사운드 클립 배열
    [SerializeField] AudioClip[] sound_Grudge;

    //사운드 크기 최대 2.0f
    [SerializeField] float sound_BoneBreakingVolume;
    [SerializeField] AudioClip[] sound_BoneBreaking;
    [SerializeField] AudioClip[] sound_boneBreakingSmall;

    [Space]
    public float rot;

    //떨림을 줄 것인가?
    public bool tremble = true;
    private void Awake()
    {
        for (int i = 0; i < bodyParts.Length; i++) bodyParts[i].Initialize();

        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    private void Update()
    {
        //흔들기 효과
        if (tremble)
        {
            for (int i = 0; i < bodyParts.Length; i++) bodyParts[i].TrembleBody(rot);
        }

        SoundCheck();
    }

    //몬스터의 기본적인 사운들르 지정합니다.
    void SoundCheck()
    {
        //사운드가 존재할 경우
        if (sound_Grudge.Length > 0)
        {
            if (Calculator.GetBetweenHeight(transform.position, PlayerEvent.instance.GetPosition()))
            {
                source.volume = 1.0f;
            }
            else
            {
                source.volume = 0.1f;
            }

            //끄어억 소리 사운드 길이만큼 대기 후 다시 재생
            grudgeTimeout -= Time.deltaTime;
            if (grudgeTimeout < 0.0f)
            {
                grudgeTimeout = GrudgeSound();
            }
        }
    }

    void UpdataPartNextRotate() { for (int i = 0; i < bodyParts.Length; i++) bodyParts[i].UpdatePartNextRotate(); }
    void UpdatePartRotate(float ratio) { for (int i = 0; i < bodyParts.Length; i++) bodyParts[i].UpdatePartRotate(ratio); }

    public void UpdateNextMonement()
    {
        if (!source.isPlaying)
        {
            source.Play();
        }

        UpdataPartNextRotate();
        UpdatePartRotate(1);
    }

    //보여지는 귀신의 실질적인 움직임을 보여준다.
    //플레이어를 쫓지 않는 상태
    public void Movement(Vector3 _position, Vector3 _forward)
    {
        //Debug.Log("귀신이 움직입니다.");
        StartCoroutine(MovementCroutine(_position , _forward));
    }

    WaitForSeconds wait_Movement = new WaitForSeconds(0.2f);
    IEnumerator MovementCroutine(Vector3 _position, Vector3 _forward)
    {
        //다음 동작 설정
        UpdataPartNextRotate();

        //다음 동작의 0.5비율만큼 변경
        UpdatePartRotate(0.8f);

        if(sound_BoneBreaking.Length>0)
            source.PlayOneShot(sound_BoneBreaking[Random.Range(0, sound_BoneBreaking.Length)], sound_BoneBreakingVolume * DataSet.Instance.SettingValue.Volume);
        

        yield return wait_Movement;
        transform.position = _position;
        transform.forward = _forward;

        yield return wait_Movement;
        //다음 동작의 1.0f비율만큼 변경
        UpdatePartRotate(1.2f);
        if (sound_BoneBreaking.Length > 0)
            source.PlayOneShot(sound_boneBreakingSmall[Random.Range(0, sound_boneBreakingSmall.Length)], sound_BoneBreakingVolume * DataSet.Instance.SettingValue.Volume);
    }

    //끄어억 소리
    float GrudgeSound()
    {
        int n = Random.Range(0, sound_Grudge.Length);
        source.PlayOneShot(sound_Grudge[n], sound_GrudgeVolume * DataSet.Instance.SettingValue.Volume);
        return sound_Grudge[n].length;
    }
}
