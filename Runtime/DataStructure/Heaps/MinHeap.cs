using System;
using System.Collections.Generic;

namespace Nolib.DataStructure
{
    public class MinHeap<T> : BaseBinaryHeap<T> where T : IComparable
    {
        public MinHeap(int reservedCapacity = 10)
        {
            this.comparer = Compare;
            this.elements = new List<T>(reservedCapacity);
        }

        private bool Compare(T a, T b)
        {
            return a.CompareTo(b) > 0;
        }
    }
}