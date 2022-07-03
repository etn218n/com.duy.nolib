using NUnit.Framework;
using Nolib.DataStructure;

namespace Tests
{
    public class TestCircularBuffer
    {
        [Test]
        public void Contains_True_ForContainedElement()
        {
            var circularBuffer = new CircularBuffer<string>(1);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");

            Assert.IsTrue(!circularBuffer.Contains("A") &&
                           circularBuffer.Contains("B"));
        }
        
        [Test]
        public void PopHead_HeadEqualsTail_WhenBufferEmpty()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushHead("A");
            circularBuffer.PushHead("B");
            circularBuffer.PushHead("C");
            circularBuffer.PushHead("D");
            circularBuffer.PopHead();
            circularBuffer.PopHead();
            circularBuffer.PopHead();
            circularBuffer.PopHead();

            Assert.IsTrue(circularBuffer.Count == 0 &&
                          circularBuffer.Head == circularBuffer.Tail);
        }
        
        [Test]
        public void PopHead_RemoveElement_FromTheHead()
        {
            var circularBuffer = new CircularBuffer<string>(2);

            circularBuffer.PushHead("A");
            circularBuffer.PushHead("B");
            circularBuffer.PushHead("C");
            
            var poppedElement = circularBuffer.PopHead();

            Assert.IsTrue(poppedElement == "C" &&
                          circularBuffer.Count == 1 &&
                          !circularBuffer.Contains("C"));
        }
        
        [Test]
        public void PopTail_HeadEqualsTail_WhenBufferEmpty()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushHead("A");
            circularBuffer.PushHead("B");
            circularBuffer.PushHead("C");
            circularBuffer.PushHead("D");
            circularBuffer.PopTail();
            circularBuffer.PopTail();
            circularBuffer.PopTail();
            circularBuffer.PopTail();

            Assert.IsTrue(circularBuffer.Count == 0 &&
                          circularBuffer.Head == circularBuffer.Tail);
        }
        
        [Test]
        public void PopTail_RemoveElement_FromTheTail()
        {
            var circularBuffer = new CircularBuffer<string>(2);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");
            circularBuffer.PushTail("C");
            
            var poppedElement = circularBuffer.PopTail();

            Assert.IsTrue(poppedElement == "C" &&
                          circularBuffer.Count == 1 &&
                          !circularBuffer.Contains("C"));
        }
        
        [Test]
        public void PushHead_CirculateElement_WhenReachCapacity()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushHead("A");
            circularBuffer.PushHead("B");
            circularBuffer.PushHead("C");
            circularBuffer.PushHead("D");

            Assert.IsTrue(circularBuffer.Count == 3 &&
                          circularBuffer.PeekHead() == "D" &&
                          circularBuffer.PeekTail() == "B");
        }
        
        [Test]
        public void PushHead_Grow_WhenBufferGrow()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushHead("A");
            circularBuffer.PushHead("B");
            circularBuffer.PushHead("C");
            circularBuffer.GrowBy(4);
            circularBuffer.PushHead("D");

            Assert.IsTrue(circularBuffer.Count == 4 &&
                          circularBuffer.PeekHead() == "D" &&
                          circularBuffer.PeekTail() == "A");
        }
        
        [Test]
        public void PushTail_CirculateElement_WhenReachCapacity()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");
            circularBuffer.PushTail("C");
            circularBuffer.PushTail("D");

            Assert.IsTrue(circularBuffer.Count == 3 &&
                          circularBuffer.PeekHead() == "B" &&
                          circularBuffer.PeekTail() == "D");
        }
        
        [Test]
        public void PushTail_Grow_WhenBufferGrow()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");
            circularBuffer.PushTail("C");
            circularBuffer.GrowBy(4);
            circularBuffer.PushTail("D");

            Assert.IsTrue(circularBuffer.Count == 4 &&
                          circularBuffer.PeekHead() == "A" &&
                          circularBuffer.PeekTail() == "D");
        }
        
        [Test]
        public void ShrinkHead_ElementCount_Changed_WhenNotEnoughSpace()
        {
            var circularBuffer = new CircularBuffer<string>(2);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");
            circularBuffer.PushTail("C");
            circularBuffer.ShrinkHeadBy(1);

            Assert.IsTrue(circularBuffer.Count == 1 &&
                          circularBuffer.PeekHead() == "C" &&
                          circularBuffer.PeekTail() == "C");
        }

        [Test]
        public void ShrinkHead_ElementCount_Unchanged_WhenEnoughSpace()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");

            var countBeforeShrink = circularBuffer.Count;
            circularBuffer.ShrinkHeadBy(1);
            var countAfterShrink = circularBuffer.Count;

            Assert.IsTrue(countAfterShrink == countBeforeShrink &&
                          circularBuffer.PeekHead() == "A" &&
                          circularBuffer.PeekTail() == "B");
        }

        [Test]
        public void ShrinkTail_ElementCount_Changed_WhenNotEnoughSpace()
        {
            var circularBuffer = new CircularBuffer<string>(2);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");
            circularBuffer.PushTail("C");
            circularBuffer.ShrinkTailBy(1);

            Assert.IsTrue(circularBuffer.Count == 1 &&
                          circularBuffer.PeekHead() == "B" &&
                          circularBuffer.PeekTail() == "B");
        }

        [Test]
        public void ShrinkTail_ElementCount_Unchanged_WhenEnoughSpace()
        {
            var circularBuffer = new CircularBuffer<string>(3);

            circularBuffer.PushTail("A");
            circularBuffer.PushTail("B");

            var countBeforeShrink = circularBuffer.Count;
            circularBuffer.ShrinkTailBy(1);
            var countAfterShrink = circularBuffer.Count;

            Assert.IsTrue(countAfterShrink == countBeforeShrink &&
                          circularBuffer.PeekHead() == "A" &&
                          circularBuffer.PeekTail() == "B");
        }
    }
}