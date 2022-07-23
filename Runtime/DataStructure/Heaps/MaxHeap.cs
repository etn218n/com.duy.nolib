using System;
using System.Collections.Generic;

namespace Nolib.DataStructure
{
    public class MaxHeap<T> : BaseBinaryHeap<T> where T : IComparable
    {
        public MaxHeap(int reservedCapacity = 10)
        {
            this.comparer = Compare;
            this.elements = new List<T>(reservedCapacity);
        }

        private bool Compare(T a, T b)
        {
            return a.CompareTo(b) < 0;
        }
    }
}