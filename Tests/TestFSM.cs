using Nolib.Node;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Tests
{
    public class TestFSM
    {
        [Test]
        public void AddNode_FirstNode_IsEntryNode()
        {
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            
            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.Start();

            Assert.IsTrue(fsm.CurrentNode == stateA);
        }
        
        [Test]
        public void Start_WithEntryNode()
        {
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.AddTransitionFrom(stateB).To(stateC).When(() => true);
            fsm.Start(stateB);
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateC);
        }
        
        [Test]
        public void RemoveNode_FromFSM()
        {
            var fsm    = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.AddTransitionFrom(stateB).To(stateC).When(() => true);
            fsm.RemoveNode(stateB);

            Assert.IsTrue(!fsm.Contains(stateB));
        }

        [Test]
        public void RemoveSubFSM_FromFSM_RemoveAllSubNodes()
        {
            var fsm    = new FSM();
            var subFSM = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();
            var stateD = new EmptyNode();

            subFSM.AddTransitionFrom(stateC).To(stateD).When(() => true);
            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.AddTransitionFrom(stateB).To(subFSM).When(() => true);
            fsm.RemoveNode(subFSM);

            Assert.IsTrue(!fsm.Contains(subFSM) && !fsm.Contains(stateC) && !fsm.Contains(stateD));
        }

        [Test]
        public void Transition_NodeToNode_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => n == 1);
            fsm.Start();
            n = 1;
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateB);
        }

        [Test]
        public void Transition_NodeToSubFSM_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var subFSM = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();

            fsm.AddTransitionFrom(stateA).To(subFSM).When(() => n == 1);
            subFSM.AddTransitionFrom(stateB).To(stateC).When(() => n == 2);
            fsm.Start();
            n = 1;
            fsm.Tick();
            n = 2;
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == subFSM && subFSM.CurrentNode == stateC);
        }

        [Test]
        public void Transition_NodeToNode_WhenUnityEventIsTriggered()
        {
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();
            var unityEventB = new UnityEvent();
            var unityEventC = new UnityEvent();

            fsm.AddTransitionFrom(stateA).To(stateB).When(unityEventB);
            fsm.AddTransitionFrom(stateB).To(stateC).When(unityEventC);
            fsm.Start();
            unityEventB.Invoke();
            fsm.Tick();
            unityEventC.Invoke();
            fsm.Tick();
            
            Assert.IsTrue(fsm.CurrentNode == stateC);
        }

        [Test]
        public void Transition_SubFSMToSubFSM_WhenPredicateIsTrue()
        {
            var n       = 0;
            var fsm     = new FSM();
            var subFSMA = new FSM();
            var subFSMB = new FSM();
            var stateA  = new EmptyNode();
            var stateB  = new EmptyNode();
            var stateC  = new EmptyNode();
            var stateD  = new EmptyNode();
            
            fsm.AddTransitionFrom(stateA).To(subFSMA).When(() => n == 1);
            subFSMA.AddTransitionFrom(stateB).To(subFSMB).When(() => n == 2);
            subFSMB.AddTransitionFrom(stateC).To(stateD).When(() => n == 3);
            fsm.Start();
            n = 1;
            fsm.Tick();
            n = 2;
            fsm.Tick();
            n = 3;
            fsm.Tick();
            
            Assert.IsTrue(fsm.CurrentNode == subFSMA && subFSMA.CurrentNode == subFSMB && subFSMB.CurrentNode == stateD);
        }

        [Test]
        public void Transition_FromAnyNode_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => n == 1);
            fsm.AddTransitionFromAnyNode().To(stateC).When(() => n == 2);
            fsm.Start();
            n = 1;
            fsm.Tick();
            n = 2;
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateC);
        }
        
        [Test]
        public void Transition_FromAnyNode_WhenEventIsTriggered()
        {
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();
            var unityEventB = new UnityEvent();
            var unityEventC = new UnityEvent();

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.AddTransitionFromAnyNode().To(stateC).When(unityEventC);
            fsm.Start();
            unityEventB.Invoke();
            fsm.Tick();
            unityEventC.Invoke();
            fsm.Tick();
            
            Assert.IsTrue(fsm.CurrentNode == stateC);
        }
        
        [Test]
        public void Transition_FromAnyNode_ExcludeItself()
        {
            var n = 0;
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new ActionNode { EnterAction = () => n = 6 };

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.AddTransitionFromAnyNode().To(stateC).When(() => n == 1);
            fsm.Start();
            n = 1;
            fsm.Tick();
            n = 1;
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateC && n == 1);
        }

        [Test]
        public void Transition_ToPreviousNode_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.AddTransitionFrom(stateB).ToPreviousNode().When(() => n == 1);
            fsm.Start();
            fsm.Tick();
            n = 1;
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateA);
        }
        
        [Test]
        public void Transition_ToPreviousNode_WhenEventIsTriggered()
        {
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var unityEvent = new UnityEvent();

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => true);
            fsm.AddTransitionFrom(stateB).ToPreviousNode().When(unityEvent);
            fsm.Start();
            fsm.Tick();
            unityEvent.Invoke();
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateA);
        }
        
        [Test]
        public void SetCurrentNode_InteruptCurrentNode_And_JumpToSetNode()
        {
            var stateName = string.Empty;
            var conditionValue = 0;
            var fsm = new FSM();
            var stateA = new ActionNode { EnterAction = () => stateName = "A" };
            var stateB = new ActionNode { EnterAction = () => stateName = "B" };
            var stateC = new ActionNode { EnterAction = () => stateName = "C" };

            fsm.AddTransitionFrom(stateA).To(stateB).When(() => conditionValue == 1);
            fsm.AddTransitionFrom(stateB).To(stateC).When(() => conditionValue == 2);
            fsm.Start();
            conditionValue = 1;
            fsm.Tick();
            conditionValue = 2;
            fsm.Tick();
            fsm.SetCurrentNode(stateA);
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateA && stateName == "A");
        }
        
        [Test]
        public void Transition_FromSelectorNode_WhenPredicateIsTrue()
        {
            var n = 2;
            var fsm = new FSM();
            var stateA = new EmptyNode();
            var stateB = new EmptyNode();
            var stateC = new EmptyNode();

            fsm.AddTransitionFromSelectorNode().To(stateA).When(() => n == 1);
            fsm.AddTransitionFromSelectorNode().To(stateB).When(() => n == 2);
            fsm.AddTransitionFromSelectorNode().To(stateC).When(() => n == 3);
            fsm.Start();
            fsm.Tick();

            Assert.IsTrue(fsm.CurrentNode == stateB);
        }
    }
}