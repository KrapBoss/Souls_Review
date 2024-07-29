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

//�� ���� ���� �� ȸ�� ���� ������ �͵��� �ִ밢�� ������ �� ����.
[System.Serializable]
public class BodyPart
{
    public string partName;

    //���� ȿ���� �ٰ��ΰ�?
    public bool CantTremble;
    //ȸ������ ������Ʈ �� �� �������� ȸ������ ������ ���ΰ���?
    public bool RandomRotation;
    public bool OnlyTremble;

    //�� ������ ���� �κ�
    public Parts[] parts;
    //�ʱ� ���� �� ȸ������ ����
    Vector3[] defaultRotates;
    //���� ����� ������ �����Ѵ�.
    Vector3[] changeRotates;
    //�� ������Ʈ�� ���۰��� �⺻������ �����.
    public void Initialize()
    {
        defaultRotates = parts.Select((g)=>g.part.transform.localRotation.eulerAngles).ToArray();

        changeRotates = new Vector3[parts.Length];
    }

    //�� ������ ���� ���� ������ ���� ������ �����Ѵ�.
    public void UpdatePartNextRotate()
    {
        if (parts.Length == 0 || OnlyTremble) return;

        for (int i = 0; i < parts.Length; i++)
        {
            //������ ��ġ���� �����ϱ� ���� ��
            int[] randomAxis = new int[3] { -1, -1, -1 };

            //1. ����ġ�� �ٸ��� �ϱ� ���� ������ ������ ��ǥ�� �����Ѵ�.
            for (int index = 0; index < randomAxis.Length; index++)
            {
                while (true)
                {
                    //�������� ����
                    int axis = Random.Range(0, randomAxis.Length);

                    //�ش� ���� �������� ���� ��쿡�� ����
                    if (!randomAxis.Contains(axis))
                    {
                        randomAxis[index] = axis;
                        break;
                    }
                }
            }

            //2. X Y Z�� ���� ����ġ�� �����Ѵ�.
            Vector3 rotateW = Vector3.zero;
            rotateW[randomAxis[0]] = Random.Range(0.5f, 0.8f);
            rotateW[randomAxis[1]] = Random.Range(0.2f, 1.0f - rotateW[randomAxis[0]]);
            rotateW[randomAxis[2]] = (1.0f - rotateW[randomAxis[0]] - rotateW[randomAxis[1]]);
            rotateW[0] *= (Random.Range(0, 2) == 0 ? 1 : -1);
            rotateW[1] *= (Random.Range(0, 2) == 0 ? 1 : -1);
            rotateW[2] *= (Random.Range(0, 2) == 0 ? 1 : -1);

            //Debug.Log($"{rotateW}");

            //3. ������ ������ ������ ������ ȸ������ �����Ѵ�.
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

            //4. ������Ʈ �����̼� ����
            //parts[i].part.localRotation = Quaternion.Euler(defaultRotates[i] + rotateW);
            changeRotates[i] = rotateW;
            //Debug.Log($"{rotateW} == {changeRotates[i]} / Euler = {Quaternion.Euler(rotateW)}");

        }

        //Debug.Log($"GhostEffect : {partName}�� ȸ���� ����");
    }

    //����� ȸ������ �����Ѵ�.
    public void UpdatePartRotate(float rotateRatio)
    {
        if (parts.Length == 0 || OnlyTremble) return;
        for (int i = 0; i < parts.Length; i++)
        {
            //Debug.Log($"{changeRotates[i]} * {rotateRatio} = {(changeRotates[i] * rotateRatio)}");
            parts[i].part.localRotation = Quaternion.Euler(defaultRotates[i] + (changeRotates[i] * rotateRatio));
        }
    }

    //�� ��Ʈ���� ������ �� ƨ��� ȿ��
    public void TrembleBody(float rot)
    {
        if (parts.Length == 0 || CantTremble) return;

        Vector3 rotate = Vector3.zero;
        rotate.x = Random.Range(0.0f, 1.0f);
        rotate.y = Random.Range(0.0f, 1.0f - rotate.x);
        rotate.z = 1.0f - rotate.x - rotate.y;
        rotate *= rot * (Random.Range(0,2) ==0? -1 :1);

        //�㸮 ���� ���
        parts[0].part.localRotation = Quaternion.Euler(defaultRotates[0] + changeRotates[0] + rotate);
    }
}

/// <summary>
/// ���������� �������� �������� ������ ��
/// </summary>
public class GhostEffect : MonoBehaviour
{

    [SerializeField] BodyPart[] bodyParts;

    //�ٽ� ����� ���� ���ð�
    float grudgeTimeout =0.0f;

    AudioSource source;

    //���� ũ�� �ִ� 2.0f
    [SerializeField] float sound_GrudgeVolume;
    //���� Ŭ�� �迭
    [SerializeField] AudioClip[] sound_Grudge;

    //���� ũ�� �ִ� 2.0f
    [SerializeField] float sound_BoneBreakingVolume;
    [SerializeField] AudioClip[] sound_BoneBreaking;
    [SerializeField] AudioClip[] sound_boneBreakingSmall;

    [Space]
    public float rot;

    //������ �� ���ΰ�?
    public bool tremble = true;
    private void Awake()
    {
        for (int i = 0; i < bodyParts.Length; i++) bodyParts[i].Initialize();

        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    private void Update()
    {
        //���� ȿ��
        if (tremble)
        {
            for (int i = 0; i < bodyParts.Length; i++) bodyParts[i].TrembleBody(rot);
        }

        SoundCheck();
    }

    //������ �⺻���� ���鸣 �����մϴ�.
    void SoundCheck()
    {
        //���尡 ������ ���
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

            //����� �Ҹ� ���� ���̸�ŭ ��� �� �ٽ� ���
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

    //�������� �ͽ��� �������� �������� �����ش�.
    //�÷��̾ ���� �ʴ� ����
    public void Movement(Vector3 _position, Vector3 _forward)
    {
        //Debug.Log("�ͽ��� �����Դϴ�.");
        StartCoroutine(MovementCroutine(_position , _forward));
    }

    WaitForSeconds wait_Movement = new WaitForSeconds(0.2f);
    IEnumerator MovementCroutine(Vector3 _position, Vector3 _forward)
    {
        //���� ���� ����
        UpdataPartNextRotate();

        //���� ������ 0.5������ŭ ����
        UpdatePartRotate(0.8f);

        if(sound_BoneBreaking.Length>0)
            source.PlayOneShot(sound_BoneBreaking[Random.Range(0, sound_BoneBreaking.Length)], sound_BoneBreakingVolume * DataSet.Instance.SettingValue.Volume);
        

        yield return wait_Movement;
        transform.position = _position;
        transform.forward = _forward;

        yield return wait_Movement;
        //���� ������ 1.0f������ŭ ����
        UpdatePartRotate(1.2f);
        if (sound_BoneBreaking.Length > 0)
            source.PlayOneShot(sound_boneBreakingSmall[Random.Range(0, sound_boneBreakingSmall.Length)], sound_BoneBreakingVolume * DataSet.Instance.SettingValue.Volume);
    }

    //����� �Ҹ�
    float GrudgeSound()
    {
        int n = Random.Range(0, sound_Grudge.Length);
        source.PlayOneShot(sound_Grudge[n], sound_GrudgeVolume * DataSet.Instance.SettingValue.Volume);
        return sound_Grudge[n].length;
    }
}
