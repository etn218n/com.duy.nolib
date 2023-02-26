using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Nolib.DataStructure;

namespace Nolib.Node
{
    public class FSM : Node
    {
        #region Fields & Properties
        protected readonly string name;
        protected readonly CircularBuffer<Node> nodeStack;
        protected readonly Dictionary<Node, List<ITransition>> transitionMap;
        protected readonly Dictionary<Node, List<ITransition>> anyNodeTransitionMap;

        protected Node selectorNode;
        protected Node exitNode;
        protected Node currentNode;
        protected Node traceBackNode;
        
        protected bool isCompleted;
        protected int maxStackSize;
        protected List<ITransition> currentTransitionSet;

        public string Name => name;
        public int StateCount => transitionMap.Keys.Count - 1;
        public int MaxStackSize => maxStackSize;
        public bool IsCompleted => isCompleted;
        public Node CurrentNode => currentNode;
        public IReadOnlyCollection<Node> Nodes => transitionMap.Keys;
        public IReadOnlyCollection<ITransition> CurrentTransitionSet => currentTransitionSet;
        public IReadOnlyCollection<ITransition> TransitionsFrom(Node node) => transitionMap[node];

        public FSM(string name = "FSM", int maxStackSize = 20)
        {
            this.name = name;
            this.maxStackSize = maxStackSize;
            
            nodeStack = new CircularBuffer<Node>(maxStackSize);
            transitionMap = new Dictionary<Node, List<ITransition>>();
            anyNodeTransitionMap = new Dictionary<Node, List<ITransition>>();
            currentTransitionSet = new List<ITransition>();
            
            selectorNode  = new EmptyNode();
            traceBackNode = new EmptyNode();
            exitNode      = new ActionNode { EnterAction = () => isCompleted = true };
            currentNode   = selectorNode;
            
            AddNode(selectorNode);
            TransitionToNode(selectorNode);
        }
        #endregion

        #region Run FSM
        public void Start(Node entryNode = null)
        {
            isCompleted = false;

            if (entryNode != null && transitionMap.ContainsKey(entryNode))
                SetEntryNode(entryNode);
            
            var qualifiedTransition = CheckForQualifiedTransition(transitionMap[selectorNode]);

            if (qualifiedTransition != null)
                TransitionToNode(qualifiedTransition.Destination);
        }

        public NodeStatus Tick()
        {
            var qualifiedTransition = CheckForQualifiedTransition(currentTransitionSet);
            
            if (qualifiedTransition != null)
            {
                if (qualifiedTransition.Destination == traceBackNode && nodeStack.Count >= 2)
                {
                    nodeStack.PopTail();
                    TransitionToNode(nodeStack.PopTail());
                }
                else
                {
                    TransitionToNode(qualifiedTransition.Destination);
                }
            }
            
            currentNode.OnTick();
            
            return NodeStatus.Running;
        }

        public void Update() => currentNode.OnUpdate();
        public void FixedUpdate() => currentNode.OnFixedUpdate();
        public void LateUpdate() => currentNode.OnLateUpdate();
        public void ProcessInput() => currentNode.OnProcessInput();
        #endregion

        #region Node Callbacks
        protected internal override void OnEnter() => Start();
        protected internal override NodeStatus OnTick() => Tick();
        protected internal override void OnUpdate() => Update();
        protected internal override void OnFixedUpdate() => FixedUpdate();
        protected internal override void OnExit()
        {
            nodeStack.Clear();
            TransitionToNode(selectorNode);
        }
        #endregion

        #region Node Operations
        public bool Contains(Node node)
        {
            if (transitionMap.ContainsKey(node))
                return true;
            
            var subFSMs = from n in Nodes
                          where n is FSM
                          select n as FSM;

            return subFSMs.Any(sub => sub.Contains(node));
        }

        protected void AddNode(Node node)
        {
            if (!node.IsChildOf(this))
                node.Attach(this);
            
            if (transitionMap.Count == 1)
                SetEntryNode(node);
            
            if (!transitionMap.ContainsKey(node))
                transitionMap.Add(node, new List<ITransition>());
        }
        
        protected void SetEntryNode(Node node)
        {
            // override all transitions from Selector node
            transitionMap[selectorNode].Clear();
            transitionMap[selectorNode].Add(new Transition(selectorNode, node, () => true));
        }

        public void RemoveNode(Node node)
        {
            var foundNode = Nodes.FirstOrDefault(s => s == node);

            if (foundNode != null && foundNode != currentNode)
            {
                foundNode.Detach(this);
                transitionMap.Remove(node);
                
                if (anyNodeTransitionMap.ContainsKey(node))
                    anyNodeTransitionMap.Remove(node);

                foreach (var n in transitionMap.Keys)
                {
                    var relatedTransitions = from transition in transitionMap[n] 
                                             where transition.Destination == node 
                                             select transition;

                    foreach (var transition in relatedTransitions.ToList())
                        transitionMap[n].Remove(transition);
                }
                
                // TODO: remove node from node stack also
            }
        }

        public void SetCurrentNode(Node node)
        {
            if (!Contains(node))
                return;
            
            TransitionToNode(node);
        }

        public RegularSourceNode AddTransitionFrom(Node source) => new RegularSourceNode(this, source);
        public AnyNode AddTransitionFromAnyNode() => new AnyNode(this);
        public SelectorNode AddTransitionFromSelectorNode() => new SelectorNode(this);

