using System;
using System.Collections.Generic;

namespace Nolib.DataStructure
{
    public class BinaryHeap<T> : BaseBinaryHeap<T>
    {
        public BinaryHeap(Func<T, T, bool> comparer, int reservedCapacity = 10)
        {
            this.comparer = comparer;
            this.elements = new List<T>(reservedCapacity);
        }
    }
}