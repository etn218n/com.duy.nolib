using System;
using System.Collections.Generic;

namespace Nolib.DataStructure
{
    public abstract class BaseBinaryHeap<T>
    {
        protected List<T> elements;
        protected Func<T, T, bool> comparer;

        public int Count => elements.Count;

        public void Push(T element)
        {
            elements.Add(element);
            SiftUp(elements.Count - 1, elements);
        }
        
        public void Push(IEnumerable<T> elements)
        {
            foreach (var element in elements)
                Push(element);
        }

        public void Clear()
        {
            elements.Clear();
        }

        public T Pop()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException();

            var rootElement = elements[0];
            var lastElementIndex = elements.Count - 1;

            elements[0] = elements[lastElementIndex];
            elements.RemoveAt(lastElementIndex);
            
            SiftDown(0, elements);

            return rootElement;
        }

        public T Peek()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException();
            
            return elements[0];
        }
        
        public T[] Flatten()
        {
            var clones = new T[elements.Count];
            
            elements.CopyTo(clones);

            return clones;
        }
        
        protected void SiftUp(int elementIndex, IList<T> list)
        {
            if (list.Count == 0)
                return;
            
            while (true)
            {
                var parentIndex = ParentIndexOf(elementIndex, list);

                if (parentIndex == -1 || comparer(list[elementIndex], list[parentIndex]))
                    return;

                Swap(elementIndex, parentIndex, list);
                elementIndex = parentIndex;
            }
        }

        protected void SiftDown(int elementIndex, IList<T> list)
        {
            if (list.Count == 0)
                return;
            
            while (true)
            {
                var leftChildIndex  = LeftChildIndexOf(elementIndex, list);
                var rightChildIndex = RightChildIndexOf(elementIndex, list);
                var largestElementIndex = elementIndex;

                if (rightChildIndex != -1 && !comparer(list[rightChildIndex], list[largestElementIndex])) 
                    largestElementIndex = rightChildIndex;

                if (leftChildIndex != -1 && !comparer(list[leftChildIndex], list[largestElementIndex])) 
                    largestElementIndex = leftChildIndex;

                if (largestElementIndex == elementIndex) 
                    return;

                Swap(largestElementIndex, elementIndex, list);
                elementIndex = largestElementIndex;
            }
        }

        protected int ParentIndexOf(int index, IList<T> list)
        {
            if (index <= 0 || index >= list.Count)
                return -1;
            
            return (int)Math.Floor((index - 1) * 0.5f);
        }
        
        protected int LeftChildIndexOf(int index, IList<T> list)
        {
            if (index < 0 || index >= list.Count)
                return -1;

            var leftChildIndex = index * 2 + 1;

            if (leftChildIndex >= list.Count)
                return -1;
            
            return leftChildIndex;
        }
        
        protected int RightChildIndexOf(int index, IList<T> list)
        {
            if (index < 0 || index >= list.Count)
                return -1;

            var rightChildIndex = index * 2 + 2;

            if (rightChildIndex >= list.Count)
                return -1;
            
            return rightChildIndex;
        }

        protected void Swap(int indexA, int indexB, IList<T> list)
        {
            var temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
    }
}