using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Door : InteractionObjects
{
    [Space]
    [Header("문에 관련한 값들")]
    [Tooltip("Pivot at left? Door direction from Scale")]
    public bool AchorisLeftDoor;
    public bool OpenCloseCheck = false;

    public float defaultY;  // 기본 Y 축 회전 위치값
    [Range(0, 179)]
    public float maxY = 90.0f;      //최대 회전 가능한 Y축 회전값

    [Header("잠겼는지 확인합니다.")]
    public bool isLocked = false;
    public float lockDeleyTime = 0.5f;

    public string soundName = null;

    public float soundRadius = 5.0f;

    short rotateMul;    //오른쪽 문과 왼쪽문을 구분하기 위함 오른쪽문 = 1; / 왼쪽문 =-1;
    short frontBackMul; //플레이어가 문의 앞 방향과 뒷 방향에 있는지를 나타낸다.
    short wheelDic;     //휠 방향 위 = 1 / 아래 = -1;

    BoxCollider collider_Default;

    IEnumerator CloseCroutine;
    IEnumerator OpenCroutine;

    Vector3 right; // 우측 nomal 벡터를 기준점으로 사용

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
        return false;//그랩 불가능
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

        //if (checkPlayerCollsion) return;//플레이어와 충돌 시 실행하지 않음.

        if (_wheelValue <= 0.0f) wheelDic = -1;
        else if (_wheelValue > 0.0f) wheelDic = 1;

        CheckFrontBack();
        OpenCloseCheck = true;
        //마우스 휠에 따른 회전 값 적용
        Quaternion e = transform.rotation * Quaternion.Euler(transform.up * _wheelValue * 250.0f * Time.deltaTime * rotateMul * frontBackMul);

        //Debug.Log(e.eulerAngles.y - transform.rotation.eulerAngles.y);

        transform.rotation = e;
        Clamp(e);
    }
    
    //문을 닫거나 여는 용도
    public override bool Func()
    {
        if (LockCheck()) { return false; }

        CheckFrontBack();

        //문을 닫는다.
        if (Mathf.Abs(transform.rotation.eulerAngles.y - defaultY) > 1.0f)// 현재 값이 기본 값과 다르다면?
        {
            if (CloseCroutine != null) return false;//기존에 동작 중인 닫기 모션이 있는 경우
            AllStopCroutines();
            CloseCroutine = CloseDoorCroutine();
            StartCoroutine(CloseCroutine);
        }
        // 문을 연다.
        else
        {
            if (OpenCroutine != null) return false;//기존에 동작 중인 열기 모션이 있는 경우
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
            frontBackMul = 1;// 문 앞에 있다.
        }
        else
        {
            frontBackMul = -1; // 문 뒤에 있다.
        }
    }

    void AllStopCroutines()
    {
        StopAllCoroutines();
        OpenCroutine = null;
        CloseCroutine = null;
    }

    //문을 닫습니다.
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

            // 원래 위치로 복귀 시킨다.
            Quaternion e = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, defaultY, 0), ratio);

            transform.rotation = e;
        }

        //사운드를 재생합니다.
        if (!soundName.Equals("")) AudioManager.instance.PlayEffectiveSound($"DoorClose_{soundName}", 1.0f, true);
        else { AudioManager.instance.PlayEffectiveSound("DoorClose", 1.0f, true); }


        collider_Default.enabled = true;


        //값을 적용한다.
        transform.rotation = Quaternion.Euler(0, defaultY, 0);

        //사운드 전달
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

        float targetRot;     // 회전 목표 값
        if (frontBackMul * rotateMul >= 0.0f) targetRot = (defaultY + maxY - 1.0f);
        else targetRot = (defaultY - maxY + 1.0f);

        if (!soundName.Equals("")) AudioManager.instance.PlayEffectiveSound($"DoorOpen_{soundName}", 1.0f, true);
        else { AudioManager.instance.PlayEffectiveSound("DoorOpen", 1.0f, true); }
        //Debug.Log("TEXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

        //사운드 이벤트 전달
        EventManager.instance.EventSound(transform, soundRadius);

        collider_Default.enabled = false;

        while (ratio < 0.8f) // 0.5초의 시간 비율로 문을 열 것이다.
        {
            if (CollisionWithPlayer())
            {
                yield break;
            }

            yield return null;
            ratio += Time.deltaTime *1.0f;

            // 원래 위치로 복귀 시킨다.
            Quaternion e = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetRot, 0), ratio);

            transform.rotation = e;
        }

        collider_Default.enabled = true;

        //값을 적용한다.
        transform.rotation = Quaternion.Euler(0, targetRot, 0);
    }


    bool CollisionWithPlayer()
    {
        if (checkPlayerCollsion)
        {
            Debug.Log("플레이어와 충돌하였습니다. 문의 동작을 중지합니다.");
            AllStopCroutines();
            return true;
        }
        return false;
    }

    //문의 최대 열리는 각도를 제한한다.
    bool Clamp(Quaternion e)
    {
        float dot = Vector3.Dot(transform.right, right);
        float theta = Mathf.Acos(dot);
        float degree = Mathf.Rad2Deg * theta;

        Quaternion q = Quaternion.identity;

        if (degree >= maxY)
        {
            //Debug.Log(degree);
            //문에 따른 플레이어의 위치 값과 휠의 입력되 방향에 따라 문의 제한을 둔다.
            if (wheelDic * frontBackMul * rotateMul < 0)
            { // 안쪽으로 회전한다,
                //Debug.Log("조건 1 :: 회전값이 -");
                q = Quaternion.Euler(0, (defaultY - maxY + 1.0f), 0);
            }
            else if (wheelDic * frontBackMul * rotateMul > 0)
            {//바깥쪽으로 회전한다.
                //Debug.Log("조건 1 :: 회전값이 +");
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

    //잠겼는지 판단
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
    //문이 잠긴 소리에 대한 딜레이
    IEnumerator LockDelayCroutine()
    {
        //사운드
        AudioManager.instance.PlayEffectiveSound("LockedDoor", 1.0f, true);

        //효과

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
