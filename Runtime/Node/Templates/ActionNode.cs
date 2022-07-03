using System;

namespace Nolib.Node
{
    public class ActionNode : INode
    {
        public Action EnterAction  = () => { };
        public Action UpdateAction = () => { };
        public Action ExitAction   = () => { };
        public Action FixedUpdateAction = () => { };

        public void OnEnter()  => EnterAction();
        public void OnUpdate() => UpdateAction();
        public void OnExit()   => ExitAction();
        public void OnFixedUpdate() => FixedUpdateAction();
    }
}