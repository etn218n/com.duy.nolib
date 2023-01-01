using System;

namespace Nolib.Node
{
    public class Sequencer : CompositeNode
    {
        private int currentIndex;
        private NodeStatus currentStatus;

        public Sequencer(params Node[] children) : base(children)
        {
            currentStatus = NodeStatus.Failure;
        }
        
        public Sequencer(Func<bool> condition, params Node[] children) : base(condition, children)
        {
            currentStatus = NodeStatus.Failure;
        }

        protected internal override NodeStatus OnTick()
        {
            if (!isConditionMet)
                return NodeStatus.Failure;
            
            while (currentIndex < children.Count)
            {
                if (currentStatus != NodeStatus.Running)
                    children[currentIndex].OnEnter();

                currentStatus = children[currentIndex].OnTick();

                switch (currentStatus)
                {
                    case NodeStatus.Running: return NodeStatus.Running;
                    case NodeStatus.Success: children[currentIndex].OnExit(); currentIndex++; break;
                    case NodeStatus.Failure: children[currentIndex].OnExit(); currentIndex = 0; return NodeStatus.Failure;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            currentIndex = 0;
            return NodeStatus.Success;
        }
    }
}