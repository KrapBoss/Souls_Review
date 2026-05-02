using System.Collections.Generic;

/// <summary>
/// NodeState 순서에 따라 리턴, 컨티뉴, 컨티뉴로 진행
/// </summary>
public class Sequence : Node
{
    public Sequence() : base() { }
    public Sequence(List<Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;

        foreach (Node node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.FAILURE:         //  실패
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.SUCCESS:         //     성공
                    continue;
                case NodeState.RUNNING:         //     진행
                    anyChildIsRunning = true;
                    continue;
                default:
                    state = NodeState.SUCCESS;
                    return state;
            }
        }

        state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}
