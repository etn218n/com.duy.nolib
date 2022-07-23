using System;
using System.Collections.Generic;
using NUnit.Framework;
using Nolib.DataStructure;
using Random = UnityEngine.Random;

namespace Tests
{
    public class TestBinaryHeap
    {
        private const int NumberOfRuns    = 100;
        private const int NumberOfSamples = 100;

        [Test]
        public void MaxHeap_Pop_ReturnHighestValue()
        {
            var results = Test.Repeat(RunRandomMaxHeapPop, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }
        
        [Test]
        public void MaxHeap_Push_GenerateValidHeap()
        {
            var results = Test.Repeat(RunRandomMaxHeapPush, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }
        
        [Test]
        public void MaxPermutationHeap_Pop_ReturnHighestValue()
        {
            var results = Test.Repeat(RunRandomMaxPermutationHeapPop, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }
        
        [Test]
        public void MaxPermutationHeap_Push_GenerateValidHeap()
        {
            var results = Test.Repeat(RunRandomMaxPermutationHeapPush, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }
        
        [Test]
        public void MinPermutationHeap_Pop_ReturnLowestValue()
        {
            var results = Test.Repeat(RunRandomMinPermutationHeapPop, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }
        
        [Test]
        public void MinPermutationHeap_Push_GenerateValidHeap()
        {
            var results = Test.Repeat(RunRandomMinPermutationHeapPush, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }

        [Test]
        public void MinHeap_Pop_ReturnLowestValue()
        {
            var results = Test.Repeat(RunRandomMinHeapPop, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }
        
        [Test]
        public void MinHeap_Push_GenerateValidHeap()
        {
            var results = Test.Repeat(RunRandomMinHeapPush, NumberOfRuns);

            Assert.IsTrue(results.TrueForAll(result => result == true));
        }
        
        private int[] CreateRandomSamples(int numberOfSamples)
        {
            var samples = new int[numberOfSamples];

            for (int i = 0; i < numberOfSamples; i++)
                samples[i] = i;

            Test.Shuffle(samples);

            return samples;
        }

        private bool RunRandomMaxHeapPush()
        {
            var maxHeap = new MaxHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));

            maxHeap.Push(samples);

            return IsValidMaxHeap(maxHeap);
        }

        private bool RunRandomMaxHeapPop()
        {
            var maxHeap = new MaxHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));
            var highest = Test.SearchHighest(samples);
            
            maxHeap.Push(samples);
            var popped = maxHeap.Pop();

            return IsValidMaxHeap(maxHeap) && popped == highest;
        }

        private bool RunRandomMinHeapPush()
        {
            var minHeap = new MinHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));

            minHeap.Push(samples);

            return IsValidMinHeap(minHeap);
        }

        private bool RunRandomMinHeapPop()
        {
            var minHeap = new MinHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));
            var lowest  = Test.SearchLowest(samples);

            minHeap.Push(samples);
            var popped = minHeap.Pop();

            return IsValidMinHeap(minHeap) && popped == lowest;
        }
        
