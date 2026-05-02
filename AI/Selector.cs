using System.Collections.Generic;

/// <summary>
/// 실패 시 다음 분기 노드로 향하도록 합니다.
/// 중간에서 뻗어나갈 가지를 고르는 역할
/// </summary>
public class Selector : Node
{
    public Selector() : base() { }
    public Selector(List<Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        foreach (Node node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.FAILURE: // 실패하면 다음 분기 진행
                    continue;
                case NodeState.SUCCESS: // 성공하면 중지
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.RUNNING: // 진행 중이면 중지
                    state = NodeState.RUNNING;
                    return state;
                default:
                    continue;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}