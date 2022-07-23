using System;
using System.Linq;
using System.Collections.Generic;
using Nolib.DataStructure;
using UnityEngine;
using UnityEngine.Events;

namespace Nolib.Node
{
    public class FSM
    {
        protected readonly string name;
        protected readonly CircularBuffer<INode> nodeStack;
        protected readonly List<INode> nodesPointedByEveryNode;
        protected readonly Dictionary<INode, List<Transition>> transitionMap;

        protected INode selectorNode;
        protected INode exitNode;
        protected INode currentNode;
        protected INode traceBackNode;
        
        protected List<Transition> currentTransitionSet;
        protected int maxNodeStackSize;

        public string Name => name;
        public int NodeCount => transitionMap.Keys.Count - 1;
        public int MaxNodeStackSize => maxNodeStackSize;
        public INode CurrentNode => currentNode;
        public IReadOnlyCollection<INode> Nodes => transitionMap.Keys;
        public IReadOnlyCollection<Transition> CurrentTransitionSet => currentTransitionSet;
        public IReadOnlyCollection<Transition> TransitionsFrom(INode node) => transitionMap[node];

        public FSM(string name = "FSM", int maxNodeStackSize = 20)
        {
            this.name = name;
            this.maxNodeStackSize = maxNodeStackSize;
            
            nodeStack = new CircularBuffer<INode>(maxNodeStackSize);
            transitionMap = new Dictionary<INode, List<Transition>>();
            currentTransitionSet = new List<Transition>();
            nodesPointedByEveryNode = new List<INode>();
            
            selectorNode  = new EmptyNode(); 
            exitNode      = new EmptyNode(); 
            currentNode   = new EmptyNode();
            traceBackNode = new EmptyNode();
            
            transitionMap.Add(selectorNode, new List<Transition>());
            SetCurrentNode(selectorNode);
        }

        public void Start()
        {
            var qualifiedTransition = CheckForQualifiedTransition(transitionMap[selectorNode]);

            if (qualifiedTransition != null)
                SetCurrentNode(qualifiedTransition.Destination);
            else
                Debug.Log($"{name}: does not have any node to start with.");
            
            currentNode.OnEnter();
        }

        public void Update()
        {
            var qualifiedTransition = CheckForQualifiedTransition(currentTransitionSet);
            
            if (qualifiedTransition != null)
            {
                currentNode.OnExit();
                
                if (qualifiedTransition.Destination == traceBackNode && nodeStack.Count >= 2)
                {
                    nodeStack.PopTail();
                    qualifiedTransition = new Transition(qualifiedTransition.Source, nodeStack.PopTail(), qualifiedTransition.Condition);
                }
                
                SetCurrentNode(qualifiedTransition.Destination);
                currentNode.OnEnter();
                return;
            }
            
            currentNode.OnUpdate();
        }

        public void FixedUpdate()
        {
            currentNode.OnFixedUpdate();
        }

        public bool Contains(INode node)
        {
            if (transitionMap.ContainsKey(node))
                return true;
            
            var subFSMs = from n in Nodes
                          where n is SubFSM
                          select n as SubFSM;

            return subFSMs.Any(sub => sub.Contains(node));
        }
        
        public bool Intersect(INode node)
        {
            if (transitionMap.ContainsKey(node))
                return true;

            return IntersectWithAnySubFSM(node);
        }
        
        public bool IntersectWithAnySubFSM(INode node)
        {
            if (node is SubFSM subFSM && subFSM.Nodes.Intersect(Nodes).Any())
                return true;
        
            var subFSMs = from n in Nodes
                          where n is SubFSM && n != node
                          select n as SubFSM;
        
            return subFSMs.Any(sub => sub.Intersect(node));
        }

        public void AddNode(INode node)
        {
            if (!IsValidNode(node))
                return;

            // Handle first node
            if (transitionMap.Count == 1)
                transitionMap[selectorNode].Add(new Transition(selectorNode, node, () => true));
            
            if (!transitionMap.ContainsKey(node))
                transitionMap.Add(node, new List<Transition>());
            
            if (node is SubFSM subFSM)
                subFSM.SetOwner(this);
        }

