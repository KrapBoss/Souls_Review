using StarterAssets;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// 몬스터의 동작을 정의한다.
/// 몬스터는 사운드에 반응하여 움직인다.
/// 1순위 플레이어
/// 2순위 오르골
/// 3순위 기타 사운드
/// </summary>
/// 
public class Ghost : MonoBehaviour
{

    [Header("귀신의 앞 기준 추격 각도와 거리 / 플레이어를 죽이는 범위")]
    [Range(0.0f, 180f)] public float chaseTheta;
    [Range(0.0f, 20.0f)] public float chaseRange;
    [Range(0.0f, 10.0f)] public float chaseDeadRange;
    public float researchTime = 0.05f;
    float applyResearchTime = 0.0f;
    [Header("추적 후 대기시간")]
    //플레이어 추적 후 대기 시간
    public float playerChaseDelayTime = 2.0f;
    public float localSoundChaseDelayTime = 1.0f;
    public float traversalChaseDelayTime = 3.0f;

    //이동 시 일정 거리에 따른 귀신을 업데이트 하는 시간 간격
    public float visualUpdateDistancePlayer = 2.0f;
    public float visualUpdateDistanceMovement = 3.0f;
    float applyVisualUpdateRate = 0.0f;

    [Header("이동에 관한 사항")]

    //추적 대상
    Vector3 targetPosition;
    //추적 상태
    public bool[] chaseState = new bool[4];
    //추적종료 후 대기시간
    float chaseDelayTime;

    [Header("이동 객체를 보여주기 위한 스크립트")]
    public GhostEffect ghostEffect;

    NavMeshAgent nav;

    //forward에서 높이값을 제외한 벡터
    Vector3 forwardFlat;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        //플레이어가 죽었을 경우 동작하지 않는다.
        if (PlayerEvent.instance.isDead || GameManager.Instance.DontMove || !nav.enabled) return;

        //1순위 플레이어 추격
        if (SearchPlayer())
        {
            //Debug.Log("Ghost :: Player 추격");
        }
        //2순위 오르골[전역사운드]
        else if (SearchGlobalSound())
        {
            //Debug.Log("Ghost :: 전역 사운드 추격");
        }
        //3순위 기타 사운드
        else if (SearchLocalSound())
        {
            //Debug.Log("Ghost :: 로컬 사운드 추격");
        }
        //마지막 순회
        else
        {
            //Debug.Log("Ghost :: 순회");
            SearchTraversal();
        }

        VisualUpdate(chaseState[0]);

        if(targetPosition != nav.destination) nav.SetDestination(targetPosition);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.blue;

        forwardFlat = transform.forward;
        forwardFlat.y = 0;
        forwardFlat.Normalize();
        Handles.DrawSolidArc(transform.position, Vector3.up, forwardFlat, -chaseTheta * 0.5f, chaseRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, forwardFlat, chaseTheta * 0.5f, chaseRange);

        Handles.color = Color.red;
        Handles.DrawSolidArc(transform.position, Vector3.up, forwardFlat, -chaseTheta * 0.5f, chaseDeadRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, forwardFlat, chaseTheta * 0.5f, chaseDeadRange);
    }
