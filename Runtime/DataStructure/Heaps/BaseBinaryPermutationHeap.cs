using System;
using System.Collections.Generic;

namespace Nolib.DataStructure
{
    public abstract class BaseBinaryPermutationHeap<T>
    {
        protected T[] elements;
        protected List<int> permutations;
        protected Func<T, T, bool> comparer;

        public int Count => elements?.Length ?? 0;
        
        protected virtual void Build(T[] elements, Func<T, T, bool> comparer)
        {
            this.elements     = elements;
            this.comparer     = comparer;
            this.permutations = new List<int>(elements.Length);

            for (int i = 0; i < elements.Length; i++)
                permutations.Add(i);

            for (int i = 0; i < elements.Length; i++)
                SiftUp(i, elements, permutations);
        }

        public void Clear()
        {
            elements = null;
            permutations.Clear();
        }

        public int Pop()
        {
            if (permutations.Count == 0)
                throw new InvalidOperationException();

            var rootIndex = permutations[0];
            var lastIndex = permutations.Count - 1;

            permutations[0] = permutations[lastIndex];
            permutations.RemoveAt(lastIndex);
            
            SiftDown(0, elements, permutations);

            return rootIndex;
        }

        public int Peek()
        {
            if (permutations.Count == 0)
                throw new InvalidOperationException();
            
            return permutations[0];
        }

        public int[] Flatten()
        {
            var clones = new int[permutations.Count];
            
            permutations.CopyTo(clones);

            return clones;
        }
        
        protected void SiftUp(int index, IList<T> elements, IList<int> permutations)
        {
            if (permutations.Count == 0)
                return;
            
            while (true)
            {
                var parentIndex = ParentIndexOf(index, permutations);

                if (parentIndex < 0)
                    return;
                
                var elementPermutationIndex = permutations[index];
                var parentPermutationIndex  = permutations[parentIndex];

                if (comparer(elements[elementPermutationIndex], elements[parentPermutationIndex]))
                    return;

                Swap(index, parentIndex, permutations);
                index = parentIndex;
            }
        }

        protected void SiftDown(int elementIndex, IList<T> elements, IList<int> permutations)
        {
            if (permutations.Count == 0)
                return;
            
            while (true)
            {
                var leftChildIndex  = LeftChildIndexOf(elementIndex, permutations);
                var rightChildIndex = RightChildIndexOf(elementIndex, permutations);
                var largestElementIndex = elementIndex;

                if (rightChildIndex != -1 && !comparer(elements[permutations[rightChildIndex]], elements[permutations[largestElementIndex]])) 
                    largestElementIndex = rightChildIndex;

                if (leftChildIndex != -1 && !comparer(elements[permutations[leftChildIndex]], elements[permutations[largestElementIndex]])) 
                    largestElementIndex = leftChildIndex;

                if (largestElementIndex == elementIndex) 
                    return;

                Swap(largestElementIndex, elementIndex, permutations);
                elementIndex = largestElementIndex;
            }
        }

        protected int ParentIndexOf(int index, IList<int> list)
        {
            if (index <= 0 || index >= list.Count)
                return -1;
            
            return (int)Math.Floor((index - 1) * 0.5f);
        }
        
        protected int LeftChildIndexOf(int index, IList<int> list)
        {
            if (index < 0 || index >= list.Count)
                return -1;

            var leftChildIndex = index * 2 + 1;

            if (leftChildIndex >= list.Count)
                return -1;
            
            return leftChildIndex;
        }
        
        protected int RightChildIndexOf(int index, IList<int> list)
        {
            if (index < 0 || index >= list.Count)
                return -1;

            var rightChildIndex = index * 2 + 2;

            if (rightChildIndex >= list.Count)
                return -1;
            
            return rightChildIndex;
        }

        protected void Swap(int indexA, int indexB, IList<int> list)
        {
            var temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
    }
}