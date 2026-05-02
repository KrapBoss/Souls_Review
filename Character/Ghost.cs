using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Ghost : MonoBehaviour
{
    [Header("귀신의 앞 기준 추격 각도와 거리 / 플레이어를 죽이는 범위")]
    [Range(0.0f, 180f)] public float chaseTheta;
    [Range(0.0f, 20.0f)] public float chaseRange;
    [Range(0.0f, 10.0f)] public float chaseDeadRange;
    public float researchTime = 0.05f;

    [Header("추적 후 대기시간")]
    public float playerChaseDelayTime = 2.0f;
    public float localSoundChaseDelayTime = 1.0f;
    public float traversalChaseDelayTime = 3.0f;

    [Header("이동 시 시각적 업데이트 간격")]
    public float visualUpdateDistancePlayer = 2.0f;
    public float visualUpdateDistanceMovement = 3.0f;

    [Header("이동 객체를 보여주기 위한 스크립트")]
    public GhostEffect ghostEffect;

    [HideInInspector] public NavMeshAgent nav;
    
    private Node _root;              // 메인 분기 노드
    private Node _chaseNode;        // 추적을 위한 노드
    private TaskUpdateVisuals visualUpdater;    // 비주얼 업데이트 전용 노드

    /// <summary> 플레이어 추적 여부 </summary>
    public bool isChasingPlayer = false;

    public enum ChaseType   // 추적 상태
    {
        PlayerChase,        // 플레이어
        GlobalSoundChase,   // 오르골
        LocalSoundChase,    // 로컬 사운드 추적 딜레이
        Default,             // 일반 추적 딜레이
        None
    }

    ChaseType _currentChaseType;
    private Vector3 target;     // 타켓 설정
    public float delayTime = 0.0f; // 현재 딜레이 타임
    public bool isWaitTimeDelay = false;  // 딜레이 대기 중


    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _root = SetupTreeRoot();
        _chaseNode = SetupTreeChase();
        visualUpdater = new TaskUpdateVisuals(this);
    }

    private void Update()
    {
        _root?.Evaluate();
        _chaseNode?.Evaluate();
        visualUpdater.Evaluate();
    }

    private Node SetupTreeRoot()
    {
        // 진행 되고 있는 노드를 기준으로 계속 시행
        Node root = new Selector(new List<Node>
        {
            // 플레이어 사망 0순위
            new Sequence(new List<Node>
            {
                new CheckPlayerInKillRange(this),
                new TaskKillPlayer(this),
            }),

            // 플레이어 추적 1순위
            new Sequence(new List<Node>
            {
                new CheckPlayerInCone(this, ChaseType.PlayerChase),
            }),

            // 전역 사운드 추적 2순위
            new Sequence(new List<Node>
            {
                new CheckForGlobalSound(this, ChaseType.GlobalSoundChase),
                new TaskScream(this),
            }),

            // 로컬 사운드 추적 3순위
            new Sequence(new List<Node>
            {
                new CheckForLocalSound(this, ChaseType.LocalSoundChase),
            }),

            // 기본 순회 4순위
            new Sequence(new List<Node>
            {
                new TaskTraversal(this, ChaseType.Default),
            })
        });


        return root;
    }

    private Node SetupTreeChase()
    {
        // 진행 되고 있는 노드를 기준으로 계속 시행
        Node root = new Sequence(new List<Node>
        {
                new TaskChase(this),
                new TaskWait(this)
        });

        return root;
    }

    /// <summary>
    /// 타켓 지정 및 도착 후 대기 시간 지정
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetType"></param>
    /// <param name="delayTime"></param>
    public void SetTarget(Vector3 target, ChaseType targetType, float delayTime)
    {
        bool update = false;
        switch (targetType)
        {
            case ChaseType.PlayerChase:
                update = true;
                break;
            case ChaseType.GlobalSoundChase:
                // 하위 우선 순위일 경우 
                if(_currentChaseType > ChaseType.GlobalSoundChase) update = true;
                // 같거나 상위 우선 순위일 때, 추적 대기가 끝나 있다면
                else if (_currentChaseType <= ChaseType.GlobalSoundChase && this.delayTime <= 0.0f) update = true;
                
                break;
            case ChaseType.LocalSoundChase:
                // 하위 우선 순위일 경우 
                if (_currentChaseType > ChaseType.LocalSoundChase) update = true;
                // 로컬 사운드는 최근에 발생한 사운드를 우선 추적합니다.
                else if(_currentChaseType == ChaseType.LocalSoundChase) update = true;
                // 같거나 상위 우선 순위일 때, 추적 대기가 끝나 있다면
                else if (_currentChaseType < ChaseType.LocalSoundChase && this.delayTime <= 0.0f) update = true;
                break;
            case ChaseType.Default:
                // 추적 후 대기가 없을 경우에만 업데이트
                if (this.delayTime <= 0.0f) update = true;
                break;
        }

        if (update)
        {
            _currentChaseType = targetType;
            this.target = target;
            this.delayTime = delayTime;
        }
    }

    // 대상 타켓 좌표를 받아옵니다.
    public Vector3 GetTarget()
    {
        return  this.target;
    }


    /// <summary> 대기 시간 반환 </summary>
    /// <returns></returns>
    public float GetWaitDelayTime() => delayTime;

    //속도 조절과 위치 지정
    public void Activation(Vector3 position)
    {
        nav.enabled = false;
        transform.position = position;
        // Commenting out for safety, as FirstPersonController might not be available
        // nav.speed = FindObjectOfType<FirstPersonController>().MoveSpeed * DataSet.Instance.GameDifficulty.GhostInitSpeedRate;
        EventManager.instance.CameraShake(2.0f, 0.25f);
        AudioManager.instance.PlayEffectiveSound("GhostScream", 1.1f, true);
        nav.enabled = true;
    }

    //영혼이 승천을 하게 될 경우
    public void AscensededSoul()
    {
        Debug.Log("영혼이 승천을 하여 속도가 빨라집니다.");
        nav.speed *= 1.15f;
        AudioManager.instance.PlayEffectiveSound("GhostScream", 0.7f, true);
    }
}
