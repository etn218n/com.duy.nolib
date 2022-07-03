using System;
using UnityEngine.Events;

namespace Nolib.Node
{
    public interface ICondition
    {
        bool IsTrue();
        void Activate();
        void Deactivate();
    }
    
    public class PollCondition : ICondition
    {
        private Func<bool> predicate;
        
        public PollCondition(Func<bool> predicate) => this.predicate = predicate;
        
        public bool IsTrue() => predicate();

        public void Activate() { }
        public void Deactivate() { }
    }

    public class UnityEventCondition : ICondition
    {
        private UnityEvent unityEvent;
        private bool isTriggered;

        public UnityEventCondition(UnityEvent unityEvent) => this.unityEvent = unityEvent;

        private void OnEventTriggered() => isTriggered = true;

        public bool IsTrue() => isTriggered;

        public void Activate()
        {
            if (unityEvent == null)
                return;

            isTriggered = false;
            unityEvent.AddListener(OnEventTriggered);
        }

        public void Deactivate()
        {
            if (unityEvent == null)
                return;
            
            unityEvent.RemoveListener(OnEventTriggered);
        }
    }
}