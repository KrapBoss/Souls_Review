using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Door : InteractionObjects
{
    [Space]
    [Header("���� ������ ����")]
    [Tooltip("Pivot at left? Door direction from Scale")]
    public bool AchorisLeftDoor;
    public bool OpenCloseCheck = false;

    public float defaultY;  // �⺻ Y �� ȸ�� ��ġ��
    [Range(0, 179)]
    public float maxY = 90.0f;      //�ִ� ȸ�� ������ Y�� ȸ����

    [Header("������ Ȯ���մϴ�.")]
    public bool isLocked = false;
    public float lockDeleyTime = 0.5f;

    public string soundName = null;

    public float soundRadius = 5.0f;

    short rotateMul;    //������ ���� ���ʹ��� �����ϱ� ���� �����ʹ� = 1; / ���ʹ� =-1;
    short frontBackMul; //�÷��̾ ���� �� ����� �� ���⿡ �ִ����� ��Ÿ����.
    short wheelDic;     //�� ���� �� = 1 / �Ʒ� = -1;

    BoxCollider collider_Default;

    IEnumerator CloseCroutine;
    IEnumerator OpenCroutine;

    Vector3 right; // ���� nomal ���͸� ���������� ���

    bool checkPlayerCollsion = false;

    IEnumerator lockDelayCroutine;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
#endif

    protected override void Init()
    {
        base.Init();

        TYPE = InteractionType.Continues;

        rotateMul = (short)(AchorisLeftDoor ? 1 : -1);

        defaultY = transform.rotation.eulerAngles.y;

        right = transform.right;

        gameObject.AddComponent<BoxCollider>().isTrigger = true;
        collider_Default = GetComponent<BoxCollider>();
    }

    public override bool GrabOn(Transform parent)
    {
        return false;//�׷� �Ұ���
    }

    public override bool GrabOff(Vector3 position, Vector3 force)
    {
        GRAB = false;
        return true;
    }

    public override void Func(float _wheelValue) // input wheel values
    {
        if (LockCheck()) { return; }
        AllStopCroutines();

        //if (checkPlayerCollsion) return;//�÷��̾�� �浹 �� �������� ����.

        if (_wheelValue <= 0.0f) wheelDic = -1;
        else if (_wheelValue > 0.0f) wheelDic = 1;

        CheckFrontBack();
        OpenCloseCheck = true;
        //���콺 �ٿ� ���� ȸ�� �� ����
        Quaternion e = transform.rotation * Quaternion.Euler(transform.up * _wheelValue * 250.0f * Time.deltaTime * rotateMul * frontBackMul);

        //Debug.Log(e.eulerAngles.y - transform.rotation.eulerAngles.y);

        transform.rotation = e;
        Clamp(e);
    }
    
    //���� �ݰų� ���� �뵵
    public override bool Func()
    {
        if (LockCheck()) { return false; }

        CheckFrontBack();

        //���� �ݴ´�.
        if (Mathf.Abs(transform.rotation.eulerAngles.y - defaultY) > 1.0f)// ���� ���� �⺻ ���� �ٸ��ٸ�?
        {
            if (CloseCroutine != null) return false;//������ ���� ���� �ݱ� ����� �ִ� ���
            AllStopCroutines();
            CloseCroutine = CloseDoorCroutine();
            StartCoroutine(CloseCroutine);
        }
        // ���� ����.
        else
        {
            if (OpenCroutine != null) return false;//������ ���� ���� ���� ����� �ִ� ���
            AllStopCroutines();
            OpenCroutine = OpenDoorCroutine();
            StartCoroutine(OpenCroutine);
        }

        //EventManager.instance.EventSound(transform, soundRadius);

        return true;
    }

    void CheckFrontBack()
    {
        float _theta = Quaternion.FromToRotation(right, (PlayerEvent.instance.transform.position - transform.position).normalized).eulerAngles.y;

        if (_theta <= 360.0f && _theta >= 180.0f)
        {
            frontBackMul = 1;// �� �տ� �ִ�.
        }
        else
        {
            frontBackMul = -1; // �� �ڿ� �ִ�.
        }
    }

    void AllStopCroutines()
    {
        StopAllCoroutines();
        OpenCroutine = null;
        CloseCroutine = null;
    }

    //���� �ݽ��ϴ�.
    IEnumerator CloseDoorCroutine()
    {
        OpenCloseCheck = false;
        float ratio = 0.0f;

        collider_Default.enabled = false;

        while (ratio < 0.8f)
        {
            if (CollisionWithPlayer())
            {
                yield break;
            }

            yield return null;
            ratio += Time.deltaTime *1.5f;

            // ���� ��ġ�� ���� ��Ų��.
            Quaternion e = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, defaultY, 0), ratio);

            transform.rotation = e;
        }

        //���带 ����մϴ�.
        if (!soundName.Equals("")) AudioManager.instance.PlayEffectiveSound($"DoorClose_{soundName}", 1.0f, true);
        else { AudioManager.instance.PlayEffectiveSound("DoorClose", 1.0f, true); }


        collider_Default.enabled = true;


        //���� �����Ѵ�.
        transform.rotation = Quaternion.Euler(0, defaultY, 0);

        //���� ����
        EventManager.instance.EventSound(transform, soundRadius);
    }

    IEnumerator OpenDoorCroutine()
    {
        if (CollisionWithPlayer())
        {
            yield break;
        }

        OpenCloseCheck = true;
        float ratio = 0.0f;

        float targetRot;     // ȸ�� ��ǥ ��
        if (frontBackMul * rotateMul >= 0.0f) targetRot = (defaultY + maxY - 1.0f);
        else targetRot = (defaultY - maxY + 1.0f);

        if (!soundName.Equals("")) AudioManager.instance.PlayEffectiveSound($"DoorOpen_{soundName}", 1.0f, true);
        else { AudioManager.instance.PlayEffectiveSound("DoorOpen", 1.0f, true); }
        //Debug.Log("TEXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

        //���� �̺�Ʈ ����
        EventManager.instance.EventSound(transform, soundRadius);

        collider_Default.enabled = false;

        while (ratio < 0.8f) // 0.5���� �ð� ������ ���� �� ���̴�.
        {
            if (CollisionWithPlayer())
            {
                yield break;
            }

            yield return null;
            ratio += Time.deltaTime *1.0f;

            // ���� ��ġ�� ���� ��Ų��.
            Quaternion e = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetRot, 0), ratio);

            transform.rotation = e;
        }

        collider_Default.enabled = true;

        //���� �����Ѵ�.
        transform.rotation = Quaternion.Euler(0, targetRot, 0);
    }


    bool CollisionWithPlayer()
    {
        if (checkPlayerCollsion)
        {
            Debug.Log("�÷��̾�� �浹�Ͽ����ϴ�. ���� ������ �����մϴ�.");
            AllStopCroutines();
            return true;
        }
        return false;
    }

    //���� �ִ� ������ ������ �����Ѵ�.
    bool Clamp(Quaternion e)
    {
        float dot = Vector3.Dot(transform.right, right);
        float theta = Mathf.Acos(dot);
        float degree = Mathf.Rad2Deg * theta;

        Quaternion q = Quaternion.identity;

        if (degree >= maxY)
        {
            //Debug.Log(degree);
            //���� ���� �÷��̾��� ��ġ ���� ���� �Էµ� ���⿡ ���� ���� ������ �д�.
            if (wheelDic * frontBackMul * rotateMul < 0)
            { // �������� ȸ���Ѵ�,
                //Debug.Log("���� 1 :: ȸ������ -");
                q = Quaternion.Euler(0, (defaultY - maxY + 1.0f), 0);
            }
            else if (wheelDic * frontBackMul * rotateMul > 0)
            {//�ٱ������� ȸ���Ѵ�.
                //Debug.Log("���� 1 :: ȸ������ +");
                q = Quaternion.Euler(0, (defaultY + maxY - 1.0f), 0);
            }
            transform.rotation = q;

            return false;
        }
        else { 
            q = e;
            transform.rotation = q; 
            return true;
        }

    }

    //������ �Ǵ�
    protected virtual bool LockCheck()
    {
        if (isLocked)
        {
            if(lockDelayCroutine == null)
            {
                lockDelayCroutine = LockDelayCroutine();
                StartCoroutine(lockDelayCroutine);
            }
            EventManager.instance.EventSound(transform, soundRadius * 1.0f);
            return true;
        }
        return false;
    }
    //���� ��� �Ҹ��� ���� ������
    IEnumerator LockDelayCroutine()
    {
        //����
        AudioManager.instance.PlayEffectiveSound("LockedDoor", 1.0f, true);

        //ȿ��

        yield return new WaitForSeconds(lockDeleyTime);
        lockDelayCroutine = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (other.CompareTag("PlayerCollision"))
        {
            //checkPlayerCollsion = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (other.CompareTag("PlayerCollision"))
        {
            //checkPlayerCollsion = false;
        }
    }
}
