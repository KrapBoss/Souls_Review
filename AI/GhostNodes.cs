using System;
using UnityEngine;
using UnityEngine.AI;
using static Ghost;

using Ramdom = UnityEngine.Random;

/// <summary>
/// 하양이 귀신용 노드 정보
/// </summary>
public abstract class GhostNode : Node
{
    protected Enum chaseType;
    protected Ghost ghost;
    protected NavMeshAgent nav;
    protected Transform transform;

    public GhostNode(Ghost ghost)
    {
        this.ghost = ghost;
        this.nav = ghost.nav;
        this.transform = ghost.transform;
    }
}

/// <summary>
/// 플레이어가 범위에 있는지 판단하여 추적 대상을 선정한다.
/// </summary>
public class CheckPlayerInCone : GhostNode
{
    public CheckPlayerInCone(Ghost ghost, Enum type) : base(ghost) { chaseType = type; }

    public override NodeState Evaluate()
    {
        //숨 참고 있다면, 추적을 실패합니다.
        if (PlayerEvent.instance == null || PlayerEvent.instance.GetHoldOnBreath())
            return NodeState.FAILURE;

        if (!Calculator.GetBetweenHeight(transform.position, PlayerEvent.instance.transform.position))
            return NodeState.FAILURE;

        float distance = Vector3.Distance(transform.position, PlayerEvent.instance.transform.position);
        if (distance > ghost.chaseRange)
            return NodeState.FAILURE;

        Vector3 directionTowardPlayer = (PlayerEvent.instance.transform.position - transform.position).normalized;
        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0;
        forwardFlat.Normalize();
        directionTowardPlayer.y = 0;
        directionTowardPlayer.Normalize();

        float theta = Mathf.Acos(Vector3.Dot(forwardFlat, directionTowardPlayer));
        if (theta * Mathf.Rad2Deg > ghost.chaseTheta)
            return NodeState.FAILURE;

        // 대상 지정
        ghost.SetTarget(PlayerEvent.instance.transform.position, (ChaseType)chaseType, ghost.playerChaseDelayTime);

        return NodeState.SUCCESS;
    }
}



// 오르골 사운드 추적 판단
public class CheckForGlobalSound : GhostNode
{
    public CheckForGlobalSound(Ghost ghost, Enum type) : base(ghost) { chaseType = type; }

