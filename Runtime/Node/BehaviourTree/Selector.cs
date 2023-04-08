using System;

namespace Nolib.Node
{
    public class Selector : CompositeNode
    {
        private int currentIndex;
        private NodeStatus currentStatus;

        public Selector(params Node[] children) : base(children)
        {
            currentStatus = NodeStatus.Failure;
        }

        public Selector(Func<bool> condition, params Node[] children) : base(condition, children)
        {
            currentStatus = NodeStatus.Failure;
        }

        protected internal override NodeStatus OnTick(float deltaTime)
        {
            if (!isConditionMet)
                return NodeStatus.Failure;
            
            while (currentIndex < children.Count)
            {
                if (currentStatus != NodeStatus.Running)
                    children[currentIndex].OnEnter();

                currentStatus = children[currentIndex].OnTick(deltaTime);

                switch (currentStatus)
                {
                    case NodeStatus.Running: return NodeStatus.Running;
                    case NodeStatus.Failure: children[currentIndex].OnExit(); currentIndex++; break;
                    case NodeStatus.Success: children[currentIndex].OnExit(); currentIndex = 0; return NodeStatus.Success;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            currentIndex = 0;
            return NodeStatus.Failure;
        }
    }
}