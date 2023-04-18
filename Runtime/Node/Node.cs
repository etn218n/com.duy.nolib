namespace Nolib.Node
{
    public enum NodeStatus
    {
        Running,
        Success,
        Failure
    }
    
    public abstract class Node
    {
        private Node parent;

        public bool HasParent => parent != null;
        
        public void Attach(Node node)
        {
            if (HasParent)
                return;

            parent = node;
        }

        public void Detach(Node node)
        { 
            if (!HasParent || parent != node)
                return;

            parent = null;
        }
        
        public bool IsChildOf(Node node)
        {
            return parent == node;
        }

        protected internal virtual void OnEnter() { }
        protected internal virtual void OnUpdate() { }
        protected internal virtual void OnFixedUpdate() { }
        protected internal virtual void OnLateUpdate() { }
        protected internal virtual void OnAnimatorMove() { }
        protected internal virtual void OnExit() { }
        protected internal virtual void OnPreTick(float deltaTime = 0) { }
        protected internal virtual NodeStatus OnTick(float deltaTime = 0) { return  NodeStatus.Failure; }
        protected internal virtual void OnPostTick(float deltaTime = 0) { }
    }
}
