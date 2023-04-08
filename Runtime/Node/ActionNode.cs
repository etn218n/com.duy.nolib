using System;

namespace Nolib.Node
{
    public class ActionNode : Node
    {
        public Action EnterAction  = () => { };
        public Action ExitAction   = () => { };
        public Action UpdateAction = () => { };
        public Action FixedUpdateAction = () => { };
        public Action LateUpdateAction  = () => { };

        public Func<float, NodeStatus> TickAction = deltaTime => NodeStatus.Failure;
        public Action<float> PreTickAction  = deltaTime => { };
        public Action<float> PostTickAction = deltaTime => { };

        protected internal override void OnEnter() => EnterAction();
        protected internal override void OnExit() => ExitAction();
        protected internal override void OnUpdate() => UpdateAction();
        protected internal override void OnFixedUpdate() => FixedUpdateAction();
        protected internal override void OnLateUpdate() => LateUpdateAction();
        protected internal override NodeStatus OnTick(float deltaTime = 0) => TickAction(deltaTime);
        protected internal override void OnPreTick(float deltaTime = 0) => PreTickAction(deltaTime);
        protected internal override void OnPostTick(float deltaTime = 0) => PostTickAction(deltaTime);
    }
    
    public class ActionNode<T> : Node
    {
        protected T context;
        
        public Action<T> EnterAction  = context => { };
        public Action<T> ExitAction   = context => { };
        public Action<T> UpdateAction = context => { };
        public Action<T> FixedUpdateAction = context => { };
        public Action<T> LateUpdateAction  = context => { };
        public Func<T, float, NodeStatus> TickAction = (context, deltaTime) => NodeStatus.Failure;
        public Action<T, float> PreTickAction  = (context, deltaTime) => { };
        public Action<T, float> PostTickAction = (context, deltaTime) => { };

        public ActionNode(T context) => this.context = context;

        protected internal override void OnEnter() => EnterAction(context);
        protected internal override void OnExit() => ExitAction(context);
        protected internal override void OnUpdate() => UpdateAction(context);
        protected internal override void OnFixedUpdate() => FixedUpdateAction(context);
        protected internal override void OnLateUpdate() => LateUpdateAction(context);
        protected internal override NodeStatus OnTick(float deltaTime = 0) => TickAction(context, deltaTime);
        protected internal override void OnPreTick(float deltaTime = 0) => PreTickAction(context, deltaTime);
        protected internal override void OnPostTick(float deltaTime = 0) => PostTickAction(context, deltaTime);
    }
}