        public void RemoveNode(INode node)
        {
            if (Nodes.Contains(node))
            {
                transitionMap.Remove(node);
                
                if (nodesPointedByEveryNode.Contains(node))
                    nodesPointedByEveryNode.Remove(node);

                foreach (var n in transitionMap.Keys)
                {
                    var relatedTransitions = from t in transitionMap[n] 
                                             where t.Destination == node 
                                             select t;

                    foreach (var transition in relatedTransitions.ToList())
                        transitionMap[n].Remove(transition);
                }
                
                // TODO: remove node from node stack also
                // TODO: resolve scenario where current node is the node the remove
            }
            else
            {
                var subFSMs = from n in Nodes
                              where n is SubFSM
                              select n as SubFSM;
                
                subFSMs.ToList().ForEach(sub => sub.RemoveNode(node));
            }
        }

        public void AddTransition(INode source, INode destination, Func<bool> predicate)
        {
            if (!IsValidNode(source) || !IsValidNode(destination))
                return;
            
            AddNode(source);
            AddNode(destination);
            RegisterTransition(new Transition(source, destination, predicate));
        }

        public void AddTransition(INode source, INode destination, UnityEvent unityEvent)
        {
            if (!IsValidNode(source) || !IsValidNode(destination))
                return;
            
            AddNode(source);
            AddNode(destination);
            RegisterTransition(new Transition(source, destination, unityEvent));
        }
        
        public void AddTransitionFromAnyNode(INode destination, Func<bool> predicate)
        {
            if (nodesPointedByEveryNode.Contains(destination))
                return;

            AddNode(destination);
            RegisterAnyNodeTransition(destination, new PollCondition(predicate));
        }

        public void AddTransitionFromAnyNode(INode destination, UnityEvent unityEvent)
        {
            if (nodesPointedByEveryNode.Contains(destination))
                return;

            AddNode(destination);
            RegisterAnyNodeTransition(destination, new UnityEventCondition(unityEvent));
        }

        public void AddTransitionToPreviousNode(INode source, Func<bool> predicate)
        {
            AddTransition(source, traceBackNode, predicate);
        }
        
        public void AddTransitionToPreviousNode(INode source, UnityEvent unityEvent)
        {
            AddTransition(source, traceBackNode, unityEvent);
        }
        
        public void AddTransitionToExitNode(INode source, Func<bool> predicate)
        {
            AddTransition(source, exitNode, predicate);
        }
        
        public void AddTransitionToExitNode(INode source, UnityEvent unityEvent)
        {
            AddTransition(source, exitNode, unityEvent);
        }

        public void AddTransitionFromSelectorNode(INode destination, Func<bool> predicate)
        {
            AddTransition(selectorNode, destination, predicate);
        }
        
        public void AddTransitionFromSelectorNode(INode destination, UnityEvent unityEvent)
        {
            AddTransition(selectorNode, destination, unityEvent);
        }

        protected void RegisterTransition(Transition transition)
        {
            var source = transition.Source;

            transitionMap[source].Add(transition);
            
            foreach (var node in nodesPointedByEveryNode.Where(node => node != selectorNode))
                transitionMap[source].Insert(0, new Transition(source, node, transition.Condition));
        }
        
        protected void RegisterAnyNodeTransition(INode destination, ICondition condition)
        {
            nodesPointedByEveryNode.Add(destination);

            foreach (var node in Nodes.Where(node => node != destination && node != selectorNode))
                transitionMap[node].Insert(0, new Transition(node, destination, condition));
        }
        
        protected Transition CheckForQualifiedTransition(List<Transition> transitionSet)
        {
            foreach (var transition in transitionSet)
            {
                if (transition.Condition.IsTrue() && transition.Destination != currentNode)
                    return transition;
            } 
            
            return null;
        }
        
        protected void SetCurrentNode(INode node)
        {
            if (transitionMap.ContainsKey(currentNode))
            {
                foreach (var transition in transitionMap[node])
                    transition.DeactivateCondition();
            }
            
            currentNode  = node;
            nodeStack.PushTail(currentNode);
            currentTransitionSet = transitionMap[currentNode];
            
            foreach (var transition in transitionMap[currentNode])
                transition.ActivateCondition();
        }
        
        public void SetEntry(INode node)
        {
            if (!transitionMap.ContainsKey(node))
            {
                Debug.Log($"{name}: entry node must be contained in this FSM.");
                return;
            }
            
            transitionMap[selectorNode].Insert(0, new Transition(selectorNode, node, () => true));
        }

        public virtual bool IsValidNode(INode node)
        {
            if (node == null)
            {
                Debug.Log($"{name}: node can not be NULL.");
                return false;
            }

            if (IntersectWithAnySubFSM(node))
            {
                Debug.Log($"{name}: already contained this node.");
                return false;
            }

            return true;
        }
    }
}