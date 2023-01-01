using System;

namespace Nolib.Node
{
    public class ActionNode : Node
    {
        public Action EnterAction = () => { };
        public Action ExitAction  = () => { };
        public Action UpdateAction = () => { };
        public Action FixedUpdateAction = () => { };
        public Func<NodeStatus> TickAction = () => NodeStatus.Failure;

        protected internal override void OnEnter() => EnterAction();
        protected internal override void OnExit() => ExitAction();
        protected internal override void OnUpdate() => UpdateAction();
        protected internal override void OnFixedUpdate() => FixedUpdateAction();
        protected internal override NodeStatus OnTick() => TickAction();
    }
    
    public class ActionNode<T> : Node
    {
        protected T context;
        
        public Action<T> EnterAction = c => { };
        public Action<T> ExitAction  = c => { };
        public Action<T> UpdateAction = c => { };
        public Action<T> FixedUpdateAction = c => { };
        public Func<T, NodeStatus> TickAction = c => NodeStatus.Failure;

        public ActionNode(T context) => this.context = context;

        protected internal override void OnEnter() => EnterAction(context);
        protected internal override void OnExit() => ExitAction(context);
        protected internal override void OnUpdate() => UpdateAction(context);
        protected internal override void OnFixedUpdate() => FixedUpdateAction(context);
        protected internal override NodeStatus OnTick() => TickAction(context);
    }
}