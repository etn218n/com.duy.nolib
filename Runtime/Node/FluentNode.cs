using System;
using UnityEngine.Events;

namespace Nolib.Node
{
    public enum NodeType { Regular, Selector, Any, Exit, Previous }
    
    public class RegularSourceNode
    {
        private FSM fsm;
        private Node source;
        
        public RegularSourceNode(FSM fsm, Node source)
        {
            this.fsm = fsm;
            this.source = source;
        }

        public RegularDestinationNode To(Node destination)
        {
            return new RegularDestinationNode(fsm, source, destination, NodeType.Regular);
        }
        
        public ExitNode ToExitNode()
        {
            return new ExitNode(fsm, source);
        }
        
        public PreviousNode ToPreviousNode()
        {
            return new PreviousNode(fsm, source);
        }
    }
    
    public class RegularDestinationNode
    {
        private FSM fsm;
        private Node source;
        private Node destination;
        private NodeType sourceType;
        
        public RegularDestinationNode(FSM fsm, Node source, Node destination, NodeType sourceType)
        {
            this.fsm = fsm;
            this.source = source;
            this.destination = destination;
            this.sourceType  = sourceType;
        }
        
        public void When(Func<bool> predicate)
        {
            switch (sourceType)
            {
                case NodeType.Any: fsm.AddTransitionFromAnyNode(destination, predicate); break;
                case NodeType.Regular: fsm.AddTransition(source, destination, predicate); break;
                case NodeType.Selector: fsm.AddTransitionFromSelectorNode(destination, predicate); break;
            }
        }
        
        public void When(UnityEvent unityEvent)
        {
            switch (sourceType)
            {
                case NodeType.Any: fsm.AddTransitionFromAnyNode(destination, unityEvent); break;
                case NodeType.Regular: fsm.AddTransition(source, destination, unityEvent); break;
                case NodeType.Selector: fsm.AddTransitionFromSelectorNode(destination, unityEvent); break;
            }
        }
        
        public void When<T>(UnityEvent<T> unityEvent)
        {
            switch (sourceType)
            {
                case NodeType.Any: fsm.AddTransitionFromAnyNode(destination, unityEvent); break;
                case NodeType.Regular: fsm.AddTransition(source, destination, unityEvent); break;
                case NodeType.Selector: fsm.AddTransitionFromSelectorNode(destination, unityEvent); break;
            }
        }
    }
    
    public class AnyNode
    {
        private FSM fsm;

        public AnyNode(FSM fsm)
        {
            this.fsm = fsm;
        }

        public RegularDestinationNode To(Node destination)
        {
            return new RegularDestinationNode(fsm, null, destination, NodeType.Any);
        }
    }
    
    public class SelectorNode
    {
        private FSM fsm;

        public SelectorNode(FSM fsm)
        {
            this.fsm = fsm;
        }

        public RegularDestinationNode To(Node destination)
        {
            return new RegularDestinationNode(fsm, null, destination, NodeType.Selector);
        }
    }
    
    public class ExitNode
    {
        private FSM fsm;
        private Node source;
        
        public ExitNode(FSM fsm, Node source)
        {
            this.fsm    = fsm;
            this.source = source;
        }

        public void When(Func<bool> predicate)
        {
            fsm.AddTransitionToExitNode(source, predicate);
        }
        
        public void When(UnityEvent unityEvent)
        {
            fsm.AddTransitionToExitNode(source, unityEvent);
        }
        
        public void When<T>(UnityEvent<T> unityEvent)
        {
            fsm.AddTransitionToExitNode(source, unityEvent);
        }
    }
    
    public class PreviousNode
    {
        private FSM fsm;
        private Node source;
        
        public PreviousNode(FSM fsm, Node source)
        {
            this.fsm    = fsm;
            this.source = source;
        }

        public void When(Func<bool> predicate)
        {
            fsm.AddTransitionToPreviousNode(source, predicate);
        }
        
        public void When(UnityEvent unityEvent)
        {
            fsm.AddTransitionToPreviousNode(source, unityEvent);
        }
        
        public void When<T>(UnityEvent<T> unityEvent)
        {
            fsm.AddTransitionToPreviousNode(source, unityEvent);
        }
    }
}