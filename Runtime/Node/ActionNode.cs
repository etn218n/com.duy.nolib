using System;

namespace Nolib.Node
{
    public class ActionNode : Node
    {
        public Action EnterAction = () => { };
        public Action ExitAction  = () => { };
        public Action UpdateAction = () => { };
        public Action FixedUpdateAction = () => { };
        public Action LateUpdateAction = () => { };
        public Action ProcessInputAction = () => { };
        public Func<NodeStatus> TickAction = () => NodeStatus.Failure;

        protected internal override void OnEnter() => EnterAction();
        protected internal override void OnExit() => ExitAction();
        protected internal override void OnUpdate() => UpdateAction();
        protected internal override void OnFixedUpdate() => FixedUpdateAction();
        protected internal override void OnLateUpdate() => LateUpdateAction();
        protected internal override void OnProcessInput() => ProcessInputAction();
        protected internal override NodeStatus OnTick() => TickAction();
    }
    
    public class ActionNode<T> : Node
    {
        protected T context;
        
        public Action<T> EnterAction = c => { };
        public Action<T> ExitAction  = c => { };
        public Action<T> UpdateAction = c => { };
        public Action<T> FixedUpdateAction = c => { };
        public Action<T> LateUpdateAction = c => { };
        public Action<T> ProcessInputAction = c => { };
        public Func<T, NodeStatus> TickAction = c => NodeStatus.Failure;

        public ActionNode(T context) => this.context = context;

        protected internal override void OnEnter() => EnterAction(context);
        protected internal override void OnExit() => ExitAction(context);
        protected internal override void OnUpdate() => UpdateAction(context);
        protected internal override void OnFixedUpdate() => FixedUpdateAction(context);
        protected internal override void OnLateUpdate() => LateUpdateAction(context);
        protected internal override void OnProcessInput() => ProcessInputAction(context);
        protected internal override NodeStatus OnTick() => TickAction(context);
    }
}