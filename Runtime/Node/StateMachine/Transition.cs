using System;
using UnityEngine.Events;

namespace Nolib.Node
{
    public interface ITransition
    {
        Node Source { get; }
        Node Destination { get; }
        ICondition Condition { get; }

        void ActivateCondition();
        void DeactivateCondition();
    }
    
    public class Transition : ITransition
    {
        public Node Source { get; }
        public Node Destination { get; }
        public ICondition Condition { get; }

        public static Transition Empty = new Transition(null, null, () => false);
        
        public Transition(Node source, Node destination, Func<bool> predicate)
        {
            Source      = source;
            Destination = destination;
            Condition   = new PollCondition(predicate);
        }
        
        public Transition(Node source, Node destination, UnityEvent unityEvent)
        {
            Source      = source;
            Destination = destination;
            Condition   = new UnityEventCondition(unityEvent);
        }

        public Transition(Node source, Node destination, ICondition condition)
        {
            Source      = source;
            Destination = destination;
            Condition   = condition;
        }

        public void ActivateCondition()
        {
            Condition.Activate();
        }
        
        public void DeactivateCondition()
        {
            Condition.Deactivate();
        }
    }

    public class Transition<T> : ITransition
    {
        public Node Source { get; }
        public Node Destination { get; }
        public ICondition Condition { get; }

        public Transition(Node source, Node destination, UnityEvent<T> unityEvent)
        {
            Source      = source;
            Destination = destination;
            Condition   = new UnityEventCondition<T>(unityEvent);
        }
        
        public void ActivateCondition()
        {
            Condition.Activate();
        }

        public void DeactivateCondition()
        {
            Condition.Deactivate();
        }
    }
}