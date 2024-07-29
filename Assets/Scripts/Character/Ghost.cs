using StarterAssets;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// ������ ������ �����Ѵ�.
/// ���ʹ� ���忡 �����Ͽ� �����δ�.
/// 1���� �÷��̾�
/// 2���� ������
/// 3���� ��Ÿ ����
/// </summary>
/// 
public class Ghost : MonoBehaviour
{

    [Header("�ͽ��� �� ���� �߰� ������ �Ÿ� / �÷��̾ ���̴� ����")]
    [Range(0.0f, 180f)] public float chaseTheta;
    [Range(0.0f, 20.0f)] public float chaseRange;
    [Range(0.0f, 10.0f)] public float chaseDeadRange;
    public float researchTime = 0.05f;
    float applyResearchTime = 0.0f;
    [Header("���� �� ���ð�")]
    //�÷��̾� ���� �� ��� �ð�
    public float playerChaseDelayTime = 2.0f;
    public float localSoundChaseDelayTime = 1.0f;
    public float traversalChaseDelayTime = 3.0f;

    //�̵� �� ���� �Ÿ��� ���� �ͽ��� ������Ʈ �ϴ� �ð� ����
    public float visualUpdateDistancePlayer = 2.0f;
    public float visualUpdateDistanceMovement = 3.0f;
    float applyVisualUpdateRate = 0.0f;

    [Header("�̵��� ���� ����")]

    //���� ���
    Vector3 targetPosition;
    //���� ����
    public bool[] chaseState = new bool[4];
    //�������� �� ���ð�
    float chaseDelayTime;

    [Header("�̵� ��ü�� �����ֱ� ���� ��ũ��Ʈ")]
    public GhostEffect ghostEffect;

    NavMeshAgent nav;

    //forward���� ���̰��� ������ ����
    Vector3 forwardFlat;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        //�÷��̾ �׾��� ��� �������� �ʴ´�.
        if (PlayerEvent.instance.isDead || GameManager.Instance.DontMove || !nav.enabled) return;