        private bool RunRandomMinPermutationHeapPush()
        {
            var minHeap = new MinPermutationHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));

            minHeap.Build(samples);

            return IsValidMinPermutationHeap(minHeap, samples);
        }
        
        private bool RunRandomMaxPermutationHeapPush()
        {
            var maxHeap = new MaxPermutationHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));

            maxHeap.Build(samples);

            return IsValidMaxPermutationHeap(maxHeap, samples);
        }
        
        private bool RunRandomMinPermutationHeapPop()
        {
            var minHeap = new MinPermutationHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));
            var lowest  = Test.SearchLowest(samples);

            minHeap.Build(samples);
            var indexOfLowestValue = minHeap.Pop();

            return IsValidMinPermutationHeap(minHeap, samples) && samples[indexOfLowestValue] == lowest;
        }
        
        private bool RunRandomMaxPermutationHeapPop()
        {
            var maxHeap = new MaxPermutationHeap<int>();
            var samples = CreateRandomSamples(Random.Range(1, NumberOfSamples));
            var highest = Test.SearchHighest(samples);

            maxHeap.Build(samples);
            var indexOfHighestValue = maxHeap.Pop();

            return IsValidMaxPermutationHeap(maxHeap, samples) && samples[indexOfHighestValue] == highest;
        }

        private bool IsValidMaxPermutationHeap<T>(MaxPermutationHeap<T> heap, IList<T> originalList) where T : IComparable
        {
            var flattenMaxHeap = heap.Flatten();

            for (int i = 0; i < flattenMaxHeap.Length; i++)
            {
                var parentIndex     = (int) Math.Floor((i - 1) * 0.5f);
                var leftChildIndex  = 2 * i + 1;
                var rightChildIndex = 2 * i + 2;

                if (parentIndex > 0 && originalList[flattenMaxHeap[i]].CompareTo(originalList[flattenMaxHeap[parentIndex]]) > 0)
                    return false;

                if (leftChildIndex < flattenMaxHeap.Length && originalList[flattenMaxHeap[i]].CompareTo(originalList[flattenMaxHeap[leftChildIndex]]) < 0)
                    return false;

                if (rightChildIndex < flattenMaxHeap.Length && originalList[flattenMaxHeap[i]].CompareTo(originalList[flattenMaxHeap[rightChildIndex]]) < 0)
                    return false;
            }

            return true;
        }
        
        private bool IsValidMinPermutationHeap<T>(MinPermutationHeap<T> heap, IList<T> originalList) where T : IComparable
        {
            var flattenMinHeap = heap.Flatten();

            for (int i = 0; i < flattenMinHeap.Length; i++)
            {
                var parentIndex     = (int) Math.Floor((i - 1) * 0.5f);
                var leftChildIndex  = 2 * i + 1;
                var rightChildIndex = 2 * i + 2;

                if (parentIndex > 0 && originalList[flattenMinHeap[i]].CompareTo(originalList[flattenMinHeap[parentIndex]]) < 0)
                    return false;

                if (leftChildIndex < flattenMinHeap.Length && originalList[flattenMinHeap[i]].CompareTo(originalList[flattenMinHeap[leftChildIndex]]) > 0)
                    return false;

                if (rightChildIndex < flattenMinHeap.Length && originalList[flattenMinHeap[i]].CompareTo(originalList[flattenMinHeap[rightChildIndex]]) > 0)
                    return false;
            }

            return true;
        }
        
        private bool IsValidMinHeap<T>(MinHeap<T> heap) where T : IComparable
        {
            var flattenMinHeap = heap.Flatten();

            for (int i = 0; i < flattenMinHeap.Length; i++)
            {
                var parentIndex     = (int) Math.Floor((i - 1) * 0.5f);
                var leftChildIndex  = 2 * i + 1;
                var rightChildIndex = 2 * i + 2;

                if (parentIndex > 0 && flattenMinHeap[i].CompareTo(flattenMinHeap[parentIndex]) < 0)
                    return false;

                if (leftChildIndex < flattenMinHeap.Length && flattenMinHeap[i].CompareTo(flattenMinHeap[leftChildIndex]) > 0)
                    return false;

                if (rightChildIndex < flattenMinHeap.Length && flattenMinHeap[i].CompareTo(flattenMinHeap[rightChildIndex]) > 0)
                    return false;
            }

            return true;
        }

        private bool IsValidMaxHeap<T>(MaxHeap<T> heap) where T : IComparable
        {
            var flattenMaxHeap = heap.Flatten();

            for (int i = 0; i < flattenMaxHeap.Length; i++)
            {
                var parentIndex     = (int) Math.Floor((i - 1) * 0.5f);
                var leftChildIndex  = 2 * i + 1;
                var rightChildIndex = 2 * i + 2;

                if (parentIndex > 0 && flattenMaxHeap[i].CompareTo(flattenMaxHeap[parentIndex]) > 0)
                    return false;

                if (leftChildIndex < flattenMaxHeap.Length && flattenMaxHeap[i].CompareTo(flattenMaxHeap[leftChildIndex]) < 0)
                    return false;

                if (rightChildIndex < flattenMaxHeap.Length && flattenMaxHeap[i].CompareTo(flattenMaxHeap[rightChildIndex]) < 0)
                    return false;
            }

            return true;
        }
    }
}