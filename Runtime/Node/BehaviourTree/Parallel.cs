using System;
using System.Collections.Generic;

namespace Nolib.Node
{
    public class Parallel : CompositeNode
    {
        public enum TerminationPolicy { AnyFailure, AllFailure }
        
        private List<NodeStatus> statuses;
        private TerminationPolicy policy;

        public Parallel(TerminationPolicy policy, params Node[] children) : base(children)
        {
            this.policy   = policy;
            this.statuses = new List<NodeStatus>(children.Length);

            foreach (var node in children)
                statuses.Add(NodeStatus.Failure);
        }

        protected override void OnChildNodeAdded(Node node)
        {
            statuses.Add(NodeStatus.Failure);
        }

        protected override void OnChildNodeRemoved(Node node)
        {
            statuses.RemoveAt(children.IndexOf(node));
        }

        protected internal override NodeStatus OnTick()
        {
            if (!isConditionMet)
                return NodeStatus.Failure;
                        
            for (int i = 0; i < children.Count; i++)
            {
                if (statuses[i] != NodeStatus.Running)
                    children[i].OnEnter();

                statuses[i] = children[i].OnTick();
            }

            var shouldTerminate = EvaluateTerminationPolicy();

            if (shouldTerminate)
            {
                foreach (var node in children)
                    node.OnExit();

                return NodeStatus.Failure;
            }

            var finalStatus = NodeStatus.Success;
            
            for (int i = 0; i < children.Count; i++)
            {
                if (statuses[i] != NodeStatus.Running)
                    children[i].OnExit();
                else
                    finalStatus = NodeStatus.Running;
            }

            return finalStatus;
        }

        private bool EvaluateTerminationPolicy()
        {
            switch (policy)
            {
                case TerminationPolicy.AnyFailure:
                {
                    for (int i = 0; i < children.Count; i++)
                        if (statuses[i] == NodeStatus.Failure)
                            return true;

                    return false;
                }
                
                case TerminationPolicy.AllFailure:
                {
                    for (int i = 0; i < children.Count; i++)
                        if (statuses[i] != NodeStatus.Failure)
                            return false;

                    return true;
                }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}