        //1���� �÷��̾� �߰�
        if (SearchPlayer())
        {
            //Debug.Log("Ghost :: Player �߰�");
        }
        //2���� ������[��������]
        else if (SearchGlobalSound())
        {
            //Debug.Log("Ghost :: ���� ���� �߰�");
        }
        //3���� ��Ÿ ����
        else if (SearchLocalSound())
        {
            //Debug.Log("Ghost :: ���� ���� �߰�");
        }
        //������ ��ȸ
        else
        {
            //Debug.Log("Ghost :: ��ȸ");
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
    //�÷��̾� �߰��� ���� ����
    bool SearchPlayer()
    {
        applyResearchTime -= Time.deltaTime;

        //�˻��ð� ����?
        if (applyResearchTime < 0)
        {
            //���� ���� �ְų� �÷��̾� �׾��ٸ� �߰����� �ʽ��ϴ�.
            if (PlayerEvent.instance.GetHoldOnBreath()) return false;
            //|| PlayerEvent.instance.isDead)

            //�˻��ð� �ʱ�ȭ
            applyResearchTime = researchTime;

            //�÷��̾ �����Ǿ���?
            if (SearchPlayerCondition())
            {
                targetPosition = PlayerEvent.instance.transform.position;
                ChaseState(0);
                chaseDelayTime = playerChaseDelayTime;
                return true;
            }
            //�÷��̾ �������� �ʾ�����, �����ϰ� �־��°�?
            else
            {
                return SearchPlayerChaseState();
            }
        }
        //�˻� ��� �ð��� ������ �ʾҴµ�, �����ϰ� �־��°�?
        else
        {
            return SearchPlayerChaseState();
        }
    }

    //�÷��̾� �˻��� ���� ����
    bool SearchPlayerCondition()
    {
        //0. �÷��̾��� ���� ���� �Ǵ�
        if (PlayerEvent.instance == null) return false;
        //Debug.Log("�÷��̾��̺�Ʈ ����");

        //1. ���� �Ǵ�
        if (!Calculator.GetBetweenHeight(transform.position, PlayerEvent.instance.transform.position)) return false;
        //Debug.Log("���� ���");

        //2. �Ÿ� �Ǵ�
        float distance = Vector3.Distance(transform.position, PlayerEvent.instance.transform.position);
        if (distance > chaseRange) return false;
        //Debug.Log("�Ÿ� ���");

        //3. ���� �Ǵ�
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

        //�÷��̾� ���
        if (distance < chaseDeadRange)
        {
            if (GhostBlack.activation)
                PlayerEvent.instance.PlayerDeadSecene(false);
            else
                PlayerEvent.instance.PlayerDeadSecene(true);
        }

        return true;
    }

    //���� ���¿� ���� ���� �Ǵ�
    bool SearchPlayerChaseState()
    {
        //�߰� ���̴�.
        if (chaseState[0])
        {
            //���� ��ҿ� ���� �ߴ°�?
            return ArriveAtDestination(0);
        }

        //���� ����
        return false;
    }
    #endregion


    #region ------GlobalSound-------- index : 2
    Vector3 beforePosition_global = Vector3.zero;
    bool SearchGlobalSound()
    {
        if (EventManager.instance.GlobalSoundTarget)
        {
            //���� �۷ι� ���尡 ���ο� ��ǥ��� �Ҹ��� ���� �Ŀ� �޷����ϴ�.
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
    //���� ���� ������ ���� ���
    bool SearchLocalSound()
    {
        // ���� ���尡 �߻��� ����ؼ� �������� ������ ������.
        //���� ���ΰ���?
        if (chaseState[2])
        {
            //���� ���ο� ���� ���� �Է��� �ִٸ� �ٽ� �����Ѵ�.
            if (EventManager.instance.LocalSoundTarget != null)
            {
                //�ش� ������ ��ǥ�� �����Ѵ�.
                targetPosition = EventManager.instance.GetLocalSoundTarget();
                chaseDelayTime = localSoundChaseDelayTime;
                Debug.LogWarning("Ghost ���ο� ���带 ���� �޽��ϴ�");

                return true;
            }

            //���ο� ���� ���尡 ���ٸ� �������θ� �Ǵ��Ѵ�.
            return SearchLocalSoundChaseState();
        }
        //���� ���� �ƴϴ�
        else
        {
            //���� ���ο� ���� ���� �Է��� �ִٸ� �Է�
            if (EventManager.instance.LocalSoundTarget != null)
            {
                //�ش� ������ ��ǥ�� �����Ѵ�.
                targetPosition = EventManager.instance.GetLocalSoundTarget();
                ChaseState(2);  //�Ѵ� ��� ����
                chaseDelayTime = localSoundChaseDelayTime;

                Debug.LogWarning("Ghost ���ο� ���带 ���� �޽��ϴ�");

                return true;
            }

            //���� ����
            return false;
        }
    }

    //�̹� �����ϰ� �ִ� ���尡 �����Ѵٴ� �����Ͽ� ���� ������ �Ǵ��Ѵ�
    bool SearchLocalSoundChaseState()
    {
        //�̹� �߰� ���� ���尡 ���������� �����Ѵ�.
        EventManager.instance.GetLocalSoundTarget();

        //�����ߴ°�?
        return ArriveAtDestination(2);
    }

    void LocalSoundUpdtate()
    {

    }
    #endregion

    #region --------Traversal---------- index : 4
    void SearchTraversal()
    {
        //�߰� ���ΰ�?
        if (chaseState[3])
        {
            ArriveAtDestination(3);
        }
        else
        {
            SetTraversalTarget();
        }
    }

    //��ȸ���� �����Ѵ�.
    void SetTraversalTarget()
    {
        int i = Random.Range(0, 2);
        string name = i == 0 ? "Inside" : "Outside";
        targetPosition = MapManager.instance.GetLocationByName(name);//MapManager.instance.GetLocationAroundPlayer();
        chaseDelayTime = traversalChaseDelayTime;
        ChaseState(3);
    }
    #endregion

    //��ǥ ������ �����ߴ��� �Ǵ��Ѵ�.
    bool ArriveAtDestination(int index)
    {
        //�����ߴ°�?
        if (nav.remainingDistance <= nav.stoppingDistance)
        {
            chaseDelayTime -= Time.deltaTime;

            //Debug.Log($"Ghost: {index} ���ǿ� �ش��ϴ� ��ġ�� �����Ͽ����ϴ�. ���ð� : {chaseDelayTime}");

            //���� �� ���ð��� ����Ǿ��°�?
            if (chaseDelayTime < 0.0f)
            {
                VisualUpdate(chaseState[0]);

                //Debug.Log($"Ghost: {index} ���ǿ� �ش��ϴ� ��ġ�� ���� �� ���ð��� �������ϴ�. ������ �����մϴ�.");
                //���� ����
                chaseState[index] = false;

                //�����߱⿡ �߰��� �����Ѵ�.
                return false;
            }
            //������� �ʾƼ� ���� ����
            else
            {
                return true;
            }
        }
        //�������� �ʾ����� ���� ����
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

        //Debug.Log($"Ghost: {index} ���ǿ� �ش��ϴ� ��ġ�� �����Ǿ� �߰��� �����մϴ�.");
        chaseState[index] = true;
    }

    //������ �������� �ͽ��� ����� ������Ʈ�Ѵ�.
    void VisualUpdate(bool chasePlayer)
    {
        applyVisualUpdateRate -= Time.deltaTime;
        if (applyVisualUpdateRate < 0.0f)
        {
            //���� �������� �̵�
            ghostEffect.Movement(transform.position, transform.forward);

            //���Ѱ� ����
            if(nav.speed == 0) { applyVisualUpdateRate = 0.5f; return; }

            //������ �ð� ����
            if (chasePlayer) { applyVisualUpdateRate = visualUpdateDistancePlayer/ nav.speed; }
            else { applyVisualUpdateRate = visualUpdateDistanceMovement/nav.speed ; }
        }
    }

    //�ӵ� ������ ��ġ ����
    public void Activation(Vector3 position)
    {
        nav.enabled = false;

        transform.position = position;

        //�⺻ �ӵ� ����
        nav.speed = FindObjectOfType<FirstPersonController>().MoveSpeed * DataSet.Instance.GameDifficulty.GhostInitSpeedRate;

        EventManager.instance.CameraShake(2.0f, 0.25f);
        AudioManager.instance.PlayEffectiveSound("GhostScream", 1.1f, true);

        nav.enabled = true;
    }

    //��ȥ�� ��õ�� �ϰ� �� ���
    public void AscensededSoul()
    {
        Debug.Log("��ȥ�� ��õ�� �Ͽ� �ӵ��� �������ϴ�.");

        //1. �ӵ� ����
        nav.speed *= 1.15f;

        //�Ҹ��� �����ϴ�.
        AudioManager.instance.PlayEffectiveSound("GhostScream", 0.7f, true);
    }
}
