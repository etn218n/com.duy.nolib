using System.Linq;
using UnityEngine;

namespace Nolib.Node
{
    public class SubFSM : FSM, INode
    {
        private FSM ownerFSM;
        private bool isFinished;

        public bool IsFinished => isFinished;
        public bool HasOwner => ownerFSM != null;

        public SubFSM(string name = "SubFSM") : base(name)
        {
            exitNode = new ActionNode { EnterAction = () => isFinished = true };
        }
        
        public void OnEnter()
        {
            isFinished = false;
            Start();
        }

        public void OnUpdate() => Update();
        public void OnFixedUpdate() => FixedUpdate();
        
        public void OnExit()
        {
            nodeStack.Clear();
            SetCurrentNode(selectorNode);
        }

        public void SetOwner(FSM owner)
        {
            ownerFSM = owner;
            
            var subFSMs = from n in Nodes
                          where n is SubFSM
                          select n as SubFSM;
            
            subFSMs.ToList().ForEach(sub => sub.SetOwner(owner));
        }

        public override bool IsValidNode(INode node)
        {
            if (HasOwner)
            {
                if (ownerFSM.Contains(node))
                {
                    Debug.Log($"{name}: owner already contained this node.");
                    return false;
                }
            }
            else
            {
                if (!base.IsValidNode(node))
                    return false;
            }

            if (node == this)
            {
                Debug.Log($"{name}: can not reference itself.");
                return false;
            }

            return true;
        }
    }
}