#endif
    #region -----------PlayerChase--------- index 0
    //플레이어 추격을 위한 판정
    bool SearchPlayer()
    {
        applyResearchTime -= Time.deltaTime;

        //검색시간 종료?
        if (applyResearchTime < 0)
        {
            //숨을 참고 있거나 플레이어 죽었다면 추격하지 않습니다.
            if (PlayerEvent.instance.GetHoldOnBreath()) return false;
            //|| PlayerEvent.instance.isDead)

            //검색시간 초기화
            applyResearchTime = researchTime;

            //플레이어가 감지되었나?
            if (SearchPlayerCondition())
            {
                targetPosition = PlayerEvent.instance.transform.position;
                ChaseState(0);
                chaseDelayTime = playerChaseDelayTime;
                return true;
            }
            //플레이어가 감지되지 않았지만, 추적하고 있었는가?
            else
            {
                return SearchPlayerChaseState();
            }
        }
        //검색 대기 시간이 끝나지 않았는데, 추적하고 있었는가?
        else
        {
            return SearchPlayerChaseState();
        }
    }

    //플레이어 검색을 위한 조건
    bool SearchPlayerCondition()
    {
        //0. 플레이어의 존재 여부 판단
        if (PlayerEvent.instance == null) return false;
        //Debug.Log("플레이어이벤트 존재");

        //1. 층수 판단
        if (!Calculator.GetBetweenHeight(transform.position, PlayerEvent.instance.transform.position)) return false;
        //Debug.Log("층수 통과");

        //2. 거리 판단
        float distance = Vector3.Distance(transform.position, PlayerEvent.instance.transform.position);
        if (distance > chaseRange) return false;
        //Debug.Log("거리 통과");

        //3. 각도 판단
        Vector3 currentForward = transform.forward;
        Vector3 directionTowardPlayer = PlayerEvent.instance.transform.position - transform.position;

        directionTowardPlayer.Normalize();

        currentForward.y = 0;
        currentForward.Normalize();
        directionTowardPlayer.y = 0;
        directionTowardPlayer.Normalize();
        //Debug.Log($"{currentForward} / {directionTowardPlayer} / {Mathf.Acos(Vector3.Dot(currentForward, directionTowardPlayer)) * Mathf.Rad2Deg}"); 
        Debug.DrawRay(transform.position, currentForward * 5.0f, Color.green);
        Debug.DrawRay(transform.position, directionTowardPlayer * 5.0f, Color.red);
        float theta = Mathf.Acos(Vector3.Dot(currentForward, directionTowardPlayer));

        if (theta * Mathf.Rad2Deg > chaseTheta) return false;

        //플레이어 사망
        if (distance < chaseDeadRange)
        {
            if (GhostBlack.activation)
                PlayerEvent.instance.PlayerDeadSecene(false);
            else
                PlayerEvent.instance.PlayerDeadSecene(true);
        }

        return true;
    }

    //추적 상태에 따른 추적 판단
    bool SearchPlayerChaseState()
    {
        //추격 중이다.
        if (chaseState[0])
        {
            //지정 장소에 도착 했는가?
            return ArriveAtDestination(0);
        }

        //추적 종료
        return false;
    }
    #endregion


    #region ------GlobalSound-------- index : 2
    Vector3 beforePosition_global = Vector3.zero;
    bool SearchGlobalSound()
    {
        if (EventManager.instance.GlobalSoundTarget)
        {
            //만약 글로벌 사운드가 새로운 좌표라면 소리를 지른 후에 달려갑니다.
            if (!beforePosition_global.Equals(EventManager.instance.GlobalSoundTarget.position))
            {
                EventManager.instance.CameraShake(1.0f, 0.25f);
                AudioManager.instance.PlayEffectiveSound("GhostScream", 1.0f, true);
                beforePosition_global = EventManager.instance.GlobalSoundTarget.position;
            }

            targetPosition = EventManager.instance.GlobalSoundTarget.position;
            ChaseState(1);
            return true;
        }
        else
        {
            beforePosition_global = Vector3.zero;
            return false;
        }
    }
    #endregion

    #region ------ChaseLocalSound-------- index : 2
    //로컬 사운드 추적을 위한 기능
    bool SearchLocalSound()
    {
        // 지역 사운드가 발생시 계속해서 그쪽으로 가도록 설정함.
        //추적 중인가요?
        if (chaseState[2])
        {
            //만약 새로운 로컬 사운드 입력이 있다면 다시 갱신한다.
            if (EventManager.instance.LocalSoundTarget != null)
            {
                //해당 지점을 목표로 지정한다.
                targetPosition = EventManager.instance.GetLocalSoundTarget();
                chaseDelayTime = localSoundChaseDelayTime;
                Debug.LogWarning("Ghost 새로운 사운드를 전달 받습니다");

                return true;
            }

            //새로운 로컬 사운드가 없다면 도착여부를 판단한다.
            return SearchLocalSoundChaseState();
        }
        //추적 중이 아니다
        else
        {
            //만약 새로운 로컬 사운드 입력이 있다면 입력
            if (EventManager.instance.LocalSoundTarget != null)
            {
                //해당 지점을 목표로 지정한다.
                targetPosition = EventManager.instance.GetLocalSoundTarget();
                ChaseState(2);  //쫓는 대상 변경
                chaseDelayTime = localSoundChaseDelayTime;

                Debug.LogWarning("Ghost 새로운 사운드를 전달 받습니다");

                return true;
            }

            //추적 종료
            return false;
        }
    }

    //이미 추적하고 있던 사운드가 존재한다는 가정하에 추적 지속을 판단한다
    bool SearchLocalSoundChaseState()
    {
        //이미 추격 중인 사운드가 존재함으로 제거한다.
        EventManager.instance.GetLocalSoundTarget();

        //도착했는가?
        return ArriveAtDestination(2);
    }

    void LocalSoundUpdtate()
    {

    }
    #endregion

    #region --------Traversal---------- index : 4
    void SearchTraversal()
    {
        //추격 중인가?
        if (chaseState[3])
        {
            ArriveAtDestination(3);
        }
        else
        {
            SetTraversalTarget();
        }
    }

    //순회지를 지정한다.
    void SetTraversalTarget()
    {
        int i = Random.Range(0, 2);
        string name = i == 0 ? "Inside" : "Outside";
        targetPosition = MapManager.instance.GetLocationByName(name);//MapManager.instance.GetLocationAroundPlayer();
        chaseDelayTime = traversalChaseDelayTime;
        ChaseState(3);
    }
    #endregion

    //목표 지점에 도착했는지 판단한다.
    bool ArriveAtDestination(int index)
    {
        //도착했는가?
        if (nav.remainingDistance <= nav.stoppingDistance)
        {
            chaseDelayTime -= Time.deltaTime;

            //Debug.Log($"Ghost: {index} 조건에 해당하는 위치에 도착하였습니다. 대기시간 : {chaseDelayTime}");

            //도착 후 대기시간이 종료되었는가?
            if (chaseDelayTime < 0.0f)
            {
                VisualUpdate(chaseState[0]);

                //Debug.Log($"Ghost: {index} 조건에 해당하는 위치에 도착 후 대기시간이 지났습니다. 추적을 종료합니다.");
                //추적 종료
                chaseState[index] = false;

                //도착했기에 추격을 종료한다.
                return false;
            }
            //종료되지 않아서 추적 지속
            else
            {
                return true;
            }
        }
        //도착하지 않았으니 추적 지속
        else
        {
            return true;
        }
    }

    void ChaseState(int index)
    {
        for (int i = 0; i < chaseState.Length; i++)
        {
            chaseState[i] = false;
        }

        //Debug.Log($"Ghost: {index} 조건에 해당하는 위치가 선정되어 추격을 시작합니다.");
        chaseState[index] = true;
    }

    //실제로 보여지는 귀신의 모습을 업데이트한다.
    void VisualUpdate(bool chasePlayer)
    {
        applyVisualUpdateRate -= Time.deltaTime;
        if (applyVisualUpdateRate < 0.0f)
        {
            //실제 보여지는 이동
            ghostEffect.Movement(transform.position, transform.forward);

            //무한값 방지
            if(nav.speed == 0) { applyVisualUpdateRate = 0.5f; return; }

            //딜레이 시간 변경
            if (chasePlayer) { applyVisualUpdateRate = visualUpdateDistancePlayer/ nav.speed; }
            else { applyVisualUpdateRate = visualUpdateDistanceMovement/nav.speed ; }
        }
    }

    //속도 조절과 위치 지정
    public void Activation(Vector3 position)
    {
        nav.enabled = false;

        transform.position = position;

        //기본 속도 지정
        nav.speed = FindObjectOfType<FirstPersonController>().MoveSpeed * DataSet.Instance.GameDifficulty.GhostInitSpeedRate;

        EventManager.instance.CameraShake(2.0f, 0.25f);
        AudioManager.instance.PlayEffectiveSound("GhostScream", 1.1f, true);

        nav.enabled = true;
    }

    //영혼이 승천을 하게 될 경우
    public void AscensededSoul()
    {
        Debug.Log("영혼이 승천을 하여 속도가 빨라집니다.");

        //1. 속도 증가
        nav.speed *= 1.15f;

        //소리를 지릅니다.
        AudioManager.instance.PlayEffectiveSound("GhostScream", 0.7f, true);
    }
}
