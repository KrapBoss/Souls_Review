using System.Collections.Generic;

public enum NodeState
    {
        RUNNING,    // 진행 중
        SUCCESS,    // 성공
        FAILURE     // 실패
    }

    /// <summary>
    /// 기본 베이스 노드
    /// </summary>
    public abstract class Node
    {
        protected NodeState state;
        protected List<Node> children = new List<Node>();

        public Node() { }

        // 순차대로 실행할 자식 노드를 저장
        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node node)
        {
            children.Add(node);
        }

        public abstract NodeState Evaluate();
    }