        protected internal void AddTransition(Node source, Node destination, Func<bool> predicate)
        {
            if (!IsValidNode(source) || !IsValidNode(destination))
                return;
            
            AddNode(source);
            AddNode(destination);
            RegisterTransition(new Transition(source, destination, predicate));
        }

        protected internal void AddTransition(Node source, Node destination, UnityEvent unityEvent)
        {
            if (!IsValidNode(source) || !IsValidNode(destination))
                return;
            
            AddNode(source);
            AddNode(destination);
            RegisterTransition(new Transition(source, destination, unityEvent));
        }
        
        protected internal void AddTransition<T>(Node source, Node destination, UnityEvent<T> unityEvent)
        {
            if (!IsValidNode(source) || !IsValidNode(destination))
                return;
            
            AddNode(source);
            AddNode(destination);
            RegisterTransition(new Transition<T>(source, destination, unityEvent));
        }
        
        protected internal void AddTransitionFromAnyNode(Node destination, Func<bool> predicate)
        {
            if (!IsValidNode(destination))
                return;

            AddNode(destination);
            RegisterAnyNodeTransition(destination, new PollCondition(predicate));
        }

        protected internal void AddTransitionFromAnyNode(Node destination, UnityEvent unityEvent)
        {
            if (!IsValidNode(destination))
                return;

            AddNode(destination);
            RegisterAnyNodeTransition(destination, new UnityEventCondition(unityEvent));
        }
        
        protected internal void AddTransitionFromAnyNode<T>(Node destination, UnityEvent<T> unityEvent)
        {
            if (!IsValidNode(destination))
                return;

            AddNode(destination);
            RegisterAnyNodeTransition(destination, new UnityEventCondition<T>(unityEvent));
        }

        protected internal void AddTransitionToPreviousNode(Node source, Func<bool> predicate) => AddTransition(source, traceBackNode, predicate);
        protected internal void AddTransitionToPreviousNode(Node source, UnityEvent unityEvent) => AddTransition(source, traceBackNode, unityEvent);
        protected internal void AddTransitionToPreviousNode<T>(Node source, UnityEvent<T> unityEvent) => AddTransition(source, traceBackNode, unityEvent);

        protected internal void AddTransitionToExitNode(Node source, Func<bool> predicate) => AddTransition(source, exitNode, predicate);
        protected internal void AddTransitionToExitNode(Node source, UnityEvent unityEvent) => AddTransition(source, exitNode, unityEvent);
        protected internal void AddTransitionToExitNode<T>(Node source, UnityEvent<T> unityEvent) => AddTransition(source, exitNode, unityEvent);

        protected internal void AddTransitionFromSelectorNode(Node destination, Func<bool> predicate) => AddTransition(selectorNode, destination, predicate);
        protected internal void AddTransitionFromSelectorNode(Node destination, UnityEvent unityEvent) => AddTransition(selectorNode, destination, unityEvent);
        protected internal void AddTransitionFromSelectorNode<T>(Node destination, UnityEvent<T> unityEvent) => AddTransition(selectorNode, destination, unityEvent);
        #endregion

        #region Transition Operations
        protected void RegisterTransition(ITransition transition)
        {
            if (transition.Source == selectorNode)
            {
                transitionMap[selectorNode].Insert(0, transition);
                return;
            }
            
            transitionMap[transition.Source].Add(transition);
        }
        
        protected void RegisterAnyNodeTransition(Node destination, ICondition condition)
        {
            if (!anyNodeTransitionMap.ContainsKey(destination))
                anyNodeTransitionMap.Add(destination, new List<ITransition>());

            anyNodeTransitionMap[destination].Add(new Transition(null, destination, condition));
        }
        
        protected ITransition CheckForQualifiedTransition(List<ITransition> transitionSet)
        {
            foreach (var destinationNode in anyNodeTransitionMap.Keys)
            {
                if (destinationNode == currentNode)
                    continue;
                
                foreach (var transition in anyNodeTransitionMap[destinationNode])
                {
                    if (transition.Condition.IsTrue())
                        return transition;
                }
            }

            foreach (var transition in transitionSet)
            {
                if (transition.Condition.IsTrue())
                    return transition;
            }
            
            return null;
        }
        
        protected void TransitionToNode(Node node)
        {
            currentNode.OnExit();
            DeactivateConditions(currentNode);
            currentNode = node;
            ActivateConditions(currentNode);
            currentNode.OnEnter();
            
            nodeStack.PushTail(currentNode);
            currentTransitionSet = transitionMap[currentNode];
        }

        protected void ActivateConditions(Node node)
        {
            foreach (var destinationNode in anyNodeTransitionMap.Keys)
            {
                foreach (var transition in anyNodeTransitionMap[destinationNode])
                    transition.ActivateCondition();
            }
            
            foreach (var transition in transitionMap[node])
                transition.ActivateCondition();
        }
        
        protected void DeactivateConditions(Node node)
        {
            foreach (var destinationNode in anyNodeTransitionMap.Keys)
            {
                foreach (var transition in anyNodeTransitionMap[destinationNode])
                    transition.DeactivateCondition();
            }
            
            foreach (var transition in transitionMap[node])
                transition.DeactivateCondition();
        }
        #endregion
        
        #region Validations
        protected bool IsValidNode(Node node)
        {
            if (node == null)
            {
                Debug.Log($"{name}: node can not be NULL.");
                return false;
            }

            if (node == this)
            {
                Debug.Log($"{name}: can not add itself.");
                return false;
            }

            if (node.HasParent && !node.IsChildOf(this))
            {
                Debug.Log($"{name}: node is already owned by other node.");
                return false;
            }

            return true;
        }
        #endregion
    }
}