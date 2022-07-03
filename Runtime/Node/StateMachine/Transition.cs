using System;
using UnityEngine.Events;

namespace Nolib.Node
{
    public class Transition
    {
        public INode Source { get; }
        public INode Destination { get; }
        public ICondition Condition { get; }

        public Transition(INode source, INode destination, Func<bool> predicate)
        {
            Source      = source;
            Destination = destination;
            Condition   = new PollCondition(predicate);
        }
        
        public Transition(INode source, INode destination, UnityEvent unityEvent)
        {
            Source      = source;
            Destination = destination;
            Condition   = new UnityEventCondition(unityEvent);
        }

        public Transition(INode source, INode destination, ICondition condition)
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
}