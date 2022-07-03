using Nolib.Node;
using NUnit.Framework;

namespace Tests
{
    public class TestSubFSM
    {
        [Test]
        public void AddNode_NotAllowDuplicateNode()
        {
            var subFSM = new FSM();
            var stateA = new StateA();
            
            subFSM.AddNode(stateA);
            subFSM.AddNode(stateA);
    
            Assert.IsTrue(subFSM.NodeCount == 1);
        }
        
        [Test]
        public void AddNode_NotAllowIntersectSubFSM()
        {
            var subFSMA = new SubFSM();
            var subFSMB = new SubFSM();
            var stateA  = new StateA();
        
            subFSMA.AddNode(stateA);
            subFSMB.AddNode(stateA);
            subFSMB.AddNode(subFSMA);
    
            Assert.IsTrue(!subFSMB.Contains(subFSMA));
        }
        
        [Test]
        public void AddNode_NotAllowOwnerNode()
        {
            var fsm    = new FSM();
            var subFSM = new SubFSM();
            var stateA = new StateA();
            
            fsm.AddNode(stateA);
            fsm.AddNode(subFSM);
            
            subFSM.AddNode(stateA);
    
            Assert.IsTrue(!subFSM.Contains(stateA));
        }
        
        [Test]
        public void Transition_NodeToSubFSM_WhenPredicateIsTrue()
        {
            var n       = 0;
            var fsm     = new FSM();
            var subFSMA = new SubFSM();
            var subFSMB = new SubFSM();
            var stateA  = new StateA();
            var stateB  = new StateB();
            var stateC  = new StateC();
            var stateD  = new StateD();
            
            fsm.AddTransition(stateA, subFSMA, () => n == 1);
            subFSMA.AddTransition(stateB, subFSMB, () => n == 2);
            subFSMB.AddTransition(stateC, stateD,  () => n == 3);
    
            fsm.Start();
            n = 1;
            fsm.Update();
            n = 2;
            fsm.Update();
            n = 3;
            fsm.Update();
    
            Assert.IsTrue(fsm.CurrentNode == subFSMA && subFSMA.CurrentNode == subFSMB && subFSMB.CurrentNode == stateD);
        }
    }
}
