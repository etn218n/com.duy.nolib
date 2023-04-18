using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nolib.Node
{
    public abstract class CompositeNode : Node
    {
        protected List<Node> children;
        protected Func<bool> condition;
        protected bool isConditionMet;

        protected CompositeNode(params Node[] children) : this(() => true, children)
        {
        }

        protected CompositeNode(Func<bool> condition, params Node[] children)
        {
            this.condition = condition;
            this.children  = new List<Node>(children.Length);

            foreach (var node in children)
            {
                if (IsValidNode(node))
                    this.children.Add(node);
            }
        }

        public void AddChildNode(Node node)
        {
            if (!IsValidNode(node))
                return;
            
            children.Add(node);
            node.Attach(this);
            OnChildNodeAdded(node);
        }
        
        public void RemoveChildNode(Node node)
        {
            if (!node.IsChildOf(this))
                return;
            
            children.Remove(node);
            node.Detach(this);
            OnChildNodeRemoved(node);
        }

        public void Start()
        {
            OnEnter();
        }

        public NodeStatus Tick(float deltaTime = 0)
        {
            return OnTick(deltaTime);
        }
        
        public void PreTick(float deltaTime = 0)
        {
            OnPreTick(deltaTime);
        }
        
        public void PostTick(float deltaTime = 0)
        {
            OnPostTick(deltaTime);
        }

        public void Update()
        {
            OnUpdate();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate();
        }
        
        public void LateUpdate()
        {
            OnLateUpdate();
        }
        
        public void AnimatorMove()
        {
            OnAnimatorMove();
        }
        
        protected internal override void OnEnter()
        {
            isConditionMet = condition.Invoke();
        }

        protected bool IsValidNode(Node node)
        {
            if (node == null)
                throw new NullReferenceException();

            if (node == this)
                throw new InvalidOperationException();

            if (node.HasParent && !node.IsChildOf(this))
                throw new InvalidOperationException();

            if (node.IsChildOf(this))
            {
                Debug.Log("Already contained this node.");
                return false;
            }
            
            return true;
        }
        
        protected virtual void OnChildNodeAdded(Node node) { }
        protected virtual void OnChildNodeRemoved(Node node) { }
    }
}