    public override NodeState Evaluate()
    {
        if (EventManager.instance.GlobalSoundTarget)
        {
            ghost.SetTarget(EventManager.instance.GlobalSoundTarget.position, (ChaseType)chaseType, ghost.localSoundChaseDelayTime);
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}

/// <summary>
/// 로컬 사운드 추적
/// </summary>
public class CheckForLocalSound : GhostNode
{
    public CheckForLocalSound(Ghost ghost, Enum type) : base(ghost) { chaseType = type; }

    public override NodeState Evaluate()
    {
        if (EventManager.instance.LocalSoundTarget != null)
        {
            ghost.SetTarget(EventManager.instance.GetLocalSoundTarget(), (ChaseType)chaseType,ghost.localSoundChaseDelayTime);
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}

/// <summary>
/// 지형 순찰
/// </summary>
public class TaskTraversal : GhostNode
{
    public TaskTraversal(Ghost ghost, Enum type) : base(ghost) { chaseType = type; }

    public override NodeState Evaluate()
    {
        // 내 외부 지역 중 랜덤한 지역의 한 좌표를 전달받아서 대상으로 지정
        int i = UnityEngine.Random.Range(0, 2);
        string name = i == 0 ? "Inside" : "Outside";
        Vector3 target = MapManager.instance.GetLocationByName(name);
        ghost.SetTarget(target, (ChaseType)chaseType, ghost.traversalChaseDelayTime);
        return NodeState.SUCCESS;
    }
}

/// <summary>
/// 플레이어 사망 판단
/// </summary>
public class CheckPlayerInKillRange : GhostNode
{
    public CheckPlayerInKillRange(Ghost ghost) : base(ghost) 
    {
    }

    public override NodeState Evaluate()
    {
        if (PlayerEvent.instance == null) return NodeState.FAILURE;

        // 숨참고 있는 중이라면, 사망 모션 실패.
        if (PlayerEvent.instance.GetHoldOnBreath())
            return NodeState.FAILURE;

        // 거리 측정 해서 사망 판단.
        float distance = Vector3.Distance(transform.position, PlayerEvent.instance.transform.position);

        if (distance <= ghost.chaseDeadRange)
        {
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}

// 플레이어 킬 진행
public class TaskKillPlayer : GhostNode
{
    private bool killed = false;
    public TaskKillPlayer(Ghost ghost) : base(ghost) { }

    public override NodeState Evaluate()
    {
        if (PlayerEvent.instance == null) return NodeState.FAILURE;

        if (!killed)
        {
            GameData.firstKill = true;
            killed = true;
        }

        //킬 애니메이션 실행
        if (GhostBlack.activation)  // 2 페이즈 괴물이 나왔을 때 실행되는 공포 애니메이션
            PlayerEvent.instance.PlayerDeadSecene(false);
        else
            PlayerEvent.instance.PlayerDeadSecene(true);

        return NodeState.SUCCESS;
    }
}

/// <summary>
/// 소리 지르는 이벤트 실행
/// </summary>
public class TaskScream : GhostNode
{
    private Vector3 lastScreamPosition;
    public TaskScream(Ghost ghost) : base(ghost) { lastScreamPosition = Vector3.zero; }

    public override NodeState Evaluate()
    {
        Vector3 target = ghost.GetTarget();
        if (target != lastScreamPosition)
        {
            EventManager.instance.CameraShake(1.0f, 0.25f);
            AudioManager.instance.PlayEffectiveSound("GhostScream", 1.0f, true);
            lastScreamPosition = target;
        }
        return NodeState.SUCCESS;
    }
}

/// <summary>
/// 타켓을 추적합니다.
/// </summary>
public class TaskChase : GhostNode
{
    public TaskChase(Ghost ghost) : base(ghost) { }

    public override NodeState Evaluate()
    {
        Vector3 target = (Vector3)ghost.GetTarget();

        if (target == Vector3.zero) return NodeState.FAILURE;

        if (nav.destination != target)
        {
            nav.SetDestination(target);
        }

        if (nav.remainingDistance <= nav.stoppingDistance)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}

/// <summary>
/// 특정 목표 이동 후 대기 진행
/// </summary>
public class TaskWait : GhostNode
{
    public TaskWait(Ghost ghost) : base(ghost)
    {
    }

    public override NodeState Evaluate()
    {
        //if (state != NodeState.RUNNING)
        //{
        //    state = NodeState.RUNNING;
        //}

        ghost.delayTime -= Time.deltaTime;

        if (ghost.delayTime <= 0.0f)
        {
            state = NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}

/// <summary>
/// 외형의 움직임을 부여
/// </summary>
public class TaskUpdateVisuals : GhostNode
{
    private float applyVisualUpdateRate = 0.0f;

    public TaskUpdateVisuals(Ghost ghost) : base(ghost)
    {
    }

    public override NodeState Evaluate()
    {
        applyVisualUpdateRate -= Time.deltaTime;
        if (applyVisualUpdateRate < 0.0f)
        {
            // 속도에 따라 커스텀 애니메이션을 실행하기 위한 위치 전달.
            ghost.ghostEffect.Movement(transform.position, transform.forward);
            if (nav.speed > 0)
            {   // 속도에 따른 애니메이션 업데이트 주기 지정
                float updateRate = ghost.isChasingPlayer ? ghost.visualUpdateDistancePlayer : ghost.visualUpdateDistanceMovement;
                applyVisualUpdateRate = updateRate / nav.speed;
            }
        }
        return NodeState.SUCCESS;
    }
}