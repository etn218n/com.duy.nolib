using Nolib.Node;
using NUnit.Framework;
using UnityEngine.Events;

namespace Tests
{
    public class TestFSM
    {
        [Test]
        public void AddNode_NotAllowDuplicateNode()
        {
            var fsm    = new FSM();
            var stateA = new StateA();

            fsm.AddNode(stateA);
            fsm.AddNode(stateA);

            Assert.IsTrue(fsm.NodeCount == 1);
        }

        [Test]
        public void AddNode_NotAllowIntersectSubFSM()
        {
            var fsm    = new FSM();
            var subFSM = new SubFSM();
            var stateA = new StateA();

            subFSM.AddNode(stateA);

            fsm.AddNode(stateA);
            fsm.AddNode(subFSM);

            Assert.IsTrue(!fsm.Contains(subFSM));
        }

        [Test]
        public void RemoveNode_FromFSM()
        {
            var fsm    = new FSM();
            var stateA = new StateA();
            var stateB = new StateB();
            var stateC = new StateC();

            fsm.AddTransition(stateA, stateB, () => true);
            fsm.AddTransition(stateB, stateC, () => true);

            fsm.RemoveNode(stateB);

            Assert.IsTrue(!fsm.Contains(stateB));
        }

        [Test]
        public void RemoveNode_FromSubFSM()
        {
            var fsm    = new FSM();
            var subFSM = new SubFSM();
            var stateA = new StateA();
            var stateB = new StateB();
            var stateD = new StateD();

            subFSM.AddNode(stateD);
            fsm.AddTransition(stateA, stateB, () => true);
            fsm.AddTransition(stateB, subFSM, () => true);
            fsm.RemoveNode(stateD);

            Assert.IsTrue(!fsm.Contains(stateD));
        }

        [Test]
        public void RemoveSubFSM_FromFSM()
        {
            var fsm    = new FSM();
            var subFSM = new SubFSM();
            var stateA = new StateA();
            var stateB = new StateB();
            var stateC = new StateC();
            var stateD = new StateD();

            subFSM.AddTransition(stateC, stateD, () => true);
            fsm.AddTransition(stateA, stateB, () => true);
            fsm.AddTransition(stateB, subFSM, () => true);
            fsm.RemoveNode(subFSM);

            Assert.IsTrue(!fsm.Contains(subFSM) && !fsm.Contains(stateC) && !fsm.Contains(stateD));
        }

        [Test]
        public void Transition_NodeToNode_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var stateA = new StateA();
            var stateB = new StateB();

            fsm.AddTransition(stateA, stateB, () => n == 1);

            fsm.Start();
            n = 1;
            fsm.Update();

            Assert.IsTrue(fsm.CurrentNode == stateB);
        }

        [Test]
        public void Transition_NodeToSubFSM_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var subFSM = new SubFSM();
            var stateA = new StateA();
            var stateB = new StateB();
            var stateC = new StateC();

            subFSM.AddTransition(stateB, stateC, () => n == 2);
            fsm.AddTransition(stateA, subFSM, () => n == 1);

            fsm.Start();
            n = 1;
            fsm.Update();
            n = 2;
            fsm.Update();

            Assert.IsTrue(fsm.CurrentNode == subFSM && subFSM.CurrentNode == stateC);
        }

        [Test]
        public void Transition_NodeToNode_WhenUnityEventIsTriggered()
        {
            var n = 0;
            var fsm = new FSM();
            var unityEvent = new UnityEvent();

            fsm.AddTransition(new ActionNode {EnterAction = () => n = 1}, 
                              new ActionNode {EnterAction = () => n = 2},
                              unityEvent);

            fsm.Start();
            unityEvent.Invoke();
            fsm.Update();

            Assert.IsTrue(n == 2);
        }

        [Test]
        public void Transition_FromAnyNode_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var stateA = new StateA();
            var stateB = new StateB();
            var stateC = new StateC();

            fsm.AddTransition(stateA, stateB, () => n == 1);
            fsm.AddTransitionFromAnyNode(stateC, () => n == 2);

            fsm.Start();
            n = 1;
            fsm.Update();
            n = 2;
            fsm.Update();

            Assert.IsTrue(fsm.CurrentNode == stateC);
        }

        [Test]
        public void Transition_ToPreviousNode_WhenPredicateIsTrue()
        {
            var n = 0;
            var fsm = new FSM();
            var stateA = new StateA();
            var stateB = new StateB();
            var stateC = new StateC();

            fsm.AddTransition(stateA, stateB, () => n == 1);
            fsm.AddTransition(stateB, stateC, () => n == 2);
            fsm.AddTransitionToPreviousNode(stateC, () => n == 3);

            fsm.Start();
            n = 1;
            fsm.Update();
            n = 2;
            fsm.Update();
            n = 3;
            fsm.Update();

            Assert.IsTrue(fsm.CurrentNode == stateB);
        }
    }
}